namespace Komi.lib.world;

public abstract class TileType
{
    public static readonly TileType Basic = new BasicTileType();

    public class BasicTileType : TileType { }

    public class Door(string text, byte unknown1) : TileType
    {
        public string Text { get; } = text;
        public byte Unknown1 { get; } = unknown1;
    }

    public class Sign(string text) : TileType
    {
        public string Text { get; } = text;
    }

    public class Lock(
        byte settings,
        uint ownerUid,
        uint accessCount,
        List<uint> accessUids,
        byte minimumLevel,
        byte[] unknown1)
        : TileType
    {
        public byte Settings { get; } = settings;
        public uint OwnerUid { get; } = ownerUid;
        public uint AccessCount { get; } = accessCount;
        public List<uint> AccessUids { get; } = accessUids;
        public byte MinimumLevel { get; } = minimumLevel;
        public byte[] Unknown1 { get; } = unknown1;
    }

    public class Seed(uint timePassed, byte itemOnTree) : TileType
    {
        public uint TimePassed { get; } = timePassed;
        public byte ItemOnTree { get; } = itemOnTree;
    }

    public class Mailbox(string unknown1, string unknown2, string unknown3, byte unknown4)
        : TileType
    {
        public string Unknown1 { get; } = unknown1;
        public string Unknown2 { get; } = unknown2;
        public string Unknown3 { get; } = unknown3;
        public byte Unknown4 { get; } = unknown4;
    }

    public class Bulletin(string unknown1, string unknown2, string unknown3, byte unknown4)
        : TileType
    {
        public string Unknown1 { get; } = unknown1;
        public string Unknown2 { get; } = unknown2;
        public string Unknown3 { get; } = unknown3;
        public byte Unknown4 { get; } = unknown4;
    }

    public class Dice(byte symbol) : TileType
    {
        public byte Symbol { get; } = symbol;
    }

    public class ChemicalSource(uint timePassed) : TileType
    {
        public uint TimePassed { get; } = timePassed;
    }

    public class AchievementBlock(uint unknown1, byte tileType) : TileType
    {
        public uint Unknown1 { get; } = unknown1;
        public byte TileType { get; } = tileType;
    }

    public class HearthMonitor(uint unknown1, string playerName) : TileType
    {
        public uint Unknown1 { get; } = unknown1;
        public string PlayerName { get; } = playerName;
    }

    public class DonationBox(string unknown1, string unknown2, string unknown3, byte unknown4)
        : TileType
    {
        public string Unknown1 { get; } = unknown1;
        public string Unknown2 { get; } = unknown2;
        public string Unknown3 { get; } = unknown3;
        public byte Unknown4 { get; } = unknown4;
    }

    public class Mannequin(
        string text,
        byte unknown1,
        uint clothing1,
        ushort clothing2,
        ushort clothing3,
        ushort clothing4,
        ushort clothing5,
        ushort clothing6,
        ushort clothing7,
        ushort clothing8,
        ushort clothing9,
        ushort clothing10)
        : TileType
    {
        public string Text { get; } = text;
        public byte Unknown1 { get; } = unknown1;
        public uint Clothing1 { get; } = clothing1;
        public ushort Clothing2 { get; } = clothing2;
        public ushort Clothing3 { get; } = clothing3;
        public ushort Clothing4 { get; } = clothing4;
        public ushort Clothing5 { get; } = clothing5;
        public ushort Clothing6 { get; } = clothing6;
        public ushort Clothing7 { get; } = clothing7;
        public ushort Clothing8 { get; } = clothing8;
        public ushort Clothing9 { get; } = clothing9;
        public ushort Clothing10 { get; } = clothing10;
    }

    public class BunnyEgg(uint eggPlaced) : TileType
    {
        public uint EggPlaced { get; } = eggPlaced;
    }

    public class GamePack(byte team) : TileType
    {
        public byte Team { get; } = team;
    }

    public class GameGenerator : TileType { }

    public class XenoniteCrystal(byte unknown1, uint unknown2) : TileType
    {
        public byte Unknown1 { get; } = unknown1;
        public uint Unknown2 { get; } = unknown2;
    }

