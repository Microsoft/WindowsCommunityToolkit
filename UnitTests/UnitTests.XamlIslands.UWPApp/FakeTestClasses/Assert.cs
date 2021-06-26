// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public static class Assert
    {
        internal static void AreEqual(object expected, object actual, string message = "")
        {
            if (!expected.Equals(actual))
            {
                throw new Exception($"Assert.AreEqual failed. Expected:<{expected}>. Actual:<{actual}>. {message}");
            }
        }

        internal static void IsNotNull(object actual, string message = "")
        {
            if (actual == null)
            {
                throw new Exception($"Assert.IsNotNull failed. Expected:<value>. Actual:<null>. {message}");
            }
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Internal Types used to mimic unit test framework.")]
    public static class CollectionAssert
    {
        internal static void AreEquivalent<T>(T[] expected, T[] actual, string message = "")
        {
            if (expected.Length != actual.Length || expected.Intersect(actual).Count() != actual.Length)
            {
                throw new Exception($"CollectionAssert.AreEquivalent failed. Expected:<{expected.Length}>. Actual:<{actual.Length}>. {message}"); // TODO: Not sure what the actual message displays here...
            }
        }
    }
}