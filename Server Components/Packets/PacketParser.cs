using MCSharp.Data;
using MCSharp.Misc.Enums;
using MCSharp.Misc;
using MCSharp.ServerComponents.Packets.ServerBound;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSharp.ServerComponents.Packets
{
    public abstract class PacketParser
    {
        public abstract ServerBoundPacket ParsePacket(Stream Stream, int Length, int PacketID, MinecraftVersion Version, ConnectionState CurrentState);
    }
    public class DefaultPacketParser : PacketParser
    {
        public override ServerBoundPacket ParsePacket(Stream Stream, int Length, int PacketID, MinecraftVersion Version, ConnectionState CurrentState)
        {
            ServerBoundPacket result = new UnknownServerBoundPacket();
            switch (CurrentState)
            {
                case ConnectionState.Handshake:
                    switch (PacketID)
                    {
                        case 0x00:
                            Handshake handshake = new Handshake();
                            handshake.ParseFromStream(Stream, Length, Version);
                            result = handshake;
                            break;
                    }
                    break;
                case ConnectionState.Status:
                    switch (PacketID)
                    {
                        case 0x00:
                            StatusRequest request = new StatusRequest();
                            request.ParseFromStream(Stream, Length, Version);
                            result = request;
                            break;
                        case 0x01:
                            StatusPing ping = new StatusPing();
                            ping.ParseFromStream(Stream, Length, Version);
                            result = ping;
                            break;
                    }
                    break;
                case ConnectionState.Login:
                    switch (PacketID)
                    {

                    }
                    break;
                case ConnectionState.Play:
                    switch (PacketID)
                    {

                    }
                    break;
            }
            return result;
        }
    }
}
