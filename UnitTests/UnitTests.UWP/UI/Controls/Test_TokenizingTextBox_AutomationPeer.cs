// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Automation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Xaml.Automation.Peers;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Automation.Peers;
using Microsoft.Toolkit.Uwp.UI.Controls;

namespace UnitTests.UWP.UI.Controls
{
    [TestClass]
    [TestCategory("Test_TokenizingTextBox")]
    public class Test_TokenizingTextBox_AutomationPeer : VisualUITestBase
    {
        [TestMethod]
        public async Task ShouldConfigureTokenizingTextBoxAutomationPeerAsync()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                const string expectedAutomationName = "MyAutomationName";
                const string expectedName = "MyName";
                const string expectedValue = "Wor";

                var items = new ObservableCollection<TokenizingTextBoxTestItem> { new() { Title = "Hello" }, new() { Title = "World" } };

                var tokenizingTextBox = new TokenizingTextBox { ItemsSource = items };

                await SetTestContentAsync(tokenizingTextBox);

                var tokenizingTextBoxAutomationPeer =
                    FrameworkElementAutomationPeer.CreatePeerForElement(tokenizingTextBox) as TokenizingTextBoxAutomationPeer;

                Assert.IsNotNull(tokenizingTextBoxAutomationPeer, "Verify that the AutomationPeer is TokenizingTextBoxAutomationPeer.");

                // Asserts the automation peer name based on the Automation Property Name value. 
                tokenizingTextBox.SetValue(AutomationProperties.NameProperty, expectedAutomationName);
                Assert.IsTrue(tokenizingTextBoxAutomationPeer.GetName().Contains(expectedAutomationName), "Verify that the UIA name contains the given AutomationProperties.Name of the TokenizingTextBox.");

                // Asserts the automation peer name based on the element Name property.
                tokenizingTextBox.Name = expectedName;
                Assert.IsTrue(tokenizingTextBoxAutomationPeer.GetName().Contains(expectedName), "Verify that the UIA name contains the given Name of the TokenizingTextBox.");

                tokenizingTextBoxAutomationPeer.SetValue(expectedValue);
                Assert.IsTrue(tokenizingTextBoxAutomationPeer.Value.Equals(expectedValue), "Verify that the Value contains the given Text of the TokenizingTextBox.");
            });
        }

        [TestMethod]
        public async Task ShouldThrowElementNotEnabledExceptionIfValueSetWhenDisabled()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                const string expectedValue = "Wor";

                var tokenizingTextBox = new TokenizingTextBox { IsEnabled = false };

                await SetTestContentAsync(tokenizingTextBox);

                var tokenizingTextBoxAutomationPeer =
                    FrameworkElementAutomationPeer.CreatePeerForElement(tokenizingTextBox) as TokenizingTextBoxAutomationPeer;

                Assert.ThrowsException<ElementNotEnabledException>(() =>
                {
                    tokenizingTextBoxAutomationPeer.SetValue(expectedValue);
                });
            });
        }

        public class TokenizingTextBoxTestItem
        {
            public string Title { get; set; }

            public override string ToString()
            {
                return Title;
            }
        }
    }
}