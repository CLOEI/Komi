namespace Komi.lib.types
{
    public struct Config
    {
        public List<BotConfig> Bots { get; set; }
        public uint Timeout { get; set; }
        public uint FindPathDelay { get; set; }
        public string SelectedBot { get; set; }
        public string GameVersion { get; set; }
    }
    
    public struct BotConfig
    {
        public string Payload { get; set; }
        public ELoginMethod LoginMethod { get; set; }
        public string Token { get; set; }
        public string Data { get; set; }
    }
}