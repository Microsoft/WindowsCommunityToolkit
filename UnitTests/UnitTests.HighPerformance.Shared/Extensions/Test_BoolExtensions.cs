// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Extensions
{
    [TestClass]
    public class Test_BoolExtensions
    {
        [TestCategory("BoolExtensions")]
        [TestMethod]
        public void Test_BoolExtensions_True()
        {
            // There tests all just run a couple of boolean expressions and validate that the extension
            // correctly produces either 1 or 0 depending on whether the expression was true or false.
            Assert.AreEqual(1, true.ToByte(), nameof(Test_BoolExtensions_True));
            Assert.AreEqual(1, (DateTime.Now.Year > 0).ToByte(), nameof(Test_BoolExtensions_True));
        }

        [TestCategory("BoolExtensions")]
        [TestMethod]
        public void Test_BoolExtensions_False()
        {
            Assert.AreEqual(0, false.ToByte(), nameof(Test_BoolExtensions_False));
            Assert.AreEqual(0, (DateTime.Now.Year > 3000).ToByte(), nameof(Test_BoolExtensions_False));
        }

        [TestCategory("BoolExtensions")]
        [TestMethod]
        [DataRow(true, -1)]
        [DataRow(false, 0)]
        public void Test_BoolExtensions_ToBitwiseMask32(bool value, int result)
        {
            Assert.AreEqual(value.ToBitwiseMask32(), result);
        }

        [TestCategory("BoolExtensions")]
        [TestMethod]
        [DataRow(true, -1)]
        [DataRow(false, 0)]
        public void Test_BoolExtensions_ToBitwiseMask64(bool value, long result)
        {
            Assert.AreEqual(value.ToBitwiseMask64(), result);
        }
    }
}
