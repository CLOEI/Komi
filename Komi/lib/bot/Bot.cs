using System.Buffers.Binary;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ENet.Managed;
using Komi.lib.types;
using Komi.lib.types.botinfo;
using Komi.lib.utils;
using Komi.lib.world;
using Serilog;
using Serilog.Core;
using ItemDatabase = Komi.lib.itemdatabase.ItemDatabase;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Komi.lib.bot;

public class Bot
{
    public Info Info { get; set; }
    public State State { get; set; }
    public Server Server { get; set; }
    public Vector2 Position { get; set; }
    public FTUE Ftue { get; set; }
    private ENetHost Host { get; set; } = null!;
    private ENetPeer Peer { get; set; }
    private Logger Log { get; set; }
    public ItemDatabase ItemDatabase { get; set; }
    public Inventory Inventory { get; set; }
    public World? World { get; set; }
    public AStar? AStar { get; set; }
    public List<Player> Players { get; set; } = [];

    public Bot(BotConfig config, ItemDatabase itemDatabase)
    {
        Log = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        var payload = TextParse.ParseAndStoreAsList(config.Payload);

        Info = new Info
        {
            Payload = payload,
            LoginMethod = config.LoginMethod,
            Token = config.Token,
            LoginInfo = new LoginInfo(),
            Ping = 0,
            Status = "Offline",
        };
        State = new State();
        Server = new Server();
        Position = new Vector2();
        Ftue = new FTUE();
        ItemDatabase = itemDatabase;
        Inventory = new Inventory();
        World = new World(itemDatabase);
        AStar = new AStar(itemDatabase);
    }

    private void ValidateLoginPayload()
    {
        int requiredPayloadCount = Info.LoginMethod == ELoginMethod.Steam ? 4 : 2;

        if (Info.Payload.Count < requiredPayloadCount)
        {
            LogError("Invalid login payload");
            throw new Exception("Invalid login payload"); // For a moment, let's just throw an exception
        }
    }

    public void SetStatus(string message)
    {
        Info.Status = message;
    }

    public void Logon(string? data)
    {
        SetStatus("logging in...");
        ValidateLoginPayload();

        if (string.IsNullOrEmpty(data))
        {
            Spoof();
        }
        else
        {
            UpdateLoginInfo(data);
        }

        State.IsRunning = true;
        CreateHost();
        Poll();
        PollEvent();
    }

