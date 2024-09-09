using Komi.lib.itemdatabase;
using System.Text;

namespace Komi.lib.world;

public class World
{
    public string Name { get; set; }
    public uint Width { get; set; }
    public uint Height { get; set; }
    public uint TileCount { get; set; }
    public List<Tile> Tiles { get; set; }
    public Dropped Dropped { get; set; }
    public ushort BaseWeather { get; set; }
    public ushort CurrentWeather { get; set; }
    public ItemDatabase ItemDatabase { get; private set; }

    public World(ItemDatabase itemDatabase)
    {
        ItemDatabase = itemDatabase;
        Reset();
    }

    public void Reset()
    {
        Name = "EXIT";
        Width = 0;
        Height = 0;
        TileCount = 0;
        Tiles = new List<Tile>();
        Dropped = new Dropped();
        BaseWeather = 0;
        CurrentWeather = 0;
    }

    public Tile GetTile(uint x, uint y)
    {
        if (x >= Width || y >= Height)
            return null;

        var index = (int)(y * Width + x);
        return Tiles[index];
    }

    public void Parse(byte[] data)
    {
        Reset();
        using var reader = new BinaryReader(new MemoryStream(data));
        reader.BaseStream.Seek(6, SeekOrigin.Begin);
        var strLen = reader.ReadUInt16();
        var nameBytes = reader.ReadBytes(strLen);
        Name = Encoding.UTF8.GetString(nameBytes);
        Width = reader.ReadUInt32();
        Height = reader.ReadUInt32();
        TileCount = reader.ReadUInt32();
        reader.BaseStream.Seek(5, SeekOrigin.Current);

        for (var i = 0; i < TileCount; i++)
        {
            var tile = new Tile
            {
                ForegroundItemId = reader.ReadUInt16(),
                BackgroundItemId = reader.ReadUInt16(),
                ParentBlockIndex = reader.ReadUInt16(),
                Flags = reader.ReadUInt16()
            };

            if ((tile.Flags & 0x1) != 0)
            {
                var extraTileType = reader.ReadByte();
                GetExtraTileData(tile, reader, extraTileType);
            }

            if ((tile.Flags & 0x2) != 0)
            {
                reader.ReadUInt16(); // Skip over data
            }

            Tiles.Add(tile);
        }

        reader.BaseStream.Seek(12, SeekOrigin.Current);
        Dropped.ItemsCount = reader.ReadUInt32();
        Dropped.LastDroppedItemUid = reader.ReadUInt32();

        for (var i = 0; i < Dropped.ItemsCount; i++)
        {
            var droppedItem = new DroppedItem
            {
                Id = reader.ReadUInt16(),
                X = reader.ReadSingle(),
                Y = reader.ReadSingle(),
                Count = reader.ReadByte(),
                Flags = reader.ReadByte(),
                Uid = reader.ReadUInt32()
            };
            Dropped.Items.Add(droppedItem);
        }

        BaseWeather = reader.ReadUInt16();
        reader.ReadUInt16();
        CurrentWeather = reader.ReadUInt16();
    }

