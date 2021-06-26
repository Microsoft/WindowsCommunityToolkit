// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Toolkit.Uwp.UI.Media.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

namespace UnitTests.UWP.Geometry
{
    [TestClass]
    public class Test_CanvasPathGeometry
    {
        [TestCategory("CanvasPathGeometry - Color")]
        [TestMethod]
        public void Test_CreateColor_1()
        {
            var color = CanvasPathGeometry.CreateColor("3FFF7FFF");
            Assert.AreEqual(63, color.A);
            Assert.AreEqual(255, color.R);
            Assert.AreEqual(127, color.G);
            Assert.AreEqual(255, color.B);
        }

        [TestCategory("CanvasPathGeometry - Color")]
        [TestMethod]
        public void Test_CreateColor_2()
        {
            var color = CanvasPathGeometry.CreateColor("FF7FFF");
            Assert.AreEqual(255, color.A);
            Assert.AreEqual(255, color.R);
            Assert.AreEqual(127, color.G);
            Assert.AreEqual(255, color.B);
        }

        [TestCategory("CanvasPathGeometry - Color")]
        [TestMethod]
        public void Test_CreateColor_3()
        {
            var color = CanvasPathGeometry.CreateColor("0.25 1 0.5 0.5");
            Assert.AreEqual(127, color.A);
            Assert.AreEqual(63, color.R);
            Assert.AreEqual(255, color.G);
            Assert.AreEqual(127, color.B);
        }

