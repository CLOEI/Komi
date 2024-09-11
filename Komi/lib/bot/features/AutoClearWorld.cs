namespace Komi.lib.bot.features;

public class AutoClearWorld
{
    private const int CaveBackground = 14;
    private const int Bedrock = 8;

    public static void Start(Bot bot)
    {
        if (!bot.IsInWorld() || bot.World == null) return;

        for (var y = 23; y < 54; y++)
        {
            for (var x = 0; x <= 99; x++)
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

                    if (!(bot.Position.X / 32.0f == x && bot.Position.Y / 32.0f == y))
                    {
                        bot.FindPath((uint)x, (uint)(y - 1));
                    }
                    bot.Punch(0, 1);
                    Thread.Sleep(250);
                }
            }
        }
        Console.WriteLine("Cleared world!");
    }
}