using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using ENet.Managed;
using Komi.lib.types;
using Komi.lib.types.botinfo;
using Komi.lib.utils;
using Serilog;
using Serilog.Core;

namespace Komi.lib.bot;

public class Bot
{
    private Info Info { get; set; }
    private State State { get; set; }
    private Server Server { get; set; }
    private Vector2 Position { get; set; }
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

    public void Logon(string data)
    {
        if (data.Length == 0)
        {
            Spoof();
        }
        else
        {
            UpdateLoginInfo(data);
        }

        State.IsRunning = true;
        CreateHost();
        PollEvent();
    }

    private void UpdateLoginInfo(string data)
    {
        var parsedData = TextParse.ParseAndStoreAsDic(data);

        foreach (var kvp in parsedData)
        {
            switch (kvp.Key)
            {
                case "UUIDToken":
                    Info.LoginInfo.Uuid = kvp.Value;
                    break;
                case "protocol":
                    Info.LoginInfo.Protocol = kvp.Value;
                    break;
                case "fhash":
                    Info.LoginInfo.FHash = kvp.Value;
                    break;
                case "mac":
                    Info.LoginInfo.Mac = kvp.Value;
                    break;
                case "requestedName":
                    Info.LoginInfo.RequestedName = kvp.Value;
                    break;
                case "hash2":
                    Info.LoginInfo.Hash2 = kvp.Value;
                    break;
                case "fz":
                    Info.LoginInfo.Fz = kvp.Value;
                    break;
                case "f":
                    Info.LoginInfo.F = kvp.Value;
                    break;
                case "player_age":
                    Info.LoginInfo.PlayerAge = kvp.Value;
                    break;
                case "game_version":
                    Info.LoginInfo.GameVersion = kvp.Value;
                    break;
                case "lmode":
                    Info.LoginInfo.LMode = kvp.Value;
                    break;
                case "cbits":
                    Info.LoginInfo.CBits = kvp.Value;
                    break;
                case "rid":
                    Info.LoginInfo.Rid = kvp.Value;
                    break;
                case "GDPR":
                    Info.LoginInfo.Gdpr = kvp.Value;
                    break;
                case "hash":
                    Info.LoginInfo.Hash = kvp.Value;
                    break;
                case "category":
                    Info.LoginInfo.Category = kvp.Value;
                    break;
                case "token":
                    Info.LoginInfo.Token = kvp.Value;
                    break;
                case "total_playtime":
                    Info.LoginInfo.TotalPlaytime = kvp.Value;
                    break;
                case "door_id":
                    Info.LoginInfo.DoorId = kvp.Value;
                    break;
                case "klv":
                    Info.LoginInfo.Klv = kvp.Value;
                    break;
                case "meta":
                    Info.LoginInfo.Meta = kvp.Value;
                    break;
                case "platformID":
                    Info.LoginInfo.PlatformId = kvp.Value;
                    break;
                case "deviceVersion":
                    Info.LoginInfo.DeviceVersion = kvp.Value;
                    break;
                case "zf":
                    Info.LoginInfo.Zf = kvp.Value;
                    break;
                case "country":
                    Info.LoginInfo.Country = kvp.Value;
                    break;
                case "user":
                    Info.LoginInfo.User = kvp.Value;
                    break;
                case "wk":
                    Info.LoginInfo.Wk = kvp.Value;
                    break;
                case "tankIDName":
                    Info.LoginInfo.TankIdName = kvp.Value;
                    break;
                case "tankIDPass":
                    Info.LoginInfo.TankIdPass = kvp.Value;
                    break;
            }
        }
    }

    private void GetToken()
    {
        if (IsTokenStillValid())
        {
            return;
        }
        
        LogInfo("Getting token for bot");
        var token = "";

        switch (Info.LoginMethod)
        {
            case ELoginMethod.Legacy:
                break;
            default:
                LogError("Invalid login method");
                break;
        }
        
        if (token.Length == 0)
        {
            LogError("Failed to get token");
            return;
        }
        
        Info.Token = token;
    }

