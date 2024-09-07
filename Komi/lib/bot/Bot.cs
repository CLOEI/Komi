using System.Net;
using ENet.Managed;
using Komi.lib.types;
using Komi.lib.types.botinfo;
using Komi.lib.utils;
using Serilog;
using Serilog.Core;

namespace Komi.lib.bot;

public class Bot
{
    Info Info { get; }
    State State { get; }
    Server Server { get; }
    Vector2 Position { get; }
    private ENetHost Host { get; set; }
    private ENetPeer Peer { get; set; }
    private Logger Log { get; set; }
    
    public Bot(BotConfig config)
    {
        Log = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        Info = new Info()
        {
            Username = config.Username,
            Password = config.Password,
            LoginMethod = config.LoginMethod,
            Token = config.Token,
            LoginInfo = new LoginInfo(),
        };
    }

    private void PollEvent()
    {
        while (true)
        {
            var enetEvent = Host.Service(TimeSpan.FromMilliseconds(100));
            switch (enetEvent.Type)
            {
                case ENetEventType.None:
                    continue;
                case ENetEventType.Connect:
                    LogInfo("Connected to the server");
                    continue;
                case ENetEventType.Receive:
                    LogInfo("Received a packet from the server");
                    continue;
                case ENetEventType.Disconnect:
                    LogInfo("Disconnected from the server");
                    break;
            }
        }
    }

    private void CreateHost()
    {
        Host = new ENetHost(null, 1, 2);
        Host.ChecksumWithCRC32();
        Host.CompressWithRangeCoder();
        Host.UsingNewPacket();
    }

    private void ConnectToServer(string ip, int port)
    {
        LogInfo("Connecting to " + ip + ":" + port); ;
        var remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        Peer = Host.Connect(remoteEndPoint, 2, 0);
    }
    
    private Dictionary<String, String> ToHttp()
    {
        while (true)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://www.growtopia1.com/growtopia/server_data.php");
                request.Headers.Add("User-Agent", "UbiServices_SDK_2022.Release.9_PC64_ansi_static");

                var response = client.Send(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    LogError("Failed to connect to the server: " + response);
                    continue;
                }

                var body = response.Content.ReadAsStringAsync().Result;
                return TextParse.ParseServerData(body);
            }
            catch (Exception e)
            {
                LogError("Failed to connect to the server: " + e);
                throw;
            }
        }
    }
    
    private void LogInfo(string message)
    {
        Log.Information("[{username}] {message}", Info.Username, message);
    }
    
    private void LogError(string message)
    {
        Log.Error("[{username}] {message}", Info.Username, message);
    }
}