using System.Text;

namespace Komi.lib.types;

public class LoginInfo
{
    public string Uuid { get; set; }
    public string TankIdName { get; set; }
    public string TankIdPass { get; set; }
    public string Protocol { get; set; }
    public string FHash { get; set; }
    public string Mac { get; set; }
    public string RequestedName { get; set; }
    public string Hash2 { get; set; }
    public string Fz { get; set; }
    public string F { get; set; }
    public string PlayerAge { get; set; }
    public string GameVersion { get; set; }
    public string LMode { get; set; }
    public string CBits { get; set; }
    public string Rid { get; set; }
    public string Gdpr { get; set; }
    public string Hash { get; set; }
    public string Category { get; set; }
    public string Token { get; set; }
    public string TotalPlaytime { get; set; }
    public string DoorId { get; set; }
    public string Klv { get; set; }
    public string Meta { get; set; }
    public string PlatformId { get; set; }
    public string DeviceVersion { get; set; }
    public string Zf { get; set; }
    public string Country { get; set; }
    public string User { get; set; }
    public string Wk { get; set; }
    
    public LoginInfo()
    {
        Uuid = string.Empty;
        TankIdName = string.Empty;
        TankIdPass = string.Empty;
        Protocol = "209";
        FHash = "-716928004";
        Mac = utils.Random.MacAddress();
        RequestedName = "BraveDuck";
        Hash2 = string.Empty;
        Fz = "47142936";
        F = "1";
        PlayerAge = "20";
        GameVersion = "4.64";
        LMode = "1";
        CBits = "1040";
        Rid = utils.Random.Hex(32, true);
        Gdpr = "3";
        Hash = "0";
        Category = "_-5100";
        Token = string.Empty;
        TotalPlaytime = "0";
        DoorId = string.Empty;
        Klv = string.Empty;
        Meta = string.Empty;
        PlatformId = "0,1,1";
        DeviceVersion = "0";
        Zf = "-821693372";
        Country = "jp";
        User = string.Empty;
        Wk = utils.Random.Hex(32, true);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"fhash|{FHash}\n");
        sb.Append($"mac|{Mac}\n");
        sb.Append($"requestedName|{RequestedName}\n");
        sb.Append($"hash2|{Hash2}\n");
        sb.Append($"fz|{Fz}\n");
        sb.Append($"f|{F}\n");
        sb.Append($"player_age|{PlayerAge}\n");
        sb.Append($"game_version|{GameVersion}\n");
        sb.Append($"lmode|{LMode}\n");
        sb.Append($"cbits|{CBits}\n");
        sb.Append($"rid|{Rid}\n");
        sb.Append($"gdpr|{Gdpr}\n");
        sb.Append($"hash|{Hash}\n");
        sb.Append($"category|{Category}\n");
        sb.Append($"token|{Token}\n");
        sb.Append($"totalPlaytime|{TotalPlaytime}\n");
        sb.Append($"doorID|{DoorId}\n");
        sb.Append($"klv|{Klv}\n");
        sb.Append($"meta|{Meta}\n");
        sb.Append($"platformID|{PlatformId}\n");
        sb.Append($"deviceVersion|{DeviceVersion}\n");
        sb.Append($"zf|{Zf}\n");
        sb.Append($"country|{Country}\n");
        sb.Append($"user|{User}\n");
        sb.Append($"wk|{Wk}\n");
        return sb.ToString();
    }
}