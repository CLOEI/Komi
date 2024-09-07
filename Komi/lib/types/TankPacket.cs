namespace Komi.lib.types;

public struct TankPacket
{
    public ETankPacketType Type { get; set; }
    public byte Unk1 { get; set; }
    public byte Unk2 { get; set; }
    public byte Unk3 { get; set; }
    public uint NetId { get; set; }
    public uint Unk4 { get; set; }
    public uint Flags { get; set; }
    public float Unk6 { get; set; }
    public uint Value { get; set; }
    public float VectorX { get; set; }
    public float VectorY { get; set; }
    public float VectorX2 { get; set; }
    public float VectorY2 { get; set; }
    public float Unk12 { get; set; }
    public int IntX { get; set; }
    public int IntY { get; set; }
    public uint ExtendedDataLength { get; set; }
}