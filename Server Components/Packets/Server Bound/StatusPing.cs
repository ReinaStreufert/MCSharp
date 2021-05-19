using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCSharp.Data;
using MCSharp.Misc;
using MCSharp.ServerComponents.Packets.ClientBound;
using MCSharp.Misc.Enums;
using MCSharp.GameplayComponents;

namespace MCSharp.ServerComponents.Packets.ServerBound
{
    public class StatusPing : ServerBoundPacket
    {
        public long Payload;
        public override int GetPacketID(MinecraftVersion Version)
        {
            return 0x01;
        }

        public override void HandlePacket(Zone Zone, Client Sender, MCSharpServer Server)
        {
            StatusPong pong = new StatusPong();
            pong.Payload = Payload;
            Sender.SendPacket(pong);
            Sender.Disconnect(DisconnectReason.ServerListPingCompleted);
        }

        public override void ParseFromStream(Stream Stream, int Length, MinecraftVersion Version)
        {
            Payload = Stream.ReadLong();
        }
    }
}
