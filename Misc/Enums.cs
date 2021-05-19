using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSharp.Misc.Enums
{
    public enum ConnectionState : int
    {
        Handshake = 0, // This is the initial state a connection starts in, in which the client transfers basic information such as its version and connection purpose
        Status = 1, // Indicates that the client is currently performing a Server List Ping (SLP), it will be disconnected after the SLP has completed.
        Login = 2, // The client and server are negotiating login in order to start playing
        Play = 3, // The client now has a player attached and is in the game
        Disconnect = 4 // The client is disconnected
    }
    public enum ServerState : byte
    {
        NotStarted = 0, // The server has not been started
        Running = 1, // The server is running
        Stopped = 2 // The server has been stopped. Stopped servers do not start again.
    }
    public enum DisconnectReason : byte
    {
        ClientClosedConnection = 0, // Indicates that the client has closed the TCP connection
        UnexpectedDataFromClient = 1, // Indicates that the client has sent unexpected or invalid data and the server is terminating the connection. This may occur if a non-minecraft client connects to the server by mistake. It can also sometimes occur by mistake when the server cannot correctly detect that the TCP connection has been closed.
        KeepAlivePacketTimedOut = 2, // Indicates that the server was sending keep alive messages that the client was not sending back in return (or taking too long to send back), so the server assumed the connection is no longer valid and is terminating it.
        ClientDisconnectedFromMCSharpApi = 3, // Indicates that the application called Client.Disconnect in order to initate this disconnection
        PlayerKickedFromMCSharpApi = 4, // Indicates that the application called Player.Kick in order to disconnect from the client. Note that ClientDisconnectInfo.CustomDisconnectMessage will already be set based on the argument passed into Player.Kick, but you may still change it.
        ClientMinecraftVersionNotAccepted = 5, // Indicates that the server is disconnecting from the client during login because the client's minecraft version was out of the servers accepted range. Note that ClientDisconnectInfo.CustomDisconnectMessage will already be set to the default MCSharp message, but you may still change it.
        ServerListPingCompleted = 6, // Indicates that the client connected in order to perform a Server List Ping (SLP) which was completed, so the server is now disconnecting.
        TimedOutDuringHandshakeOrLogin = 7, // Indicates that the server is terminating the connection because the handshake and login process has taken too long (usually because the server is expecting packets that the client isn't sending)
        ServerAtMaxCapacity = 8 // Indicates that the server is at maximum capacity (set by MCSharp.MaxPlayers) and cannot accept a new client
    }
    public enum ConnectionPurpose : byte
    {
        ServerListPing = 0, // The client has connected in order to perform a Server List Ping (SLP), it will be disconnected after the SLP has completed.
        LoginAndPlay = 1 // The client has connected in order to login and play.
    }
    public static class NamespaceRegistries
    {
        public static string SoundEvent = "minecraft:sound_event";
        public static string Fluid = "minecraft:fluid";
        public static string MobEffect = "minecraft:mob_effect";
        public static string Block = "minecrat:block";
        public static string Enchantment = "minecraft:enchantment";
        public static string EntityType = "minecraft:entity_type";
        public static string Item = "minecraft:item";
        public static string Potion = "minecraft:potion";
        public static string ParticleType = "minecraft:particle_type";
        public static string BlockEntityType = "minecraft:block_entity_type";
        public static string Motive = "minecraft:motive";
        public static string CustomStat = "minecraft:custom_stat";
        public static string ChunkStatus = "minecraft:chunk_status";
        public static string RuleTest = "minecraft:rule_test";
        public static string PosRuleTest = "minecraft:pos_rule_test";
        public static string Menu = "minecraft:menu";
        public static string RecipeType = "minecraft:recipe_type";
        public static string RecipeSerializer = "minecraft:recipe_serializer";
        public static string Attribute = "minecraft:attribute";
        public static string StatType = "minecraft:stat_type";
        public static string VillagerType = "minecraft:villager_type";
        public static string VillagerPofession = "minecraft:villager_profession";
        public static string PointOfInterestType = "minecraft:point_of_interest_type";
        public static string MemoryModuleType = "minecraft:memory_module_type";
        public static string SensorType = "minecraft:sensor_type";
        public static string Schedule = "minecraft:schedule";
        public static string Activity = "minecraft:activity";
        public static string LootPoolEntryType = "minecraft:loot_pool_entry_type";
        public static string LootFunctionType = "minecraft:loot_function_type";
        public static string LootConditionType = "minecraft:loot_condition_type";
    }
}
