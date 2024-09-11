using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Komi.lib.types;
using Komi.lib.world;
using Serilog;

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
                    case ETankPacketType.NetGamePacketAppIntegrityFail:
                        break;
                    case ETankPacketType.NetGamePacketSendMapData:
                    {
                        using (var fileStream = File.Create("world.dat"))
                        {
                            fileStream.Write(data[56..]);
                        }

                        bot.LogWarning("Created world.dat");
                        bot.World?.Parse(data.ToArray()[56..]);
                        bot.AStar?.Update(bot.World);
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
                    case ETankPacketType.NetGamePacketTileChangeRequest:
                    {
                        var shouldUpdateInventory = bot.State.NetId == tankPacket.NetId && tankPacket.Value != 18;
                        if (shouldUpdateInventory)
                        {
                            if (bot.Inventory.Items.TryGetValue((ushort)tankPacket.Value, out InventoryItem item))
                            {
                                item.Amount--;
                                if (item.Amount is 0 or > 200)
                                {
                                    bot.Inventory.Items.Remove((ushort)tankPacket.Value);
                                }
                            }
                        }

                        var tile = bot.World?.GetTile((uint)tankPacket.IntX, (uint)tankPacket.IntY);
                        if (tile != null)
                        {
                            if (tankPacket.Value == 18)
                            {
                                if (tile.ForegroundItemId != 0)
                                {
                                    tile.ForegroundItemId = 0;
                                }
                                else
                                {
                                    tile.BackgroundItemId = 0;
                                }
                            }
                            else
                            {
                                var item = bot.ItemDatabase.GetItem(tankPacket.Value);
                                if (item.ActionType is 22 or 28 or 18)
                                {
                                    tile.BackgroundItemId = (ushort)tankPacket.Value;
                                }
                                else
                                {
                                    bot.LogInfo($"TileChangeRequest: {tankPacket}");
                                    tile.ForegroundItemId = (ushort)tankPacket.Value;
                                }
                            }
                        }
                        bot.AStar?.Update(bot.World);
                        break;
                    }
                    case ETankPacketType.NetGamePacketItemChangeObject:
                    {
                        if (tankPacket.NetId == uint.MaxValue)
                        {
                            var item = new DroppedItem()
                            {
                                Id = (ushort)tankPacket.Value,
                                X = (float)Math.Ceiling(tankPacket.VectorX),
                                Y = (float)Math.Ceiling(tankPacket.VectorY),
                                Count = (byte)tankPacket.Unk6,
                                Uid = bot.World!.Dropped.LastDroppedItemUid + 1
                            };

                            bot.World.Dropped.Items.Add(item);
                            bot.World.Dropped.LastDroppedItemUid += 1;
                            bot.World.Dropped.ItemsCount += 1;
                            return;
                        }

                        if (tankPacket.NetId == uint.MaxValue - 3)
                        {
                            foreach (var obj in bot.World.Dropped.Items)
                            {
                                if (obj.Id == (ushort)tankPacket.Value &&
                                    obj.X == (float)Math.Ceiling(tankPacket.VectorX) &&
                                    obj.Y == (float)Math.Ceiling(tankPacket.VectorY))
                                {
                                    obj.Count = (byte)tankPacket.Unk6;
                                    break;
                                }
                            }
                        }
                        else if (tankPacket.NetId > 0)
                        {
                            int? removeIndex = null;
                            for (int i = 0; i < bot.World.Dropped.Items.Count; i++)
                            {
                                var obj = bot.World.Dropped.Items[i];
                                if (obj.Uid == tankPacket.Value)
                                {
                                    if (tankPacket.NetId == bot.State.NetId)
                                    {
                                        if (obj.Id == 112)
                                        {
                                            bot.State.Gems += obj.Count;
                                        }
                                        else
                                        {
                                            lock (bot.Inventory)
                                            {
                                                if (bot.Inventory.Items.TryGetValue(obj.Id, out var item))
                                                {
                                                    var temp = item.Amount + obj.Count;
                                                    item.Amount = (ushort)(temp > 200 ? 200 : temp);
                                                }
                                                else
                                                {
                                                    var newItem = new InventoryItem
                                                    {
                                                        Id = obj.Id,
                                                        Amount = obj.Count
                                                    };
                                                    bot.Inventory.Items[obj.Id] = newItem;
                                                }
                                            }
                                        }
                                    }

                                    removeIndex = i;
                                    break;
                                }
                            }

                            if (removeIndex.HasValue)
                            {
                                bot.World.Dropped.Items.RemoveAt(removeIndex.Value);
                                bot.World.Dropped.ItemsCount -= 1;
                            }
                        }

                        break;
                    }
                }

                break;
            }
        }
    }
}