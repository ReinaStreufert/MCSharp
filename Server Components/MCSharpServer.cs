using MCSharp.Data;
using MCSharp.GameplayComponents;
using MCSharp.Misc.Enums;
using MCSharp.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MCSharp.ServerComponents.Packets;
using static MCSharp.Misc.EventDelegates;
using System.IO;

namespace MCSharp
{
    public class MCSharpServer
    {
        public MinecraftVersion MinimumSupportedVersion { get; } = new MinecraftVersion(754);
        public MinecraftVersion MaximumSupportedVersion { get; } = new MinecraftVersion(754);
        public int MaxPlayers { get; set; } = 100;
        public Chat ServerDescription { get; set; }
        private bool onlineMode = false;
        public bool OnlineMode
        {
            get
            {
                return onlineMode;
            }
            set
            {
                if (State == ServerState.Running)
                {
                    throw new InvalidOperationException("This field cannot be changed while the server is running. Set it before starting the server.");
                } else
                {
                    onlineMode = value;
                }
            }
        }
        private bool compressionEnabled = false;
        public bool CompressionEnabled
        {
            get
            {
                return compressionEnabled;
            }
            set
            {
                if (State == ServerState.Running)
                {
                    throw new InvalidOperationException("This field cannot be changed while the server is running. Set it before starting the server.");
                } else
                {
                    compressionEnabled = value;
                }
            }
        }
        private int compressionThreshold = 256;
        public int CompressionThreshold
        {
            get
            {
                return compressionThreshold;
            }
            set
            {
                if (State == ServerState.Running)
                {
                    throw new InvalidOperationException("This field cannot be changed while the server is running. Set it before starting the server.");
                } else
                {
                    compressionThreshold = value;
                }
            }
        }
        private string faviconPath = "";
        public string FaviconBase64 { get; set; }
        public string FaviconPath
        {
            get
            {
                return faviconPath;
            }
            set
            {
                if (value == "")
                {
                    FaviconBase64 = "";
                    faviconPath = "";
                    return;
                }
                faviconPath = value;
                byte[] bytes = File.ReadAllBytes(faviconPath);
                FaviconBase64 = Convert.ToBase64String(bytes, Base64FormattingOptions.None);
            }
        }
        public bool VerboseLogging { get; set; } = true;
        public TimeSpan TimeoutPeriod { get; set; } = new TimeSpan(0, 0, 0, 10, 0);
        private List<Client> clients = new List<Client>();
        private List<Zone> zones = new List<Zone>();
        private Zone mainZone;
        private Timer tickTimer;
        private DateTime lastTick;
        public ScheduledThread ServerThread { get; private set; }

        public PacketParser PacketParser { get; set; } = new DefaultPacketParser();
        public event UncomfirmedClientAcceptedEvent OnUncomfirmedClientAccepted;
        public event ClientConnectionConfirmedEvent OnClientConnectionConfirmed;
        public event TickEvent OnServerTick;

        public int PlayerCount { get; private set; }

        private MinecraftVersion minimumAcceptedVersion;
        private MinecraftVersion maximumAcceptedVersion;

        public MinecraftVersion MinimumAcceptedVersion
        {
            get
            {
                return minimumAcceptedVersion;
            }
            set
            {
                if (value < MinimumSupportedVersion)
                {
                    throw new NotSupportedException("That version is below the minimum version MCSharp 1.0 supports");
                } else if (value > MaximumSupportedVersion)
                {
                    throw new NotSupportedException("That version is above the maximum version MCSharp 1.0 supports. A newer version of MCSharp may support this version.");
                }
                minimumAcceptedVersion = value;
                if (maximumAcceptedVersion < minimumAcceptedVersion)
                {
                    maximumAcceptedVersion = minimumAcceptedVersion;
                }
            }
        }
        public MinecraftVersion MaximumAcceptedVersion
        {
            get
            {
                return maximumAcceptedVersion;
            }
            set
            {
                if (value < MinimumSupportedVersion)
                {
                    throw new NotSupportedException("That version is below the minimum version that MCSharp 1.0 supports");
                }
                else if (value > MaximumSupportedVersion)
                {
                    throw new NotSupportedException("That version is above the maximum version that MCSharp 1.0 supports. A newer version of MCSharp may support this version.");
                }
                maximumAcceptedVersion = value;
                if (minimumAcceptedVersion > maximumAcceptedVersion)
                {
                    minimumAcceptedVersion = maximumAcceptedVersion;
                }
            }
        }
        public Zone MainZone
        {
            get
            {
                return mainZone;
            }
            set
            {
                if (zones.Find((Zone z) => { return z == value; }) != null)
                {
                    mainZone = value;
                } else
                {
                    throw new ArgumentException("You must add the zone using MCSharpServer.AddZone before setting it to the main zone.");
                }
            }
        }

