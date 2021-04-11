using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCSharp.Data;
using MCSharp.Misc.Enums;
using MCSharp.Misc;
using MCSharp.GameplayComponents;
using static MCSharp.Misc.EventDelegates;

namespace MCSharp.ServerComponents.Packets.ServerBound
{
    public class Handshake : ServerBoundPacket
    {
        public MinecraftVersion Version;
        public string ServerAddress;
        public ushort ServerPort;
        public ConnectionState NextState;

        public override int GetPacketID(MinecraftVersion Version)
        {
            return 0x00;
        }
        public override void HandlePacket(Zone Zone, Client Sender, MCSharpServer Server)
        {
            Sender.Version = Version;
            ClientConnectionConfirmedInfo confirmationInfo = new ClientConnectionConfirmedInfo();
            confirmationInfo.ConnectedClient = Sender;
            if (NextState == ConnectionState.Login)
            {
                confirmationInfo.ConnectionPurpose = ConnectionPurpose.LoginAndPlay;
            } else
            {
                confirmationInfo.ConnectionPurpose = ConnectionPurpose.ServerListPing;
            }
            if ((Version > Server.MaximumAcceptedVersion || Version < Server.MinimumAcceptedVersion) && NextState == ConnectionState.Login)
            {
                Sender.SetState(NextState);
                Chat Message = new Chat();
                Message.Components.Add(new ChatComponent("This server only accepts minecraft versions ranging from "));
                Message.Components.Add(new ChatComponent(Server.MinimumAcceptedVersion.VersionName + " to " + Server.MaximumAcceptedVersion.VersionName, ChatSyle.Bold, ChatColor.Purple));
                Sender.Disconnect(DisconnectReason.ClientMinecraftVersionNotAccepted, Message);
            } else if (Server.PlayerCount >= Server.MaxPlayers && NextState == ConnectionState.Login)
            {
                Sender.SetState(NextState);
                Chat Message = new Chat();
                Message.Components.Add(new ChatComponent("This server is at its maximum capacity of "));
                Message.Components.Add(new ChatComponent(Server.MaxPlayers + " players.", ChatSyle.Bold, ChatColor.Purple));
                Sender.Disconnect(DisconnectReason.ServerAtMaxCapacity, Message);
            } else
            {
                Sender.ConnectionConfirmed = true;
                Server.confirmClient(confirmationInfo);
                Sender.SetState(NextState);
            }
        }
        public override void ParseFromStream(Stream Stream, int Length, MinecraftVersion Version)
        {
            int protocolVersion = Stream.ReadVarInt();
            this.Version = new MinecraftVersion(protocolVersion);
            ServerAddress = Stream.ReadString();
            ServerPort = Stream.ReadUshort();
            NextState = (ConnectionState)Stream.ReadVarInt();
            if (NextState != ConnectionState.Login && NextState != ConnectionState.Status)
            {
                throw new FormatException();
            }
        }
    }
}