    public class PhoneBooth(
        ushort clothing1,
        ushort clothing2,
        ushort clothing3,
        ushort clothing4,
        ushort clothing5,
        ushort clothing6,
        ushort clothing7,
        ushort clothing8,
        ushort clothing9)
        : TileType
    {
        public ushort Clothing1 { get; } = clothing1;
        public ushort Clothing2 { get; } = clothing2;
        public ushort Clothing3 { get; } = clothing3;
        public ushort Clothing4 { get; } = clothing4;
        public ushort Clothing5 { get; } = clothing5;
        public ushort Clothing6 { get; } = clothing6;
        public ushort Clothing7 { get; } = clothing7;
        public ushort Clothing8 { get; } = clothing8;
        public ushort Clothing9 { get; } = clothing9;
    }

    public class Crystal(string unknown1) : TileType
    {
        public string Unknown1 { get; } = unknown1;
    }

    public class CrimeInProgress(string unknown1, uint unknown2, byte unknown3) : TileType
    {
        public string Unknown1 { get; } = unknown1;
        public uint Unknown2 { get; } = unknown2;
        public byte Unknown3 { get; } = unknown3;
    }

    public class DisplayBlock(uint itemId) : TileType
    {
        public uint ItemId { get; } = itemId;
    }

    public class VendingMachine(uint itemId, int price) : TileType
    {
        public uint ItemId { get; } = itemId;
        public int Price { get; } = price;
    }

    public class FishTankPort(byte flags, List<FishInfo> fishes) : TileType
    {
        public byte Flags { get; } = flags;
        public List<FishInfo> Fishes { get; } = fishes;
    }

    public class SolarCollector(byte[] unknown1) : TileType
    {
        public byte[] Unknown1 { get; } = unknown1;
    }

    public class Forge(uint temperature) : TileType
    {
        public uint Temperature { get; } = temperature;
    }

    public class GivingTree(ushort unknown1, uint unknown2) : TileType
    {
        public ushort Unknown1 { get; } = unknown1;
        public uint Unknown2 { get; } = unknown2;
    }

    public class SteamOrgan(byte instrumentType, uint note) : TileType
    {
        public byte InstrumentType { get; } = instrumentType;
        public uint Note { get; } = note;
    }

    public class SilkWorm(
        byte type,
        string name,
        uint age,
        uint unknown1,
        uint unknown2,
        byte canBeFed,
        SilkWormColor color,
        uint sickDuration)
        : TileType
    {
        public byte Type { get; } = type;
        public string Name { get; } = name;
        public uint Age { get; } = age;
        public uint Unknown1 { get; } = unknown1;
        public uint Unknown2 { get; } = unknown2;
        public byte CanBeFed { get; } = canBeFed;
        public SilkWormColor Color { get; } = color;
        public uint SickDuration { get; } = sickDuration;
    }

    public class SewingMachine(List<uint> boltIdList) : TileType
    {
        public List<uint> BoltIdList { get; } = boltIdList;
    }

    public class CountryFlag(string country) : TileType
    {
        public string Country { get; } = country;
    }

    public class LobsterTrap : TileType { }

    public class PaintingEasel(uint itemId, string label) : TileType
    {
        public uint ItemId { get; } = itemId;
        public string Label { get; } = label;
    }

    public class PetBattleCage(string label, uint basePet, uint combinedPet1, uint combinedPet2)
        : TileType
    {
        public string Label { get; } = label;
        public uint BasePet { get; } = basePet;
        public uint CombinedPet1 { get; } = combinedPet1;
        public uint CombinedPet2 { get; } = combinedPet2;
    }

    public class PetTrainer(string name, uint petTotalCount, uint unknown1, List<uint> petsId)
        : TileType
    {
        public string Name { get; } = name;
        public uint PetTotalCount { get; } = petTotalCount;
        public uint Unknown1 { get; } = unknown1;
        public List<uint> PetsId { get; } = petsId;
    }

