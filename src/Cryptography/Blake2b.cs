using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using static Interop.Libsodium;

namespace NSec.Cryptography
{
    //
    //  BLAKE2b (unkeyed)
    //
    //  References:
    //
    //      RFC 7693 - The BLAKE2 Cryptographic Hash and Message Authentication
    //          Code (MAC)
    //
    //  Parameters:
    //
    //      Input Size - Between 0 and 2^128-1 bytes. (A Span<byte> can only
    //          hold up to 2^31-1 bytes.)
    //
    //      Hash Size - Between 1 and 64 bytes. For 128 bits of security, the
    //          output length should not be less than 32 bytes (BLAKE2b-256).
    //
    public sealed class Blake2b : HashAlgorithm
    {
        private static int s_selfTest;

        public Blake2b() : base(
            hashSize: crypto_generichash_blake2b_BYTES)
        {
            Debug.Assert(HashSize >= 32);
            Debug.Assert(HashSize <= crypto_generichash_blake2b_BYTES_MAX);

            if (s_selfTest == 0)
            {
                SelfTest();
                Interlocked.Exchange(ref s_selfTest, 1);
            }
        }

        public Blake2b(int hashSize) : base(
            hashSize: hashSize)
        {
            if (hashSize < 32 ||
                hashSize > crypto_generichash_blake2b_BYTES_MAX)
            {
                throw Error.ArgumentOutOfRange_HashSize(nameof(hashSize), hashSize.ToString(), 32.ToString(), crypto_generichash_blake2b_BYTES_MAX.ToString());
            }
            if (s_selfTest == 0)
            {
                SelfTest();
                Interlocked.Exchange(ref s_selfTest, 1);
            }
        }

        internal override bool FinalizeAndTryVerifyCore(
            ref IncrementalHashState state,
            ReadOnlySpan<byte> hash)
        {
            Debug.Assert(hash.Length >= crypto_generichash_blake2b_BYTES_MIN);
            Debug.Assert(hash.Length <= crypto_generichash_blake2b_BYTES_MAX);

            Span<byte> buffer = stackalloc byte[63 + Unsafe.SizeOf<crypto_generichash_blake2b_state>()];
            ref crypto_generichash_blake2b_state state_ = ref AlignPinnedReference(ref MemoryMarshal.GetReference(buffer));

            Span<byte> temp = stackalloc byte[hash.Length];

            state_ = state.blake2b;

            crypto_generichash_blake2b_final(ref state_, ref MemoryMarshal.GetReference(temp), (UIntPtr)temp.Length);

            int result = sodium_memcmp(in MemoryMarshal.GetReference(temp), in MemoryMarshal.GetReference(hash), (UIntPtr)hash.Length);

            state.blake2b = state_;

            return result == 0;
        }

        internal override void FinalizeCore(
            ref IncrementalHashState state,
            Span<byte> hash)
        {
            Debug.Assert(hash.Length >= crypto_generichash_blake2b_BYTES_MIN);
            Debug.Assert(hash.Length <= crypto_generichash_blake2b_BYTES_MAX);

            Span<byte> buffer = stackalloc byte[63 + Unsafe.SizeOf<crypto_generichash_blake2b_state>()];
            ref crypto_generichash_blake2b_state state_ = ref AlignPinnedReference(ref MemoryMarshal.GetReference(buffer));

            state_ = state.blake2b;

            crypto_generichash_blake2b_final(ref state_, ref MemoryMarshal.GetReference(hash), (UIntPtr)hash.Length);

            state.blake2b = state_;
        }

        internal override void InitializeCore(
            int hashSize,
            out IncrementalHashState state)
        {
            Debug.Assert(hashSize >= crypto_generichash_blake2b_BYTES_MIN);
            Debug.Assert(hashSize <= crypto_generichash_blake2b_BYTES_MAX);

            Span<byte> buffer = stackalloc byte[63 + Unsafe.SizeOf<crypto_generichash_blake2b_state>()];
            ref crypto_generichash_blake2b_state aligned_ = ref AlignPinnedReference(ref MemoryMarshal.GetReference(buffer));

            crypto_generichash_blake2b_init(out aligned_, IntPtr.Zero, UIntPtr.Zero, (UIntPtr)hashSize);

            state.blake2b = aligned_;
        }

        internal override void UpdateCore(
            ref IncrementalHashState state,
            ReadOnlySpan<byte> data)
        {
            Span<byte> buffer = stackalloc byte[63 + Unsafe.SizeOf<crypto_generichash_blake2b_state>()];
            ref crypto_generichash_blake2b_state state_ = ref AlignPinnedReference(ref MemoryMarshal.GetReference(buffer));

            state_ = state.blake2b;

            crypto_generichash_blake2b_update(ref state_, in MemoryMarshal.GetReference(data), (ulong)data.Length);

            state.blake2b = state_;
        }

        private protected override void HashCore(
            ReadOnlySpan<byte> data,
            Span<byte> hash)
        {
            Debug.Assert(hash.Length >= crypto_generichash_blake2b_BYTES_MIN);
            Debug.Assert(hash.Length <= crypto_generichash_blake2b_BYTES_MAX);

            crypto_generichash_blake2b(ref MemoryMarshal.GetReference(hash), (UIntPtr)hash.Length, in MemoryMarshal.GetReference(data), (ulong)data.Length, IntPtr.Zero, UIntPtr.Zero);
        }

        private protected override bool TryVerifyCore(
            ReadOnlySpan<byte> data,
            ReadOnlySpan<byte> hash)
        {
            Debug.Assert(hash.Length >= crypto_generichash_blake2b_BYTES_MIN);
            Debug.Assert(hash.Length <= crypto_generichash_blake2b_BYTES_MAX);

            Span<byte> temp = stackalloc byte[hash.Length];

            crypto_generichash_blake2b(ref MemoryMarshal.GetReference(temp), (UIntPtr)temp.Length, in MemoryMarshal.GetReference(data), (ulong)data.Length, IntPtr.Zero, UIntPtr.Zero);

            int result = sodium_memcmp(in MemoryMarshal.GetReference(temp), in MemoryMarshal.GetReference(hash), (UIntPtr)hash.Length);

            return result == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ref crypto_generichash_blake2b_state AlignPinnedReference(ref byte value)
        {
            return ref sizeof(byte*) == sizeof(uint)
                ? ref Unsafe.AsRef<crypto_generichash_blake2b_state>((void*)(((uint)Unsafe.AsPointer(ref value) + 63u) & ~63u))
                : ref Unsafe.AsRef<crypto_generichash_blake2b_state>((void*)(((ulong)Unsafe.AsPointer(ref value) + 63ul) & ~63ul));
        }

        private static void SelfTest()
        {
            if ((crypto_generichash_blake2b_bytes() != (UIntPtr)crypto_generichash_blake2b_BYTES) ||
                (crypto_generichash_blake2b_bytes_max() != (UIntPtr)crypto_generichash_blake2b_BYTES_MAX) ||
                (crypto_generichash_blake2b_bytes_min() != (UIntPtr)crypto_generichash_blake2b_BYTES_MIN) ||
                (crypto_generichash_blake2b_statebytes() != (UIntPtr)Unsafe.SizeOf<crypto_generichash_blake2b_state>()))
            {
                throw Error.Cryptographic_InitializationFailed(9461.ToString("X"));
            }
        }
    }
}
