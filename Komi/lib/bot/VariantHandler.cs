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
        }
    }
}