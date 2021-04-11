using MCSharp.GameplayComponents;
using MCSharp.Misc.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSharp.Misc
{
    public static class EventDelegates
    {
        public delegate void ClientDisconnectEvent(ClientDisconnectInfo Info); // Fired right before the server disconnects from a client, or after the server has detected the client is no longer connected to the server
        public class ClientDisconnectInfo
        {
            public DisconnectReason Reason;
            public Chat CustomDisconnectMessage; // This field is only to be used when the server initiates the disconnect and the client was in the login state or play state. Otherwise, its value is ignored.
            public Client DisconnectingClient;
            // The DisconnectReasons that count as a server initiated disconnection are:
            //  -   KeepAlivePacketTimedOut
            //  -   ClientDisconnectedFromMCSharpApi
            //  -   PlayerKickedFromMCSharpApi
            //  -   ClientMinecraftVersionNotAccepted
            //  -   ServerAtMaxCapacity
            //  -   TimedOutDuringHandshakeOrLogin
        }
        public delegate void ClientStateChangeEvent(ClientStateChangeInfo Info); // Fired right before a client changes state. When a disconnection occurs, this event is NOT fired despite the state technically changing to Disconnect (handle OnClientDisconnect instead). It is also not fired when the client is first connected, even though you could consider that a state "change" to Handshake.
        public struct ClientStateChangeInfo
        {
            public Client StateChangingClient;
            public ConnectionState NewState;
        }
        public delegate void UncomfirmedClientAcceptedEvent(UncomfirmedClientAcceptedInfo Info); // Fired as soon as a minecraft client, or possibly something else, initiates a tcp connection to the server. For most events, OnClientConnectionConfirmed is preferable because it indicates that the client and server have completed the handshake process and the client appears to be a minecraft client.
        public struct UncomfirmedClientAcceptedInfo
        {
            public Client AcceptedClient;
        }
        public delegate void ClientConnectionConfirmedEvent(ClientConnectionConfirmedInfo Info); // Fired as soon as a minecraft client has officially connected to the server and completed the handshake process. This is fired BEFORE the Client.OnStateChanged event that indicates the client has left handshake mode.
        public struct ClientConnectionConfirmedInfo
        {
            public Client ConnectedClient;
            public ConnectionPurpose ConnectionPurpose;
        }
        public delegate void TickEvent(TickInfo Info); // Fired every server tick (approximately every 50ms, but server load and timer accuracy causes this to vary)
        public struct TickInfo
        {
            public TimeSpan TimeSinceLastTick; // Should be around 50ms, but server load and timer accuracy causes this to vary
        }
    }
}
