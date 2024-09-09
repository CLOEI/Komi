using System.Text;

namespace Komi.lib.itemdatabase;

public class Item
{
    public uint Id { get; set; }
    public ushort Flags { get; set; }
    public byte ActionType { get; set; }
    public byte Material { get; set; }
    public string Name { get; set; }
    public string TextureFileName { get; set; }
    public uint TextureHash { get; set; }
    public byte VisualEffect { get; set; }
    public uint CookingIngredient { get; set; }
    public byte TextureX { get; set; }
    public byte TextureY { get; set; }
    public byte RenderType { get; set; }
    public byte IsStripeyWallpaper { get; set; }
    public byte CollisionType { get; set; }
    public byte BlockHealth { get; set; }
    public uint DropChance { get; set; }
    public byte ClothingType { get; set; }
    public ushort Rarity { get; set; }
    public byte MaxItem { get; set; }
    public string FileName { get; set; }
    public uint FileHash { get; set; }
    public uint AudioVolume { get; set; }
    public string PetName { get; set; }
    public string PetPrefix { get; set; }
    public string PetSuffix { get; set; }
    public string PetAbility { get; set; }
    public byte SeedBaseSprite { get; set; }
    public byte SeedOverlaySprite { get; set; }
    public byte TreeBaseSprite { get; set; }
    public byte TreeOverlaySprite { get; set; }
    public uint BaseColor { get; set; }
    public uint OverlayColor { get; set; }
    public uint Ingredient { get; set; }
    public uint GrowTime { get; set; }
    public ushort IsRayman { get; set; }
    public string ExtraOptions { get; set; }
    public string TexturePath2 { get; set; }
    public string ExtraOption2 { get; set; }
    public string PunchOption { get; set; }
}

public class ItemDatabase
{
    public ushort Version { get; set; }
    public uint ItemCount { get; set; }
    private List<Item> Items { get; set; } = new();

    public void AddItem(Item item)
    {
        Items.Add(item);
    }

    public Item GetItem(uint index)
    {
        return Items[(int)index];
    }
}

public static class ItemDatabaseLoader
{
    private const string Secret = "PBG892FXX982ABC*";

    public static ItemDatabase LoadFromFile(string path)
    {
        var itemDatabase = new ItemDatabase();

        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(fileStream);
        itemDatabase.Version = reader.ReadUInt16();
        itemDatabase.ItemCount = reader.ReadUInt32();

        for (uint i = 0; i < itemDatabase.ItemCount; i++)
        {
            var item = new Item();

            item.Id = reader.ReadUInt32();
            item.Flags = reader.ReadUInt16();
            item.ActionType = reader.ReadByte();
            item.Material = reader.ReadByte();
            item.Name = DecipherItemName(reader, item.Id);
            item.TextureFileName = ReadString(reader);
            item.TextureHash = reader.ReadUInt32();
            item.VisualEffect = reader.ReadByte();
            item.CookingIngredient = reader.ReadUInt32();
            item.TextureX = reader.ReadByte();
            item.TextureY = reader.ReadByte();
            item.RenderType = reader.ReadByte();
            item.IsStripeyWallpaper = reader.ReadByte();
            item.CollisionType = reader.ReadByte();
            item.BlockHealth = reader.ReadByte();
            item.DropChance = reader.ReadUInt32();
            item.ClothingType = reader.ReadByte();
            item.Rarity = reader.ReadUInt16();
            item.MaxItem = reader.ReadByte();
            item.FileName = ReadString(reader);
            item.FileHash = reader.ReadUInt32();
            item.AudioVolume = reader.ReadUInt32();
            item.PetName = ReadString(reader);
            item.PetPrefix = ReadString(reader);
            item.PetSuffix = ReadString(reader);
            item.PetAbility = ReadString(reader);
            item.SeedBaseSprite = reader.ReadByte();
            item.SeedOverlaySprite = reader.ReadByte();
            item.TreeBaseSprite = reader.ReadByte();
            item.TreeOverlaySprite = reader.ReadByte();
            item.BaseColor = reader.ReadUInt32();
            item.OverlayColor = reader.ReadUInt32();
            item.Ingredient = reader.ReadUInt32();
            item.GrowTime = reader.ReadUInt32();
            reader.BaseStream.Seek(2, SeekOrigin.Current);
            item.IsRayman = reader.ReadUInt16();
            item.ExtraOptions = ReadString(reader);
            item.TexturePath2 = ReadString(reader);
            item.ExtraOption2 = ReadString(reader);
            
            reader.BaseStream.Seek(80, SeekOrigin.Current);


            if (itemDatabase.Version >= 11)
            {
                item.PunchOption = ReadString(reader);
            }

            if (itemDatabase.Version >= 12)
            {
                reader.BaseStream.Seek(13, SeekOrigin.Current);
            }

            if (itemDatabase.Version >= 13)
            {
                reader.BaseStream.Seek(4, SeekOrigin.Current);
            }

            if (itemDatabase.Version >= 14)
            {
                reader.BaseStream.Seek(4, SeekOrigin.Current);
            }

            if (itemDatabase.Version >= 15)
            {
                reader.BaseStream.Seek(25, SeekOrigin.Current);
                ReadString(reader);
            }

            if (itemDatabase.Version >= 16)
            {
                ReadString(reader);
            }

            if (itemDatabase.Version >= 17)
            {
                reader.BaseStream.Seek(4, SeekOrigin.Current);
            }

            if (itemDatabase.Version >= 18)
            {
                reader.BaseStream.Seek(4, SeekOrigin.Current);
            }

            if (i != item.Id)
            {
                Console.WriteLine($"Item id mismatch: expected {i}, got {item.Id}");
                Console.WriteLine($"Current memory position: {reader.BaseStream.Position}");
                throw new InvalidOperationException("Item id mismatch");
            }

            itemDatabase.AddItem(item);
        }

        return itemDatabase;
    }

    private static string ReadString(BinaryReader reader)
    {
        var length = reader.ReadUInt16();
        var bytes = reader.ReadBytes(length);
        return Encoding.ASCII.GetString(bytes);
    }

    private static string DecipherItemName(BinaryReader reader, uint itemId)
    {
        var length = reader.ReadUInt16();
        var itemName = new StringBuilder(length);

        for (var i = 0; i < length; i++)
        {
            var charPos = (i + itemId) % Secret.Length;
            var secretChar = Secret[(int)charPos];
            var inputChar = reader.ReadByte();
            itemName.Append((char)(inputChar ^ secretChar));
        }

        return itemName.ToString();
    }
}