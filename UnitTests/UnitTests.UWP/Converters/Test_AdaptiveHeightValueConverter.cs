// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UnitTests.Converters
{
    [TestClass]
    public class Test_AdaptiveHeightValueConverter
    {
        [TestCategory("Converters")]
        [TestMethod]
        public void Convert_ValueIsNull_ReturnsNaN()
        {
            var converter = new AdaptiveHeightValueConverter();
            var result = converter.Convert(null, null, null, null);
            Assert.AreEqual(result, double.NaN);
        }

        [TestCategory("Converters")]
        [TestMethod]
        public void Convert_ParameterIsNull_ReturnsValue()
        {
            var converter = new AdaptiveHeightValueConverter();
            double value = 100;
            var result = converter.Convert(value, null, null, null);
            Assert.AreEqual(result, value);
        }

        [TestCategory("Converters")]
        [TestMethod]
        public void Convert_GridViewIsNull_ReturnsValue()
        {
            var converter = new AdaptiveHeightValueConverter();
            double value = 100;
            var result = converter.Convert(value, null, null, null);
            Assert.AreEqual(result, value);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Convert_GridViewWithItemContainerStyle_ReturnsHeightPlusItemMargin()
        {
            var converter = new AdaptiveHeightValueConverter();
            double value = 100;
            var gridView = new AdaptiveGridView();

            var margin = new Thickness(10);
            var style = new Style(typeof(GridViewItem));
            style.Setters.Add(new Setter(GridViewItem.MarginProperty, margin));
            gridView.ItemContainerStyle = style;

            var result = converter.Convert(value, null, gridView, null);
            Assert.AreEqual(result, value + margin.Bottom + margin.Top);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Convert_GridViewWithNoItemContainerStyleAndNoItems_ReturnsHeightPlusDefaultItemMargin()
        {
            var converter = new AdaptiveHeightValueConverter();
            double value = 100;
            var gridView = new AdaptiveGridView { ItemContainerStyle = null };
            var result = converter.Convert(value, null, gridView, null);
            Assert.AreEqual(result, value + converter.DefaultItemMargin.Bottom + converter.DefaultItemMargin.Top);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Convert_GridViewWithPadding_ReturnsHeightPlusPadding()
        {
            var converter = new AdaptiveHeightValueConverter { DefaultItemMargin = new Thickness(0) };
            double value = 100;
            var gridView = new AdaptiveGridView { ItemContainerStyle = null, Padding = new Thickness(10) };
            var result = converter.Convert(value, null, gridView, null);
            Assert.AreEqual(result, value + gridView.Padding.Bottom + gridView.Padding.Top);
        }
    }
}