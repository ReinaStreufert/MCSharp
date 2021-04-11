using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCSharp.Data;
using MCSharp.Misc;

namespace MCSharp.ServerComponents.Packets.ClientBound
{
    public class StatusPong : ClientBoundPacket
    {
        public override int GetLength(MinecraftVersion Version)
        {
            return 8;
        }

        public long Payload { get; set; }

        public override int GetPacketID(MinecraftVersion Version)
        {
            return 0x01;
        }

        public override void WriteToStream(Stream TargetStream, MinecraftVersion Version)
        {
            TargetStream.WriteNum(Payload);
        }
    }
}
