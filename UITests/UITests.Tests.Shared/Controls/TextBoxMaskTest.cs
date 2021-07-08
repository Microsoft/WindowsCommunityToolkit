// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Reflection;
using Microsoft.UI.Xaml.Tests.MUXControls.InteractionTests.Common;
using Microsoft.UI.Xaml.Tests.MUXControls.InteractionTests.Infra;
using Microsoft.Windows.Apps.Test.Foundation.Controls;

#if USING_TAEF
using WEX.Logging.Interop;
using WEX.TestExecution;
using WEX.TestExecution.Markup;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace UITests.Tests
{
    [TestClass]
    public class TextBoxMaskTest : UITestBase
    {
        [ClassInitialize]
        [TestProperty("RunAs", "User")]
        [TestProperty("Classification", "ScenarioTestSuite")]
        [TestProperty("Platform", "Any")]
        public static void ClassInitialize(TestContext testContext)
        {
            TestEnvironment.Initialize(testContext, UITestsAppSampleApp);
        }

        [TestMethod]
        [TestPage("TextBoxMaskTestPage")]
        public void TestTextBoxMaskBinding_Property()
        {
            var initialValue = FindElement.ById<TextBlock>("InitialValueTextBlock").GetText();
            var textBox = FindElement.ById<Edit>("TextBox");

            Verify.AreEqual(initialValue, textBox.GetText());

            var changeButton = FindElement.ById<Button>("ChangeButton");

            changeButton.Click();
            Wait.ForIdle();

            var newValue = FindElement.ById<TextBlock>("NewValueTextBlock").GetText();

            Verify.AreEqual(newValue, textBox.GetText());
        }
    }
}