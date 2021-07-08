// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Microsoft.UI.Xaml;

namespace UnitTests.Converters
{
    [TestClass]
    public class Test_TypeToObjectConverter
    {
        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_StringConvertStringToVisibility()
        {
            var converter = new TypeToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed,
                Type = typeof(string)
            };
            var result = converter.Convert("anything", typeof(Visibility), null, "en-us");
            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_StringDoesntConvertBoolToVisibility()
        {
            var converter = new TypeToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed,
                Type = typeof(string)
            };
            var result = converter.Convert(true, typeof(Visibility), null, "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_StringConvertStringToVisibilityWithNegateTrue()
        {
            var converter = new TypeToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed,
                Type = typeof(string)
            };
            var result = converter.Convert("anything", typeof(Visibility), "true", "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }
    }
}