// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    public class Test_EmptyStringToObjectConverter
    {
        private static readonly object NullString = null;
        private static readonly object EmptyString = string.Empty;
        private static readonly object NotEmptyString = "Hello, world";

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNullStringToVisibility()
        {
            var converter = new EmptyStringToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(NullString, typeof(Visibility), null, "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNotEmptyStringToVisibility()
        {
            var converter = new EmptyStringToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(EmptyString, typeof(Visibility), null, "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertEmptyStringToVisibility()
        {
            var converter = new EmptyStringToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(EmptyString, typeof(Visibility), null, "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNotEmptyStringToVisibilityWithNegateTrue()
        {
            var converter = new EmptyStringToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(NotEmptyString, typeof(Visibility), "true", "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertEmptyStringToVisibilityWithNegateTrue()
        {
            var converter = new EmptyStringToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(EmptyString, typeof(Visibility), "true", "en-us");
            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNotEmptyStringToVisibilityWithNegateFalse()
        {
            var converter = new EmptyStringToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(NotEmptyString, typeof(Visibility), "false", "en-us");
            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertEmptyStringToVisibilityWithNegateFalse()
        {
            var converter = new EmptyStringToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(EmptyString, typeof(Visibility), "false", "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNotEmptyStringToVisibilityWithInvalidNegate()
        {
            var converter = new EmptyStringToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(NotEmptyString, typeof(Visibility), 42, "en-us");
            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertEmptyStringToVisibilityWithInvalidNegate()
        {
            var converter = new EmptyStringToObjectConverter
            {
                NotEmptyValue = Visibility.Visible,
                EmptyValue = Visibility.Collapsed
            };
            var result = converter.Convert(EmptyString, typeof(Visibility), 42, "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNotEmptyStringToBrush()
        {
            var greenBrush = new SolidColorBrush(Colors.Green);
            var redBrush = new SolidColorBrush(Colors.Red);
            var converter = new EmptyStringToObjectConverter
            {
                NotEmptyValue = greenBrush,
                EmptyValue = redBrush
            };
            var result = converter.Convert(NotEmptyString, typeof(Brush), null, "en-us");
            Assert.AreEqual(greenBrush, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertEmptyStringToBrush()
        {
            var greenBrush = new SolidColorBrush(Colors.Green);
            var redBrush = new SolidColorBrush(Colors.Red);
            var converter = new EmptyStringToObjectConverter
            {
                NotEmptyValue = greenBrush,
                EmptyValue = redBrush
            };
            var result = converter.Convert(EmptyString, typeof(Brush), null, "en-us");
            Assert.AreEqual(redBrush, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNotEmptyStringToBitmapImageWithTypeConversion()
        {
            var converter = new EmptyStringToObjectConverter
            {
                NotEmptyValue = "ms-appx:///Assets/NotEmptyString.png",
                EmptyValue = "ms-appx:///Assets/EmptyString.png"
            };

            var result = converter.Convert(NotEmptyString, typeof(ImageSource), null, "en-us");
            Assert.IsInstanceOfType(result, typeof(BitmapImage));
            Assert.AreEqual(converter.NotEmptyValue, ((BitmapImage)result).UriSource.ToString());
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertNotEmptyStringToBrushWithTypeConversion()
        {
            var converter = new EmptyStringToObjectConverter
            {
                NotEmptyValue = "Green",
                EmptyValue = "Red"
            };

            var result = converter.Convert(NotEmptyString, typeof(Brush), null, "en-us");
            Assert.IsInstanceOfType(result, typeof(SolidColorBrush));
            Assert.AreEqual(Colors.Green, ((SolidColorBrush)result).Color);
        }
    }
}