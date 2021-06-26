// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.Connectivity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Helpers
{
    //// TODO: Need Mock to WinRT Issue #3196 - https://github.com/windows-toolkit/WindowsCommunityToolkit/issues/3196
    [TestClass]
    public class Test_ConnectionHelper
    {
        [Ignore]
        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ConnectionHelper_IsInternetOnMeteredConnection()
        {
            Assert.IsFalse(NetworkHelper.Instance.ConnectionInformation.IsInternetOnMeteredConnection);
        }

        [Ignore]
        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ConnectionHelper_IsInternetAvailable()
        {
            Assert.IsTrue(NetworkHelper.Instance.ConnectionInformation.IsInternetAvailable);
        }
    }
}