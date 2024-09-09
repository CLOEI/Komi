using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
                    var message =
                        $"protocol|{loginInfo.Protocol}\\nltoken|{bot.Info.Token}\\nplatformID|{loginInfo.PlatformId}\\n";
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

                if (message.Contains("UPDATE REQUIRED"))
                {
                    var regex = new Regex(@"\$V(\d+\.\d+)");
                    var match = regex.Match(message);
                    if (match.Success)
                    {
                        var version = match.Groups[1].Value;
                        bot.LogWarning($"Update required: {version}, updating...");

                        bot.Info.LoginInfo.GameVersion = version;

                        utils.Config.EditGameVersion(version);
                        utils.Config.SaveTokenToBot(bot.Info.Payload[0], string.Empty, string.Empty);
                    }
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
                    case ETankPacketType.NetGamePacketSendInventoryState:
                        bot.Inventory.Parse(data.ToArray()[56..]);
                        break;
                    case ETankPacketType.NetGamePacketSendMapData:
                    {
                        using (var fileStream = File.Create("world.dat"))
                        {
                            fileStream.Write(data[56..]);
                        }

                        bot.LogWarning("Created world.dat");
                        bot.World.Parse(data.ToArray()[56..]);
                        break;
                    }
                    case ETankPacketType.NetGamePacketPingRequest:
                    {
                        var packet = new TankPacket()
                        {
                            Type = ETankPacketType.NetGamePacketPingReply,
                            VectorX = 64.0f,
                            VectorY = 64.0f,
                            VectorX2 = (int)1000.0,
                            VectorY2 = (int)250.0,
                            Value = tankPacket.Value,
                            Unk4 = utils.Proton.HashString(tankPacket.Value.ToString())
                        };
                        bot.SendPacketRaw(packet);
                        break;
                    }
                }

                break;
            }
        }
    }
}