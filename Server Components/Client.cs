using MCSharp.Data;
using MCSharp.Misc.Enums;
using MCSharp.Misc;
using MCSharp.ServerComponents.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MCSharp.GameplayComponents;
using static MCSharp.Misc.EventDelegates;
using MCSharp.ServerComponents.Packets.ClientBound;
using System.IO;
using Ionic.Zlib;

namespace MCSharp
{
    public class Client
    {
        private TcpClient tcpClient;
        private NetworkStream tcpStream;
        public MCSharpServer Server { get; private set; }
        public MinecraftVersion Version = new MinecraftVersion();
        public DateTime CreationTime;
        public DateTime LastKeepAliveResponse;
        public bool ConnectionConfirmed { get; internal set; }

        public Player AttatchedPlayer = null;
        private ScheduledThread sendThread = new ScheduledThread();

        public ConnectionState CurrentState { get; private set; } = ConnectionState.Handshake;
        public event ClientStateChangeEvent OnStateChange;
        public event ClientDisconnectEvent OnDisconnect;

        private byte[] firstByte = new byte[1];

        public void SendPacket(ClientBoundPacket Packet)
        {
            sendThread.Queue(completeSendPacket, Packet);
        }
        public void Disconnect()
        {
            completeDisconnect(DisconnectReason.ClientDisconnectedFromMCSharpApi);
        }
        internal void Disconnect(DisconnectReason Reason)
        {
            completeDisconnect(Reason);
        }
        internal void Disconnect(DisconnectReason Reason, Chat Message)
        {
            completeDisconnect(Reason, Message);
        }
        internal Client(TcpClient TcpClient, MCSharpServer Server)
        {
            this.Server = Server;
            tcpClient = TcpClient;
            tcpStream = tcpClient.GetStream();
            CreationTime = DateTime.Now;
        }
        internal void startListening()
        {
            tcpStream.BeginRead(firstByte, 0, 1, onPacketReceive, null);
        }
        private void onPacketReceive(IAsyncResult result)
        {
            if (CurrentState == ConnectionState.Disconnect) // mmm sexy race condition
            {
                return; 
            }
            int bytesRead;
            try
            {
                bytesRead = tcpStream.EndRead(result);
            } catch (ObjectDisposedException) // mmm sexy race condition pt. 2
            {
                return;
            }
            if (bytesRead == 0)
            {
                Server.ServerThread.Queue(completeDisconnect, DisconnectReason.ClientClosedConnection);
                Console.WriteLine("blah");
            } else
            {
                try
                {
                    int len = tcpStream.ReadVarInt(firstByte[0]);
                    ServerBoundPacket packet;
                    if (Server.CompressionEnabled)
                    {
                        byte[] wholePacket = new byte[len];
                        tcpStream.Read(wholePacket, 0, len);
                        using (MemoryStream ms = new MemoryStream(wholePacket))
                        {
                            int dataLength = ms.ReadVarInt();
                            if (dataLength == 0) // uncompressed
                            {
                                int read;
                                int packetID = ms.ReadVarInt(out read);
                                packet = Server.PacketParser.ParsePacket(tcpStream, len - read - DataUtils.MeasureVarInt(0), packetID, Version, CurrentState);
                            } else
                            {
                                using (ZlibStream zlibStream = new ZlibStream(ms, CompressionMode.Decompress))
                                {
                                    int read;
                                    int packetID = zlibStream.ReadVarInt(out read);
                                    packet = Server.PacketParser.ParsePacket(zlibStream, dataLength - read, packetID, Version, CurrentState);
                                }
                            }
                        }
                    } else
                    {
                        int read;
                        int packetID = tcpStream.ReadVarInt(out read);
                        packet = Server.PacketParser.ParsePacket(tcpStream, len - read, packetID, Version, CurrentState);
                    }
                    Server.ServerThread.Queue(completePacketReceive, packet);
                } catch
                {
                    Server.ServerThread.Queue(completeDisconnect, DisconnectReason.UnexpectedDataFromClient);
                }
            }
        }
        private void completeDisconnect(params object[] args)
        {
            if (CurrentState == ConnectionState.Disconnect) { return; }

            DisconnectReason reason = (DisconnectReason)args[0];
            Chat message = null;
            if (args.Length > 1)
            {
                message = (Chat)args[1];
            }
            ClientDisconnectInfo cdi = new ClientDisconnectInfo();
            cdi.CustomDisconnectMessage = message;
            cdi.DisconnectingClient = this;
            cdi.Reason = reason;
            OnDisconnect?.Invoke(cdi);
            message = cdi.CustomDisconnectMessage;
            if (message != null && (cdi.Reason == DisconnectReason.KeepAlivePacketTimedOut || cdi.Reason == DisconnectReason.ClientDisconnectedFromMCSharpApi || cdi.Reason == DisconnectReason.PlayerKickedFromMCSharpApi || cdi.Reason == DisconnectReason.ClientMinecraftVersionNotAccepted || cdi.Reason == DisconnectReason.TimedOutDuringHandshakeOrLogin || cdi.Reason == DisconnectReason.ServerAtMaxCapacity))
            {
                if (CurrentState == ConnectionState.Login)
                {
                    DisconnectLogin disconnectLogin = new DisconnectLogin();
                    disconnectLogin.DisconnectReason = message;
                    SendPacket(disconnectLogin);
                } else if (CurrentState == ConnectionState.Play)
                {
                    // TODO: DisconnectPlay
                }
            }
            sendThread.FinishUp();
            sendThread.Dispose();
            tcpClient.Close();
            
            Server.removeClient(this);
            CurrentState = ConnectionState.Disconnect;
        }
        private void completePacketReceive(params object[] args)
        {
            ServerBoundPacket packet = (ServerBoundPacket)args[0];
            if (CurrentState == ConnectionState.Play)
            {
                packet.HandlePacket(AttatchedPlayer.CurrentZone, this, Server);
            } else
            {
                packet.HandlePacket(null, this, Server);
            }
            if (CurrentState != ConnectionState.Disconnect) // Some packet handlers will disconnect the client, we must check for this
            {
                tcpStream.BeginRead(firstByte, 0, 1, onPacketReceive, null);
            }
        }
        private void completeSendPacket(params object[] args)
        {
            ClientBoundPacket Packet = (ClientBoundPacket)args[0];
            try
            {
                if (Server.CompressionEnabled)
                {
                    int dataLength = Packet.GetLength(Version) + DataUtils.MeasureVarInt(Packet.GetPacketID(Version));
                    if (dataLength > Server.CompressionThreshold)
                    {
                        byte[] uncompressedData = new byte[dataLength];
                        using (MemoryStream ms = new MemoryStream(uncompressedData))
                        {
                            ms.WriteVarInt(Packet.GetPacketID(Version));
                            Packet.WriteToStream(ms, Version);
                        }
                        byte[] compressedData = ZlibStream.CompressBuffer(uncompressedData);
                        tcpStream.WriteVarInt(DataUtils.MeasureVarInt(dataLength) + compressedData.Length);
                        tcpStream.WriteVarInt(dataLength);
                        tcpStream.Write(compressedData, 0, compressedData.Length);
                    } else
                    {
                        tcpStream.WriteVarInt(dataLength + DataUtils.MeasureVarInt(0));
                        tcpStream.WriteVarInt(0); // uncompresed
                        tcpStream.WriteVarInt(Packet.GetPacketID(Version));
                        Packet.WriteToStream(tcpStream, Version);
                    }
                } else
                {
                    tcpStream.WritePacketHeader(Packet.GetLength(Version), Packet.GetPacketID(Version));
                    Packet.WriteToStream(tcpStream, Version);
                }
            } catch (IOException)
            {
                Server.ServerThread.Queue(completeDisconnect, DisconnectReason.ClientClosedConnection);
            }
        }
        public void SetState(ConnectionState State)
        {
            CurrentState = State;
            OnStateChange?.Invoke(new ClientStateChangeInfo() { NewState = State, StateChangingClient = this });
        }
    }
}
