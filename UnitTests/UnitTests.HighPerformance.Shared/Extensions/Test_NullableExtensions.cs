// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET5_0

using System.Numerics;
using Microsoft.Toolkit.HighPerformance.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Extensions
{
    [TestClass]
    public class Test_NullableExtensions
    {
        [TestCategory("NullableExtensions")]
        [TestMethod]
        public void Test_NullableExtensions_DangerousGetReference()
        {
            static void Test<T>(T before, T after)
                where T : struct
            {
                T? nullable = before;
                ref T reference = ref nullable.DangerousGetValueOrDefaultReference();

                Assert.AreEqual(nullable.Value, before);

                reference = after;

                Assert.AreEqual(nullable.Value, after);
            }

            Test(0, 42);
            Test(1.3f, 3.14f);
            Test(0.555, 8.49);
            Test(Vector4.Zero, new Vector4(1, 5.55f, 2, 3.14f));
            Test(Matrix4x4.Identity, Matrix4x4.CreateOrthographic(35, 88.34f, 9.99f, 24.6f));
        }
    }
}

#endif
