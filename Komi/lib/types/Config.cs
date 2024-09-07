namespace Komi.lib.types
{
    public struct Config
    {
        List<BotConfig> Bots { get; set; }
        uint Timeout { get; set; }
        uint FindPathDelay { get; set; }
        string SelectedBot { get; set; }
        string GameVersion { get; set; }
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