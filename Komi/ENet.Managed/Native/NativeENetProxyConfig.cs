using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using ENet.Managed.Internal;

namespace ENet.Managed.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NativeENetProxyConfig
    {
        public NativeENetAddress Address;
        public IntPtr Username;
        public IntPtr Password;
    }
}