    public class SteamEngine(uint temperature) : TileType
    {
        public uint Temperature { get; } = temperature;
    }

    public class LockBot(uint timePassed) : TileType
    {
        public uint TimePassed { get; } = timePassed;
    }

    public class WeatherMachine(uint settings) : TileType
    {
        public uint Settings { get; } = settings;
    }

    public class SpiritStorageUnit(uint ghostJarCount) : TileType
    {
        public uint GhostJarCount { get; } = ghostJarCount;
    }

    public class DataBedrock : TileType { }

    public class Shelf(uint topLeftItemId, uint topRightItemId, uint bottomLeftItemId, uint bottomRightItemId)
        : TileType
    {
        public uint TopLeftItemId { get; } = topLeftItemId;
        public uint TopRightItemId { get; } = topRightItemId;
        public uint BottomLeftItemId { get; } = bottomLeftItemId;
        public uint BottomRightItemId { get; } = bottomRightItemId;
    }

    public class VipEntrance(byte unknown1, uint ownerUid, List<uint> accessUids) : TileType
    {
        public byte Unknown1 { get; } = unknown1;
        public uint OwnerUid { get; } = ownerUid;
        public List<uint> AccessUids { get; } = accessUids;
    }

    public class ChallangeTimer : TileType { }

    public class FishWallMount(string label, uint itemId, byte lb) : TileType
    {
        public string Label { get; } = label;
        public uint ItemId { get; } = itemId;
        public byte Lb { get; } = lb;
    }

    public class Portrait(
        string label,
        uint unknown1,
        uint unknown2,
        uint unknown3,
        uint unknown4,
        uint face,
        uint hat,
        uint hair,
        ushort unknown5,
        ushort unknown6)
        : TileType
    {
        public string Label { get; } = label;
        public uint Unknown1 { get; } = unknown1;
        public uint Unknown2 { get; } = unknown2;
        public uint Unknown3 { get; } = unknown3;
        public uint Unknown4 { get; } = unknown4;
        public uint Face { get; } = face;
        public uint Hat { get; } = hat;
        public uint Hair { get; } = hair;
        public ushort Unknown5 { get; } = unknown5;
        public ushort Unknown6 { get; } = unknown6;
    }

    public class GuildWeatherMachine(uint unknown1, uint gravity, byte flags) : TileType
    {
        public uint Unknown1 { get; } = unknown1;
        public uint Gravity { get; } = gravity;
        public byte Flags { get; } = flags;
    }

    public class FossilPrepStation(uint unknown1) : TileType
    {
        public uint Unknown1 { get; } = unknown1;
    }

    public class DnaExtractor : TileType { }

    public class Howler : TileType { }

    public class ChemsynthTank(uint currentChem, uint targetChem) : TileType
    {
        public uint CurrentChem { get; } = currentChem;
        public uint TargetChem { get; } = targetChem;
    }

    public class StorageBlock(List<StorageBlockItemInfo> items) : TileType
    {
        public List<StorageBlockItemInfo> Items { get; } = items;
    }

    public class CookingOven(
        uint temperatureLevel,
        List<CookingOvenIngredientInfo> ingredients,
        uint unknown1,
        uint unknown2,
        uint unknown3)
        : TileType
    {
        public uint TemperatureLevel { get; } = temperatureLevel;
        public List<CookingOvenIngredientInfo> Ingredients { get; } = ingredients;
        public uint Unknown1 { get; } = unknown1;
        public uint Unknown2 { get; } = unknown2;
        public uint Unknown3 { get; } = unknown3;
    }

    public class AudioRack(string note, uint volume) : TileType
    {
        public string Note { get; } = note;
        public uint Volume { get; } = volume;
    }

    public class GeigerCharger(uint unknown1) : TileType
    {
        public uint Unknown1 { get; } = unknown1;
    }

    public class AdventureBegins : TileType { }

    public class TombRobber : TileType { }

    public class BalloonOMatic(uint totalRarity, byte teamType) : TileType
    {
        public uint TotalRarity { get; } = totalRarity;
        public byte TeamType { get; } = teamType;
    }

