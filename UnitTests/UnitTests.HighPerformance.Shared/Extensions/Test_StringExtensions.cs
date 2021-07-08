// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Extensions
{
    [TestClass]
    public class Test_StringExtensions
    {
        [TestCategory("StringExtensions")]
        [TestMethod]
        public void Test_StringExtensions_DangerousGetReference()
        {
            string text = "Hello, world!";

            ref char r0 = ref text.DangerousGetReference();
            ref char r1 = ref Unsafe.AsRef(text.AsSpan()[0]);

            Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
        }

        [TestCategory("StringExtensions")]
        [TestMethod]
        public void Test_StringExtensions_DangerousGetReferenceAt_Zero()
        {
            string text = "Hello, world!";

            ref char r0 = ref text.DangerousGetReference();
            ref char r1 = ref text.DangerousGetReferenceAt(0);

            Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
        }

        [TestCategory("StringExtensions")]
        [TestMethod]
        public void Test_StringExtensions_DangerousGetReferenceAt_Index()
        {
            string text = "Hello, world!";

            ref char r0 = ref text.DangerousGetReferenceAt(5);
            ref char r1 = ref Unsafe.AsRef(text.AsSpan()[5]);

            Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
        }
    }
}