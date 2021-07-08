// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.HighPerformance.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Helpers
{
    [TestClass]
    public class Test_BitHelper
    {
        [TestCategory("BitHelper")]
        [TestMethod]
        public void Test_BitHelper_HasFlag_UInt32()
        {
            uint value = 0b10_1001110101_0110010010_1100100010;

            Assert.IsFalse(BitHelper.HasFlag(value, 0));
            Assert.IsTrue(BitHelper.HasFlag(value, 1));
            Assert.IsFalse(BitHelper.HasFlag(value, 2));
            Assert.IsTrue(BitHelper.HasFlag(value, 9));
            Assert.IsFalse(BitHelper.HasFlag(value, 10));
            Assert.IsFalse(BitHelper.HasFlag(value, 30));
            Assert.IsTrue(BitHelper.HasFlag(value, 31));
        }

        [TestCategory("BitHelper")]
        [TestMethod]
        [DataRow(0, true)]
        [DataRow(1, false)]
        [DataRow(6, true)]
        [DataRow(7, false)]
        [DataRow(12, true)]
        [DataRow(14, true)]
        [DataRow(15, false)]
        [DataRow(20, false)]
        [DataRow(21, true)]
        [DataRow(30, false)]
        [DataRow(31, true)]
        [DataRow(42, false)]
        [DataRow(43741384, false)]
        [DataRow(int.MaxValue, false)]
        public void Test_BitHelper_HasLookupFlag_Default(int index, bool flag)
        {
            const uint value = 0xAAAA5555u;

            Assert.AreEqual(flag, BitHelper.HasLookupFlag(value, index));
        }

        [TestCategory("BitHelper")]
        [TestMethod]
        [DataRow('+', true)]
        [DataRow('-', true)]
        [DataRow('>', true)]
        [DataRow('<', true)]
        [DataRow('.', true)]
        [DataRow(',', true)]
        [DataRow('(', true)]
        [DataRow(')', true)]
        [DataRow(':', true)]
        [DataRow('a', false)]
        [DataRow('%', false)]
        [DataRow('A', false)]
        [DataRow('€', false)]
        [DataRow(0, false)]
        [DataRow(1, false)]
        [DataRow(-1, false)]
        [DataRow(int.MaxValue, false)]
        [DataRow(int.MinValue, false)]
        [DataRow(short.MaxValue, false)]
        [DataRow(short.MinValue, false)]
        public void Test_BitHelper_HasLookupFlag32_WithMin(int x, bool flag)
        {
            const uint mask = 8126587;

            Assert.AreEqual(flag, BitHelper.HasLookupFlag(mask, x, 40));
        }

        [TestCategory("BitHelper")]
        [TestMethod]
        [DataRow(0x0000_0000u, true)]
        [DataRow(0x0000_0001u, true)]
        [DataRow(0x0000_0011u, true)]
        [DataRow(0x0011_1111u, true)]
        [DataRow(0x1100_1111u, true)]
        [DataRow(0x1011_0011u, true)]
        [DataRow(0x1755_0055u, true)]
        [DataRow(0x0055_B255u, true)]
        [DataRow(0x1755_B200u, true)]
        [DataRow(0x1111_1111u, false)]
        [DataRow(0x1755_B255u, false)]
        [DataRow(0x1705_B255u, false)]
        [DataRow(0x1755_B055u, false)]
        public void Test_BitHelper_HasZeroByte_UInt32(uint x, bool result)
        {
            Assert.AreEqual(result, BitHelper.HasZeroByte(x));
        }

        [TestCategory("BitHelper")]
        [TestMethod]
        [DataRow(0x0000_0000_0000_0000ul, true)]
        [DataRow(0x0000_0000_0000_0001ul, true)]
        [DataRow(0x0000_0000_0000_0011ul, true)]
        [DataRow(0x0011_1111_1111_1111ul, true)]
        [DataRow(0x1111_0011_1111_1111ul, true)]
        [DataRow(0x7234_AB00_DEAD_BEEFul, true)]
        [DataRow(0x7234_A542_DEAD_BEEFul, false)]
        [DataRow(0x1111_1111_1111_1111ul, false)]
        [DataRow(0x7234_A542_DEAD_B0EFul, false)]
        [DataRow(0x7030_A040_0E0D_B0E0ul, false)]
        public void Test_BitHelper_HasZeroByte_UInt64(ulong x, bool result)
        {
            Assert.AreEqual(result, BitHelper.HasZeroByte(x));
        }

        [TestCategory("BitHelper")]
        [TestMethod]
        [DataRow(0x0000_0000u, 0x7B, false)]
        [DataRow(0x0000_0001u, 0x7B, false)]
        [DataRow(0x0000_1010u, 0x7B, false)]
        [DataRow(0x0111_7A00u, 0x7B, false)]
        [DataRow(0x0000_07B0u, 0x7B, false)]
        [DataRow(0x1111_1111u, 0x7B, false)]
        [DataRow(0x0000_FEFEu, 0xFF, false)]
        [DataRow(0xF00F_0FF0u, 0xFF, false)]
        [DataRow(0x0000_0000u, 0x00, true)]
        [DataRow(0x0000_007Bu, 0x7B, true)]
        [DataRow(0x0000_7B7Bu, 0x7B, true)]
        [DataRow(0x7B00_0110u, 0x7B, true)]
        [DataRow(0x00FF_0000u, 0xFF, true)]
        [DataRow(0xFFFF_FFFFu, 0xFF, true)]
        [DataRow(0x1515_1515u, 0x15, true)]
        public void Test_BitHelper_HasByteEqualTo_UInt32(uint x, int target, bool result)
        {
            Assert.AreEqual(result, BitHelper.HasByteEqualTo(x, unchecked((byte)target)));
        }

        [TestCategory("BitHelper")]
        [TestMethod]
        [DataRow(0x0000_0000_0000_0000u, 0x7B, false)]
        [DataRow(0x0000_0000_0000_0001u, 0x7B, false)]
        [DataRow(0x0000_0000_0000_1010u, 0x7B, false)]
        [DataRow(0x0111_0000_0000_7A00u, 0x7B, false)]
        [DataRow(0x0000_0000_0000_07B0u, 0x7B, false)]
        [DataRow(0x1111_1111_0000_0000u, 0x7B, false)]
        [DataRow(0x0000_FEFE_0000_0000u, 0xFF, false)]
        [DataRow(0xF00F_0000_0000_0FF0u, 0xFF, false)]
        [DataRow(0x0000_0000_0000_0000u, 0x00, true)]
        [DataRow(0x0000_0000_0000_007Bu, 0x7B, true)]
        [DataRow(0x0000_7B7B_0000_0000u, 0x7B, true)]
        [DataRow(0x7B00_0110_0000_0000u, 0x7B, true)]
        [DataRow(0x00FF_0000_0000_0000u, 0xFF, true)]
        [DataRow(0xFFFF_FFFF_FFFF_FFFFu, 0xFF, true)]
        [DataRow(0x1515_1515_1515_1515u, 0x15, true)]
        public void Test_BitHelper_HasByteEqualTo_UInt64(ulong x, int target, bool result)
        {
            Assert.AreEqual(result, BitHelper.HasByteEqualTo(x, unchecked((byte)target)));
        }

        [TestCategory("BitHelper")]
        [TestMethod]
        public void Test_BitHelper_SetFlag_UInt32()
        {
            Assert.AreEqual(0b1u, BitHelper.SetFlag(0u, 0, true));
            Assert.AreEqual(4u, BitHelper.SetFlag(4u, 1, false));
            Assert.AreEqual(0b110u, BitHelper.SetFlag(2u, 2, true));
            Assert.AreEqual(unchecked((uint)int.MinValue), BitHelper.SetFlag(0u, 31, true));
        }

        [TestCategory("BitHelper")]
        [TestMethod]
        [DataRow(0u, (byte)0, (byte)0, 0u)]
        [DataRow(uint.MaxValue, (byte)0, (byte)8, 255u)]
        [DataRow(uint.MaxValue, (byte)24, (byte)8, 255u)]
        [DataRow(12345u << 7, (byte)7, (byte)16, 12345u)]
        [DataRow(3u << 1, (byte)1, (byte)2, 3u)]
        [DataRow(21u << 17, (byte)17, (byte)5, 21u)]
        [DataRow(1u << 31, (byte)31, (byte)1, 1u)]
        public void Test_BitHelper_ExtractRange_UInt32(uint value, byte start, byte length, uint result)
        {
            Assert.AreEqual(result, BitHelper.ExtractRange(value, start, length));
        }

        [TestCategory("BitHelper")]
        [TestMethod]
        [DataRow((byte)0, (byte)0, 0u)]
        [DataRow((byte)0, (byte)8, 255u)]
        [DataRow((byte)24, (byte)8, 255u)]
        [DataRow((byte)0, (byte)31, (uint)int.MaxValue)]
        [DataRow((byte)29, (byte)3, 5u)]
        [DataRow((byte)7, (byte)14, 12345u)]
        [DataRow((byte)1, (byte)2, 3u)]
        [DataRow((byte)17, (byte)5, 21u)]
        [DataRow((byte)31, (byte)1, 1u)]
        public void Test_BitHelper_SetRange_UInt32(byte start, byte length, uint flags)
        {
            // Specific initial bit mask to check for unwanted modifications
            const uint value = 0xAAAA5555u;

            uint
                backup = BitHelper.ExtractRange(value, start, length),
                result = BitHelper.SetRange(value, start, length, flags),
                extracted = BitHelper.ExtractRange(result, start, length),
                restored = BitHelper.SetRange(result, start, length, backup);

            Assert.AreEqual(extracted, flags);
            Assert.AreEqual(restored, value);
        }

        [TestCategory("BitHelper")]
        [TestMethod]
        public void Test_UInt64Extensions_HasFlag()
        {
            ulong value = 0b10_1001110101_0110010010_1100100010;

            value |= 1ul << 63;

            Assert.IsFalse(BitHelper.HasFlag(value, 0));
            Assert.IsTrue(BitHelper.HasFlag(value, 1));
            Assert.IsFalse(BitHelper.HasFlag(value, 2));
            Assert.IsTrue(BitHelper.HasFlag(value, 9));
            Assert.IsFalse(BitHelper.HasFlag(value, 10));
            Assert.IsFalse(BitHelper.HasFlag(value, 30));
            Assert.IsTrue(BitHelper.HasFlag(value, 31));
            Assert.IsTrue(BitHelper.HasFlag(value, 63));
        }

        [TestCategory("BitHelper")]
        [TestMethod]
        [DataRow('+', true)]
        [DataRow('-', true)]
        [DataRow('>', true)]
        [DataRow('<', true)]
        [DataRow('.', true)]
        [DataRow(',', true)]
        [DataRow('[', true)]
        [DataRow(']', true)]
        [DataRow('(', true)]
        [DataRow(')', true)]
        [DataRow(':', true)]
        [DataRow('a', false)]
        [DataRow('%', false)]
        [DataRow('A', false)]
        [DataRow('€', false)]
        [DataRow(0, false)]
        [DataRow(1, false)]
        [DataRow(-1, false)]
        [DataRow(int.MaxValue, false)]
        [DataRow(int.MinValue, false)]
        [DataRow(short.MaxValue, false)]
        [DataRow(short.MinValue, false)]
        public void Test_BitHelper_HasLookupFlag64_WithMin(int x, bool flag)
        {
            const ulong mask = 11258999073931387;

            Assert.AreEqual(flag, BitHelper.HasLookupFlag(mask, x, 40));
        }

        [TestCategory("BitHelper")]
        [TestMethod]
        public void Test_UInt64Extensions_SetFlag()
        {
            Assert.AreEqual(0b1ul, BitHelper.SetFlag(0u, 0, true));
            Assert.AreEqual(4ul, BitHelper.SetFlag(4u, 1, false));
            Assert.AreEqual(0b110ul, BitHelper.SetFlag(2u, 2, true));
            Assert.AreEqual(1ul << 31, BitHelper.SetFlag(0ul, 31, true));
            Assert.AreEqual(1ul << 63, BitHelper.SetFlag(0ul, 63, true));
        }

        [TestCategory("BitHelper")]
        [TestMethod]
        [DataRow(0u, (byte)0, (byte)0, 0u)]
        [DataRow(uint.MaxValue, (byte)0, (byte)8, 255u)]
        [DataRow(uint.MaxValue, (byte)24, (byte)8, 255u)]
        [DataRow(12345u << 7, (byte)7, (byte)16, 12345u)]
        [DataRow(3u << 1, (byte)1, (byte)2, 3u)]
        [DataRow(21u << 17, (byte)17, (byte)5, 21u)]
        [DataRow(1u << 31, (byte)31, (byte)1, 1u)]
        [DataRow(ulong.MaxValue, (byte)0, (byte)8, 255u)]
        [DataRow(ulong.MaxValue, (byte)24, (byte)8, 255u)]
        [DataRow(ulong.MaxValue, (byte)44, (byte)8, 255u)]
        [DataRow(12345ul << 35, (byte)35, (byte)16, 12345ul)]
        [DataRow(3ul << 56, (byte)56, (byte)2, 3ul)]
        [DataRow(21ul << 37, (byte)37, (byte)5, 21ul)]
        [DataRow(1ul << 63, (byte)63, (byte)1, 1ul)]
        public void Test_BitHelper_ExtractRange_UInt64(ulong value, byte start, byte length, ulong result)
        {
            Assert.AreEqual(result, BitHelper.ExtractRange(value, start, length));
        }

        [TestCategory("BitHelper")]
        [TestMethod]
        [DataRow((byte)0, (byte)0, 0u)]
        [DataRow((byte)0, (byte)8, 255ul)]
        [DataRow((byte)24, (byte)8, 255ul)]
        [DataRow((byte)0, (byte)31, (ulong)int.MaxValue)]
        [DataRow((byte)29, (byte)3, 5ul)]
        [DataRow((byte)7, (byte)14, 12345ul)]
        [DataRow((byte)1, (byte)2, 3ul)]
        [DataRow((byte)17, (byte)5, 21ul)]
        [DataRow((byte)31, (byte)1, 1ul)]
        [DataRow((byte)44, (byte)8, 255ul)]
        [DataRow((byte)23, (byte)8, 255ul)]
        [DataRow((byte)55, (byte)3, 5ul)]
        [DataRow((byte)34, (byte)14, 12345ul)]
        [DataRow((byte)59, (byte)2, 3ul)]
        [DataRow((byte)45, (byte)5, 21ul)]
        [DataRow((byte)62, (byte)1, 1ul)]
        public void Test_BitHelper_SetRange_UInt64(byte start, byte length, ulong flags)
        {
            // Specific initial bit mask to check for unwanted modifications
            const ulong value = 0xAAAA5555AAAA5555u;

            ulong
                backup = BitHelper.ExtractRange(value, start, length),
                result = BitHelper.SetRange(value, start, length, flags),
                extracted = BitHelper.ExtractRange(result, start, length),
                restored = BitHelper.SetRange(result, start, length, backup);

            Assert.AreEqual(extracted, flags);
            Assert.AreEqual(restored, value);
        }
    }
}