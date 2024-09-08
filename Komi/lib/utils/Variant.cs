using System.Text;

namespace Komi.lib.utils;

public enum VariantType
{
    Unknown,
    Float,
    String,
    Vec2,
    Vec3,
    Unsigned,
    Signed,
}

public static class VariantTypeExtensions
{
    public static VariantType FromByte(byte value)
    {
        return value switch
        {
            0 => VariantType.Unknown,
            1 => VariantType.Float,
            2 => VariantType.String,
            3 => VariantType.Vec2,
            4 => VariantType.Vec3,
            5 => VariantType.Unsigned,
            9 => VariantType.Signed,
            _ => VariantType.Unknown,
        };
    }
}

public abstract class Variant
{
    public abstract string AsString();
    public virtual int AsInt32() => 0;
    public virtual (float, float) AsVec2() => (0.0f, 0.0f);
}

public class FloatVariant : Variant
{
    public float Value { get; }

    public FloatVariant(float value) => Value = value;

    public override string AsString() => Value.ToString();
}

public class StringVariant : Variant
{
    public string Value { get; }

    public StringVariant(string value) => Value = value;

    public override string AsString() => Value;
}

public class Vec2Variant : Variant
{
    public (float, float) Value { get; }

    public Vec2Variant((float, float) value) => Value = value;

    public override string AsString() => $"{Value.Item1}, {Value.Item2}";
    public override (float, float) AsVec2() => Value;
}

public class Vec3Variant : Variant
{
    public (float, float, float) Value { get; }

    public Vec3Variant((float, float, float) value) => Value = value;

    public override string AsString() => $"{Value.Item1}, {Value.Item2}, {Value.Item3}";
}

public class UnsignedVariant : Variant
{
    public uint Value { get; }

    public UnsignedVariant(uint value) => Value = value;

    public override string AsString() => Value.ToString();
}

public class SignedVariant : Variant
{
    public int Value { get; }

    public SignedVariant(int value) => Value = value;

    public override string AsString() => Value.ToString();
    public override int AsInt32() => Value;
}

public class UnknownVariant : Variant
{
    public override string AsString() => "Unknown";
}

public class VariantList
{
    public List<Variant> Variants { get; }

    public VariantList(List<Variant> variants) => Variants = variants;

    public static VariantList Deserialize(byte[] data)
    {
        using var memoryStream = new MemoryStream(data);
        using var reader = new BinaryReader(memoryStream);

        var size = reader.ReadByte();
        var variants = new List<Variant>(size);

        for (int i = 0; i < size; i++)
        {
            var _ = reader.ReadByte();
            var varType = VariantTypeExtensions.FromByte(reader.ReadByte());

            Variant variant = varType switch
            {
                VariantType.Float => new FloatVariant(reader.ReadSingle()),
                VariantType.String => new StringVariant(Encoding.UTF8.GetString(reader.ReadBytes(reader.ReadInt32()))),
                VariantType.Vec2 => new Vec2Variant((reader.ReadSingle(), reader.ReadSingle())),
                VariantType.Vec3 => new Vec3Variant((reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())),
                VariantType.Unsigned => new UnsignedVariant(reader.ReadUInt32()),
                VariantType.Signed => new SignedVariant(reader.ReadInt32()),
                _ => new UnknownVariant(),
            };

            variants.Add(variant);
        }

        return new VariantList(variants);
    }

    public Variant Get(int index) => Variants[index];
}