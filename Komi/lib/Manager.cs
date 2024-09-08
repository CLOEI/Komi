using Komi.lib.bot;
using Komi.lib.types;

namespace Komi.lib;

public class Manager
{
    private List<Bot> bots { get; set; } = new();
    
    public void AddBot(BotConfig config)
    {
        var bot = new Bot(config);
        var thread = new Thread(() => bot.Logon(null))
        {
            IsBackground = true
        };
        thread.Start();
        
        bots.Add(bot);
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
        bots.Remove(bot);
    }
    
    public Bot? GetBot(string username)
    {
        return bots.FirstOrDefault(bot => bot.Info.Username == username);
    }
}