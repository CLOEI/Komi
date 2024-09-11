using Komi.lib.bot;
using Komi.lib.itemdatabase;
using Komi.lib.types;

namespace Komi.lib;

public class Manager
{
    private List<Bot> Bots { get; set; } = new();

    public ItemDatabase ItemDatabase { get; set; } =
        ItemDatabaseLoader.LoadFromFile(Directory.GetCurrentDirectory() + "/items.dat");

    public void AddBot(BotConfig config)
    {
        var bot = new Bot(config, ItemDatabase);
        var thread = new Thread(() =>
        {
            try
            {
                bot.Logon(config.Data);
            }
            catch (Exception ex)
            {
                bot.LogError($"An error occurred: {ex.Message}");
            }
        })
        {
            IsBackground = true
        };
        thread.Start();

        Bots.Add(bot);
    }

    public void RemoveBot(string username)
    {
        var bot = GetBot(username);
        if (bot == null)
        {
            return;
        }

        bot.State.IsRunning = false;
        bot.Disconnect();
        Bots.Remove(bot);
    }

    public Bot? GetBot(string username)
    {
        return Bots.FirstOrDefault(bot => bot.Info.Payload[0] == username);
    }
}