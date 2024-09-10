using Komi.lib.types;

namespace Komi.lib.bot.features;

public class AutoTutorial
{
    public static void LockTheWorld(Bot bot)
    {
        var thread = new Thread(() =>
        {
            bot.SendPacket(EPacketType.NetMessageGenericText, "ftue_start_popup_close`");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            bot.Place(0, -1, 9640); // 9640 = newbie world lock
        })
        {
            IsBackground = true
        };
        thread.Start();
    }
}