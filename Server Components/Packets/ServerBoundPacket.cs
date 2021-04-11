using MCSharp.Data;
using MCSharp.GameplayComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSharp.ServerComponents.Packets
{
    public abstract class ServerBoundPacket
    {
        public abstract int GetPacketID(MinecraftVersion Version);
        public abstract void ParseFromStream(Stream Stream, int Length, MinecraftVersion Version);
        public abstract void HandlePacket(Zone Zone, Client Sender, MCSharpServer Server);
    }
    public class UnknownServerBoundPacket : ServerBoundPacket
    {
        public override int GetPacketID(MinecraftVersion Version)
        {
            return 0x00;
        }

        public override void HandlePacket(Zone Zone, Client Sender, MCSharpServer Server)
        {
            return;
        }

        public override void ParseFromStream(Stream Stream, int Length, MinecraftVersion Version)
        {
            Stream.Seek(Length, SeekOrigin.Current);
        }
    }
}