    private void UpdateLoginInfo(string data)
    {
        SetStatus("Updating login info");
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

        SetStatus("Getting token");
        LogInfo("Getting token for bot");
        var token = "";

        switch (Info.LoginMethod)
        {
            case ELoginMethod.Legacy:
                token = Login.GetLegacyToken(Info.OauthLinks[2], Info.Payload[0], Info.Payload[1]);
                break;
            case ELoginMethod.Steam:
                Info.LoginInfo.PlatformId = "15,1,0";
                token = Login.GetUbisoftToken(Info.LoginInfo.ToString(), Info.Payload[0], Info.Payload[1],
                    Info.Payload[2], Info.Payload[3]);
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

        LogInfo("Token received: " + token);
        Info.Token = token;
    }

    private bool IsTokenStillValid()
    {
        SetStatus("Checking refresh token");
        LogInfo("Checking if token is still valid");
        if (string.IsNullOrEmpty(Info.Token)) return false;

        while (true)
        {
            try
            {
                using var client = new HttpClient();
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
                    Sleep();
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
            catch (Exception ex)
            {
                LogError($"Request error: {ex.Message}, retrying...");
                Sleep();
            }
        }
    }

    public void Reconnect()
    {
        if (!Peer.IsNull)
        {
            Disconnect();
        }
        SetStatus("Reconnecting...");
        ToHttp();
        Info.LoginInfo.Meta = Info.ServerData["meta"];

        if (Info.LoginMethod != ELoginMethod.Steam && Info.OauthLinks.Count == 0)
        {
            Info.OauthLinks = GetOauthLinks();
        }

        GetToken();
        ConnectToServer(Info.ServerData["server"], int.Parse(Info.ServerData["port"]));
    }

    private void Poll()
    {
        var thread = new Thread(() =>
        {
            while (State.IsRunning)
            {
                try
                {
                    if (!Peer.IsNull)
                    {
                        Info.Ping = Peer.RoundTripTime;
                        Collect(this);
                    }
                }
                catch (Exception)
                {
                    Info.Ping = 0;
                }

                Thread.Sleep(TimeSpan.FromMilliseconds(500));
            }
        })
        {
            IsBackground = true
        };
        thread.Start();
    }

    private List<string> GetOauthLinks()
    {
        SetStatus("Getting OAuth links");
        LogInfo("Getting OAuth links");

        while (true)
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36 Edg/128.0.0.0");
                var content = new StringContent(WebUtility.UrlEncode(Info.LoginInfo.ToString()),
                    Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = client.PostAsync("https://login.growtopiagame.com/player/login/dashboard", content)
                    .Result;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    Log.Warning("Failed to get OAuth links");
                    Sleep();
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
            catch (Exception ex)
            {
                LogError($"Request error: {ex.Message}, retrying...");
                Sleep();
            }
        }
    }

    private void PollEvent()
    {
        while (State.IsRunning)
        {
            if (State.IsRedirecting)
            {
                LogInfo("Redirecting to " + Server.Host + ":" + Server.Port);
                ConnectToServer(Server.Host, Server.Port);
            }
            else
            {
                Reconnect();
            }

            while (true)
            {
                var enetEvent = Host.Service(TimeSpan.FromMilliseconds(100));
                switch (enetEvent.Type)
                {
                    case ENetEventType.None:
                        continue;
                    case ENetEventType.Connect:
                        SetStatus("Online");
                        LogInfo("Connected to the server");
                        continue;
                    case ENetEventType.Receive:
                        var packetData = enetEvent.Packet.Data;
                        if (packetData.Length < 4)
                        {
                            continue;
                        }

                        var packetId = BinaryPrimitives.ReadInt32LittleEndian(packetData);
                        var packetType = (EPacketType)packetId;
                        LogInfo($"Received packet type: {packetType}");
                        PacketHandler.Handle(this, packetType, packetData[4..]);

                        enetEvent.Packet.Destroy();
                        continue;
                    case ENetEventType.Disconnect:
                        SetStatus("Offline");
                        LogInfo("Disconnected from the server");
                        break;
                }

                break;
            }
        }
    }

    public void Spoof()
    {
        SetStatus("Spoofing bot data");
        LogInfo("Spoofing bot data");
        Info.LoginInfo.Klv =
            Proton.GenerateKlv(Info.LoginInfo.Protocol, Info.LoginInfo.GameVersion, Info.LoginInfo.Rid);
        Info.LoginInfo.Hash = Proton.HashString($"{Info.LoginInfo.Mac}RT").ToString();
        Info.LoginInfo.Hash2 = Proton.HashString($"{utils.Random.Hex(16, true)}RT").ToString();
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
        SetStatus("Connecting to server");
        LogInfo("Connecting to " + ip + ":" + port);
        var remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        Peer = Host.Connect(remoteEndPoint, 2, 0);
    }

    private void ToHttp()
    {
        SetStatus("Fetching server data");
        while (true)
        {
            try
            {
                using var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post,
                    "https://www.growtopia1.com/growtopia/server_data.php");
                request.Headers.Add("User-Agent", "UbiServices_SDK_2022.Release.9_PC64_ansi_static");

                var response = client.Send(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    LogError("Failed to connect to the server, retrying...");
                    Sleep();
                    continue;
                }

                var body = response.Content.ReadAsStringAsync().Result;
                Info.ServerData = TextParse.ParseServerData(body);
                break;
            }
            catch (Exception)
            {
                LogError("Failed to connect to the server, retrying...");
                Sleep();
            }
        }
    }

    public void SendPacket(EPacketType packetType, string message)
    {
        if (Peer.IsNull) return;
        try
        {
            var packetTypeByte = BitConverter.GetBytes((int)packetType);
            var data = Encoding.ASCII.GetBytes(message);

            var packetData = new byte[packetTypeByte.Length + data.Length];
            packetTypeByte.CopyTo(packetData, 0);
            data.CopyTo(packetData, packetTypeByte.Length);

            Peer.Send(0, packetData, ENetPacketFlags.Reliable);
        }
        catch (ENetException ex)
        {
            LogError($"Failed to send packet: {ex.Message}");
            throw;
        }
    }

    public void SendPacketRaw(TankPacket packet)
    {
        if (Peer.IsNull) return;
        try
        {
            // sizeof(int) is the size of EPacketType
            var packetSize = sizeof(int) + Marshal.SizeOf(typeof(TankPacket)) + (int)packet.ExtendedDataLength;
            var enetPacketData = new byte[packetSize];

            const int packetType = (int)EPacketType.NetMessageGamePacket;
            var packetTypeBytes = BitConverter.GetBytes(packetType);
            Buffer.BlockCopy(packetTypeBytes, 0, enetPacketData, 0, packetTypeBytes.Length);

            var tankPacketBytes = Binary.StructToByteArray(packet);
            Buffer.BlockCopy(tankPacketBytes, 0, enetPacketData, packetTypeBytes.Length, tankPacketBytes.Length);

            Peer.Send(0, enetPacketData, ENetPacketFlags.Reliable);
        }
        catch (ENetException ex)
        {
            LogError($"Failed to send packet: {ex.Message}");
            throw;
        }
    }

    public static void Collect(Bot bot)
    {
        if (bot.World == null) return;
        if (!bot.IsInWorld())
        {
            return;
        }

        var botX = bot.Position.X;
        var botY = bot.Position.Y;
        var items = bot.World.Dropped.Items;

        foreach (var obj in items)
        {
            var dx = Math.Abs(botX - obj.X) / 32.0f;
            var dy = Math.Abs(botY - obj.Y) / 32.0f;
            var distance = Math.Sqrt(dx * dx + dy * dy);
            if (!(distance <= 5.0)) continue;
            var canCollect = true;

            if (bot.Inventory.Items.TryGetValue(obj.Id, out var item))
            {
                if (item.Amount >= 200) continue;
            }

            if (!canCollect) continue;
            var pkt = new TankPacket
            {
                Type = ETankPacketType.NetGamePacketItemActivateObjectRequest,
                VectorX = obj.X,
                VectorY = obj.Y,
                Value = obj.Uid
            };
            bot.SendPacketRaw(pkt);
            bot.LogInfo("Collect packet sent");
        }
    }

    public bool IsInWorld()
    {
        return World?.Name != "EXIT";
    }

    public void Place(int offsetX, int offsetY, uint itemId)
    {
        var packet = new TankPacket()
        {
            Type = ETankPacketType.NetGamePacketTileChangeRequest,
            VectorX = Position.X,
            VectorY = Position.Y,
            IntX = (int)(Math.Floor(Position.X / 32.0) + offsetX),
            IntY = (int)(Math.Floor(Position.Y / 32.0) + offsetY),
            Value = itemId
        };

        var (xPos, yPos) = ((int)(Position.X / 32.0), (int)(Position.Y / 32.0));

        if (packet.IntX <= xPos + 4 && packet.IntX >= xPos - 4 && packet.IntY <= yPos + 4 && packet.IntY >= yPos - 4)
        {
            SendPacketRaw(packet);
        }
    }

    public void Punch(int offsetX, int offsetY)
    {
        Place(offsetX, offsetY, 18);
    }

    public void FindPath(uint x, uint y)
    {
        if (AStar == null)
        {
            LogError("AStar is not initialized");
            return;
        }

        var paths = AStar.FindPath((uint)(Position.X / 32.0), (uint)(Position.Y / 32.0), x, y);
        var delay = utils.Config.GetFindPathDelay();

        foreach (var path in paths)
        {
            Position.X = path.X * 32f;
            Position.Y = path.Y * 32f;
            Walk((int)path.X, (int)path.Y, true);
            Thread.Sleep(TimeSpan.FromMilliseconds(delay));
        }
    }

    public void Warp(string worldName)
    {
        SendPacket(EPacketType.NetMessageGameMessage, $"action|join_request\nname|{worldName}\ninvitedWorld|0\n");
    }

    public void Talk(string message)
    {
        SendPacket(EPacketType.NetMessageGameMessage, $"action|input\n|text|{message}\n");
    }

    public void Leave()
    {
        SendPacket(EPacketType.NetMessageGameMessage, $"action|quit_to_exit\n");
    }

    public void Walk(int x, int y, bool ap)
    {
        if (!ap)
        {
            Position.X += (x * 32);
            Position.Y += (y * 32);
        }

        var packet = new TankPacket()
        {
            Type = ETankPacketType.NetGamePacketState,
            VectorX = Position.X,
            VectorY = Position.Y,
            IntX = -1,
            IntY = -1,
            Flags = 0 | (1 << 1) | (1 << 5)
        };

        SendPacketRaw(packet);
    }

    public void Disconnect()
    {
        Peer.Disconnect(0);
    }

    public void Sleep()
    {
        Info.Timeout += utils.Config.GetTimeout();
        while (Info.Timeout > 0)
        {
            Info.Timeout--;
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
    }

    public void LogInfo(string message)
    {
        string username;
        username = string.IsNullOrEmpty(Info.LoginInfo.TankIdName) ? Info.Payload[0] : Info.LoginInfo.TankIdName;

        Log.Information("[{username}] {message}", username, message);
    }

    public void LogError(string message)
    {
        string username;
        username = string.IsNullOrEmpty(Info.LoginInfo.TankIdName) ? Info.Payload[0] : Info.LoginInfo.TankIdName;

        Log.Error("[{username}] {message}", username, message);
    }

    public void LogWarning(string message)
    {
        string username;
        username = string.IsNullOrEmpty(Info.LoginInfo.TankIdName) ? Info.Payload[0] : Info.LoginInfo.TankIdName;

        Log.Warning("[{username}] {message}", username, message);
    }
}