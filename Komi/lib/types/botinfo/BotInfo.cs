namespace Komi.lib.types.botinfo
{
    public class Info
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public ELoginMethod LoginMethod { get; set; }
        public List<string> OauthLinks { get; set; }
        public Dictionary<string, string> ServerData { get; set; }
        public string Token { get; set; }
        public LoginInfo LoginInfo { get; set; }
        public uint Ping { get; set; }
        public string Status { get; set; }
        public uint Timeout { get; set; }
        public ProxyInfo? ProxyInfo { get; set; }
        
        public Info()
        {
            OauthLinks = new List<string>();
            ServerData = new Dictionary<string, string>();
        }
    }
    
    public class ProxyInfo
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class State
    {
        public uint NetId { get; set; }
        public int Gems { get; set; }
        public bool IsRunning { get; set; }
        public bool IsRedirecting { get; set; }
        public bool IsIngame { get; set; }
        public bool IsNotAllowedToWarp { get; set; }
        public bool IsBanned { get; set; }
        public bool IsTutorial { get; set; }
    }

    public class Server
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }

    public class FTUE
    {
        public int CurrentProgress { get; set; }
        public int TotalProgress { get; set; }
        public string Info { get; set; }
    }
}