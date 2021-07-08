// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Media.Geometry.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.UWP.Geometry
{
    [TestClass]
    public class Test_RegexFactory
    {
        [TestCategory("Geometry - RegexFactory")]
        [TestMethod]
        public void Test_Regex_Color()
        {
            Assert.AreEqual(true, RegexFactory.ColorRegex.IsMatch("#FF0000"));
            Assert.AreEqual(false, RegexFactory.ColorRegex.IsMatch("#FL000"));
            Assert.AreEqual(true, RegexFactory.ColorRegex.IsMatch("FF0000"));
            Assert.AreEqual(true, RegexFactory.ColorRegex.IsMatch("FFFF00A7"));
            Assert.AreEqual(false, RegexFactory.ColorRegex.IsMatch("GHFF00T7"));
            Assert.AreEqual(true, RegexFactory.ColorRegex.IsMatch("#FFFF00A7"));
            Assert.AreEqual(false, RegexFactory.ColorRegex.IsMatch("#GHIJ00K7"));
            Assert.AreEqual(true, RegexFactory.ColorRegex.IsMatch("0.9333333, 0.3176471, 0.1411765, 1"));
            Assert.AreEqual(true, RegexFactory.ColorRegex.IsMatch("0.9333333 0.3176471 0.1411765 1"));
        }

        [TestCategory("Geometry - RegexFactory")]
        [TestMethod]
        public void Test_Regex_SolidBrush()
        {
            Assert.AreEqual(true, RegexFactory.CanvasBrushRegex.IsMatch("SC #FFAABBCC"));
            Assert.AreEqual(true, RegexFactory.CanvasBrushRegex.IsMatch("SC FF1233AA O .75"));
            Assert.AreEqual(false, RegexFactory.CanvasBrushRegex.IsMatch("SC 5 FF1233AA O .75"));
            Assert.AreEqual(true, RegexFactory.CanvasBrushRegex.IsMatch("SC 0.1 0.3 0.4 0.5"));
            Assert.AreEqual(true, RegexFactory.CanvasBrushRegex.IsMatch("SC 0.1 0.3 0.4 0.5 O 0.9"));
        }

        [TestCategory("Geometry - RegexFactory")]
        [TestMethod]
        public void Test_Regex_LinearGradientBrush()
        {
            Assert.AreEqual(true, RegexFactory.CanvasBrushRegex.IsMatch("LG M 0 80 Z0 0 S 0.00 #ffee5124, 0.18 #fff05627, 0.26 #fff15b29, 0.72 #fff58535, 1.00 #fff9af41"));
            Assert.AreEqual(true, RegexFactory.CanvasBrushRegex.IsMatch("LG M 0 80 Z0 0 O 0.75 A 1 E 2 S 0.00 #ffee5124, 0.3 #fff05627, 0.75 #fff58535, 1.00 #fff9af41"));
            Assert.AreEqual(false, RegexFactory.CanvasBrushRegex.IsMatch("LG M 0 8 0 Z0 0 O 0.75 A 1 E 2 S 0.00 #ffee5124, 0.3 #fff05627, 0.75 #fff58535, 1.00 #fff9af41"));
        }

        [TestCategory("Geometry - RegexFactory")]
        [TestMethod]
        public void Test_Regex_LinearGradientBrushHdr()
        {
            Assert.AreEqual(true, RegexFactory.CanvasBrushRegex.IsMatch("LH M 0 80 Z0 0 P1 R1 S 0.00 0.9333333, 0.3176471, 0.1411765, 1, 0.18 0.9411765, 0.3372549, 0.1529412, 1, 0.26 0.945098, 0.3568628, 0.1607843, 1, 0.72 0.9607843, 0.5215687, 0.2078431, 1, 1.00 0.9764706, 0.6862745, 0.254902, 1"));
            Assert.AreEqual(false, RegexFactory.CanvasBrushRegex.IsMatch("LH M 0 80 Z0 0 P1 R10 S 0.00 0.9333333, 0.3176471, 0.1411765, 1, 0.18 0.9411765, 0.3372549, 0.1529412, 1, 0.26 0.945098, 0.3568628, 0.1607843, 1, 0.72 0.9607843, 0.5215687, 0.2078431, 1, 1.00 0.9764706, 0.6862745, 0.254902, 1"));
        }

        [TestCategory("Geometry - RegexFactory")]
        [TestMethod]
        public void Test_Regex_RadialGradientBrush()
        {
            Assert.AreEqual(true, RegexFactory.CanvasBrushRegex.IsMatch("RG 40 60 320 400 S 0.00 #ffee5124, 0.18 #fff05627, 0.26 #fff15b29, 0.72 #fff58535, 1.00 #fff9af41"));
            Assert.AreEqual(true, RegexFactory.CanvasBrushRegex.IsMatch("RG 40 60 320 400 A 1 B2 E 2 S 0.00 #ffee5124, 0.3 #fff05627, 0.75 #fff58535, 1.00 #fff9af41"));
            Assert.AreEqual(false, RegexFactory.CanvasBrushRegex.IsMatch("RG 40 60 3 20 400 A 1 B2 E 2 S 0.00 #ffee5124, 0.3 #fff05627, 0.75 #fff58535, 1.00 #fff9af41"));
            Assert.AreEqual(false, RegexFactory.CanvasBrushRegex.IsMatch("RG 40 60 320 400 A 1 B7 E 2 S 0.00 #ffee5124, 0.3 #fff05627, 0.75 #fff58535, 1.00 #fff9af41"));
        }

        [TestCategory("Geometry - RegexFactory")]
        [TestMethod]
        public void Test_Regex_RadialGradientBrushHdr()
        {
            Assert.AreEqual(true, RegexFactory.CanvasBrushRegex.IsMatch("RH 400 400 320 320 O 0.94 A 1 E2 S 0.00 0.9333333, 0.3176471, 0.1411765, 1, 0.18 0.9411765, 0.3372549, 0.1529412, 1, 0.26 0.945098, 0.3568628, 0.1607843, 1, 0.72 0.9607843, 0.5215687, 0.2078431, 1, 1.00 0.9764706, 0.6862745, 0.254902, 1"));
            Assert.AreEqual(false, RegexFactory.CanvasBrushRegex.IsMatch("RH 400 40 0 320 3 20 O 0.94 A 1 E2 S 0.00 0.9333333, 0.3176471, 0.1411765, 1, 0.18 0.9411765, 0.3372549, 0.1529412, 1, 0.26 0.945098, 0.3568628, 0.1607843, 1, 0.72 0.9607843, 0.5215687, 0.2078431, 1, 1.00 0.9764706, 0.6862745, 0.254902, 1"));
        }

        [TestCategory("Geometry - RegexFactory")]
        [TestMethod]
        public void Test_Regex_GradientStop()
        {
            Assert.AreEqual(true, RegexFactory.GradientStopRegex.IsMatch("S 0.00 #ffee5124, 0.18 #fff05627, 0.26 #fff15b29, 0.72 #fff58535, 1.00 #fff9af41"));
            Assert.AreEqual(true, RegexFactory.GradientStopRegex.IsMatch("S 0.00 #ffee5124 0.18 #fff05627 0.26 #fff15b29    0.72 #fff58535 1.00 #fff9af41"));
            Assert.AreEqual(false, RegexFactory.GradientStopRegex.IsMatch("S #ffee5124 0.18 #fff05627 0.26 #fff15b29    0.72 #fff58535 1.00 #fff9af41"));
        }

        [TestCategory("Geometry - RegexFactory")]
        [TestMethod]
        public void Test_Regex_GradientStopHdr()
        {
            Assert.AreEqual(true, RegexFactory.GradientStopHdrRegex.IsMatch("S 0.00 0.9333333, 0.3176471, 0.1411765, 1, 0.18 0.9411765, 0.3372549, 0.1529412, 1, 0.26 0.945098, 0.3568628, 0.1607843, 1, 0.72 0.9607843, 0.5215687, 0.2078431, 1, 1.00 0.9764706, 0.6862745, 0.254902, 1"));
            Assert.AreEqual(true, RegexFactory.GradientStopHdrRegex.IsMatch("S 0.00 0.9333333 0.3176471 0.1411765 1 0.18 0.9411765 0.3372549 0.1529412 1 0.26 0.945098 0.3568628 0.1607843 1 0.72  0.9607843   0.5215687 0.2078431 1 1.00 0.9764706 0.6862745 0.254902 1"));
            Assert.AreEqual(false, RegexFactory.GradientStopHdrRegex.IsMatch("ST 0.00 0.9333333 0.3176471 0.1411765 1 0.18 0.9411765 0.3372549 0.1529412 1 0.26 0.945098 0.3568628 0.1607843 1 0.72  0.9607843   0.5215687 0.2078431 1 1.00 0.9764706 0.6862745 0.254902 1"));
        }

        [TestCategory("Geometry - RegexFactory")]
        [TestMethod]
        public void Test_Regex_CanvasStrokeStyle()
        {
            Assert.AreEqual(true, RegexFactory.CanvasStrokeStyleRegex.IsMatch("CSS CDS 2 2 0 2 1 3"));
            Assert.AreEqual(true, RegexFactory.CanvasStrokeStyleRegex.IsMatch("CSS"));
            Assert.AreEqual(false, RegexFactory.CanvasStrokeStyleRegex.IsMatch("CSD CDS 2 2 0 2 1 3"));
            Assert.AreEqual(true, RegexFactory.CanvasStrokeStyleRegex.IsMatch("CSS DS2 DO2 SC 1 EC 2"));
            Assert.AreEqual(true, RegexFactory.CanvasStrokeStyleRegex.IsMatch("CSS DS 1 LJ1 ML 0.5 DO4 SC 0 EC1 DC3 TB1CDS 2 2 0 2 1 3"));
            Assert.AreEqual(false, RegexFactory.CanvasStrokeStyleRegex.IsMatch("CSD DSD 1 LJ1 ML 0.5 DO4 SC 0 EC1 DC3 TB1CDS 2 2 0 2 1 3"));
        }

        [TestCategory("Geometry - RegexFactory")]
        [TestMethod]
        public void Test_Regex_CanvasStroke()
        {
            Assert.AreEqual(true, RegexFactory.CanvasStrokeRegex.IsMatch("ST 4.5 LG M 0 0 Z80 80 S 0.00 #ffff0000, 0.5 #ff00ff00, 0.99 0000ff CSS DS2 Do2 SC 1 EC 2 CDS 2 2 0 2 1 3"));
            Assert.AreEqual(false, RegexFactory.CanvasStrokeRegex.IsMatch("ST 4 5 LG M 0 0 Z80 80 S 0.00 #ffff0000, 0.5 #ff00ff00, 0.99 0000ff CSS DS2 Do2 SC 1 EC 2 CDS 2 2 0 2 1 3"));
            Assert.AreEqual(false, RegexFactory.CanvasStrokeRegex.IsMatch("ST 2 3 SC #ff0000 5"));
            Assert.AreEqual(true, RegexFactory.CanvasStrokeRegex.IsMatch("ST 2 SC #ff0000"));
        }

        [TestCategory("Geometry - RegexFactory")]
        [TestMethod]
        public void Test_Regex_CustomDashAttribute()
        {
            Assert.AreEqual(true, RegexFactory.CustomDashAttributeRegex.IsMatch("2 2 0 2 1 3"));
            Assert.AreEqual(false, RegexFactory.CustomDashAttributeRegex.IsMatch("2"));
        }

        [TestCategory("Geometry - RegexFactory")]
        [TestMethod]
        public void Test_Regex_CanvasGeometry()
        {
            const string sample1 = "F0 M 656.500,400.500 C 656.500,350.637 598.572,307.493 514.292,286.708 C 493.507,202.428 450.363,144.500 400.500,144.500 C 350.637,144.500 307.493,202.428 286.708,286.708 C 202.428,307.493 144.500,350.637 144.500,400.500 C 144.500,450.363 202.428,493.507 286.708,514.292 C 307.493,598.572 350.637,656.500 400.500,656.500 C 450.363,656.500 493.507,598.572 514.292,514.292 C 598.572,493.507 656.500,450.363 656.500,400.500 ZM 581.519,219.481 C 546.261,184.222 474.793,194.676 400.500,239.574 C 326.207,194.676 254.739,184.222 219.481,219.481 C 184.222,254.739 194.676,326.207 239.574,400.500 C 194.676,474.792 184.222,546.261 219.481,581.519 C 254.739,616.778 326.207,606.324 400.500,561.426 C 474.793,606.324 546.261,616.778 581.519,581.519 C 616.778,546.261 606.324,474.792 561.426,400.500 C 606.324,326.207 616.778,254.739 581.519,219.481 ZU 112.5 112.5 570 570 36 36";
            const string sample2 = "F1 M 331.341,81.975 L 398.766,218.593 L 549.533,240.500 L 440.437,346.842 L 466.191,497.000 L 331.341,426.105 L 196.491,497.000 L 222.245,346.842 L 113.150,240.500 L 263.916,218.593 L 331.341,81.975 Z";
            const string sample3 = "F1 M 545.497,397.058 C 454.492,512.882 286.824,533.003 171.000,441.998 C 78.340,369.194 62.244,235.059 135.048,142.400 C 193.291,68.272 300.599,55.395 374.726,113.639 C 434.028,160.233 444.330,246.079 397.736,305.381 C 360.460,352.823 291.783,361.064 244.341,323.788 C 206.388,293.968 199.795,239.026 229.616,201.073 C 253.472,170.711 297.425,165.436 327.788,189.293 C 352.078,208.378 356.297,243.540 337.212,267.830 C 321.944,287.262 293.814,290.638 274.382,275.370 C 258.836,263.155 256.136,240.651 268.350,225.106 C 278.122,212.669 296.125,210.509 308.562,220.280 C 318.511,228.098 320.239,242.500 312.422,252.449";
            const string sample4 = "F1 M 311.717,332.110 C 285.669,332.110 264.552,310.994 264.552,284.945 C 264.552,258.897 285.669,237.781 311.717,237.781 C 337.765,237.781 358.881,258.897 358.881,284.945 C 358.881,310.994 337.765,332.110 311.717,332.110 Z M 505.712,232.846 C 634.939,203.833 411.705,171.395 371.772,213.383 C 411.705,171.395 311.872,-30.889 311.872,92.013 C 311.872,-30.889 212.038,171.395 251.972,213.383 C 212.038,171.395 -11.196,203.833 118.031,232.846 C -11.196,203.833 150.338,361.289 214.951,327.320 C 150.338,361.289 112.205,583.622 192.072,460.719 C 112.205,583.622 311.872,478.651 311.872,397.737 C 311.872,478.651 511.538,583.622 431.672,460.719 C 511.538,583.622 473.405,361.289 408.792,327.320 C 473.405,361.289 634.939,203.833 505.712,232.846 Z";
            const string sample5 = "F1 MB 391.853,348.284 C 391.853,357.113 384.696,364.271 375.867,364.271 L 301.927,364.271 C 293.098,364.271 285.940,357.113 285.940,348.284 L 285.940,274.345 C 285.940,265.515 293.098,258.358 301.927,258.358 L 375.867,258.358 C 384.696,258.358 391.853,265.515 391.853,274.345 L 391.853,348.284 Z M 544.748,282.990 L 485.488,267.081 C 472.521,263.600 466.301,248.839 472.866,237.128 L 502.867,183.604 C 512.642,166.166 494.336,146.433 476.214,154.872 L 420.592,180.776 C 408.421,186.445 394.169,179.136 391.670,165.944 L 380.248,105.658 C 376.526,86.017 349.819,82.667 341.362,100.780 L 315.403,156.378 C 309.723,168.543 294.107,172.105 283.714,163.607 L 236.213,124.767 C 220.737,112.113 198.125,126.714 203.289,146.025 L 219.141,205.301 C 222.610,218.271 212.937,231.038 199.512,231.208 L 138.159,231.988 C 118.170,232.242 110.233,257.962 126.602,269.436 L 176.847,304.655 C 187.841,312.361 188.638,328.358 178.464,337.118 L 131.965,377.153 C 116.816,390.196 127.269,415.001 147.184,413.268 L 208.312,407.950 C 221.687,406.786 232.580,418.529 230.417,431.779 L 220.531,492.336 C 217.310,512.066 241.261,524.348 255.403,510.220 L 298.811,466.854 C 308.310,457.365 324.202,459.358 331.062,470.899 L 362.415,523.643 C 372.629,540.827 398.872,534.840 400.624,514.927 L 406.001,453.804 C 407.178,440.430 420.634,431.742 433.307,436.173 L 491.227,456.425 C 510.098,463.022 526.353,441.568 514.895,425.187 L 479.725,374.908 C 472.030,363.906 476.753,348.601 489.310,343.850 L 546.697,322.133 C 565.393,315.057 564.054,288.173 544.748,282.990 Z";
            const string sample6 = "G1 N 331.341,81.975 Q 398.766,218.593 H 549.533,240.500 H 440.437,346.842 H 466.191,497.000 H 331.341,426.105 H 196.491,497.000 H 222.245,346.842 H 113.150,240.500 H 263.916,218.593 H 331.341,81.975 Z";

            Assert.AreEqual(true, RegexFactory.CanvasGeometryRegex.IsMatch(sample1));
            Assert.AreEqual(true, RegexFactory.CanvasGeometryRegex.IsMatch(sample2));
            Assert.AreEqual(true, RegexFactory.CanvasGeometryRegex.IsMatch(sample3));
            Assert.AreEqual(true, RegexFactory.CanvasGeometryRegex.IsMatch(sample4));
            Assert.AreEqual(true, RegexFactory.CanvasGeometryRegex.IsMatch(sample5));
            Assert.AreEqual(false, RegexFactory.CanvasGeometryRegex.IsMatch(sample6));
        }
    }
}