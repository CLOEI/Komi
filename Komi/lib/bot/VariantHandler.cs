using Komi.lib.types;
using Komi.lib.utils;

namespace Komi.lib.bot;

public class VariantHandler
{
    public static void Handle(Bot bot, Span<byte> data)
    {
        var variant = VariantList.Deserialize(data.ToArray());
        var function = variant.Get(0).AsString();
        bot.LogInfo($"Received function call: {function}");

        switch (function)
        {
            case "OnSendToServer":
            {
                var port = variant.Get(1).AsInt32();
                var token = variant.Get(2).AsInt32();
                var userId = variant.Get(3).AsInt32();
                var serverData = variant.Get(4).AsString();
                var parsedServerData = TextParse.ParseAndStoreAsList(serverData);

                bot.State.IsRedirecting = true;
                bot.Server.Host = parsedServerData[0];
                bot.Server.Port = port;
                bot.Info.LoginInfo.Token = token.ToString();
                bot.Info.LoginInfo.User = userId.ToString();
                bot.Info.LoginInfo.DoorId = parsedServerData[1];
                bot.Info.LoginInfo.Uuid = parsedServerData[2];
                
                bot.Disconnect();
                break;
            }
            case "OnSuperMainStartAcceptLogonHrdxs47254722215a":
            {
                bot.SendPacket(EPacketType.NetMessageGenericText, "action|enter_game\n");
                bot.State.IsRedirecting = false;
                break;
            }
            case "OnConsoleMessage":
            {
                var message = variant.Get(1).AsString();
                bot.LogInfo($"Received console message: {message}");
                break;
            }
            case "OnDialogRequest":
            {
                var dialog = variant.Get(1).AsString();
                if (dialog.Contains("Gazette"))
                {
                    bot.SendPacket(EPacketType.NetMessageGenericText, "action|dialog_return\ndialog_name|gazette\nbuttonClicked|banner\n");
                }

                break;
            }
            case "OnSetBux":
            {
                var bux = variant.Get(1).AsInt32();
                bot.State.Gems = bux;
                break;
            }
            case "OnSetPos":
            {
                var (x, y) = variant.Get(1).AsVec2();
                bot.Position.X = x;
                bot.Position.Y = y;
                break;
            }
            case "SetHasGrowID":
            {
                var growid = variant.Get(2).AsString();
                bot.Info.LoginInfo.TankIdName = growid;
                utils.Config.SaveTokenToBot(bot.Info.Username, bot.Info.Token, bot.Info.LoginInfo.ToString());
                break;
            }
            case "OnFtueButtonDataSet":
            {
                var unknown = variant.Get(1).AsInt32();
                var currentProgress = variant.Get(2).AsInt32();
                var totalProgress = variant.Get(3).AsInt32();
                var info = variant.Get(4).AsString();
                
                bot.LogInfo($"Received FTUE button data set: {unknown} {currentProgress} {totalProgress} {info}");
                
                bot.Ftue.CurrentProgress = currentProgress;
                bot.Ftue.TotalProgress = totalProgress;
                bot.Ftue.Info = info;
                break;
            }
            case "OnTalkBubble":
            {
                var message = variant.Get(1).AsString();
                bot.LogInfo($"Received talk bubble: {message}");
                break;
            }
        }
    }
}