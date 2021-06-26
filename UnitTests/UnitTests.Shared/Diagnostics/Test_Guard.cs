// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Diagnostics
{
    [TestClass]
    public partial class Test_Guard
    {
        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsNull_Ok()
        {
            Guard.IsNull<object>(null, nameof(Test_Guard_IsNull_Ok));
            Guard.IsNull<int>(null, nameof(Test_Guard_IsNull_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsNull_ClassFail()
        {
            Guard.IsNull(new object(), nameof(Test_Guard_IsNull_ClassFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsNull_StructFail()
        {
            Guard.IsNull<int>(7, nameof(Test_Guard_IsNull_StructFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsNotNull_Ok()
        {
            Guard.IsNotNull(new object(), nameof(Test_Guard_IsNotNull_Ok));
            Guard.IsNotNull<int>(7, nameof(Test_Guard_IsNotNull_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Guard_IsNotNull_ClassFail()
        {
            Guard.IsNotNull<object>(null, nameof(Test_Guard_IsNotNull_ClassFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_Guard_IsNotNull_StructFail()
        {
            Guard.IsNotNull<int>(null, nameof(Test_Guard_IsNotNull_StructFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsOfT_Ok()
        {
            Guard.IsOfType<string>("Hello", nameof(Test_Guard_IsOfT_Ok));
            Guard.IsOfType<int>(7, nameof(Test_Guard_IsOfT_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsOfT_Fail()
        {
            Guard.IsOfType<string>(7, nameof(Test_Guard_IsOfT_Fail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsOfType_Ok()
        {
            Guard.IsOfType("Hello", typeof(string), nameof(Test_Guard_IsOfType_Ok));
            Guard.IsOfType(7, typeof(int), nameof(Test_Guard_IsOfType_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsOfType_Fail()
        {
            Guard.IsOfType(7, typeof(string), nameof(Test_Guard_IsOfType_Fail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsAssignableToT_Ok()
        {
            Guard.IsAssignableToType<string>("Hello", nameof(Test_Guard_IsAssignableToT_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsAssignableToT_Fail()
        {
            Guard.IsAssignableToType<string>(7, nameof(Test_Guard_IsAssignableToT_Fail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsAssignableToType_Ok()
        {
            Guard.IsAssignableToType("Hello", typeof(string), nameof(Test_Guard_IsAssignableToType_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsAssignableToType_Fail()
        {
            Guard.IsAssignableToType(7, typeof(string), nameof(Test_Guard_IsAssignableToType_Fail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsEqualTo_Ok()
        {
            Guard.IsEqualTo("Hello", "Hello", nameof(Test_Guard_IsEqualTo_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsEqualTo_Fail()
        {
            Guard.IsEqualTo("Hello", "World", nameof(Test_Guard_IsEqualTo_Fail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsNotEqualTo_Ok()
        {
            Guard.IsNotEqualTo("Hello", "World", nameof(Test_Guard_IsNotEqualTo_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsNotEqualTo_Fail()
        {
            Guard.IsNotEqualTo("Hello", "Hello", nameof(Test_Guard_IsNotEqualTo_Fail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsBitwiseEqualTo_Ok()
        {
            Guard.IsBitwiseEqualTo(byte.MaxValue, byte.MaxValue, nameof(Test_Guard_IsBitwiseEqualTo_Ok));
            Guard.IsBitwiseEqualTo(MathF.PI, MathF.PI, nameof(Test_Guard_IsBitwiseEqualTo_Ok));
            Guard.IsBitwiseEqualTo(double.Epsilon, double.Epsilon, nameof(Test_Guard_IsBitwiseEqualTo_Ok));

            var guid = Guid.NewGuid();
            Guard.IsBitwiseEqualTo(guid, guid, nameof(Test_Guard_IsBitwiseEqualTo_Ok));

            // tests the >16 byte case where the loop is called
            var biggerThanLimit = new BiggerThanLimit(0, 3, ulong.MaxValue, ulong.MinValue);
            Guard.IsBitwiseEqualTo(biggerThanLimit, biggerThanLimit, nameof(Test_Guard_IsBitwiseEqualTo_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsBitwiseEqualTo_Size8Fail()
        {
            Guard.IsBitwiseEqualTo(double.PositiveInfinity, double.Epsilon, nameof(Test_Guard_IsBitwiseEqualTo_Size8Fail));
            Guard.IsBitwiseEqualTo(DateTime.Now, DateTime.Today, nameof(Test_Guard_IsBitwiseEqualTo_Size8Fail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsBitwiseEqualTo_Size16Fail()
        {
            Guard.IsBitwiseEqualTo(decimal.MaxValue, decimal.MinusOne, nameof(Test_Guard_IsBitwiseEqualTo_Size16Fail));
            Guard.IsBitwiseEqualTo(Guid.NewGuid(), Guid.NewGuid(), nameof(Test_Guard_IsBitwiseEqualTo_Size16Fail));
        }

        // a >16 byte struct for testing IsBitwiseEqual's pathway for >16 byte types
        private struct BiggerThanLimit
        {
            public BiggerThanLimit(ulong a, ulong b, ulong c, ulong d)
            {
                A = a;
                B = b;
                C = c;
                D = d;
            }

            public ulong A;

            public ulong B;

            public ulong C;

            public ulong D;
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsBitwiseEqualTo_SequenceEqualFail()
        {
            // tests the >16 byte case where the loop is called
            var biggerThanLimit0 = new BiggerThanLimit(0, 3, ulong.MaxValue, ulong.MinValue);
            var biggerThanLimit1 = new BiggerThanLimit(long.MaxValue + 1UL, 99, ulong.MaxValue ^ 0xF7UL, ulong.MinValue ^ 5555UL);

            Guard.IsBitwiseEqualTo(biggerThanLimit0, biggerThanLimit1, nameof(Test_Guard_IsBitwiseEqualTo_SequenceEqualFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsReferenceEqualTo_Ok()
        {
            var obj = new object();

            Guard.IsReferenceEqualTo(obj, obj, nameof(Test_Guard_IsReferenceEqualTo_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsReferenceEqualTo_Fail()
        {
            Guard.IsReferenceEqualTo(new object(), new object(), nameof(Test_Guard_IsReferenceEqualTo_Fail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsReferenceNotEqualTo_Ok()
        {
            Guard.IsReferenceNotEqualTo(new object(), new object(), nameof(Test_Guard_IsReferenceEqualTo_Fail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsReferenceNotEqualTo_Fail()
        {
            var obj = new object();

            Guard.IsReferenceNotEqualTo(obj, obj, nameof(Test_Guard_IsReferenceEqualTo_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsTrue_Ok()
        {
            Guard.IsTrue(true, nameof(Test_Guard_IsTrue_Ok));
            Guard.IsTrue(true, nameof(Test_Guard_IsTrue_Ok), "Hello world");
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsTrue_Fail()
        {
            Guard.IsTrue(false, nameof(Test_Guard_IsTrue_Fail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsTrue_Fail_WithMessage()
        {
            try
            {
                Guard.IsTrue(false, nameof(Test_Guard_IsTrue_Fail_WithMessage), "Hello world");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.Contains("\"Hello world\""));

                return;
            }

            // Compiler detects this is unreachable from attribute,
            // but we leave the assertion to double check that's valid
            Assert.Fail();
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsFalse_Ok()
        {
            Guard.IsFalse(false, nameof(Test_Guard_IsFalse_Ok));
            Guard.IsFalse(false, nameof(Test_Guard_IsFalse_Ok), "Hello world");
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_Guard_IsFalse_Fail()
        {
            Guard.IsFalse(true, nameof(Test_Guard_IsFalse_Fail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsFalse_Fail_WithMessage()
        {
            try
            {
                Guard.IsFalse(true, nameof(Test_Guard_IsFalse_Fail_WithMessage), "Hello world");
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.Contains("\"Hello world\""));

                return;
            }

            Assert.Fail();
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsLessThan_Ok()
        {
            Guard.IsLessThan(1, 2, nameof(Test_Guard_IsLessThan_Ok));
            Guard.IsLessThan(1.2f, 3.14f, nameof(Test_Guard_IsLessThan_Ok));
            Guard.IsLessThan(DateTime.Now, DateTime.MaxValue, nameof(Test_Guard_IsLessThan_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsLessThan_EqualsFalse()
        {
            Guard.IsLessThan(1, 1, nameof(Test_Guard_IsLessThan_EqualsFalse));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsLessThan_GreaterFalse()
        {
            Guard.IsLessThan(2, 1, nameof(Test_Guard_IsLessThan_GreaterFalse));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsLessThanOrEqualTo_Ok()
        {
            Guard.IsLessThanOrEqualTo(1, 2, nameof(Test_Guard_IsLessThanOrEqualTo_Ok));
            Guard.IsLessThanOrEqualTo(1, 1, nameof(Test_Guard_IsLessThanOrEqualTo_Ok));
            Guard.IsLessThanOrEqualTo(0.1f, MathF.PI, nameof(Test_Guard_IsLessThanOrEqualTo_Ok));
            Guard.IsLessThanOrEqualTo(MathF.PI, MathF.PI, nameof(Test_Guard_IsLessThanOrEqualTo_Ok));
            Guard.IsLessThanOrEqualTo(DateTime.Today, DateTime.MaxValue, nameof(Test_Guard_IsLessThanOrEqualTo_Ok));
            Guard.IsLessThanOrEqualTo(DateTime.MaxValue, DateTime.MaxValue, nameof(Test_Guard_IsLessThanOrEqualTo_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsLessThanOrEqualTo_False()
        {
            Guard.IsLessThanOrEqualTo(2, 1, nameof(Test_Guard_IsLessThanOrEqualTo_False));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsGreaterThan_Ok()
        {
            Guard.IsGreaterThan(2, 1, nameof(Test_Guard_IsGreaterThan_Ok));
            Guard.IsGreaterThan(3.14f, 2.1f, nameof(Test_Guard_IsGreaterThan_Ok));
            Guard.IsGreaterThan(DateTime.MaxValue, DateTime.Today, nameof(Test_Guard_IsGreaterThan_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsGreaterThan_EqualsFalse()
        {
            Guard.IsGreaterThan(1, 1, nameof(Test_Guard_IsGreaterThan_EqualsFalse));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsGreaterThan_LowerFalse()
        {
            Guard.IsGreaterThan(1, 2, nameof(Test_Guard_IsGreaterThan_LowerFalse));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsGreaterThanOrEqualTo_Ok()
        {
            Guard.IsGreaterThanOrEqualTo(2, 1, nameof(Test_Guard_IsGreaterThanOrEqualTo_Ok));
            Guard.IsGreaterThanOrEqualTo(1, 1, nameof(Test_Guard_IsGreaterThanOrEqualTo_Ok));
            Guard.IsGreaterThanOrEqualTo(MathF.PI, 1, nameof(Test_Guard_IsGreaterThanOrEqualTo_Ok));
            Guard.IsGreaterThanOrEqualTo(MathF.PI, MathF.PI, nameof(Test_Guard_IsGreaterThanOrEqualTo_Ok));
            Guard.IsGreaterThanOrEqualTo(DateTime.MaxValue, DateTime.Today, nameof(Test_Guard_IsGreaterThanOrEqualTo_Ok));
            Guard.IsGreaterThanOrEqualTo(DateTime.MaxValue, DateTime.MaxValue, nameof(Test_Guard_IsGreaterThanOrEqualTo_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsGreaterThanOrEqualTo_False()
        {
            Guard.IsGreaterThanOrEqualTo(1, 2, nameof(Test_Guard_IsGreaterThanOrEqualTo_False));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsInRange_Ok()
        {
            Guard.IsInRange(1, 0, 4, nameof(Test_Guard_IsInRange_Ok));
            Guard.IsInRange(0, 0, 2, nameof(Test_Guard_IsInRange_Ok));
            Guard.IsInRange(3.14f, 0, 10, nameof(Test_Guard_IsInRange_Ok));
            Guard.IsInRange(1, 0, 3.14f, nameof(Test_Guard_IsInRange_Ok));
            Guard.IsInRange(1, -50, 2, nameof(Test_Guard_IsInRange_Ok));
            Guard.IsInRange(-44, -44, 0, nameof(Test_Guard_IsInRange_Ok));
            Guard.IsInRange(3.14f, -float.Epsilon, 22, nameof(Test_Guard_IsInRange_Ok));
            Guard.IsInRange(1, int.MinValue, int.MaxValue, nameof(Test_Guard_IsInRange_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsInRange_LowerFail()
        {
            Guard.IsInRange(-3, 0, 4, nameof(Test_Guard_IsInRange_LowerFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsInRange_EqualFail()
        {
            Guard.IsInRange(0, 4, 4, nameof(Test_Guard_IsInRange_EqualFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsInRange_HigherFail()
        {
            Guard.IsInRange(0, 20, 4, nameof(Test_Guard_IsInRange_HigherFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsNotInRange_Ok()
        {
            Guard.IsNotInRange(0, 4, 10, nameof(Test_Guard_IsNotInRange_Ok));
            Guard.IsNotInRange(-4, 0, 2, nameof(Test_Guard_IsNotInRange_Ok));
            Guard.IsNotInRange(12f, 0, 10, nameof(Test_Guard_IsNotInRange_Ok));
            Guard.IsNotInRange(-1, 0, 3.14f, nameof(Test_Guard_IsNotInRange_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsNotInRange_LowerEqualFail()
        {
            Guard.IsNotInRange(0, 0, 4, nameof(Test_Guard_IsNotInRange_LowerEqualFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsNotInRange_InnerFail()
        {
            Guard.IsNotInRange(2, 0, 4, nameof(Test_Guard_IsNotInRange_InnerFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsInRangeFor_Ok()
        {
            Span<int> span = stackalloc int[10];

            Guard.IsInRangeFor(0, span, nameof(Test_Guard_IsInRangeFor_Ok));
            Guard.IsInRangeFor(4, span, nameof(Test_Guard_IsInRangeFor_Ok));
            Guard.IsInRangeFor(9, span, nameof(Test_Guard_IsInRangeFor_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsInRangeFor_LowerFail()
        {
            Span<int> span = stackalloc int[10];

            Guard.IsInRangeFor(-2, span, nameof(Test_Guard_IsInRangeFor_LowerFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsInRangeFor_EqualFail()
        {
            Span<int> span = stackalloc int[10];

            Guard.IsInRangeFor(10, span, nameof(Test_Guard_IsInRangeFor_EqualFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsInRangeFor_HigherFail()
        {
            Span<int> span = stackalloc int[10];

            Guard.IsInRangeFor(99, span, nameof(Test_Guard_IsInRangeFor_HigherFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsNotInRangeFor_Ok()
        {
            Span<int> span = stackalloc int[10];

            Guard.IsNotInRangeFor(-2, span, nameof(Test_Guard_IsNotInRangeFor_Ok));
            Guard.IsNotInRangeFor(10, span, nameof(Test_Guard_IsNotInRangeFor_Ok));
            Guard.IsNotInRangeFor(2222, span, nameof(Test_Guard_IsNotInRangeFor_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsNotInRangeFor_LowerFail()
        {
            Span<int> span = stackalloc int[10];

            Guard.IsNotInRangeFor(0, span, nameof(Test_Guard_IsNotInRangeFor_LowerFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsNotInRangeFor_MiddleFail()
        {
            Span<int> span = stackalloc int[10];

            Guard.IsNotInRangeFor(6, span, nameof(Test_Guard_IsNotInRangeFor_MiddleFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsBetween_Ok()
        {
            Guard.IsBetween(1, 0, 4, nameof(Test_Guard_IsBetween_Ok));
            Guard.IsBetween(3.14f, 0, 10, nameof(Test_Guard_IsBetween_Ok));
            Guard.IsBetween(1, 0, 3.14, nameof(Test_Guard_IsBetween_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsBetween_LowerFail()
        {
            Guard.IsBetween(-1, 0, 4, nameof(Test_Guard_IsBetween_LowerFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsBetween_EqualFail()
        {
            Guard.IsBetween(0, 0, 4, nameof(Test_Guard_IsBetween_EqualFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsBetween_HigherFail()
        {
            Guard.IsBetween(6, 0, 4, nameof(Test_Guard_IsBetween_HigherFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsNotBetween_Ok()
        {
            Guard.IsNotBetween(0, 0, 4, nameof(Test_Guard_IsNotBetween_Ok));
            Guard.IsNotBetween(10, 0, 10, nameof(Test_Guard_IsNotBetween_Ok));
            Guard.IsNotBetween(-5, 0, 3.14, nameof(Test_Guard_IsNotBetween_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsNotBetween_Fail()
        {
            Guard.IsNotBetween(1, 0, 4, nameof(Test_Guard_IsNotBetween_Fail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsBetweenOrEqualTo_Ok()
        {
            Guard.IsBetweenOrEqualTo(1, 0, 4, nameof(Test_Guard_IsBetweenOrEqualTo_Ok));
            Guard.IsBetweenOrEqualTo(10, 0, 10, nameof(Test_Guard_IsBetweenOrEqualTo_Ok));
            Guard.IsBetweenOrEqualTo(1, 0, 3.14, nameof(Test_Guard_IsBetweenOrEqualTo_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsBetweenOrEqualTo_LowerFail()
        {
            Guard.IsBetweenOrEqualTo(-1, 0, 4, nameof(Test_Guard_IsBetweenOrEqualTo_LowerFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsBetweenOrEqualTo_HigherFail()
        {
            Guard.IsBetweenOrEqualTo(6, 0, 4, nameof(Test_Guard_IsBetweenOrEqualTo_HigherFail));
        }

        [TestCategory("Guard")]
        [TestMethod]
        public void Test_Guard_IsNotBetweenOrEqualTo_Ok()
        {
            Guard.IsNotBetweenOrEqualTo(6, 0, 4, nameof(Test_Guard_IsNotBetweenOrEqualTo_Ok));
            Guard.IsNotBetweenOrEqualTo(-10, 0, 10, nameof(Test_Guard_IsNotBetweenOrEqualTo_Ok));
        }

        [TestCategory("Guard")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Test_Guard_IsNotBetweenOrEqualTo_Fail()
        {
            Guard.IsNotBetweenOrEqualTo(3, 0, 4, nameof(Test_Guard_IsNotBetweenOrEqualTo_Fail));
        }
    }
}