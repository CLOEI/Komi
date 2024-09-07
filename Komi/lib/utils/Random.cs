namespace Komi.lib.utils;

public class Random
{
    public static string MacAddress()
    {
        var mac = new byte[6];
        new System.Random().NextBytes(mac);
        return string.Join(":", mac.Select(b => b.ToString("X2")));
    }

    public static string Hex(int length, bool upperCase = false)
    {
        var chars = "0123456789abcdef";
        if (upperCase)
            chars = chars.ToUpper();

        var random = new System.Random();
        var result = new char[length];
        for (var i = 0; i < length; i++)
            result[i] = chars[random.Next(chars.Length)];

        return new string(result);
    }
}