        [TestCategory("CanvasPathGeometry - Color")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CreateColor_4()
        {
            var color1 = CanvasPathGeometry.CreateColor("XYZXYZ");
        }

        [TestCategory("CanvasPathGeometry - Color")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CreateColor_5()
        {
            var color1 = CanvasPathGeometry.CreateColor("FFF");
        }

        [TestCategory("CanvasPathGeometry - Color")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CreateColor_6()
        {
            var color1 = CanvasPathGeometry.CreateColor("F112233");
        }

        [TestCategory("CanvasPathGeometry - Color")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CreateColor_7()
        {
            var color1 = CanvasPathGeometry.CreateColor("4 5 6 7");
        }

        [TestCategory("CanvasPathGeometry - Color")]
        [TestMethod]
        public void Test_CreateColor_8()
        {
            var color = CanvasPathGeometry.CreateColor(new Vector4(4, 5, 6, 7));
            Assert.AreEqual(255, color.A);
            Assert.AreEqual(255, color.R);
            Assert.AreEqual(255, color.G);
            Assert.AreEqual(255, color.B);
        }

        [TestCategory("CanvasPathGeometry - Color")]
        [TestMethod]
        public void Test_CreateColor_9()
        {
            var color = CanvasPathGeometry.CreateColor(new Vector4(4, 0.25f, 6, 0.5f));
            Assert.AreEqual(255, color.R);
            Assert.AreEqual(63, color.G);
            Assert.AreEqual(255, color.B);
            Assert.AreEqual(127, color.A);
        }

        [TestCategory("CanvasPathGeometry - CanvasStrokeStyle")]
        [TestMethod]
        public void Test_CanvasStrokeStyle_1()
        {
            var style = CanvasPathGeometry.CreateStrokeStyle("CSS DS 1 LJ1 ML 0.5 DO4 SC 0 EC1 DC3 TB1CDS 2 2 0 2 1 3");
            Assert.AreEqual(true, style.LineJoin == CanvasLineJoin.Bevel);
            Assert.AreEqual(true, style.MiterLimit == 0.5f);
            Assert.AreEqual(true, style.StartCap == CanvasCapStyle.Flat);
            Assert.AreEqual(true, style.EndCap == CanvasCapStyle.Square);
            Assert.AreEqual(true, style.TransformBehavior == CanvasStrokeTransformBehavior.Fixed);
        }

        [TestCategory("CanvasPathGeometry - CanvasStrokeStyle")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasStrokeStyle_2()
        {
            // Invalid data
            var style = CanvasPathGeometry.CreateStrokeStyle("CDS DS 1 LJ1 ML 0.5 DO4 SC 0 EC1 DC3 TB1CDS 2 2 0 2 1 3");
        }

        [TestCategory("CanvasPathGeometry - CanvasStrokeStyle")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasStrokeStyle_3()
        {
            // Multiple CanvasStrokeStyles in the same data
            var style = CanvasPathGeometry.CreateStrokeStyle("CSS DS 1 LJ1 ML 0.5 DO4 SC 0 EC1 DC3 TB1CDS 2 2 0 2 1 3 CSS DS 1 LJ1 ML 0.5 DO4 SC 0 EC1 DC3 TB1");
        }

        [TestCategory("CanvasPathGeometry - CanvasStrokeStyle")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasStrokeStyle_4()
        {
            // Extra characters in the data
            var style = CanvasPathGeometry.CreateStrokeStyle("CSS DS 1 LJ1 ML 0.5 DO4 SC 0 EC1 DC3 TB1CDS 2 2 0 2 1 3 12 23 34");
        }

        [TestCategory("CanvasPathGeometry - CanvasStroke")]
        [UITestMethod]
        public void Test_CanvasStroke_1()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var stroke = CanvasPathGeometry.CreateStroke(device, "ST 4.5 LG M 0 0 Z80 80 S 0.00 #ffff0000, 0.5 #ff00ff00, 0.99 #ff0000ff CSS DS 2 LJ1 Do 2 SC 1 EC 2 CDS 2 2 0 2 1 3");
            Assert.AreEqual(true, stroke.Width == 4.5f);
            Assert.AreEqual(true, stroke.Style.LineJoin == CanvasLineJoin.Bevel);
            Assert.AreEqual(true, stroke.Style.StartCap == CanvasCapStyle.Square);
            Assert.AreEqual(true, stroke.Style.EndCap == CanvasCapStyle.Round);
            Assert.AreEqual(true, stroke.Style.TransformBehavior == CanvasStrokeTransformBehavior.Normal);
        }

        [TestCategory("CanvasPathGeometry - CanvasStroke")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasStroke_2()
        {
            // Invalid data
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var stroke = CanvasPathGeometry.CreateStroke(device, "ST 4.5 6 LG M 0 0 Z80 80 S 0.00 #ffff0000, 0.5 #ff00ff00, 0.99 #ff0000ff CSS DS 2 Do 2 SC 1 EC 2 CDS 2 2 0 2 1 3");
        }

        [TestCategory("CanvasPathGeometry - CanvasStroke")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasStroke_3()
        {
            // Multiple CanvasStrokes in the same data
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var stroke = CanvasPathGeometry.CreateStroke(device, "ST 4.5 LG M 0 0 Z80 80 S 0.00 #ffff0000, 0.5 #ff00ff00, 0.99 #ff0000ff CSS DS 2 Do 2 SC 1 EC 2 CDS 2 2 0 2 1 3 ST 4.5 LG M 0 0 Z80 80 S 0.00 #ffff0000, 0.5 #ff00ff00, 0.99 #ff0000ff CSS DS 2 Do 2 SC 1 EC 2 CDS 2 2 0 2 1 3");
        }

        [TestCategory("CanvasPathGeometry - CanvasStroke")]
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasStroke_4()
        {
            // Extra characters in the stroke data
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var stroke = CanvasPathGeometry.CreateStroke(device, "ST 4.5 LG M 0 0 Z80 80 S 0.00 #ffff0000, 0.5 #ff00ff00, 0.99 #ff0000ff CSS DS 2 Do 2 SC 1 EC 2 CDS 2 2 0 2 1 3 A 10");
        }

        [TestCategory("CanvasPathGeometry - CanvasBrush")]
        [UITestMethod]
        public void Test_CanvasBrush()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var solidbrush = CanvasPathGeometry.CreateBrush(device, "SC #FF1233AA O .75");
            Assert.IsNotNull(solidbrush, "Could not create SolidColorBrush instance.");
            var linearbrush = CanvasPathGeometry.CreateBrush(device, "LG M 0 80 Z0 0 S 0.00 #ffee5124, 0.18 #fff05627, 0.26 #fff15b29, 0.6 #fff58535, 1.00 #fff9af41");
            Assert.IsNotNull(linearbrush, "Could not create LinearGradientBrush instance.");
            var radialbrush = CanvasPathGeometry.CreateBrush(device, "RG 40 60 320 400 S 0.00 #ffee5124, 0.18 #fff05627, 0.26 #fff15b29, 0.72 #fff58535, 1.00 #fff9af41");
            Assert.IsNotNull(radialbrush, "Could not create RadialGradientBrush instance.");
        }

        [TestCategory("CanvasPathGeometry - CanvasBrush")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasBrush_SolidBrush_1()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var solidbrush = CanvasPathGeometry.CreateBrush(device, "SC 3 #FF1233AA O .75");
        }

        [TestCategory("CanvasPathGeometry - CanvasBrush")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasBrush_SolidBrush_2()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var solidbrush = CanvasPathGeometry.CreateBrush(device, "SC #FF1233AA O .75 SC #FF1233AA O .75");
        }

        [TestCategory("CanvasPathGeometry - CanvasBrush")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasBrush_SolidBrush_3()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var solidbrush = CanvasPathGeometry.CreateBrush(device, "SC #FF1233AA O .75 23 35");
        }

        [TestCategory("CanvasPathGeometry - CanvasBrush")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasBrush_LinearGradientBrush_1()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var linearbrush = CanvasPathGeometry.CreateBrush(device, "LG M 0 8 0 Z0 0 S 0.00 #ffee5124, 0.18 #fff05627, 0.26 #fff15b29, 0.6 #fff58535, 1.00 #fff9af41");
        }

        [TestCategory("CanvasPathGeometry - CanvasBrush")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasBrush_LinearGradientBrush_2()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var linearbrush = CanvasPathGeometry.CreateBrush(device, "LG M 0 80 Z0 0 S 0.00 #ffee5124, 0.18 #fff05627, 0.26 #fff15b29, 0.6 #fff58535, 1.00 #fff9af41 LG M 0 80 Z0 0 S 0.00 #ffee5124, 0.18 #fff05627, 0.26 #fff15b29, 0.6 #fff58535, 1.00 #fff9af41");
        }

        [TestCategory("CanvasPathGeometry - CanvasBrush")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasBrush_LinearGradientBrush_3()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var linearbrush = CanvasPathGeometry.CreateBrush(device, "LG M 0 80 Z0 0 S 0.00 #ffee5124, 0.18 #fff05627, 0.26 #fff15b29, 0.6 #fff58535, 1.00 #fff9af41 3.0");
        }

        [TestCategory("CanvasPathGeometry - CanvasBrush")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasBrush_RadialGradientBrush_1()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var radialbrush = CanvasPathGeometry.CreateBrush(device, "RG 4 0 6 0 320 400 S 0.00 #ffee5124, 0.18 #fff05627, 0.26 #fff15b29, 0.72 #fff58535, 1.00 #fff9af41");
        }

        [TestCategory("CanvasPathGeometry - CanvasBrush")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasBrush_RadialGradientBrush_2()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var radialbrush = CanvasPathGeometry.CreateBrush(device, "RG 40 60 320 400 S 0.00 #ffee5124, 0.18 #fff05627, 0.26 #fff15b29, 0.72 #fff58535, 1.00 #fff9af41 RG 40 60 320 400 S 0.00 #ffee5124, 0.18 #fff05627, 0.26 #fff15b29, 0.72 #fff58535, 1.00 #fff9af41");
        }

        [TestCategory("CanvasPathGeometry - CanvasBrush")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasBrush_RadialGradientBrush_3()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var radialbrush = CanvasPathGeometry.CreateBrush(device, "RG 40 60 320 400 S 0.00 #ffee5124, 0.18 #fff05627, 0.26 #fff15b29, 0.72 #fff58535, 1.00 #fff9af41 5.0");
        }

        [TestCategory("CanvasPathGeometry - CanvasBrush")]
        [UITestMethod]
        public void Test_CanvasBrush_Squircle()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            var squircle = CanvasPathGeometry.CreateSquircle(device, 10, 10, 200, 200, 130, 130);
            Assert.IsNotNull(squircle, "Could not create Squircle CanvasGeometry");
        }

        [TestCategory("CanvasPathGeometry - CanvasGeometry")]
        [UITestMethod]
        public void Test_CanvasGeometry_1()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            const string sample = "F1 M 331.341,81.975 L 398.766,218.593 L 549.533,240.500 L 440.437,346.842 L 466.191,497.000 L 331.341,426.105 L 196.491,497.000 L 222.245,346.842 L 113.150,240.500 L 263.916,218.593 L 331.341,81.975 Z";

            var geometry = CanvasPathGeometry.CreateGeometry(device, sample);
            Assert.IsNotNull(geometry, "Unable to create CanvasGeometry!");

            geometry = CanvasPathGeometry.CreateGeometry(sample);
            Assert.IsNotNull(geometry, "Unable to create CanvasGeometry!");
        }

        [TestCategory("CanvasPathGeometry - CanvasGeometry")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasGeometry_2()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            const string sample = "F1 M 331.341,81.975 L 398.766,218.593 L 549.533,240.500 L 440.437,346.842 L 466.191,497.000 L 331.341,426.105 L 196.491,497.000 L 222.245,346.842 L 113.150,240.500 L 263.916,218.593 L 331.341,81.975 Z";

            var geometry = CanvasPathGeometry.CreateGeometry(device, $"{sample} {sample}");
        }

        [TestCategory("CanvasPathGeometry - CanvasGeometry")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Test_CanvasGeometry_3()
        {
            CanvasDevice device = new CanvasDevice();
            Assert.IsNotNull(device, "Could not create CanvasDevice instance.");

            const string sample = "F1 M 331.341,81.975 L 398.766,218.593 L 549.533,240.500 L 440.437,346.842 L 466.191,497.000 L 331.341,426.105 L 196.491,497.000 L 222.245,346.842 L 113.150,240.500 L 263.916,218.593 L 331.341,81.975 Z S A M P L E E R R O R";

            var geometry = CanvasPathGeometry.CreateGeometry(device, sample);
        }
    }
}