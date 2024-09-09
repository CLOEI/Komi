using System.Text;
using System.Text.Json;
using Komi.lib.types;

namespace Komi.lib.utils;

public class Config
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };
    
    private static types.Config ParseConfig()
    {
        using var reader = File.OpenText("config.json");
        var config = JsonSerializer.Deserialize<types.Config>(reader.ReadToEnd());
        return config;
    }

    public static void AddBot(BotConfig botConfig)
    {
        var config = ParseConfig();
        config.Bots.Add(botConfig);
        var jsonString = JsonSerializer.Serialize(config, Options);
        using var writer = File.Create("config.json");
        writer.Write(Encoding.UTF8.GetBytes(jsonString));
    }
    
    public static List<BotConfig> GetBots()
    {
        var config = ParseConfig();
        return config.Bots;
    }

    public static void RemoveBot(string username)
    {
        var config = ParseConfig();
        for (var i = 0; i < config.Bots.Count; i++)
        {
            var payload = TextParse.ParseAndStoreAsList(config.Bots[i].Payload);
            if (payload[0] != username) continue;
            config.Bots.RemoveAt(i);
        }
    }
    
    public static uint GetTimeout()
    {
        var config = ParseConfig();
        return config.Timeout;
    }
    
    public static void EditTimeout(uint timeout)
    {
        var config = ParseConfig();
        config.Timeout = timeout;
        var jsonString = JsonSerializer.Serialize(config, Options);
        using var writer = File.Create("config.json");
        writer.Write(Encoding.UTF8.GetBytes(jsonString));
    }
    
    public static void EditFindPathDelay(uint findPathDelay)
    {
        var config = ParseConfig();
        config.FindPathDelay = findPathDelay;
        var jsonString = JsonSerializer.Serialize(config, Options);
        using var writer = File.Create("config.json");
        writer.Write(Encoding.UTF8.GetBytes(jsonString));
    }
    
    public static uint GetFindPathDelay()
    {
        var config = ParseConfig();
        return config.FindPathDelay;
    }
    
    public static string GetSelectedBot()
    {
        var config = ParseConfig();
        return config.SelectedBot;
    }
    
    public static string GetGameVersion()
    {
        var config = ParseConfig();
        return config.GameVersion;
    }
    
    public static void EditGameVersion(string gameVersion)
    {
        var config = ParseConfig();
        config.GameVersion = gameVersion;
        var jsonString = JsonSerializer.Serialize(config, Options);
        using var writer = File.Create("config.json");
        writer.Write(Encoding.UTF8.GetBytes(jsonString));
    }
    
    public static void SaveTokenToBot(string username, string token, string data)
    {
        var config = ParseConfig();
        for (var i = 0; i < config.Bots.Count; i++)
        {
            var payload = TextParse.ParseAndStoreAsList(config.Bots[i].Payload);
            if (payload[0] != username) continue;
            var bot = config.Bots[i];
            bot.Token = token;
            bot.Data = data;
            config.Bots[i] = bot;
        }
        var jsonString = JsonSerializer.Serialize(config, Options);
        using var writer = File.Create("config.json");
        writer.Write(Encoding.UTF8.GetBytes(jsonString));
    }
}