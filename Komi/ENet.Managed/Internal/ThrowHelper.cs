﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace ENet.Managed.Internal
{
    internal static class ThrowHelper
    {
        public static void ThrowIfArgumentNull<T>(T? arg, string argName) where T : class
        {
            if (arg == null)
                throw new ArgumentNullException(argName);
        }

        [DoesNotReturn]
        public static void ThrowENetInitializationFailed()
        {
            throw new ENetException("ENet library initializion failed.");
        }

        [DoesNotReturn]
        public static void ThrowENetIsNotInitialized()
        {
            throw new ENetException("ENet library is not initialized");
        }

        [DoesNotReturn]
        public static void ThrowENetAllocatorRefIsNull()
        {
            throw new NullReferenceException($"{nameof(Allocators.ENetAllocator)} reference is null unexpectedly");
        }

        [DoesNotReturn]
        public static void ThrowENetPacketPointerNull()
        {
            throw new NullReferenceException("Pointer to packet structure is null.");
        }

        [DoesNotReturn]
        public static void ThrowENetPacketResizeFailed()
        {
            throw new ENetException("ENet packet resizing failed.");
        }

        [DoesNotReturn]
        public static void ThrowENetHostNoChecksumInUse()
        {
            throw new InvalidOperationException("Host is not using any checksum method.");
        }

        [DoesNotReturn]
        public static void ThrowENetHostIsUsingCRC32()
        {
            throw new InvalidOperationException("Host is using ENet's builtin CRC32 checksum method.");
        }

        [DoesNotReturn]
        public static void ThrowENetHostNoCompresserInUse()
        {
            throw new InvalidOperationException("Host is not using any compression method.");
        }

        [DoesNotReturn]
        public static void ThrowENetHostIsUsingRangeCoder()
        {
            throw new InvalidOperationException("Host is using ENet's builtin range-coder compression method.");
        }

        [DoesNotReturn]
        public static void ThrowENetHostIsNotUsingInterceptor()
        {
            throw new InvalidOperationException("Host is not using any interceptor.");
        }

        [DoesNotReturn]
        public static void ThrowENetNullPeerPointer()
        {
            throw new NullReferenceException("Pointer to peer structure is null.");
        }

        [DoesNotReturn]
        public static void ThrowENetPeerSendFailed()
        {
            throw new ENetException("Failed to send packet.");
        }

        [DoesNotReturn]
        public static void ThrowENetHostSetCompressWithRangeCoderFailed()
        {
            throw new ENetException("Failed to set compressor to ENet's builtin compressor");
        }

        [DoesNotReturn]
        public static void ThrowENetFailure()
        {
            throw new ENetException("ENet method returned failure code.");
        }

        [DoesNotReturn]
        public static void ThrowENetConnectFailure()
        {
            throw new ENetException("ENet failed connection failure.");
        }

        [DoesNotReturn]
        public static void ThrowENetCreateHostFailed()
        {
            throw new ENetException("ENet host creation failed.");
        }
    }
}
