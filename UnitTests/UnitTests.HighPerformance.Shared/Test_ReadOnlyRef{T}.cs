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
    public class Test_ReadOnlyRefOfT
    {
        [TestCategory("ReadOnlyRefOfT")]
        [TestMethod]
#if WINDOWS_UWP
        public void Test_RefOfT_CreateRefOfT()
        {
            var model = new ReadOnlyFieldOwner();
            var reference = new ReadOnlyRef<int>(model, model.Value);

            Assert.IsTrue(Unsafe.AreSame(ref Unsafe.AsRef(model.Value), ref Unsafe.AsRef(reference.Value)));
        }

        /// <summary>
        /// A dummy model that owns an <see cref="int"/> field.
        /// </summary>
        private sealed class ReadOnlyFieldOwner
        {
            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401", Justification = "Ref readonly access for tests")]
            public readonly int Value = 1;
        }
#else
        public void Test_RefOfT_CreateRefOfT()
        {
            int value = 1;
            var reference = new ReadOnlyRef<int>(value);

            Assert.IsTrue(Unsafe.AreSame(ref value, ref Unsafe.AsRef(reference.Value)));
        }
#endif
    }
}