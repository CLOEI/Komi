namespace Komi.lib.utils;

public class TextParse
{
    public static Dictionary<String, String> ParseServerData(string data)
    {
        var serverData = data
            .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(line => line.Split(new[] { '|' }, 2))
            .Where(parts => parts.Length == 2)
            .ToDictionary(parts => parts[0], parts => parts[1]);
           
        return serverData;
    }
    
    public static List<string> ParseAndStoreAsList(string input)
    {
        return input.Split('|').ToList();
    }

    public static Dictionary<string, string> ParseAndStoreAsDic(string input)
    {
        var map = new Dictionary<string, string>();
        var lines = input.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            var parts = line.Split('|');
            if (parts.Length >= 2)
            {
                var key = parts[0];
                var value = string.Join("|", parts.Skip(1));
                map[key] = value;
            }
        }

        return map;
    }
    
    public static string FormatByteAsSteamToken(byte[] data)
    {
        return string.Join("+", data.Select(b => b.ToString("x2")));
    }
}