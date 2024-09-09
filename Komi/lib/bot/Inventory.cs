namespace Komi.lib.bot;

public class Inventory
{
    public uint Size { get; set; }
    public ushort ItemCount { get; set; }
    public Dictionary<ushort, InventoryItem> Items { get; set; } = new();

    public void Parse(byte[] data)
    {
        Reset();
        using var stream = new MemoryStream(data);
        using var reader = new BinaryReader(stream);

        stream.Position += 1;
        Size = reader.ReadUInt32();
        ItemCount = reader.ReadUInt16();
        for (int i = 0; i < ItemCount; i++)
        {
            var id = reader.ReadUInt16();
            var amount = reader.ReadUInt16();
            Items[id] = new InventoryItem { Id = id, Amount = amount };
        }
    }

    public void Reset()
    {
        Size = 0;
        ItemCount = 0;
        Items.Clear();
    }
}

public class InventoryItem
{
    public ushort Id { get; set; }
    public ushort Amount { get; set; }
}