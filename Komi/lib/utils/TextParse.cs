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
}