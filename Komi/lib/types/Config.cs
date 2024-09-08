using MessagePack;

namespace Komi.lib.types
{
    [MessagePackObject]
    public struct Config
    {
        [Key(0)]
        public List<BotConfig> Bots { get; set; }
        [Key(1)]
        public uint Timeout { get; set; }
        [Key(2)]
        public uint FindPathDelay { get; set; }
        [Key(3)]
        public string SelectedBot { get; set; }
        [Key(4)]
        public string GameVersion { get; set; }
    }
    
    public struct BotConfig
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public ELoginMethod LoginMethod { get; set; }
        public string Token { get; set; }
        public string Data { get; set; }
    }
}