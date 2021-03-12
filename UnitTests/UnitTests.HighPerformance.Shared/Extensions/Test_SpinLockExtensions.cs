// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Extensions
{
    [TestClass]
    public class Test_SpinLockExtensions
    {
        [TestCategory("SpinLockExtensions")]
        [TestMethod]
        public unsafe void Test_ArrayExtensions_Pointer()
        {
            SpinLock spinLock = default;
            SpinLock* p = &spinLock;

            int sum = 0;

            Parallel.For(0, 1000, i =>
            {
                for (int j = 0; j < 10; j++)
                {
                    using (SpinLockExtensions.Enter(p))
                    {
                        sum++;
                    }
                }
            });

            Assert.AreEqual(sum, 1000 * 10);
        }

        [TestCategory("SpinLockExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_Ref()
        {
            var spinLockOwner = new SpinLockOwner();

            int sum = 0;

            Parallel.For(0, 1000, i =>
            {
                for (int j = 0; j < 10; j++)
                {
#if WINDOWS_UWP
                    using (SpinLockExtensions.Enter(spinLockOwner, ref spinLockOwner.Lock))
#else
                    using (spinLockOwner.Lock.Enter())
#endif
                    {
                        sum++;
                    }
                }
            });

            Assert.AreEqual(sum, 1000 * 10);
        }

        /// <summary>
        /// A dummy model that owns a <see cref="SpinLock"/> object.
        /// </summary>
        private sealed class SpinLockOwner
        {
            [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401", Justification = "Quick ref access for tests")]
            public SpinLock Lock;
        }
    }
}
