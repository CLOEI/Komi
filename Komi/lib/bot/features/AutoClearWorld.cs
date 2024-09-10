namespace Komi.lib.bot.features;

public class AutoClearWorld
{
    private const int CaveBackground = 14;
    private const int Bedrock = 8;

    public static void Start(Bot bot)
    {
        if (!bot.IsInWorld() || bot.World == null) return;
        var worldWidth = bot.World.Width;
        var worldHeight = bot.World.Height;

        for (var y = 23; y < worldHeight - 6; y++)
        {
            for (var x = 0; x < worldWidth; x++)
            {
                while (bot.IsInWorld())
                {
                    int foregroundId, backgroundId;
                    var tile = bot.World.GetTile((uint)x, (uint)y);
                    if (tile != null)
                    {
                        foregroundId = tile.ForegroundItemId;
                        backgroundId = tile.BackgroundItemId;
                    }
                    else
                    {
                        break;
                    }

                    if (backgroundId != CaveBackground || foregroundId == Bedrock)
                    {
                        break;
                    }

                    bot.FindPath((uint)x, (uint)(y - 1));
                    bot.Punch(0, 1);
                    Thread.Sleep(250);
                }
            }
        }
    }
}