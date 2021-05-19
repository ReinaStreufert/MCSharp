using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCSharp.Data;
using MCSharp.ServerComponents.Packets.ClientBound;
using MCSharp.GameplayComponents;

namespace MCSharp.ServerComponents.Packets.ServerBound
{
    public class StatusRequest : ServerBoundPacket
    {
        public override int GetPacketID(MinecraftVersion Version)
        {
            return 0x00;
        }

        public override void HandlePacket(Zone Zone, Client Sender, MCSharpServer Server)
        {
            StatusResponse response = new StatusResponse();
            response.MaxPlayers = Server.MaxPlayers;
            response.OnlinePlayers = Server.PlayerCount;
            response.Version = Sender.Version;
            response.ServerDescription = Server.ServerDescription;
            Sender.SendPacket(response);
        }

        public override void ParseFromStream(Stream Stream, int Length, MinecraftVersion Version)
        {
            return;
        }
    }
}
