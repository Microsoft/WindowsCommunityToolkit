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
    public class Test_BoolToObjectConverter
    {
        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertTrueToVisibility()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed
            };
            var result = converter.Convert(true, typeof(Visibility), null, "en-us");
            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertFalseToVisibility()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed
            };
            var result = converter.Convert(false, typeof(Visibility), null, "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertTrueToVisibilityWithNegateTrue()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed
            };
            var result = converter.Convert(true, typeof(Visibility), "true", "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertFalseToVisibilityWithNegateTrue()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed
            };
            var result = converter.Convert(false, typeof(Visibility), "true", "en-us");
            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertTrueToVisibilityWithNegateFalse()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed
            };
            var result = converter.Convert(true, typeof(Visibility), "false", "en-us");
            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertFalseToVisibilityWithNegateFalse()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed
            };
            var result = converter.Convert(false, typeof(Visibility), "false", "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertTrueToVisibilityWithInvalidNegate()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed
            };
            var result = converter.Convert(true, typeof(Visibility), 42, "en-us");
            Assert.AreEqual(Visibility.Visible, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertFalseToVisibilityWithInvalidNegate()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed
            };
            var result = converter.Convert(false, typeof(Visibility), 42, "en-us");
            Assert.AreEqual(Visibility.Collapsed, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertTrueToBrush()
        {
            var greenBrush = new SolidColorBrush(Colors.Green);
            var redBrush = new SolidColorBrush(Colors.Red);
            var converter = new BoolToObjectConverter
            {
                TrueValue = greenBrush,
                FalseValue = redBrush
            };
            var result = converter.Convert(true, typeof(Brush), null, "en-us");
            Assert.AreEqual(greenBrush, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertFalseToBrush()
        {
            var greenBrush = new SolidColorBrush(Colors.Green);
            var redBrush = new SolidColorBrush(Colors.Red);
            var converter = new BoolToObjectConverter
            {
                TrueValue = greenBrush,
                FalseValue = redBrush
            };
            var result = converter.Convert(false, typeof(Brush), null, "en-us");
            Assert.AreEqual(redBrush, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertTrueToBitmapImageWithTypeConversion()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = "ms-appx:///Assets/True.png",
                FalseValue = "ms-appx:///Assets/False.png"
            };

            var result = converter.Convert(true, typeof(ImageSource), null, "en-us");
            Assert.IsInstanceOfType(result, typeof(BitmapImage));
            Assert.AreEqual(converter.TrueValue, ((BitmapImage)result).UriSource.ToString());
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertTrueToBrushWithTypeConversion()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = "Green",
                FalseValue = "Red"
            };

            var result = converter.Convert(true, typeof(Brush), null, "en-us");
            Assert.IsInstanceOfType(result, typeof(SolidColorBrush));
            Assert.AreEqual(Colors.Green, ((SolidColorBrush)result).Color);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertBackVisible()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed
            };
            var result = converter.ConvertBack(Visibility.Visible, typeof(bool), null, "en-us");
            Assert.AreEqual(true, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertBackCollapsed()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed
            };
            var result = converter.ConvertBack(Visibility.Collapsed, typeof(bool), null, "en-us");
            Assert.AreEqual(false, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertBackVisibleWithNegateTrue()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed
            };
            var result = converter.ConvertBack(Visibility.Visible, typeof(bool), "true", "en-us");
            Assert.AreEqual(false, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertBackCollapseWithNegateTrue()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed
            };
            var result = converter.ConvertBack(Visibility.Collapsed, typeof(bool), "true", "en-us");
            Assert.AreEqual(true, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertBackVisibleWithNegateFalse()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed
            };
            var result = converter.ConvertBack(Visibility.Visible, typeof(bool), "false", "en-us");
            Assert.AreEqual(true, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertBackCollaspedWithNegateFalse()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = Visibility.Visible,
                FalseValue = Visibility.Collapsed
            };
            var result = converter.ConvertBack(Visibility.Collapsed, typeof(bool), "false", "en-us");
            Assert.AreEqual(false, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertBackBrush()
        {
            var greenBrush = new SolidColorBrush(Colors.Green);
            var redBrush = new SolidColorBrush(Colors.Red);
            var converter = new BoolToObjectConverter
            {
                TrueValue = greenBrush,
                FalseValue = redBrush
            };
            var result = converter.ConvertBack(greenBrush, typeof(bool), null, "en-us");
            Assert.AreEqual(true, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertBackBrush2()
        {
            var greenBrush = new SolidColorBrush(Colors.Green);
            var redBrush = new SolidColorBrush(Colors.Red);
            var converter = new BoolToObjectConverter
            {
                TrueValue = greenBrush,
                FalseValue = redBrush
            };
            var result = converter.ConvertBack(redBrush, typeof(bool), null, "en-us");
            Assert.AreEqual(false, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertBackRefTypeWithTypeConversion()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = "ms-appx:///Assets/True.png",
                FalseValue = "ms-appx:///Assets/False.png"
            };

            var result = converter.ConvertBack(converter.TrueValue, typeof(bool), null, "en-us");
            Assert.AreEqual(true, result);
        }

        [TestCategory("Converters")]
        [UITestMethod]
        public void Test_ConvertBackRefTypeWithTypeConversion2()
        {
            var converter = new BoolToObjectConverter
            {
                TrueValue = "ms-appx:///Assets/True.png",
                FalseValue = "ms-appx:///Assets/False.png"
            };

            var result = converter.ConvertBack(converter.FalseValue, typeof(bool), null, "en-us");
            Assert.AreEqual(false, result);
        }
    }
}