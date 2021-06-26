// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.HighPerformance.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Extensions
{
    [TestClass]
    public class Test_ObjectMarshal
    {
        [TestCategory("ObjectMarshal")]
        [TestMethod]
        public void Test_DangerousGetObjectDataByteOffset()
        {
            var a = new TestClass { Number = 42, Character = 'a', Text = "Hello" };

            IntPtr ptr = ObjectMarshal.DangerousGetObjectDataByteOffset(a, ref a.Number);

            ref int number = ref ObjectMarshal.DangerousGetObjectDataReferenceAt<int>(a, ptr);

            Assert.IsTrue(Unsafe.AreSame(ref a.Number, ref number));

            ptr = ObjectMarshal.DangerousGetObjectDataByteOffset(a, ref a.Character);

            ref char character = ref ObjectMarshal.DangerousGetObjectDataReferenceAt<char>(a, ptr);

            Assert.IsTrue(Unsafe.AreSame(ref a.Character, ref character));

            ptr = ObjectMarshal.DangerousGetObjectDataByteOffset(a, ref a.Text);

            ref string text = ref ObjectMarshal.DangerousGetObjectDataReferenceAt<string>(a, ptr);

            Assert.IsTrue(Unsafe.AreSame(ref a.Text, ref text));
        }

        internal class TestClass
        {
#pragma warning disable SA1401 // Fields should be private
            public int Number;
            public char Character;
            public string Text;
#pragma warning restore SA1401
        }

        [TestCategory("ObjectMarshal")]
        [TestMethod]
        public void Test_BoxOfT_PrimitiveTypes()
        {
            Test(true);
            Test(false);
            Test<byte>(27);
            Test('a');
            Test(4221124);
            Test(3.14f);
            Test(8394324ul);
            Test(184013.234324);
        }

        [TestCategory("ObjectMarshal")]
        [TestMethod]
        public void Test_BoxOfT_OtherTypes()
        {
            Test(DateTime.Now);
            Test(Guid.NewGuid());
        }

        internal struct TestStruct : IEquatable<TestStruct>
        {
            public int Number;
            public char Character;
            public string Text;

            /// <inheritdoc/>
            public bool Equals(TestStruct other)
            {
                return
                    this.Number == other.Number &&
                    this.Character == other.Character &&
                    this.Text == other.Text;
            }
        }

        [TestCategory("ObjectMarshal")]
        [TestMethod]
        public void TestBoxOfT_CustomStruct()
        {
            var a = new TestStruct { Number = 42, Character = 'a', Text = "Hello" };
            var b = new TestStruct { Number = 38293, Character = 'z', Text = "World" };

            Test(a);
            Test(b);
        }

        /// <summary>
        /// Tests the extensions type for a given value.
        /// </summary>
        /// <typeparam name="T">The type to test.</typeparam>
        /// <param name="value">The initial <typeparamref name="T"/> value.</param>
        private static void Test<T>(T value)
            where T : struct, IEquatable<T>
        {
            object obj = value;

            bool success = obj.TryUnbox(out T result);

            Assert.IsTrue(success);
            Assert.AreEqual(value, result);

            success = obj.TryUnbox(out decimal test);

            Assert.IsFalse(success);
            Assert.AreEqual(test, default);

            result = ObjectMarshal.DangerousUnbox<T>(obj);

            Assert.AreEqual(value, result);
        }
    }
}