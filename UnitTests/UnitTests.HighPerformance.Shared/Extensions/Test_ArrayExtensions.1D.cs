// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.HighPerformance;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Extensions
{
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601", Justification = "Partial test class")]
    public partial class Test_ArrayExtensions
    {
        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_DangerousGetReference()
        {
            string[] tokens = "aa,bb,cc,dd,ee,ff,gg,hh,ii".Split(',');

            // In all these "DangerousGetReference" tests, we need to ensure that a reference to a given
            // item within an array is effectively the one corresponding to the one whe expect, which is
            // either a reference to the first item if we use "DangerousGetReference", or one to the n-th
            // item if we use "DangerousGetReferenceAt". So all these tests just invoke the API and then
            // compare the returned reference against an existing baseline (like the built-in array indexer)
            // to ensure that the two are the same. These are all managed references, so no need for pinning.
            ref string r0 = ref Unsafe.AsRef(tokens.DangerousGetReference());
            ref string r1 = ref Unsafe.AsRef(tokens[0]);

            Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_DangerousGetReferenceAt_Zero()
        {
            string[] tokens = "aa,bb,cc,dd,ee,ff,gg,hh,ii".Split(',');

            ref string r0 = ref Unsafe.AsRef(tokens.DangerousGetReference());
            ref string r1 = ref Unsafe.AsRef(tokens.DangerousGetReferenceAt(0));

            Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
        }

        [TestCategory("ArrayExtensions")]
        [TestMethod]
        public void Test_ArrayExtensions_DangerousGetReferenceAt_Index()
        {
            string[] tokens = "aa,bb,cc,dd,ee,ff,gg,hh,ii".Split(',');

            ref string r0 = ref Unsafe.AsRef(tokens.DangerousGetReferenceAt(5));
            ref string r1 = ref Unsafe.AsRef(tokens[5]);

            Assert.IsTrue(Unsafe.AreSame(ref r0, ref r1));
        }
    }
}