namespace Komi.lib.types;

public class Player
{
    public string Type { get; set; } // only local player have this field
    public string Avatar { get; set; }
    public int NetId { get; set; } // netid
    public string OnlineId { get; set; } // onlineID
    public string EId { get; set; }
    public string Ip { get; set; }
    public string Colrect { get; set; }
    public string TitleIcon { get; set; } // titleIcon
    public uint MState { get; set; }
    public uint UserId { get; set; } // userID
    public bool Invisible { get; set; } // could be mods?
    public string Name { get; set; }
    public string Country { get; set; }
    public Vector2 Position { get; set; } // posXY
}