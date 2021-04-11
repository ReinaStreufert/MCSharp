using MCSharp.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSharp.ServerComponents.Packets
{
    public abstract class ClientBoundPacket
    {
        public abstract int GetPacketID(MinecraftVersion Version);
        public abstract void WriteToStream(Stream TargetStream, MinecraftVersion Version);
        public abstract int GetLength(MinecraftVersion Version);
    }
}
