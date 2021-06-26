// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Helpers
{
    [TestClass]
    public class Test_ColorHelper
    {
        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_ToColor_Predifined()
        {
            Assert.AreEqual("Red".ToColor(), Windows.UI.Colors.Red);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_ToColor_Hex8Digits()
        {
            Assert.AreEqual("#FFFF0000".ToColor(), Windows.UI.Colors.Red);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_ToColor_Hex6Digits()
        {
            Assert.AreEqual("#FF0000".ToColor(), Windows.UI.Colors.Red);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_ToColor_Hex4Digits()
        {
            Assert.AreEqual("#FF00".ToColor(), Windows.UI.Colors.Red);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_ToColor_Hex3Digits()
        {
            Assert.AreEqual("#F00".ToColor(), Windows.UI.Colors.Red);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_ToColor_ScreenColor()
        {
            Assert.AreEqual("sc#1.0,1.0,0,0".ToColor(), Windows.UI.Colors.Red);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_ToHex()
        {
            Assert.AreEqual(Windows.UI.Colors.Red.ToHex(), "#FFFF0000");
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_ToInt()
        {
            Assert.AreEqual(Windows.UI.Colors.Red.ToInt(), -65536);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_ToHsl()
        {
            HslColor hslRed;
            hslRed.A = 1.0;  // Alpha
            hslRed.H = 0.0;  // Hue
            hslRed.S = 1.0;  // Saturation
            hslRed.L = 0.5;  // Lightness

            Assert.AreEqual(Windows.UI.Colors.Red.ToHsl(), hslRed);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_ToHsl_White()
        {
            HslColor hslWhite;
            hslWhite.A = 1.0;  // Alpha
            hslWhite.H = 0.0;  // Hue
            hslWhite.S = 0.0;  // Saturation
            hslWhite.L = 1.0;  // Lightness

            Assert.AreEqual(Windows.UI.Colors.White.ToHsl(), hslWhite);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_ToHsl_MaxR()
        {
            // Test when given an RGB value where R is the max value.
            HslColor hslColor;
            hslColor.A = 1.0;        // Alpha
            hslColor.H = 330.0;      // Hue
            hslColor.S = 1.0;        // Saturation
            hslColor.L = 0.7058823;  // Lightness

            const double delta = 0.000001d;
            var color = Windows.UI.Color.FromArgb(255, 255, 105, 180).ToHsl();
            Assert.AreEqual(color.H, hslColor.H, delta);
            Assert.AreEqual(color.S, hslColor.S, delta);
            Assert.AreEqual(color.L, hslColor.L, delta);
            Assert.AreEqual(color.A, hslColor.A, delta);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_ToHsv()
        {
            HsvColor hsvColor;
            hsvColor.A = 1.0;   // Alpha
            hsvColor.H = 100;   // Hue
            hsvColor.S = 0.25;  // Saturation
            hsvColor.V = 0.80;  // Value

            // Use a test color with non-zero/non-max values for both RGB and HSV
            var color = Windows.UI.Color.FromArgb(255, 170, 204, 153).ToHsv();

            // These still may not come out exactly even, so define a delta so that
            // if the two are almost equal, the test still passes.
            const double delta = 0.000001d;
            Assert.AreEqual(color.H, hsvColor.H, delta);
            Assert.AreEqual(color.S, hsvColor.S, delta);
            Assert.AreEqual(color.V, hsvColor.V, delta);
            Assert.AreEqual(color.A, hsvColor.A, delta);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_ToHsv_MaxR()
        {
            HsvColor hsvColor;
            hsvColor.A = 1.0;        // Alpha
            hsvColor.H = 330;        // Hue
            hsvColor.S = 0.58823529; // Saturation
            hsvColor.V = 1;          // Value

            // Use a test color with non-zero/non-max values for both RGB and HSV
            var color = Windows.UI.Color.FromArgb(255, 255, 105, 180).ToHsv();

            // These still may not come out exactly even, so define a delta so that
            // if the two are almost equal, the test still passes.
            const double delta = 0.000001d;
            Assert.AreEqual(color.H, hsvColor.H, delta);
            Assert.AreEqual(color.S, hsvColor.S, delta);
            Assert.AreEqual(color.V, hsvColor.V, delta);
            Assert.AreEqual(color.A, hsvColor.A, delta);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_FromHsl()
        {
            Assert.AreEqual(ColorHelper.FromHsl(0.0, 1.0, 0.5), Windows.UI.Colors.Red);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_ColorHelper_FromHsv()
        {
            Assert.AreEqual(ColorHelper.FromHsv(0.0, 1.0, 1.0), Windows.UI.Colors.Red);
        }
    }
}