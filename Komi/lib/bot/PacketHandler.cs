using System.Runtime.InteropServices;
using System.Text;
using Komi.lib.types;

namespace Komi.lib.bot;

public class PacketHandler
{
    public static void Handle(Bot bot, EPacketType packetType, Span<byte> data)
    {
        switch (packetType)
        {
            case EPacketType.NetMessageServerHello:
            {
                var isRedirecting = bot.State.IsRedirecting;
                var loginInfo = bot.Info.LoginInfo;
                if (isRedirecting)
                {
                    var message =
                        $"UUIDToken|{loginInfo.Uuid}\nprotocol|{loginInfo.Protocol}\nfhash|{loginInfo.FHash}\nmac|{loginInfo.Mac}\nrequestedName|{loginInfo.RequestedName}\nhash2|{loginInfo.Hash2}\nfz|{loginInfo.Fz}\nf|{loginInfo.F}\nplayer_age|{loginInfo.PlayerAge}\ngame_version|{loginInfo.GameVersion}\nlmode|{loginInfo.LMode}\ncbits|{loginInfo.CBits}\nrid|{loginInfo.Rid}\nGDPR|{loginInfo.Gdpr}\nhash|{loginInfo.Hash}\ncategory|{loginInfo.Category}\ntoken|{loginInfo.Token}\ntotal_playtime|{loginInfo.TotalPlaytime}\ndoor_id|{loginInfo.DoorId}\nklv|{loginInfo.Klv}\nmeta|{loginInfo.Meta}\nplatformID|{loginInfo.PlatformId}\ndeviceVersion|{loginInfo.DeviceVersion}\nzf|{loginInfo.Zf}\ncountry|{loginInfo.Country}\nuser|{loginInfo.User}\nwk|{loginInfo.Wk}\n";
                    bot.SendPacket(EPacketType.NetMessageGenericText, message);
                }
                else
                {
                    var message = $"protocol|{loginInfo.Protocol}\\nltoken|{bot.Info.Token}\\nplatformID|{loginInfo.PlatformId}\\n";
                    bot.SendPacket(EPacketType.NetMessageGenericText, message);
                }
                break;
            }
            case EPacketType.NetMessageGameMessage:
            {
                var message = Encoding.UTF8.GetString(data);
                bot.LogInfo($"Received game message: {message}");
                
                if (message.Contains("logon_fail"))
                {
                    bot.State.IsRedirecting = false;
                    bot.Disconnect();
                }
                if (message.Contains("currently banned"))
                {
                    bot.State.IsRunning = false;
                    bot.State.IsBanned = true;
                    bot.Disconnect();
                }
                if (message.Contains("Advanced Account Protection"))
                {
                    bot.State.IsRunning = false;
                    bot.Disconnect();
                }
                if (message.Contains("temporarily suspended"))
                {
                    bot.State.IsRunning = false;
                    bot.Disconnect();
                }
                if (message.Contains("has been suspended"))
                {
                    bot.State.IsRunning = false;
                    bot.State.IsBanned = true;
                    bot.Disconnect();
                }
                
                break;
            }
            case EPacketType.NetMessageGamePacket:
            {
                var tankPacket = MemoryMarshal.Read<TankPacket>(data);
                bot.LogInfo($"Received game packet: {tankPacket.Type}");

                switch (tankPacket.Type)
                {
                    case ETankPacketType.NetGamePacketCallFunction:
                        VariantHandler.Handle(bot, data[56..]);
                        break;
                }
                
                break;
            }
        }
    }
}