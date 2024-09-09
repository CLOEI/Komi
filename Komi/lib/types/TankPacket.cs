using System.Runtime.InteropServices;

namespace Komi.lib.types;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TankPacket
{
    public ETankPacketType Type { get; set; }
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