    private void GetExtraTileData(Tile tile, BinaryReader reader, byte itemType)
    {
        switch (itemType)
        {
            case 1:
            {
                var doorTextLen = reader.ReadUInt16();
                var doorText = Encoding.UTF8.GetString(reader.ReadBytes(doorTextLen));
                var doorUnknown = reader.ReadByte();
                tile.TileType = new TileType.Door(doorText, doorUnknown);
                break;
            }
            case 2:
            {
                var signTextLen = reader.ReadUInt16();
                var signText = Encoding.UTF8.GetString(reader.ReadBytes(signTextLen));
                reader.ReadUInt32();
                tile.TileType = new TileType.Sign(signText);
                break;
            }
            case 3:
            {
                var settings = reader.ReadByte();
                var ownerUid = reader.ReadUInt32();
                var accessCount = reader.ReadUInt32();
                var accessUids = new List<uint>();
                for (int i = 0; i < accessCount; i++)
                {
                    accessUids.Add(reader.ReadUInt32());
                }

                var minimumLevel = reader.ReadByte();
                var unknown1 = reader.ReadBytes(7);
                tile.TileType = new TileType.Lock(settings, ownerUid, accessCount, accessUids, minimumLevel, unknown1);
                break;
            }
            case 4:
            {
                var timePassed = reader.ReadUInt32();
                var itemOnTree = reader.ReadByte();
                tile.TileType = new TileType.Seed(timePassed, itemOnTree);
                break;
            }
            case 6:
            {
                var mailboxUnknown1Len = reader.ReadUInt16();
                var mailboxUnknown1 = Encoding.UTF8.GetString(reader.ReadBytes(mailboxUnknown1Len));
                var mailboxUnknown2Len = reader.ReadUInt16();
                var mailboxUnknown2 = Encoding.UTF8.GetString(reader.ReadBytes(mailboxUnknown2Len));
                var mailboxUnknown3Len = reader.ReadUInt16();
                var mailboxUnknown3 = Encoding.UTF8.GetString(reader.ReadBytes(mailboxUnknown3Len));
                var mailboxUnknown4 = reader.ReadByte();
                tile.TileType =
                    new TileType.Mailbox(mailboxUnknown1, mailboxUnknown2, mailboxUnknown3, mailboxUnknown4);
                break;
            }
            case 7:
            {
                var bulletinUnknown1Len = reader.ReadUInt16();
                var bulletinUnknown1 = Encoding.UTF8.GetString(reader.ReadBytes(bulletinUnknown1Len));
                var bulletinUnknown2Len = reader.ReadUInt16();
                var bulletinUnknown2 = Encoding.UTF8.GetString(reader.ReadBytes(bulletinUnknown2Len));
                var bulletinUnknown3Len = reader.ReadUInt16();
                var bulletinUnknown3 = Encoding.UTF8.GetString(reader.ReadBytes(bulletinUnknown3Len));
                var bulletinUnknown4 = reader.ReadByte();
                tile.TileType = new TileType.Bulletin(bulletinUnknown1, bulletinUnknown2, bulletinUnknown3,
                    bulletinUnknown4);
                break;
            }
            case 8:
            {
                var diceSymbol = reader.ReadByte();
                tile.TileType = new TileType.Dice(diceSymbol);
                break;
            }
            case 9:
            {
                var chemicalTimePassed = reader.ReadUInt32();
                tile.TileType = new TileType.ChemicalSource(chemicalTimePassed);
                break;
            }
            case 10:
            {
                var achievementUnknown1 = reader.ReadUInt32();
                var achievementTileType = reader.ReadByte();
                tile.TileType = new TileType.AchievementBlock(achievementUnknown1, achievementTileType);
                break;
            }
            case 11:
            {
                var hearthMonitorUnknown1 = reader.ReadUInt32();
                var hearthMonitorPlayerNameLen = reader.ReadUInt16();
                var hearthMonitorPlayerName = Encoding.UTF8.GetString(reader.ReadBytes(hearthMonitorPlayerNameLen));
                tile.TileType = new TileType.HearthMonitor(hearthMonitorUnknown1, hearthMonitorPlayerName);
                break;
            }
            case 12:
            {
                var donationUnknown1Len = reader.ReadUInt16();
                var donationUnknown1 = Encoding.UTF8.GetString(reader.ReadBytes(donationUnknown1Len));
                var donationUnknown2Len = reader.ReadUInt16();
                var donationUnknown2 = Encoding.UTF8.GetString(reader.ReadBytes(donationUnknown2Len));
                var donationUnknown3Len = reader.ReadUInt16();
                var donationUnknown3 = Encoding.UTF8.GetString(reader.ReadBytes(donationUnknown3Len));
                var donationUnknown4 = reader.ReadByte();
                tile.TileType = new TileType.DonationBox(donationUnknown1, donationUnknown2, donationUnknown3,
                    donationUnknown4);
                break;
            }
            case 14:
            {
                var mannequinTextLen = reader.ReadUInt16();
                var mannequinText = Encoding.UTF8.GetString(reader.ReadBytes(mannequinTextLen));
                var mannequinUnknown1 = reader.ReadByte();
                var mannequinClothing1 = reader.ReadUInt32();
                var mannequinClothing2 = reader.ReadUInt16();
                var mannequinClothing3 = reader.ReadUInt16();
                var mannequinClothing4 = reader.ReadUInt16();
                var mannequinClothing5 = reader.ReadUInt16();
                var mannequinClothing6 = reader.ReadUInt16();
                var mannequinClothing7 = reader.ReadUInt16();
                var mannequinClothing8 = reader.ReadUInt16();
                var mannequinClothing9 = reader.ReadUInt16();
                var mannequinClothing10 = reader.ReadUInt16();
                tile.TileType = new TileType.Mannequin(mannequinText, mannequinUnknown1, mannequinClothing1,
                    mannequinClothing2, mannequinClothing3, mannequinClothing4, mannequinClothing5, mannequinClothing6,
                    mannequinClothing7, mannequinClothing8, mannequinClothing9, mannequinClothing10);
                break;
            }
            case 15:
            {
                var bunnyEggPlaced = reader.ReadUInt32();
                tile.TileType = new TileType.BunnyEgg(bunnyEggPlaced);
                break;
            }
            case 16:
            {
                var gamePackTeam = reader.ReadByte();
                tile.TileType = new TileType.GamePack(gamePackTeam);
                break;
            }
            case 17:
            {
                tile.TileType = new TileType.GameGenerator();
                break;
            }
            case 18:
            {
                var xenoniteUnknown1 = reader.ReadByte();
                var xenoniteUnknown2 = reader.ReadUInt32();
                tile.TileType = new TileType.XenoniteCrystal(xenoniteUnknown1, xenoniteUnknown2);
                break;
            }
            case 19:
            {
                var phoneBoothClothing1 = reader.ReadUInt16();
                var phoneBoothClothing2 = reader.ReadUInt16();
                var phoneBoothClothing3 = reader.ReadUInt16();
                var phoneBoothClothing4 = reader.ReadUInt16();
                var phoneBoothClothing5 = reader.ReadUInt16();
                var phoneBoothClothing6 = reader.ReadUInt16();
                var phoneBoothClothing7 = reader.ReadUInt16();
                var phoneBoothClothing8 = reader.ReadUInt16();
                var phoneBoothClothing9 = reader.ReadUInt16();
                tile.TileType = new TileType.PhoneBooth(phoneBoothClothing1, phoneBoothClothing2, phoneBoothClothing3,
                    phoneBoothClothing4, phoneBoothClothing5, phoneBoothClothing6, phoneBoothClothing7,
                    phoneBoothClothing8, phoneBoothClothing9);
                break;
            }
            case 20:
            {
                var crystalUnknown1Len = reader.ReadUInt16();
                var crystalUnknown1 = Encoding.UTF8.GetString(reader.ReadBytes(crystalUnknown1Len));
                tile.TileType = new TileType.Crystal(crystalUnknown1);
                break;
            }
            case 21:
            {
                var crimeUnknown1Len = reader.ReadUInt16();
                var crimeUnknown1 = Encoding.UTF8.GetString(reader.ReadBytes(crimeUnknown1Len));
                var crimeUnknown2 = reader.ReadUInt32();
                var crimeUnknown3 = reader.ReadByte();
                tile.TileType = new TileType.CrimeInProgress(crimeUnknown1, crimeUnknown2, crimeUnknown3);
                break;
            }
            case 23:
            {
                var displayItemId = reader.ReadUInt32();
                tile.TileType = new TileType.DisplayBlock(displayItemId);
                break;
            }
            case 24:
            {
                var vendingItemId = reader.ReadUInt32();
                var vendingPrice = reader.ReadInt32();
                tile.TileType = new TileType.VendingMachine(vendingItemId, vendingPrice);
                break;
            }
            case 25:
            {
                var fishTankFlags = reader.ReadByte();
                var fishCount = reader.ReadUInt32();
                var fishes = new List<FishInfo>();
                for (var i = 0; i < fishCount; i++)
                {
                    var fishItemId = reader.ReadUInt32();
                    var lbs = reader.ReadUInt32();
                    fishes.Add(new FishInfo { FishItemId = fishItemId, Lbs = lbs });
                }

                tile.TileType = new TileType.FishTankPort(fishTankFlags, fishes);
                break;
            }
            case 26:
            {
                var solarCollectorUnknown1 = reader.ReadBytes(5);
                tile.TileType = new TileType.SolarCollector(solarCollectorUnknown1);
                break;
            }
            case 27:
            {
                var forgeTemperature = reader.ReadUInt32();
                tile.TileType = new TileType.Forge(forgeTemperature);
                break;
            }
            case 28:
            {
                var givingTreeUnknown1 = reader.ReadUInt16();
                var givingTreeUnknown2 = reader.ReadUInt32();
                tile.TileType = new TileType.GivingTree(givingTreeUnknown1, givingTreeUnknown2);
                break;
            }
            case 30:
            {
                var steamOrganType = reader.ReadByte();
                var steamOrganNote = reader.ReadUInt32();
                tile.TileType = new TileType.SteamOrgan(steamOrganType, steamOrganNote);
                break;
            }
            case 31:
            {
                var silkType = reader.ReadByte();
                var silkNameLen = reader.ReadUInt16();
                var silkName = Encoding.UTF8.GetString(reader.ReadBytes(silkNameLen));
                var silkAge = reader.ReadUInt32();
                var silkUnknown1 = reader.ReadUInt32();
                var silkUnknown2 = reader.ReadUInt32();
                var silkCanBeFed = reader.ReadByte();
                var silkColor = reader.ReadUInt32();
                var silkSickDuration = reader.ReadUInt32();
                tile.TileType = new TileType.SilkWorm(silkType, silkName, silkAge, silkUnknown1, silkUnknown2,
                    silkCanBeFed,
                    new SilkWormColor
                    {
                        A = (byte)(silkColor >> 24), R = (byte)(silkColor >> 16), G = (byte)(silkColor >> 8),
                        B = (byte)silkColor
                    }, silkSickDuration);
                break;
            }
            case 32:
            {
                var boltLen = reader.ReadUInt16();
                var boltList = new List<uint>();
                for (var i = 0; i < boltLen; i++)
                {
                    var boltId = reader.ReadUInt32();
                    boltList.Add(boltId);
                }

                tile.TileType = new TileType.SewingMachine(boltList);
                break;
            }
            case 33:
            {
                var countryLen = reader.ReadUInt16();
                var country = Encoding.UTF8.GetString(reader.ReadBytes(countryLen));
                tile.TileType = new TileType.CountryFlag(country);
                break;
            }
            case 34:
            {
                tile.TileType = new TileType.LobsterTrap();
                break;
            }
            case 35:
            {
                var easelItemId = reader.ReadUInt32();
                var easelLabelLen = reader.ReadUInt16();
                var easelLabel = Encoding.UTF8.GetString(reader.ReadBytes(easelLabelLen));
                tile.TileType = new TileType.PaintingEasel(easelItemId, easelLabel);
                break;
            }
            case 36:
            {
                var battleLabelLen = reader.ReadUInt16();
                var battleLabel = Encoding.UTF8.GetString(reader.ReadBytes(battleLabelLen));
                var basePet = reader.ReadUInt32();
                var combinedPet1 = reader.ReadUInt32();
                var combinedPet2 = reader.ReadUInt32();
                tile.TileType = new TileType.PetBattleCage(battleLabel, basePet, combinedPet1, combinedPet2);
                break;
            }
            case 37:
            {
                var petTrainerNameLen = reader.ReadUInt16();
                var petTrainerName = Encoding.UTF8.GetString(reader.ReadBytes(petTrainerNameLen));
                var petTotalCount = reader.ReadUInt32();
                var unknown1 = reader.ReadUInt32();
                var petIds = new List<uint>();
                for (var i = 0; i < petTotalCount; i++)
                {
                    petIds.Add(reader.ReadUInt32());
                }

                tile.TileType = new TileType.PetTrainer(petTrainerName, petTotalCount, unknown1, petIds);
                break;
            }
            case 38:
            {
                var steamEngineTemperature = reader.ReadUInt32();
                tile.TileType = new TileType.SteamEngine(steamEngineTemperature);
                break;
            }
            case 39:
            {
                var lockBotTimePassed = reader.ReadUInt32();
                tile.TileType = new TileType.LockBot(lockBotTimePassed);
                break;
            }
            case 40:
            {
                var weatherMachineSettings = reader.ReadUInt32();
                tile.TileType = new TileType.WeatherMachine(weatherMachineSettings);
                break;
            }
            case 41:
            {
                var ghostJarCount = reader.ReadUInt32();
                tile.TileType = new TileType.SpiritStorageUnit(ghostJarCount);
                break;
            }
            case 42:
            {
                reader.BaseStream.Seek(21, SeekOrigin.Current);
                tile.TileType = new TileType.DataBedrock();
                break;
            }
            case 43:
            {
                var topLeftItemId = reader.ReadUInt32();
                var topRightItemId = reader.ReadUInt32();
                var bottomLeftItemId = reader.ReadUInt32();
                var bottomRightItemId = reader.ReadUInt32();
                tile.TileType = new TileType.Shelf(topLeftItemId, topRightItemId, bottomLeftItemId, bottomRightItemId);
                break;
            }
            case 44:
            {
                var vipUnknown1 = reader.ReadByte();
                var vipOwnerUid = reader.ReadUInt32();
                var vipAccessCount = reader.ReadUInt32();
                var vipAccessUids = new List<uint>();
                for (var i = 0; i < vipAccessCount; i++)
                {
                    vipAccessUids.Add(reader.ReadUInt32());
                }

                tile.TileType = new TileType.VipEntrance(vipUnknown1, vipOwnerUid, vipAccessUids);
                break;
            }
            case 45:
            {
                tile.TileType = new TileType.ChallangeTimer();
                break;
            }
            case 47:
            {
                var fishMountLabelLen = reader.ReadUInt16();
                var fishMountLabel = Encoding.UTF8.GetString(reader.ReadBytes(fishMountLabelLen));
                var fishMountItemId = reader.ReadUInt32();
                var fishMountLb = reader.ReadByte();
                tile.TileType = new TileType.FishWallMount(fishMountLabel, fishMountItemId, fishMountLb);
                break;
            }
            case 48:
            {
                var portraitLabelLen = reader.ReadUInt16();
                var portraitLabel = Encoding.UTF8.GetString(reader.ReadBytes(portraitLabelLen));
                var portraitUnknown1 = reader.ReadUInt32();
                var portraitUnknown2 = reader.ReadUInt32();
                var portraitUnknown3 = reader.ReadUInt32();
                var portraitUnknown4 = reader.ReadUInt32();
                var portraitFace = reader.ReadUInt32();
                var portraitHat = reader.ReadUInt32();
                var portraitHair = reader.ReadUInt32();
                var portraitUnknown5 = reader.ReadUInt16();
                var portraitUnknown6 = reader.ReadUInt16();
                tile.TileType = new TileType.Portrait(portraitLabel, portraitUnknown1, portraitUnknown2,
                    portraitUnknown3, portraitUnknown4, portraitFace, portraitHat, portraitHair, portraitUnknown5,
                    portraitUnknown6);
                break;
            }
            case 49:
            {
                var guildWeatherUnknown1 = reader.ReadUInt32();
                var gravity = reader.ReadUInt32();
                var flags = reader.ReadByte();
                tile.TileType = new TileType.GuildWeatherMachine(guildWeatherUnknown1, gravity, flags);
                break;
            }
            case 50:
            {
                var fossilUnknown1 = reader.ReadUInt32();
                tile.TileType = new TileType.FossilPrepStation(fossilUnknown1);
                break;
            }
            case 51:
            {
                tile.TileType = new TileType.DnaExtractor();
                break;
            }
            case 52:
            {
                tile.TileType = new TileType.Howler();
                break;
            }
            case 53:
            {
                var currentChem = reader.ReadUInt32();
                var targetChem = reader.ReadUInt32();
                tile.TileType = new TileType.ChemsynthTank(currentChem, targetChem);
                break;
            }
            case 54:
            {
                var storageDataLen = reader.ReadUInt16();
                var storageItems = new List<StorageBlockItemInfo>();
                for (var i = 0; i < storageDataLen / 13; i++)
                {
                    reader.BaseStream.Seek(3, SeekOrigin.Current);
                    var storageId = reader.ReadUInt32();
                    reader.BaseStream.Seek(2, SeekOrigin.Current);
                    var storageAmount = reader.ReadUInt32();
                    storageItems.Add(new StorageBlockItemInfo { Id = storageId, Amount = storageAmount });
                }

                tile.TileType = new TileType.StorageBlock(storageItems);
                break;
            }
            case 55:
            {
                var ovenTemperatureLevel = reader.ReadUInt32();
                var ovenIngredientCount = reader.ReadUInt32();
                var ovenIngredients = new List<CookingOvenIngredientInfo>();
                for (var i = 0; i < ovenIngredientCount; i++)
                {
                    var ovenItemId = reader.ReadUInt32();
                    var ovenTimeAdded = reader.ReadUInt32();
                    ovenIngredients.Add(
                        new CookingOvenIngredientInfo { ItemId = ovenItemId, TimeAdded = ovenTimeAdded });
                }

                var ovenUnknown1 = reader.ReadUInt32();
                var ovenUnknown2 = reader.ReadUInt32();
                var ovenUnknown3 = reader.ReadUInt32();
                tile.TileType = new TileType.CookingOven(ovenTemperatureLevel, ovenIngredients, ovenUnknown1,
                    ovenUnknown2, ovenUnknown3);
                break;
            }
            case 56:
            {
                var audioNoteLen = reader.ReadUInt16();
                var audioNote = Encoding.UTF8.GetString(reader.ReadBytes(audioNoteLen));
                var audioVolume = reader.ReadUInt32();
                tile.TileType = new TileType.AudioRack(audioNote, audioVolume);
                break;
            }
            case 57:
            {
                var geigerUnknown1 = reader.ReadUInt32();
                tile.TileType = new TileType.GeigerCharger(geigerUnknown1);
                break;
            }
            case 58:
            {
                tile.TileType = new TileType.AdventureBegins();
                break;
            }
            case 59:
            {
                tile.TileType = new TileType.TombRobber();
                break;
            }
            case 60:
            {
                var balloonTotalRarity = reader.ReadUInt32();
                var balloonTeamType = reader.ReadByte();
                tile.TileType = new TileType.BalloonOMatic(balloonTotalRarity, balloonTeamType);
                break;
            }
            case 61:
            {
                var trainingFishLb = reader.ReadUInt32();
                var trainingFishStatus = reader.ReadUInt16();
                var trainingFishId = reader.ReadUInt32();
                var trainingFishTotalExp = reader.ReadUInt32();
                var trainingFishLevel = reader.ReadUInt32();
                var trainingUnknown2 = reader.ReadUInt32();
                tile.TileType = new TileType.TrainingPort(trainingFishLb, trainingFishStatus, trainingFishId,
                    trainingFishTotalExp, trainingFishLevel, trainingUnknown2);
                break;
            }
            case 62:
            {
                var suckerItemId = reader.ReadUInt32();
                var suckerAmount = reader.ReadUInt32();
                var suckerFlags = reader.ReadUInt16();
                var suckerLimit = reader.ReadUInt32();
                tile.TileType = new TileType.ItemSucker(suckerItemId, suckerAmount, suckerFlags, suckerLimit);
                break;
            }
            case 63:
            {
                var cybotSyncTimer = reader.ReadUInt32();
                var cybotActivated = reader.ReadUInt32();
                var cybotCommandDataCount = reader.ReadUInt32();
                var commandDatas = new List<CyBotCommandData>();
                for (var i = 0; i < cybotCommandDataCount; i++)
                {
                    var commandId = reader.ReadUInt32();
                    var isCommandUsed = reader.ReadUInt32();
                    reader.BaseStream.Seek(7, SeekOrigin.Current);
                    commandDatas.Add(new CyBotCommandData { CommandId = commandId, IsCommandUsed = isCommandUsed });
                }

                tile.TileType = new TileType.CyBot(cybotSyncTimer, cybotActivated, commandDatas);
                break;
            }
            case 65:
            {
                reader.BaseStream.Seek(17, SeekOrigin.Current);
                tile.TileType = new TileType.GuildItem();
                break;
            }
            case 66:
            {
                var growScanUnknown1 = reader.ReadByte();
                tile.TileType = new TileType.Growscan(growScanUnknown1);
                break;
            }
            case 67:
            {
                var containmentGhostJarCount = reader.ReadUInt32();
                var containmentUnknown1Size = reader.ReadUInt32();
                var containmentUnknown1 = new List<uint>();
                for (var i = 0; i < containmentUnknown1Size; i++)
                {
                    containmentUnknown1.Add(reader.ReadUInt32());
                }

                tile.TileType = new TileType.ContainmentFieldPowerNode(containmentGhostJarCount, containmentUnknown1);
                break;
            }
            case 68:
            {
                var spiritBoardUnknown1 = reader.ReadUInt32();
                var spiritBoardUnknown2 = reader.ReadUInt32();
                var spiritBoardUnknown3 = reader.ReadUInt32();
                tile.TileType = new TileType.SpiritBoard(spiritBoardUnknown1, spiritBoardUnknown2, spiritBoardUnknown3);
                break;
            }
            case 72:
            {
                var stormyStingDuration = reader.ReadUInt32();
                var stormyIsSolid = reader.ReadUInt32();
                var stormyNonSolidDuration = reader.ReadUInt32();
                tile.TileType = new TileType.StormyCloud(stormyStingDuration, stormyIsSolid, stormyNonSolidDuration);
                break;
            }
            case 73:
            {
                var tempPlatformUnknown1 = reader.ReadUInt32();
                tile.TileType = new TileType.TemporaryPlatform(tempPlatformUnknown1);
                break;
            }
            case 74:
            {
                tile.TileType = new TileType.SafeVault();
                break;
            }
            case 75:
            {
                var angelicIsRaffling = reader.ReadUInt32();
                var angelicUnknown1 = reader.ReadUInt16();
                var angelicAsciiCode = reader.ReadByte();
                tile.TileType = new TileType.AngelicCountingCloud(angelicIsRaffling, angelicUnknown1, angelicAsciiCode);
                break;
            }
            case 77:
            {
                var infinityIntervalMinutes = reader.ReadUInt32();
                var weatherMachineListSize = reader.ReadUInt32();
                var weatherMachineList = new List<uint>();
                for (var i = 0; i < weatherMachineListSize; i++)
                {
                    weatherMachineList.Add(reader.ReadUInt32());
                }

                tile.TileType = new TileType.InfinityWeatherMachine(infinityIntervalMinutes, weatherMachineList);
                break;
            }
            case 79:
            {
                tile.TileType = new TileType.PineappleGuzzler();
                break;
            }
            case 80:
            {
                var krakenPatternIndex = reader.ReadByte();
                var krakenUnknown1 = reader.ReadUInt32();
                var krakenR = reader.ReadByte();
                var krakenG = reader.ReadByte();
                var krakenB = reader.ReadByte();
                tile.TileType =
                    new TileType.KrakenGalaticBlock(krakenPatternIndex, krakenUnknown1, krakenR, krakenG, krakenB);
                break;
            }
            case 81:
            {
                var friendEntranceOwnerUid = reader.ReadUInt32();
                var friendEntranceUnknown1 = reader.ReadUInt16();
                var friendEntranceUnknown2 = reader.ReadUInt16();
                tile.TileType = new TileType.FriendsEntrance(friendEntranceOwnerUid, friendEntranceUnknown1,
                    friendEntranceUnknown2);
                break;
            }
            default:
            {
                tile.TileType = TileType.Basic;
                break;
            }
        }
    }
}

public class Tile
{
    public ushort ForegroundItemId { get; set; }
    public ushort BackgroundItemId { get; set; }
    public ushort ParentBlockIndex { get; set; }
    public ushort Flags { get; set; }
    public TileType TileType { get; set; } = TileType.Basic;

    public Tile()
    {
    }
}

public class Dropped
{
    public uint ItemsCount { get; set; }
    public uint LastDroppedItemUid { get; set; }
    public List<DroppedItem> Items { get; set; } = new();
}

public class DroppedItem
{
    public ushort Id { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public byte Count { get; set; }
    public byte Flags { get; set; }
    public uint Uid { get; set; }
}