        public ServerState State { get; private set; }
        private TcpListener tcpServer;

        public MCSharpServer(IPAddress Address, int Port)
        {
            minimumAcceptedVersion = MinimumSupportedVersion;
            maximumAcceptedVersion = MaximumSupportedVersion;
            tcpServer = new TcpListener(Address, Port);

            ServerDescription = new Chat();
            ServerDescription.Components.Add(new ChatComponent("A Minecraft Server built with "));
            ServerDescription.Components.Add(new ChatComponent("MCSharp.", ChatSyle.Bold, ChatColor.Purple));
        }
        public void AddZone(Zone Zone)
        {
            zones.Add(Zone);
            Zone.Server = this;
        }
        public void RemoveZone(Zone Zone, Zone FlushPlayersTo)
        {
            // v v v unfinished
            zones.Remove(Zone);
        }
        public void Start()
        {
            if (State == ServerState.NotStarted)
            {
                ServerThread = new ScheduledThread();
                State = ServerState.Running;
                tcpServer.Start();
                tcpServer.BeginAcceptTcpClient(onClientAccept, null);
                tickTimer = new Timer(onTick, null, 50, 50);
                lastTick = DateTime.Now;
            } else
            {
                throw new InvalidOperationException("The server is already running or has been stopped.");
            }
        }
        private void onTick(object state)
        {
            ServerThread.Queue(processTick);
        }
        private void processTick(object[] args)
        {
            // Check if any clients have timed out
            foreach (Client c in clients)
            {
                if (c.CurrentState == ConnectionState.Play)
                {
                    // TODO: Check for keep alive timeouts
                } else
                {
                    if ((DateTime.Now - c.CreationTime) >= TimeoutPeriod)
                    {
                        ServerThread.Queue((object[] a) =>
                        {
                            c.Disconnect(DisconnectReason.TimedOutDuringHandshakeOrLogin);
                        });
                    }
                }
            }
            TimeSpan sinceLastTick = DateTime.Now - lastTick;
            TickInfo ti = new TickInfo();
            ti.TimeSinceLastTick = sinceLastTick;
            OnServerTick?.Invoke(ti);

            lastTick = DateTime.Now;
        }
        private void onClientAccept(IAsyncResult result)
        {
            TcpClient c = tcpServer.EndAcceptTcpClient(result);
            ServerThread.Queue(completeAcceptClient, c);
            tcpServer.BeginAcceptTcpClient(onClientAccept, null);
        }
        private void completeAcceptClient(params object[] args)
        {
            TcpClient tcpClient = (TcpClient)args[0];
            Client client = new Client(tcpClient, this);
            clients.Add(client);
            client.startListening();
            OnUncomfirmedClientAccepted?.Invoke(new UncomfirmedClientAcceptedInfo() { AcceptedClient = client });
        }
        internal void removeClient(Client c)
        {
            if (c.ConnectionConfirmed && c.CurrentState == ConnectionState.Login || c.CurrentState == ConnectionState.Play)
            {
                PlayerCount--;
            }
            clients.Remove(c);
        }
        internal void confirmClient(ClientConnectionConfirmedInfo Info)
        {
            if (Info.ConnectionPurpose == ConnectionPurpose.LoginAndPlay)
            {
                PlayerCount++;
            }
            OnClientConnectionConfirmed?.Invoke(Info);
        }
    }
}