    public class TrainingPort(
        uint fishLb,
        ushort fishStatus,
        uint fishId,
        uint fishTotalExp,
        uint fishLevel,
        uint unknown2)
        : TileType
    {
        public uint FishLb { get; } = fishLb;
        public ushort FishStatus { get; } = fishStatus;
        public uint FishId { get; } = fishId;
        public uint FishTotalExp { get; } = fishTotalExp;
        public uint FishLevel { get; } = fishLevel;
        public uint Unknown2 { get; } = unknown2;
    }

    public class ItemSucker(uint itemIdToSuck, uint itemAmount, ushort flags, uint limit)
        : TileType
    {
        public uint ItemIdToSuck { get; } = itemIdToSuck;
        public uint ItemAmount { get; } = itemAmount;
        public ushort Flags { get; } = flags;
        public uint Limit { get; } = limit;
    }

    public class CyBot(uint syncTimer, uint activated, List<CyBotCommandData> commandDatas)
        : TileType
    {
        public uint SyncTimer { get; } = syncTimer;
        public uint Activated { get; } = activated;
        public List<CyBotCommandData> CommandDatas { get; } = commandDatas;
    }

    public class GuildItem : TileType { }

    public class Growscan(byte unknown1) : TileType
    {
        public byte Unknown1 { get; } = unknown1;
    }

    public class ContainmentFieldPowerNode(uint ghostJarCount, List<uint> unknown1) : TileType
    {
        public uint GhostJarCount { get; } = ghostJarCount;
        public List<uint> Unknown1 { get; } = unknown1;
    }

    public class SpiritBoard(uint unknown1, uint unknown2, uint unknown3) : TileType
    {
        public uint Unknown1 { get; } = unknown1;
        public uint Unknown2 { get; } = unknown2;
        public uint Unknown3 { get; } = unknown3;
    }

    public class StormyCloud(uint stingDuration, uint isSolid, uint nonSolidDuration) : TileType
    {
        public uint StingDuration { get; } = stingDuration;
        public uint IsSolid { get; } = isSolid;
        public uint NonSolidDuration { get; } = nonSolidDuration;
    }

    public class TemporaryPlatform(uint unknown1) : TileType
    {
        public uint Unknown1 { get; } = unknown1;
    }

    public class SafeVault : TileType { }

    public class AngelicCountingCloud(uint isRaffling, ushort unknown1, byte asciiCode) : TileType
    {
        public uint IsRaffling { get; } = isRaffling;
        public ushort Unknown1 { get; } = unknown1;
        public byte AsciiCode { get; } = asciiCode;
    }

    public class InfinityWeatherMachine(uint intervalMinutes, List<uint> weatherMachineList) : TileType
    {
        public uint IntervalMinutes { get; } = intervalMinutes;
        public List<uint> WeatherMachineList { get; } = weatherMachineList;
    }

    public class PineappleGuzzler : TileType { }

    public class KrakenGalaticBlock(byte patternIndex, uint unknown1, byte r, byte g, byte b)
        : TileType
    {
        public byte PatternIndex { get; } = patternIndex;
        public uint Unknown1 { get; } = unknown1;
        public byte R { get; } = r;
        public byte G { get; } = g;
        public byte B { get; } = b;
    }

    public class FriendsEntrance(uint ownerUserId, ushort unknown1, ushort unknown2) : TileType
    {
        public uint OwnerUserId { get; } = ownerUserId;
        public ushort Unknown1 { get; } = unknown1;
        public ushort Unknown2 { get; } = unknown2;
    }
}

public class FishInfo
{
    public uint FishItemId { get; set; }
    public uint Lbs { get; set; }
}

public class SilkWormColor
{
    public byte A { get; set; }
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
}

public class StorageBlockItemInfo
{
    public uint Id { get; set; }
    public uint Amount { get; set; }
}

public class CookingOvenIngredientInfo
{
    public uint ItemId { get; set; }
    public uint TimeAdded { get; set; }
}

public class CyBotCommandData
{
    public uint CommandId { get; set; }
    public uint IsCommandUsed { get; set; }
}