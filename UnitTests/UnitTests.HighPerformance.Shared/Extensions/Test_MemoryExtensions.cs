// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Extensions
{
    [TestClass]
    public class Test_MemoryExtensions
    {
        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_Cast_Empty()
        {
            // Casting an empty memory of any size should always be valid
            // and result in another empty memory, regardless of types.
            Memory<byte> m1 = default;
            Memory<byte> mc1 = m1.Cast<byte, byte>();

            Assert.IsTrue(mc1.IsEmpty);

            Memory<byte> m2 = default;
            Memory<float> mc2 = m2.Cast<byte, float>();

            Assert.IsTrue(mc2.IsEmpty);

            Memory<short> m3 = default;
            Memory<Guid> mc3 = m3.Cast<short, Guid>();

            Assert.IsTrue(mc3.IsEmpty);

            // Same as above, but with a sliced memory (length 12, slide from 12, so length of 0)
            Memory<byte> m4 = new byte[12].AsMemory(12);
            Memory<int> mc4 = m4.Cast<byte, int>();

            Assert.IsTrue(mc4.IsEmpty);

            // Same as above, but slicing to 12 in two steps
            Memory<byte> m5 = new byte[12].AsMemory().Slice(4).Slice(8);
            Memory<int> mc5 = m5.Cast<byte, int>();

            Assert.IsTrue(mc5.IsEmpty);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_Cast_TooShort()
        {
            // One int is 4 bytes, so casting from 3 rounds down to 0
            Memory<byte> m1 = new byte[3];
            Memory<int> mc1 = m1.Cast<byte, int>();

            Assert.IsTrue(mc1.IsEmpty);

            // Same as above, 13 / sizeof(int) == 3
            Memory<byte> m2 = new byte[13];
            Memory<float> mc2 = m2.Cast<byte, float>();

            Assert.AreEqual(mc2.Length, 3);

            // 16 - 5 = 11 ---> 11 / sizeof(int) = 2
            Memory<byte> m3 = new byte[16].AsMemory(5);
            Memory<float> mc3 = m3.Cast<byte, float>();

            Assert.AreEqual(mc3.Length, 2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastFromByte()
        {
            // Test for a standard cast from bytes with an evenly divisible length
            Memory<byte> memoryOfBytes = new byte[128];
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();

            Assert.AreEqual(memoryOfFloats.Length, 128 / sizeof(float));

            Span<byte> spanOfBytes = memoryOfBytes.Span;
            Span<float> spanOfFloats = memoryOfFloats.Span;

            // We also need to check that the Span<T> returned from the caast memory
            // actually has the initial reference pointing to the same location as
            // the one to the same item in the span from the original memory.
            Assert.AreEqual(memoryOfFloats.Length, spanOfFloats.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfBytes[0],
                ref Unsafe.As<float, byte>(ref spanOfFloats[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastToByte()
        {
            // Cast from float to bytes to verify casting works when the target type
            // as a smaller byte size as well (so the resulting length will be larger).
            Memory<float> memoryOfFloats = new float[128];
            Memory<byte> memoryOfBytes = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, 128 * sizeof(float));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<byte> spanOfBytes = memoryOfBytes.Span;

            // Same as above, we need to verify that the resulting span has matching
            // starting references with the one produced by the original memory.
            Assert.AreEqual(memoryOfBytes.Length, spanOfBytes.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<byte, float>(ref spanOfBytes[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastToShort()
        {
            // Similar test as above, just with different types to double check
            Memory<float> memoryOfFloats = new float[128];
            Memory<short> memoryOfShorts = memoryOfFloats.Cast<float, short>();

            Assert.AreEqual(memoryOfShorts.Length, 128 * sizeof(float) / sizeof(short));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<short> spanOfShorts = memoryOfShorts.Span;

            Assert.AreEqual(memoryOfShorts.Length, spanOfShorts.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<short, float>(ref spanOfShorts[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastFromByteAndBack()
        {
            // Here we start from a byte array, get a memory, then cast to float and then
            // back to byte. We want to verify that the final memory is both valid and
            // consistent, as well that our internal optimization works and that the final
            // memory correctly skipped the indirect memory managed and just wrapped the original
            // array instead. This is documented in the custom array memory manager in the package.
            var data = new byte[128];
            Memory<byte> memoryOfBytes = data;
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();
            Memory<byte> memoryBack = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, memoryBack.Length);

            // Here we get the array from the final memory and check that it does exist and
            // the associated parameters match the ones we'd expect here (same length, offset of 0).
            Assert.IsTrue(MemoryMarshal.TryGetArray<byte>(memoryBack, out var segment));
            Assert.AreSame(segment.Array!, data);
            Assert.AreEqual(segment.Offset, 0);
            Assert.AreEqual(segment.Count, data.Length);

            Assert.IsTrue(memoryOfBytes.Equals(memoryBack));

            Span<byte> span1 = memoryOfBytes.Span;
            Span<byte> span2 = memoryBack.Span;

            // Also validate the initial and final spans for reference equality
            Assert.IsTrue(span1 == span2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_Cast_TooShort_WithSlice()
        {
            // Like we did above, we have some more tests where we slice an initial memory and
            // validate the length of the resulting, accounting for the expected rounding down.
            Memory<byte> m1 = new byte[8].AsMemory().Slice(4, 3);
            Memory<int> mc1 = m1.Cast<byte, int>();

            Assert.IsTrue(mc1.IsEmpty);

            Memory<byte> m2 = new byte[20].AsMemory().Slice(4, 13);
            Memory<float> mc2 = m2.Cast<byte, float>();

            Assert.AreEqual(mc2.Length, 3);

            Memory<byte> m3 = new byte[16].AsMemory().Slice(5);
            Memory<float> mc3 = m3.Cast<byte, float>();

            Assert.AreEqual(mc3.Length, 2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastFromByte_WithSlice()
        {
            // Same exact test as the cast from byte to float did above, but with a slice. This is done
            // to ensure the cast still works correctly when the memory is internally storing an offset.
            Memory<byte> memoryOfBytes = new byte[512].AsMemory().Slice(128, 128);
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();

            Assert.AreEqual(memoryOfFloats.Length, 128 / sizeof(float));

            Span<byte> spanOfBytes = memoryOfBytes.Span;
            Span<float> spanOfFloats = memoryOfFloats.Span;

            Assert.AreEqual(memoryOfFloats.Length, spanOfFloats.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfBytes[0],
                ref Unsafe.As<float, byte>(ref spanOfFloats[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastToByte_WithSlice()
        {
            // Same as above, just with inverted source and destination types
            Memory<float> memoryOfFloats = new float[512].AsMemory().Slice(128, 128);
            Memory<byte> memoryOfBytes = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, 128 * sizeof(float));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<byte> spanOfBytes = memoryOfBytes.Span;

            Assert.AreEqual(memoryOfBytes.Length, spanOfBytes.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<byte, float>(ref spanOfBytes[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastToShort_WithSlice()
        {
            // Once again the same test but with types both different in size than 1. We're mostly
            // just testing the rounding logic in a number of different case to ensure it's correct.
            Memory<float> memoryOfFloats = new float[512].AsMemory().Slice(128, 128);
            Memory<short> memoryOfShorts = memoryOfFloats.Cast<float, short>();

            Assert.AreEqual(memoryOfShorts.Length, 128 * sizeof(float) / sizeof(short));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<short> spanOfShorts = memoryOfShorts.Span;

            Assert.AreEqual(memoryOfShorts.Length, spanOfShorts.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<short, float>(ref spanOfShorts[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromArray_CastFromByteAndBack_WithSlice()
        {
            // Just like the equivalent test above, but with a slice thrown in too
            var data = new byte[512];
            Memory<byte> memoryOfBytes = data.AsMemory().Slice(128, 128);
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();
            Memory<byte> memoryBack = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, memoryBack.Length);

            // Here we now also have to validate the starting offset from the extracted array
            Assert.IsTrue(MemoryMarshal.TryGetArray<byte>(memoryBack, out var segment));
            Assert.AreSame(segment.Array!, data);
            Assert.AreEqual(segment.Offset, 128);
            Assert.AreEqual(segment.Count, 128);

            Assert.IsTrue(memoryOfBytes.Equals(memoryBack));

            Span<byte> span1 = memoryOfBytes.Span;
            Span<byte> span2 = memoryBack.Span;

            Assert.IsTrue(span1 == span2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastFromByte()
        {
            // This test is just like the ones above, but this time we're casting a memory
            // that wraps a custom memory manager and not an array. This is done to ensure
            // the casting logic works correctly in all cases, as it'll use a different
            // memory manager internally (a memory can wrap a string, an array or a manager).
            Memory<byte> memoryOfBytes = new ArrayMemoryManager<byte>(128);
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();

            Assert.AreEqual(memoryOfFloats.Length, 128 / sizeof(float));

            Span<byte> spanOfBytes = memoryOfBytes.Span;
            Span<float> spanOfFloats = memoryOfFloats.Span;

            Assert.AreEqual(memoryOfFloats.Length, spanOfFloats.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfBytes[0],
                ref Unsafe.As<float, byte>(ref spanOfFloats[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastToByte()
        {
            // Same as above, but with inverted types
            Memory<float> memoryOfFloats = new ArrayMemoryManager<float>(128);
            Memory<byte> memoryOfBytes = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, 128 * sizeof(float));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<byte> spanOfBytes = memoryOfBytes.Span;

            Assert.AreEqual(memoryOfBytes.Length, spanOfBytes.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<byte, float>(ref spanOfBytes[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastToShort()
        {
            // Same as above, but with types different in size than 1, just in case
            Memory<float> memoryOfFloats = new ArrayMemoryManager<float>(128);
            Memory<short> memoryOfShorts = memoryOfFloats.Cast<float, short>();

            Assert.AreEqual(memoryOfShorts.Length, 128 * sizeof(float) / sizeof(short));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<short> spanOfShorts = memoryOfShorts.Span;

            Assert.AreEqual(memoryOfShorts.Length, spanOfShorts.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<short, float>(ref spanOfShorts[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastFromByteAndBack()
        {
            // Equivalent to the one with an array, but with a memory manager
            var data = new ArrayMemoryManager<byte>(128);
            Memory<byte> memoryOfBytes = data;
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();
            Memory<byte> memoryBack = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, memoryBack.Length);

            // Here we expect to get back the original memory manager, due to the same optimization we
            // checked for when using an array. We need to check they're the same, and the other parameters.
            Assert.IsTrue(MemoryMarshal.TryGetMemoryManager<byte, ArrayMemoryManager<byte>>(memoryBack, out var manager, out var start, out var length));
            Assert.AreSame(manager!, data);
            Assert.AreEqual(start, 0);
            Assert.AreEqual(length, 128);

            Assert.IsTrue(memoryOfBytes.Equals(memoryBack));

            Span<byte> span1 = memoryOfBytes.Span;
            Span<byte> span2 = memoryBack.Span;

            Assert.IsTrue(span1 == span2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastFromByte_WithSlice()
        {
            // Same as the ones with an array, but with an extra slice
            Memory<byte> memoryOfBytes = new ArrayMemoryManager<byte>(512).Memory.Slice(128, 128);
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();

            Assert.AreEqual(memoryOfFloats.Length, 128 / sizeof(float));

            Span<byte> spanOfBytes = memoryOfBytes.Span;
            Span<float> spanOfFloats = memoryOfFloats.Span;

            Assert.AreEqual(memoryOfFloats.Length, spanOfFloats.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfBytes[0],
                ref Unsafe.As<float, byte>(ref spanOfFloats[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastToByte_WithSlice()
        {
            // Same as above, but with inverted types
            Memory<float> memoryOfFloats = new ArrayMemoryManager<float>(512).Memory.Slice(128, 128);
            Memory<byte> memoryOfBytes = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, 128 * sizeof(float));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<byte> spanOfBytes = memoryOfBytes.Span;

            Assert.AreEqual(memoryOfBytes.Length, spanOfBytes.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<byte, float>(ref spanOfBytes[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastToShort_WithSlice()
        {
            // Same as above but with different types
            Memory<float> memoryOfFloats = new ArrayMemoryManager<float>(512).Memory.Slice(128, 128);
            Memory<short> memoryOfShorts = memoryOfFloats.Cast<float, short>();

            Assert.AreEqual(memoryOfShorts.Length, 128 * sizeof(float) / sizeof(short));

            Span<float> spanOfFloats = memoryOfFloats.Span;
            Span<short> spanOfShorts = memoryOfShorts.Span;

            Assert.AreEqual(memoryOfShorts.Length, spanOfShorts.Length);
            Assert.IsTrue(Unsafe.AreSame(
                ref spanOfFloats[0],
                ref Unsafe.As<short, float>(ref spanOfShorts[0])));
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromMemoryManager_CastFromByteAndBack_WithSlice()
        {
            // Just like the one above, but with the slice
            var data = new ArrayMemoryManager<byte>(512);
            Memory<byte> memoryOfBytes = data.Memory.Slice(128, 128);
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>();
            Memory<byte> memoryBack = memoryOfFloats.Cast<float, byte>();

            Assert.AreEqual(memoryOfBytes.Length, memoryBack.Length);

            // Here we also need to validate that the offset was maintained
            Assert.IsTrue(MemoryMarshal.TryGetMemoryManager<byte, ArrayMemoryManager<byte>>(memoryBack, out var manager, out var start, out var length));
            Assert.AreSame(manager!, data);
            Assert.AreEqual(start, 128);
            Assert.AreEqual(length, 128);

            Assert.IsTrue(memoryOfBytes.Equals(memoryBack));

            Span<byte> span1 = memoryOfBytes.Span;
            Span<byte> span2 = memoryBack.Span;

            Assert.IsTrue(span1 == span2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        [DataRow(64, 0, 0)]
        [DataRow(64, 4, 0)]
        [DataRow(64, 0, 4)]
        [DataRow(64, 4, 4)]
        [DataRow(64, 4, 0)]
        [DataRow(256, 16, 0)]
        [DataRow(256, 4, 16)]
        [DataRow(256, 64, 0)]
        [DataRow(256, 64, 8)]
        public unsafe void Test_MemoryExtensions_FromArray_CastFromByte_Pin(int size, int preOffset, int postOffset)
        {
            // Here we need to validate that pinning works correctly in a number of cases. First we allocate
            // an array of the requested size, then get a memory after slicing to a target position, then cast
            // and then slice again. We do so to ensure that pinning correctly tracks the correct index with
            // respect to the original array through a number of internal offsets. As in, when pinning the
            // final memory, our internal custom memory manager should be able to pin the item in the original
            // array at offset preOffset + (postOffset * sizeof(float)), accounting for the cast as well.
            var data = new byte[size];
            Memory<byte> memoryOfBytes = data.AsMemory(preOffset);
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>().Slice(postOffset);

            using var handle = memoryOfFloats.Pin();

            void* p1 = handle.Pointer;
            void* p2 = Unsafe.AsPointer(ref data[preOffset + (postOffset * sizeof(float))]);

            Assert.IsTrue(p1 == p2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        [DataRow(64, 0, 0)]
        [DataRow(64, 4, 0)]
        [DataRow(64, 0, 4)]
        [DataRow(64, 4, 4)]
        [DataRow(64, 4, 0)]
        [DataRow(256, 16, 0)]
        [DataRow(256, 4, 16)]
        [DataRow(256, 64, 0)]
        [DataRow(256, 64, 8)]
        public unsafe void Test_MemoryExtensions_FromMemoryManager_CastFromByte_Pin(int size, int preOffset, int postOffset)
        {
            // Just like the test above, but this type the initial memory wraps a memory manager
            var data = new ArrayMemoryManager<byte>(size);
            Memory<byte> memoryOfBytes = data.Memory.Slice(preOffset);
            Memory<float> memoryOfFloats = memoryOfBytes.Cast<byte, float>().Slice(postOffset);

            using var handle = memoryOfFloats.Pin();

            void* p1 = handle.Pointer;
            void* p2 = Unsafe.AsPointer(ref data.GetSpan()[preOffset + (postOffset * sizeof(float))]);

            Assert.IsTrue(p1 == p2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_FromString_CastFromByteAndBack()
        {
            // This is the same as the tests above, but here we're testing the
            // other remaining case, that is when a memory is wrapping a string.
            var data = new string('a', 128);
            Memory<char> memoryOfChars = MemoryMarshal.AsMemory(data.AsMemory());
            Memory<float> memoryOfFloats = memoryOfChars.Cast<char, float>();
            Memory<char> memoryBack = memoryOfFloats.Cast<float, char>();

            Assert.AreEqual(memoryOfChars.Length, memoryBack.Length);

            // Get the original string back (to validate the optimization too) and check the params
            Assert.IsTrue(MemoryMarshal.TryGetString(memoryOfChars, out var text, out int start, out int length));
            Assert.AreSame(text!, data);
            Assert.AreEqual(start, 0);
            Assert.AreEqual(length, data.Length);

            Assert.IsTrue(memoryOfChars.Equals(memoryBack));

            Span<char> span1 = memoryOfChars.Span;
            Span<char> span2 = memoryBack.Span;

            Assert.IsTrue(span1 == span2);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        [DataRow(64, 0, 0)]
        [DataRow(64, 4, 0)]
        [DataRow(64, 0, 4)]
        [DataRow(64, 4, 4)]
        [DataRow(64, 4, 0)]
        [DataRow(256, 16, 0)]
        [DataRow(256, 4, 16)]
        [DataRow(256, 64, 0)]
        [DataRow(256, 64, 8)]
        public unsafe void Test_MemoryExtensions_FromString_CastAndPin(int size, int preOffset, int postOffset)
        {
            // Same test as before to validate pinning, but starting from a string
            var data = new string('a', size);
            Memory<char> memoryOfChars = MemoryMarshal.AsMemory(data.AsMemory()).Slice(preOffset);
            Memory<byte> memoryOfBytes = memoryOfChars.Cast<char, byte>().Slice(postOffset);

            using (var handle1 = memoryOfBytes.Pin())
            {
                void* p1 = handle1.Pointer;
                void* p2 = Unsafe.AsPointer(ref data.DangerousGetReferenceAt(preOffset + (postOffset * sizeof(byte) / sizeof(char))));

                Assert.IsTrue(p1 == p2);
            }

            // Here we also add an extra test just like the one above, but casting to a type
            // that is bigger in byte size than char. Just to double check the casting logic.
            Memory<int> memoryOfInts = memoryOfChars.Cast<char, int>().Slice(postOffset);

            using (var handle2 = memoryOfInts.Pin())
            {
                void* p3 = handle2.Pointer;
                void* p4 = Unsafe.AsPointer(ref data.DangerousGetReferenceAt(preOffset + (postOffset * sizeof(int) / sizeof(char))));

                Assert.IsTrue(p3 == p4);
            }
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_EmptyMemoryStream()
        {
            Memory<byte> memory = default;

            // Creating a stream from a default memory is valid, it's just empty
            Stream stream = memory.AsStream();

            Assert.IsNotNull(stream);
            Assert.AreEqual(stream.Length, memory.Length);
            Assert.IsTrue(stream.CanWrite);
        }

        [TestCategory("MemoryExtensions")]
        [TestMethod]
        public void Test_MemoryExtensions_MemoryStream()
        {
            Memory<byte> memory = new byte[1024];

            Stream stream = memory.AsStream();

            Assert.IsNotNull(stream);
            Assert.AreEqual(stream.Length, memory.Length);
            Assert.IsTrue(stream.CanWrite);
        }

        private sealed class ArrayMemoryManager<T> : MemoryManager<T>
            where T : unmanaged
        {
            private readonly T[] array;

            public ArrayMemoryManager(int size)
            {
                this.array = new T[size];
            }

            public override Span<T> GetSpan()
            {
                return this.array;
            }

            public override unsafe MemoryHandle Pin(int elementIndex = 0)
            {
                GCHandle handle = GCHandle.Alloc(this.array, GCHandleType.Pinned);
                ref T r0 = ref this.array[elementIndex];
                void* p = Unsafe.AsPointer(ref r0);

                return new MemoryHandle(p, handle);
            }

            public override void Unpin()
            {
            }

            protected override void Dispose(bool disposing)
            {
            }

            public static implicit operator Memory<T>(ArrayMemoryManager<T> memoryManager)
            {
                return memoryManager.Memory;
            }
        }
    }
}
