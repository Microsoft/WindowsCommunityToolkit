// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using CommunityToolkit.HighPerformance;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance
{
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649", Justification = "Test class for generic type")]
    public class Test_RefOfT
    {
        [TestCategory("RefOfT")]
        [TestMethod]
#if WINDOWS_UWP
        public void Test_RefOfT_CreateRefOfT()
        {
            var model = new FieldOwner { Value = 1 };
            var reference = new Ref<int>(model, ref model.Value);

            Assert.IsTrue(Unsafe.AreSame(ref model.Value, ref reference.Value));

            reference.Value++;

            Assert.AreEqual(model.Value, 2);
        }

        /// <summary>
        /// A dummy model that owns an <see cref="int"/> field.
        /// </summary>
        private sealed class FieldOwner
        {
            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401", Justification = "Quick ref access for tests")]
            public int Value;
        }
#else
        public void Test_RefOfT_CreateRefOfT()
        {
            int value = 1;
            var reference = new Ref<int>(ref value);

            Assert.IsTrue(Unsafe.AreSame(ref value, ref reference.Value));

            reference.Value++;

            Assert.AreEqual(value, 2);
        }
#endif
    }
}