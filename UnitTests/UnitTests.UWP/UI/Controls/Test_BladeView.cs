// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml.Automation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Windows.UI.Xaml.Automation.Peers;
using Microsoft.Toolkit.Uwp.UI.Automation.Peers;

namespace UnitTests.UWP.UI.Controls
{
    [TestClass]
    [TestCategory("Test_BladeView")]
    public class Test_BladeView
    {
        [UITestMethod]
        public void ShouldConfigureBladeViewAutomationPeer()
        {
            const string automationName = "MyAutomationBlades";
            const string name = "MyBlades";

            var bladeView = new BladeView();
            var bladeViewAutomationPeer = FrameworkElementAutomationPeer.CreatePeerForElement(bladeView) as BladeViewAutomationPeer;

            Assert.IsNotNull(bladeViewAutomationPeer, "Verify that the AutomationPeer is BladeViewAutomationPeer.");

            bladeView.Name = name;
            Assert.IsTrue(bladeViewAutomationPeer.GetName().Contains(name), "Verify that the UIA name contains the given Name of the BladeView.");

            bladeView.SetValue(AutomationProperties.NameProperty, automationName);
            Assert.IsTrue(bladeViewAutomationPeer.GetName().Contains(automationName), "Verify that the UIA name contains the given AutomationProperties.Name of the BladeView.");
        }
    }
}