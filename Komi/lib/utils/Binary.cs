using System.Runtime.InteropServices;

namespace Komi.lib.utils;

public class Binary
{
    public static byte[] StructToByteArray<T>(T packet) where T : struct
    {
        var size = Marshal.SizeOf(packet);
        var buffer = new byte[size];

        var ptr = Marshal.AllocHGlobal(size);
        try
        {
            Marshal.StructureToPtr(packet, ptr, true);
            Marshal.Copy(ptr, buffer, 0, size);
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }

        return buffer;
    }
}