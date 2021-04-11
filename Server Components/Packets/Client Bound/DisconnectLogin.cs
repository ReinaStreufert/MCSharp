using MCSharp.ServerComponents.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCSharp.Data;
using System.IO;
using MCSharp.GameplayComponents;
using MCSharp.Misc;

namespace MCSharp.ServerComponents.Packets.ClientBound
{
    public class DisconnectLogin : ClientBoundPacket
    {
        private string JSON;
        public Chat DisconnectReason
        {
            set
            {
                JSON = value.ToJSON().ToString();
            }
        }

        public override int GetLength(MinecraftVersion Version)
        {
            return DataUtils.MeasureString(JSON);
        }
        public override int GetPacketID(MinecraftVersion Version)
        {
            return 0x00;
        }

        public override void WriteToStream(Stream TargetStream, MinecraftVersion Version)
        {
            TargetStream.WriteString(JSON);
        }
    }
}
