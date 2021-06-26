// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NETCOREAPP3_1 || NET5_0

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance
{
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649", Justification = "Test class for generic type")]
    public class Test_NullableReadOnlyRefOfT
    {
        [TestCategory("NullableReadOnlyRefOfT")]
        [TestMethod]
        public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_Ok()
        {
            int value = 1;
            var reference = new NullableReadOnlyRef<int>(value);

            Assert.IsTrue(reference.HasValue);
            Assert.IsTrue(Unsafe.AreSame(ref value, ref Unsafe.AsRef(reference.Value)));
        }

        [TestCategory("NullableReadOnlyRefOfT")]
        [TestMethod]
        public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_Null()
        {
            Assert.IsFalse(default(NullableReadOnlyRef<int>).HasValue);
            Assert.IsFalse(NullableReadOnlyRef<int>.Null.HasValue);

            Assert.IsFalse(default(NullableReadOnlyRef<string>).HasValue);
            Assert.IsFalse(NullableReadOnlyRef<string>.Null.HasValue);
        }

        [TestCategory("NullableReadOnlyRefOfT")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_Null_Exception()
        {
            NullableReadOnlyRef<int> reference = default;

            _ = reference.Value;
        }

        [TestCategory("NullableReadOnlyRefOfT")]
        [TestMethod]
        public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_ImplicitRefCast()
        {
            int value = 42;
            var reference = new Ref<int>(ref value);
            NullableReadOnlyRef<int> nullableRef = reference;

            Assert.IsTrue(nullableRef.HasValue);
            Assert.IsTrue(Unsafe.AreSame(ref reference.Value, ref Unsafe.AsRef(nullableRef.Value)));
        }

        [TestCategory("NullableReadOnlyRefOfT")]
        [TestMethod]
        public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_ImplicitReadOnlyRefCast()
        {
            int value = 42;
            var reference = new ReadOnlyRef<int>(value);
            NullableReadOnlyRef<int> nullableRef = reference;

            Assert.IsTrue(nullableRef.HasValue);
            Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(reference.Value), ref Unsafe.AsRef(nullableRef.Value)));
        }

        [TestCategory("NullableReadOnlyRefOfT")]
        [TestMethod]
        public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_ImplicitNullableRefCast()
        {
            int value = 42;
            var reference = new NullableRef<int>(ref value);
            NullableReadOnlyRef<int> nullableRef = reference;

            Assert.IsTrue(nullableRef.HasValue);
            Assert.IsTrue(Unsafe.AreSame(ref reference.Value, ref Unsafe.AsRef(nullableRef.Value)));
        }

        [TestCategory("NullableReadOnlyRefOfT")]
        [TestMethod]
        public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_ExplicitCastOfT()
        {
            int value = 42;
            var reference = new NullableRef<int>(ref value);

            Assert.AreEqual(value, (int)reference);
        }

        [TestCategory("NullableReadOnlyRefOfT")]
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Test_NullableReadOnlyRefOfT_CreateNullableReadOnlyRefOfT_ExplicitCastOfT_Exception()
        {
            NullableRef<int> invalid = default;

            _ = (int)invalid;
        }
    }
}

#endif