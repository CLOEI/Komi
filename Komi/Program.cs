using System.Net;
using Serilog;
using ENet.Managed;

namespace Komi
{
    class Program
    {
        static void Main(string[] args)
        {
            using var log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            var serverData = ToHttp();
            var startupConfig = new ENetStartupOptions
            {
                ModulePath = Directory.GetCurrentDirectory() + "/enet.dll"
            };
            ManagedENet.Startup(startupConfig);

            IPEndPoint? listenEndPoint = null;
            var host = new ENetHost(listenEndPoint, 1, 2, 0, 0);
            host.ChecksumWithCRC32();
            host.CompressWithRangeCoder();
            host.UsingNewPacket();
            
            var ip = serverData.GetValueOrDefault("server");
            var port = serverData.GetValueOrDefault("port");
            if (ip == null || port == null)
            {
                log.Error("Failed to get server data");
                return;
            }
            log.Information("Connecting to {ip}:{port}", ip, port);
            IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
            var peer = host.Connect(remoteEndPoint, 2, 0);
            
            while (true)
            {
                var enetEvent = host.Service(TimeSpan.FromMilliseconds(100));
                switch (enetEvent.Type)
                {
                    case ENetEventType.None:
                        continue;
                    case ENetEventType.Connect:
                        log.Information("Connected to the server");
                        continue;
                    case ENetEventType.Receive:
                        log.Information("Received a packet from the server");
                        continue;
                    case ENetEventType.Disconnect:
                        log.Information("Disconnected from the server");
                        break;
                }
            }
        }

        static Dictionary<String, String> ToHttp()
        {
            while (true)
            {
                try
                {
                    var client = new HttpClient();
                    var request = new HttpRequestMessage(HttpMethod.Post, "https://www.growtopia1.com/growtopia/server_data.php");
                    request.Headers.Add("User-Agent", "UbiServices_SDK_2022.Release.9_PC64_ansi_static");

                    var response = client.Send(request);

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Console.WriteLine("Failed to connect to the server");
                        continue;
                    }

                    var body = response.Content.ReadAsStringAsync().Result;
                    return ParseServerData(body);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        static Dictionary<String, String> ParseServerData(string data)
        {
            var serverData = data
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Split(new[] { '|' }, 2))
                .Where(parts => parts.Length == 2)
                .ToDictionary(parts => parts[0], parts => parts[1]);
           
            return serverData;
        }
    }
}

