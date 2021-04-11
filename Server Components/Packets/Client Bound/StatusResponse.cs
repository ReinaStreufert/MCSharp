using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCSharp.Data;
using MCSharp.GameplayComponents;
using Newtonsoft.Json.Linq;
using MCSharp.Misc;

namespace MCSharp.ServerComponents.Packets.ClientBound
{
    public class StatusResponse : ClientBoundPacket
    {
        private bool invalidated = true;
        private string JSON;

        private MinecraftVersion version;
        private int maxPlayers;
        private int onlinePlayers;
        private Chat serverDescription;
        private string faviconBase64;
        public MinecraftVersion Version
        {
            get
            {
                return version;
            }
            set
            {
                version = value;
                invalidated = true;
            }
        }
        public int MaxPlayers
        {
            get
            {
                return maxPlayers;
            }
            set
            {
                maxPlayers = value;
                invalidated = true;
            }
        }
        public int OnlinePlayers
        {
            get
            {
                return onlinePlayers;
            }
            set
            {
                onlinePlayers = value;
                invalidated = true;
            }
        }
        public Chat ServerDescription
        {
            get
            {
                return serverDescription;
            }
            set
            {
                serverDescription = value;
                invalidated = true;
            }
        }
        public string FaviconBase64
        {
            get
            {
                return faviconBase64;
            }
            set
            {
                faviconBase64 = value;
                invalidated = true;
            }
        }
        public override int GetPacketID(MinecraftVersion Version)
        {
            return 0x00;
        }

        public override void WriteToStream(Stream TargetStream, MinecraftVersion Version)
        {
            if (invalidated)
            {
                update();
            }
            TargetStream.WriteString(JSON);
        }
        private void update()
        {
            JObject response = new JObject();

            JObject version = new JObject();
            version.Add("name", this.Version.VersionName);
            version.Add("protocol", this.Version.ProtocolVersion);
            response.Add("version", version);

            JObject players = new JObject();
            players.Add("max", MaxPlayers);
            players.Add("online", OnlinePlayers);
            players.Add("sample", new JArray());
            response.Add("players", players);

            response.Add("description", ServerDescription.ToJSON());
            if (FaviconBase64 != "")
            {
                response.Add("favicon", "data:image/png;base64," + FaviconBase64);
            }

            JSON = response.ToString();
            invalidated = false;
        }
        public override int GetLength(MinecraftVersion Version)
        {
            if (invalidated)
            {
                update();
            }
            return DataUtils.MeasureString(JSON);
        }
    }
}
