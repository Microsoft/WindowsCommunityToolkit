// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.UI.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace UnitTests.Converters
{
    [TestClass]
    public class Test_EmptyCollectionToObjectConverter
    {
        private static readonly object NullCollection = null;
        private static readonly object EmptyCollection = Array.Empty<object>();
        private static readonly object NotEmptyCollection = new[] { new object() };

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNullCollectionToVisibility()
        {
            var converter = new EmptyCollectionToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(NullCollection, typeof(Visibility), null, "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNotEmptyCollectionToVisibility()
        {
            var converter = new EmptyCollectionToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(EmptyCollection, typeof(Visibility), null, "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertEmptyCollectionToVisibility()
        {
            var converter = new EmptyCollectionToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(EmptyCollection, typeof(Visibility), null, "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNotEmptyCollectionToVisibilityWithNegateTrue()
        {
            var converter = new EmptyCollectionToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(NotEmptyCollection, typeof(Visibility), "true", "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertEmptyCollectionToVisibilityWithNegateTrue()
        {
            var converter = new EmptyCollectionToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(EmptyCollection, typeof(Visibility), "true", "en-us");
            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNotEmptyCollectionToVisibilityWithNegateFalse()
        {
            var converter = new EmptyCollectionToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(NotEmptyCollection, typeof(Visibility), "false", "en-us");
            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertEmptyCollectionToVisibilityWithNegateFalse()
        {
            var converter = new EmptyCollectionToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(EmptyCollection, typeof(Visibility), "false", "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNotEmptyCollectionToVisibilityWithInvalidNegate()
        {
            var converter = new EmptyCollectionToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(NotEmptyCollection, typeof(Visibility), 42, "en-us");
            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertEmptyCollectionToVisibilityWithInvalidNegate()
        {
            var converter = new EmptyCollectionToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(EmptyCollection, typeof(Visibility), 42, "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNotEmptyCollectionToBrush()
        {
            var greenBrush = new SolidColorBrush(Colors.Green);
            var redBrush = new SolidColorBrush(Colors.Red);
            var converter = new EmptyCollectionToObjectConverter
            {
                NotEmptyValue = greenBrush,
                EmptyValue = redBrush
            };
            var result = converter.Convert(NotEmptyCollection, typeof(Brush), null, "en-us");
            Assert.AreEqual(greenBrush, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertEmptyCollectionToBrush()
        {
            var greenBrush = new SolidColorBrush(Colors.Green);
            var redBrush = new SolidColorBrush(Colors.Red);
            var converter = new EmptyCollectionToObjectConverter
            {
                NotEmptyValue = greenBrush,
                EmptyValue = redBrush
            };
            var result = converter.Convert(EmptyCollection, typeof(Brush), null, "en-us");
            Assert.AreEqual(redBrush, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNotEmptyCollectionToBitmapImageWithTypeConversion()
        {
            var converter = new EmptyCollectionToObjectConverter
            {
                NotEmptyValue = "ms-appx:///Assets/NotEmptyCollection.png",
                EmptyValue = "ms-appx:///Assets/EmptyCollection.png"
            };

            var result = converter.Convert(NotEmptyCollection, typeof(ImageSource), null, "en-us");
            Assert.IsInstanceOfType(result, typeof(BitmapImage));
            Assert.AreEqual(converter.NotEmptyValue, ((BitmapImage)result).UriSource.ToString());
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNotEmptyCollectionToBrushWithTypeConversion()
        {
            var converter = new EmptyCollectionToObjectConverter
            {
                NotEmptyValue = "Green",
                EmptyValue = "Red"
            };

            var result = converter.Convert(NotEmptyCollection, typeof(Brush), null, "en-us");
            Assert.IsInstanceOfType(result, typeof(SolidColorBrush));
            Assert.AreEqual(Colors.Green, ((SolidColorBrush)result).Color);
        }
    }
}