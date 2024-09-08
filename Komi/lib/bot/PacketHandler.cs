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
                        $"UUIDToken|{loginInfo.Uuid}\\nprotocol|{loginInfo.Protocol}\\nfhash|{loginInfo.FHash}\\nmac|{loginInfo.Mac}\\nrequestedName|{loginInfo.RequestedName}\\nhash2|{loginInfo.Hash2}\\nfz|{loginInfo.Fz}\\nf|{loginInfo.F}\\nplayer_age|{loginInfo.PlayerAge}\\ngame_version|{loginInfo.GameVersion}\\nlmode|{loginInfo.LMode}\\ncbits|{loginInfo.CBits}\\nrid|{loginInfo.Rid}\\nGDPR|{loginInfo.Gdpr}\\nhash|{loginInfo.Hash}\\ncategory|{loginInfo.Category}\\ntoken|{loginInfo.Token}\\ntotal_playtime|{loginInfo.TotalPlaytime}\\ndoor_id|{loginInfo.DoorId}\\nklv|{loginInfo.Klv}\\nmeta|{loginInfo.Meta}\\nplatformID|{loginInfo.PlatformId}\\ndeviceVersion|{loginInfo.DeviceVersion}\\nzf|{loginInfo.Zf}\\ncountry|{loginInfo.Country}\\nuser|{loginInfo.User}\\nwk|{loginInfo.Wk}\\n";
                    bot.SendPacket(EPacketType.NetMessageGenericText, message);
                }
                else
                {
                    var message = $"protocol|{loginInfo.Protocol}\\nltoken|{bot.Info.Token}\\nplatformID|{loginInfo.PlatformId}\\n";
                    bot.SendPacket(EPacketType.NetMessageGenericText, message);
                }
                break;
            }
        }
    }
}