    private bool IsTokenStillValid()
    {
        LogInfo("Checking if token is still valid");

        while (true)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "UbiServices_SDK_2022.Release.9_PC64_ansi_static");
                    var content = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("refreshToken", Info.Token),
                        new KeyValuePair<string, string>("clientData", Info.LoginInfo.ToString())
                    });

                    var response = client
                        .PostAsync(
                            "https://login.growtopiagame.com/player/growid/checktoken?valKey=40db4045f2d8c572efe8c4a060605726",
                            content).Result;

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        LogError("Failed to refresh token, retrying...");
                        Thread.Sleep(1000);
                        continue;
                    }

                    var responseText = response.Content.ReadAsStringAsync().Result;
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseText);

                    if (jsonResponse.GetProperty("status").GetString() == "success")
                    {
                        var newToken = jsonResponse.GetProperty("token").GetString();
                        LogInfo($"Token is still valid | new token: {newToken}");

                        if (newToken == null)
                        {
                            LogError("Token is invalid");
                            return false;
                        }

                        Info.Token = newToken;
                        return true;
                    }

                    LogError("Token is invalid");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogError($"Request error: {ex.Message}, retrying...");
                Thread.Sleep(1000);
            }
        }
    }

    public void Reconnect()
    {
        ToHttp();
        Info.LoginInfo.Meta = Info.ServerData["meta"];

        if (Info.LoginMethod != ELoginMethod.Steam && Info.OauthLinks.Count == 0)
        {
            Info.OauthLinks = GetOauthLinks();
        }

        GetToken();
        ConnectToServer(Info.ServerData["server"], int.Parse(Info.ServerData["port"]));
    }

    public List<string> GetOauthLinks()
    {
        LogInfo("Getting OAuth links");

        while (true)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "USER_AGENT");
                    var content = new StringContent(WebUtility.UrlEncode(Info.LoginInfo.ToString()),
                        System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
                    var response = client.PostAsync("https://login.growtopiagame.com/player/login/dashboard", content)
                        .Result;

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        Log.Warning("Failed to get OAuth links");
                        Thread.Sleep(1000);
                        continue;
                    }

                    var body = response.Content.ReadAsStringAsync().Result;
                    var pattern =
                        new Regex(
                            @"https://login\.growtopiagame\.com/(apple|google|player/growid)/(login|redirect)\?token=[^""]+");
                    var matches = pattern.Matches(body);

                    var links = new List<string>();
                    foreach (Match match in matches)
                    {
                        links.Add(match.Value);
                    }

                    LogInfo("Successfully got OAuth links");
                    return links;
                }
            }
            catch (Exception ex)
            {
                LogError($"Request error: {ex.Message}, retrying...");
                Thread.Sleep(1000);
            }
        }
    }

    private void PollEvent()
    {
        while (true)
        {
            if (!State.IsRunning)
            {
                break;
            }

            if (State.IsRedirecting)
            {
                LogInfo("Redirecting to " + Server.Host + ":" + Server.Port);
            }
            else
            {
                Reconnect();
            }

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

    public void Spoof()
    {
        LogInfo("Spoofing bot data");
        Info.LoginInfo.Klv =
            Proton.GenerateKlv(Info.LoginInfo.Protocol, Info.LoginInfo.GameVersion, Info.LoginInfo.Rid);
        Info.LoginInfo.Hash = Proton.HashString(string.Format("{0}RT", Info.LoginInfo.Mac)).ToString();
        Info.LoginInfo.Hash = Proton.HashString(string.Format("{0}RT", utils.Random.Hex(16, true))).ToString();
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
        LogInfo("Connecting to " + ip + ":" + port);
        var remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        Peer = Host.Connect(remoteEndPoint, 2, 0);
    }

    private void ToHttp()
    {
        while (true)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post,
                    "https://www.growtopia1.com/growtopia/server_data.php");
                request.Headers.Add("User-Agent", "UbiServices_SDK_2022.Release.9_PC64_ansi_static");

                var response = client.Send(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    LogError("Failed to connect to the server: " + response);
                    Thread.Sleep(1000);
                    continue;
                }

                var body = response.Content.ReadAsStringAsync().Result;
                Info.ServerData = TextParse.ParseServerData(body);
                break;
            }
            catch (Exception e)
            {
                LogError("Failed to connect to the server: " + e);
                Thread.Sleep(1000);
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