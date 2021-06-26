// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace UnitTests.UI.Controls
{
    [TestClass]
    public class Test_TokenizingTextBox_General
    {
        [TestCategory("Test_TokenizingTextBox_General")]
        [UITestMethod]
        public void Test_Clear()
        {
            var treeRoot = XamlReader.Load(
@"<Page
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:controls=""using:Microsoft.Toolkit.Uwp.UI.Controls"">

    <controls:TokenizingTextBox x:Name=""tokenboxname"">
    </controls:TokenizingTextBox>

</Page>") as FrameworkElement;

            Assert.IsNotNull(treeRoot, "Could not load XAML tree.");

            var tokenBox = treeRoot.FindChild("tokenboxname") as TokenizingTextBox;

            Assert.IsNotNull(tokenBox, "Could not find TokenizingTextBox in tree.");

            Assert.AreEqual(tokenBox.Items.Count, 1, "Token default items failed");

            tokenBox.AddTokenItem("TokenItem1");
            tokenBox.AddTokenItem("TokenItem2");
            tokenBox.AddTokenItem("TokenItem3");
            tokenBox.AddTokenItem("TokenItem4");

            Assert.AreEqual(tokenBox.Items.Count, 5, "Token Add count failed");

            // now test clear
            Assert.IsTrue(tokenBox.ClearAsync().Wait(200), "Failed to wait for Clear() to finish");

            Assert.AreEqual(tokenBox.Items.Count, 1, "Clear Failed to clear");

            // test cancelled clear
            tokenBox.AddTokenItem("TokenItem1");
            tokenBox.AddTokenItem("TokenItem2");
            tokenBox.AddTokenItem("TokenItem3");
            tokenBox.AddTokenItem("TokenItem4");

            Assert.AreEqual(tokenBox.Items.Count, 5, "Token Add count failed");

            tokenBox.TokenItemRemoving += (sender, args) => { args.Cancel = true; };

            Assert.IsTrue(tokenBox.ClearAsync().Wait(200), "Failed to wait for Clear() to finish");

            Assert.AreEqual(tokenBox.Items.Count, 5, "Cancelled Clear Failed ");
        }
    }
}