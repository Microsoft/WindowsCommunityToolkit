// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using CommunityToolkit.WinUI.UI.Media.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;

namespace UnitTests.UWP.Geometry
{
    [TestClass]
    public class Test_Utils
    {
        [TestCategory("Geometry - Double")]
        [TestMethod]
        public void Test_Utils_Double_IsCloseTo()
        {
            double d1 = 4.000000000000000001d;
            double d2 = 4.000000000000000002d;
            Assert.AreEqual(true, d1.IsCloseTo(d2));

            double d3 = 5.0001d;
            double d4 = 5.00000000000000001d;
            Assert.AreEqual(false, d3.IsCloseTo(d4));
        }

        [TestCategory("Geometry - Double")]
        [TestMethod]
        public void Test_Utils_Double_IsLessThan()
        {
            double d1 = 4.000010000000000001d;
            double d2 = 4.000020000000000002d;
            Assert.AreEqual(true, d1.IsLessThan(d2));

            double d3 = double.PositiveInfinity;
            double d4 = 5.00000000000000001d;
            Assert.AreEqual(false, d3.IsLessThan(d4));
        }

        [TestCategory("Geometry - Double")]
        [TestMethod]
        public void Test_Utils_Double_IsGreaterThan()
        {
            double d1 = 4.000010000000000001d;
            double d2 = 4.000020000000000002d;
            Assert.AreEqual(false, d1.IsGreaterThan(d2));

            double d3 = double.PositiveInfinity;
            double d4 = 5.00000000000000001d;
            Assert.AreEqual(true, d3.IsGreaterThan(d4));
        }

        [TestCategory("Geometry - Double")]
        [TestMethod]
        public void Test_Utils_Double_IsOne()
        {
            double d1 = 0.9999999999d;
            Assert.AreEqual(false, d1.IsOne());

            double d3 = 1.00000000000000001d;
            Assert.AreEqual(true, d3.IsOne());
        }

        [TestCategory("Geometry - Double")]
        [TestMethod]
        public void Test_Utils_Double_IsZero()
        {
            double d1 = 0.0000000009d;
            Assert.AreEqual(false, d1.IsZero());

            double d3 = 0.00000000000000001e-275d;
            Assert.AreEqual(false, d3.IsZero());
        }

        [TestCategory("Geometry - Float")]
        [TestMethod]
        public void Test_Utils_Float_IsCloseTo()
        {
            float f1 = 4.000000000000000001f;
            float f2 = 4.000000000000000002f;
            Assert.AreEqual(true, f1.IsCloseTo(f2));

            float f3 = 5.0001f;
            float f4 = 5.00000000000000001f;
            Assert.AreEqual(false, f3.IsCloseTo(f4));
        }

        [TestCategory("Geometry - Float")]
        [TestMethod]
        public void Test_Utils_Float_IsLessThan()
        {
            float f1 = 4.000010000000000001f;
            float f2 = 4.000020000000000002f;
            Assert.AreEqual(true, f1.IsLessThan(f2));

            float f3 = float.PositiveInfinity;
            float f4 = 5.00000000000000001f;
            Assert.AreEqual(false, f3.IsLessThan(f4));
        }

        [TestCategory("Geometry - Float")]
        [TestMethod]
        public void Test_Utils_Float_IsGreaterThan()
        {
            float f1 = 4.000010000000000001f;
            float f2 = 4.000020000000000002f;
            Assert.AreEqual(false, f1.IsGreaterThan(f2));

            float f3 = float.PositiveInfinity;
            float f4 = 5.00000000000000001f;
            Assert.AreEqual(true, f3.IsGreaterThan(f4));
        }

        [TestCategory("Geometry - Float")]
        [TestMethod]
        public void Test_Utils_Float_IsOne()
        {
            float f1 = 0.99999f;
            Assert.AreEqual(false, f1.IsOne());

            float f2 = 1.00000000000000001f;
            Assert.AreEqual(true, f2.IsOne());
        }

        [TestCategory("Geometry - Float")]
        [TestMethod]
        public void Test_Utils_Float_IsZero()
        {
            float f1 = 0.000009f;
            Assert.AreEqual(false, f1.IsZero());

            float f2 = 0.0000000000000000001e-27f;
            Assert.AreEqual(true, f2.IsZero());
        }

        [TestCategory("Geometry - Point")]
        [TestMethod]
        public void Test_Utils_Point_IsCloseTo()
        {
            Point p1 = new Point(4.000000000000000001f, 6.000000000000000001f);
            Point p2 = new Point(4.000000000000000002f, 6.000000000000000002f);
            Assert.AreEqual(true, p1.IsCloseTo(p2));

            Point p3 = new Point(4, 5.0001f);
            Point p4 = new Point(4, 5.00000000000000001f);
            Assert.AreEqual(false, p3.IsCloseTo(p4));
        }

        [TestCategory("Geometry - Size")]
        [TestMethod]
        public void Test_Utils_Size_IsCloseTo()
        {
            Size s1 = new Size(14.000000000000000001f, 26.000000000000000001f);
            Size s2 = new Size(14.000000000000000002f, 26.000000000000000002f);
            Assert.AreEqual(true, s1.IsCloseTo(s2));

            Size s3 = new Size(54, 55.0001f);
            Size s4 = new Size(54, 55.00000000000000001f);
            Assert.AreEqual(false, s3.IsCloseTo(s4));
        }

        [TestCategory("Geometry - Rect")]
        [TestMethod]
        public void Test_Utils_Rect_IsCloseTo()
        {
            Rect r1 = new Rect(4.000000000000000001f, 6.000000000000000001f, 54, 55.0001f);
            Rect r2 = new Rect(4.000000000000000002f, 6.000000000000000002f, 54, 55.0001f);
            Assert.AreEqual(true, r1.IsCloseTo(r2));

            Rect r3 = new Rect(4, 5.0001f, 54, 55.0001f);
            Rect r4 = new Rect(4, 5.00000000000000001f, 54, 55.0001f);
            Assert.AreEqual(false, r3.IsCloseTo(r4));
        }

        [TestCategory("Geometry - Double")]
        [TestMethod]
        public void Test_Utils_Double_RoundLayoutValue()
        {
            Assert.AreEqual(6d, Utils.RoundLayoutValue(5.5d, 1.000000000000000000001d));

            Assert.AreEqual(3.5d, Utils.RoundLayoutValue(3.4d, 2d));

            Assert.AreEqual(3.4d, Utils.RoundLayoutValue(3.4d, 0.0d));
        }

        [TestCategory("Geometry - Float")]
        [TestMethod]
        public void Test_Utils_Float_Lerp()
        {
            Assert.AreEqual(3f, 2f.Lerp(4f, 0.5f));

            Assert.AreEqual(9.6f, 6f.Lerp(10f, 0.9f));
        }

        [TestCategory("Geometry - Thickness")]
        [TestMethod]
        public void Test_Utils_Thickness_IsValid()
        {
            Assert.AreEqual(true, new Thickness(5).IsValid(false, false, false, false));

            Assert.AreEqual(true, new Thickness(5, -5, 5, -5).IsValid(true, false, false, false));
            Assert.AreEqual(false, new Thickness(5, -5, 5, -5).IsValid(false, false, false, false));

            Assert.AreEqual(false, new Thickness(5, double.NaN, 5, 5).IsValid(false, false, false, false));
            Assert.AreEqual(true, new Thickness(5, double.NaN, 5, 5).IsValid(false, true, false, false));

            Assert.AreEqual(true, new Thickness(5, double.PositiveInfinity, double.NegativeInfinity, 5).IsValid(true, false, true, true));
            Assert.AreEqual(false, new Thickness(5, double.PositiveInfinity, double.NegativeInfinity, 5).IsValid(false, false, false, true));
            Assert.AreEqual(false, new Thickness(5, double.PositiveInfinity, double.NegativeInfinity, 5).IsValid(false, false, true, false));
            Assert.AreEqual(false, new Thickness(5, double.PositiveInfinity, double.NegativeInfinity, 5).IsValid(false, false, false, false));
        }

        [TestCategory("Geometry - Thickness")]
        [TestMethod]
        public void Test_Utils_Thickness_CollapseThickness()
        {
            var th = new Thickness(10, 20, 30, 40);
            Assert.AreEqual(new Size(40, 60), th.CollapseThickness());
        }

        [TestCategory("Geometry - Thickness")]
        [TestMethod]
        public void Test_Utils_Thickness_IsZero()
        {
            var th1 = new Thickness(10, 20, 30, 40);
            Assert.AreEqual(false, th1.IsZero());

            var th2 = new Thickness(0, 0, 0, 0.0e-279);
            Assert.AreEqual(true, th2.IsZero());
        }

        [TestCategory("Geometry - Thickness")]
        [TestMethod]
        public void Test_Utils_Thickness_IsUniform()
        {
            var th1 = new Thickness(10, 20, 30, 40);
            Assert.AreEqual(false, th1.IsUniform());

            var th2 = new Thickness(10, 10, 10, 10);
            Assert.AreEqual(true, th2.IsUniform());
        }

        [TestCategory("Geometry - Thickness")]
        [TestMethod]
        public void Test_Utils_Thickness_ToVector4()
        {
            var th1 = new Thickness(10, 20, 30, 40);
            Assert.AreEqual(new Vector4(10, 20, 30, 40), th1.ToVector4());
        }

        [TestCategory("Geometry - Thickness")]
        [TestMethod]
        public void Test_Utils_Thickness_ToAbsVector4()
        {
            var th1 = new Thickness(10, -20, 30, 40);
            Assert.AreEqual(new Vector4(10, 20, 30, 40), th1.ToAbsVector4());
        }

        [TestCategory("Geometry - Thickness")]
        [TestMethod]
        public void Test_Utils_Thickness_GetOffset()
        {
            var th1 = new Thickness(10, -20, 30, 40);
            Assert.AreEqual(new Vector2(10, -20), th1.GetOffset());
        }

        [TestCategory("Geometry - CornerRadius")]
        [TestMethod]
        public void Test_Utils_CornerRadius_IsZero()
        {
            var cr1 = new CornerRadius(10, 20, 30, 40);
            Assert.AreEqual(false, cr1.IsZero());

            var cr2 = new CornerRadius(0, 0, 0, 0.0e-279);
            Assert.AreEqual(true, cr2.IsZero());
        }

        [TestCategory("Geometry - CornerRadius")]
        [TestMethod]
        public void Test_Utils_CornerRadius_IsUniform()
        {
            var cr1 = new CornerRadius(10, 20, 30, 40);
            Assert.AreEqual(false, cr1.IsUniform());

            var cr2 = new CornerRadius(10, 10, 10, 10);
            Assert.AreEqual(true, cr2.IsUniform());
        }

        [TestCategory("Geometry - CornerRadius")]
        [TestMethod]
        public void Test_Utils_ConvertToValidCornerValue()
        {
            Assert.AreEqual(0d, Utils.ConvertToValidCornerValue(double.NaN));
            Assert.AreEqual(0d, Utils.ConvertToValidCornerValue(double.PositiveInfinity));
            Assert.AreEqual(0d, Utils.ConvertToValidCornerValue(double.NegativeInfinity));
            Assert.AreEqual(0d, Utils.ConvertToValidCornerValue(-2d));
            Assert.AreEqual(2d, Utils.ConvertToValidCornerValue(2d));
        }

        [TestCategory("Geometry - CornerRadius")]
        [TestMethod]
        public void Test_Utils_CornerRadius_ToVector4()
        {
            var cr1 = new CornerRadius(10, 20, 30, 40);
            Assert.AreEqual(new Vector4(10, 20, 30, 40), cr1.ToVector4());
        }

        [TestCategory("Geometry - Rect")]
        [TestMethod]
        public void Test_Utils_Rect_Deflate()
        {
            var rect1 = new Rect(10, 20, 30, 40);
            Assert.AreEqual(new Rect(20, 30, 10, 20), rect1.Deflate(new Thickness(10)));

            var rect2 = new Rect(10, 20, 30, 40);
            Assert.AreEqual(new Rect(30, 40, 0, 0), rect2.Deflate(new Thickness(20)));

            var rect3 = new Rect(10, 20, 30, 40);
            Assert.AreEqual(new Rect(0, 10, 50, 60), rect3.Deflate(new Thickness(-10)));
        }

        [TestCategory("Geometry - Rect")]
        [TestMethod]
        public void Test_Utils_Rect_Inflate()
        {
            var rect1 = new Rect(10, 20, 30, 40);
            Assert.AreEqual(new Rect(0, 10, 50, 60), rect1.Inflate(new Thickness(10)));

            var rect2 = new Rect(10, 20, 30, 40);
            Assert.AreEqual(new Rect(-10, 0, 70, 80), rect2.Inflate(new Thickness(20)));

            var rect3 = new Rect(10, 20, 30, 40);
            Assert.AreEqual(new Rect(30, 40, 0, 0), rect3.Inflate(new Thickness(-20)));
        }

        [TestCategory("Geometry - Brush")]
        [UITestMethod]
        public void Test_Utils_IsOpaqueSolidColorBrush()
        {
            Assert.AreEqual(true, new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)).IsOpaqueSolidColorBrush());
            Assert.AreEqual(false, new SolidColorBrush(Color.FromArgb(155, 255, 0, 0)).IsOpaqueSolidColorBrush());
            Assert.AreEqual(false, new LinearGradientBrush().IsOpaqueSolidColorBrush());
        }

        [TestCategory("Geometry - Brush")]
        [UITestMethod]
        public void Test_Utils_Brush_IsEqualTo()
        {
            var scb1 = new SolidColorBrush(Colors.Red);
            var scb2 = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            var scb3 = new SolidColorBrush(Color.FromArgb(255, 128, 128, 255));
            Assert.AreEqual(true, scb1.IsEqualTo(scb2));
            Assert.AreEqual(false, scb1.IsEqualTo(scb3));
            Assert.AreEqual(false, scb2.IsEqualTo(scb3));

            var gColl1 = new GradientStopCollection
            {
                new GradientStop() { Color = Colors.Red, Offset = 0 },
                new GradientStop() { Color = Colors.Green, Offset = 0.5 },
                new GradientStop() { Color = Colors.Blue, Offset = 1 }
            };
            var lgb1 = new LinearGradientBrush()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                GradientStops = gColl1
            };

            var gColl2 = new GradientStopCollection
            {
                new GradientStop() { Color = Colors.Red, Offset = 0 },
                new GradientStop() { Color = Colors.Green, Offset = 0.5 },
                new GradientStop() { Color = Colors.Blue, Offset = 1 }
            };
            var lgb2 = new LinearGradientBrush()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                GradientStops = gColl2
            };

            var gColl3 = new GradientStopCollection
            {
                new GradientStop() { Color = Colors.Red, Offset = 0 },
                new GradientStop() { Color = Colors.Green, Offset = 0.5 },
                new GradientStop() { Color = Colors.Yellow, Offset = 1 }
            };
            var lgb3 = new LinearGradientBrush()
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
                GradientStops = gColl3
            };

            Assert.AreEqual(true, lgb1.IsEqualTo(lgb2));
            Assert.AreEqual(false, lgb1.IsEqualTo(lgb3));
            Assert.AreEqual(false, lgb2.IsEqualTo(lgb3));
        }

        [TestCategory("Geometry - Uri")]
        [TestMethod]
        public void Test_Utils_Uri_IsEqualTo()
        {
            var u1 = new Uri("https://www.microsoft.com/uwp");
            var u2 = new Uri("https://www.microsoft.com/");
            var u3 = new Uri(u2, new Uri("uwp", UriKind.Relative));

            Assert.AreEqual(true, u1.IsEqualTo(u3));
            Assert.AreEqual(false, u1.IsEqualTo(u2));
            Assert.AreEqual(false, u2.IsEqualTo(u3));
        }

        [TestCategory("Geometry - Vector2")]
        [TestMethod]
        public void Test_Utils_Vector2_Reflect()
        {
            var a = new Vector2(10, 10);
            var b = Vector2.Zero;
            var c = new Vector2(-10, -10);

            Assert.AreEqual(c, Utils.Reflect(a, b));
        }

        [TestCategory("Geometry - Vector2")]
        [TestMethod]
        public void Test_Utils_Vector2_ToVector3()
        {
            var a = new Vector2(10, 10);
            var b = new Vector3(10, 10, 0);

            Assert.AreEqual(b, a.ToVector3());
        }

        [TestCategory("Geometry - Vector4")]
        [TestMethod]
        public void Test_Utils_Vector4_IsZero()
        {
            var a = new Vector4(10, 10, 0, 0);
            var b = new Vector4(0);

            Assert.AreEqual(false, a.IsZero());
            Assert.AreEqual(true, b.IsZero());
        }

        [TestCategory("Geometry - Vector4")]
        [TestMethod]
        public void Test_Utils_Vector4_Collapse()
        {
            var a = new Vector4(10, 20, 30, 40);
            var b = new Vector2(40, 60);

            Assert.AreEqual(b, a.Collapse());
        }

        [TestCategory("Geometry - Vector4")]
        [TestMethod]
        public void Test_Utils_Vector4_ToSize()
        {
            var a = new Vector4(10, 20, 30, 40);
            var b = new Size(40, 60);

            Assert.AreEqual(b, a.ToSize());
        }

        [TestCategory("Geometry - Vector4")]
        [TestMethod]
        public void Test_Utils_Vector4_Thickness()
        {
            var a = new Vector4(10, 20, 30, 40);
            var b = new Thickness(10, 20, 30, 40);

            Assert.AreEqual(b, a.ToThickness());
        }

        [TestCategory("Geometry - Vector4")]
        [TestMethod]
        public void Test_Utils_Vector4_ToCornerRadius()
        {
            var a = new Vector4(10, 20, 30, 40);
            var b = new CornerRadius(10, 20, 30, 40);

            Assert.AreEqual(b, a.ToCornerRadius());
        }

        [TestCategory("Geometry - Color")]
        [TestMethod]
        public void Test_Utils_Color_Lerp()
        {
            var colorFrom = Colors.Red;
            var colorTo = Colors.Green;

            var expectedColor = CanvasPathGeometry.CreateColor("#FF7F4000");

            Assert.AreEqual(expectedColor, colorFrom.Lerp(colorTo, 0.5f));
        }

        [TestCategory("Geometry - Color")]
        [TestMethod]
        public void Test_Utils_Color_DarkerBy()
        {
            var colorFrom = Colors.Red;
            var expectedColor = CanvasPathGeometry.CreateColor("#FF7F0000");

            Assert.AreEqual(expectedColor, colorFrom.DarkerBy(0.5f));
        }

        [TestCategory("Geometry - Color")]
        [TestMethod]
        public void Test_Utils_Color_LighterBy()
        {
            var colorFrom = Colors.Red;
            var expectedColor = CanvasPathGeometry.CreateColor("#FFFF7F7F");

            Assert.AreEqual(expectedColor, colorFrom.LighterBy(0.5f));
        }

        [TestCategory("Geometry - Point")]
        [TestMethod]
        public void Test_Utils_Point_ToVector3()
        {
            var point = new Point(10.5d, 15.5d);
            var vecPoint = new Vector3(10.5f, 15.5f, 0f);

            Assert.AreEqual(vecPoint, point.ToVector3());
        }

        [TestCategory("Geometry - Rect")]
        [TestMethod]
        public void Test_Utils_Rect_GetOptimumSize()
        {
            var srcWidth = 500;
            var srcHeight = 400;
            var destWidth = 800;
            var destHeight = 800;

            Assert.AreEqual(new Rect(0, 0, 500, 400), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.None, AlignmentX.Left, AlignmentY.Top));
            Assert.AreEqual(new Rect(0, 200, 500, 400), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.None, AlignmentX.Left, AlignmentY.Center));
            Assert.AreEqual(new Rect(0, 400, 500, 400), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.None, AlignmentX.Left, AlignmentY.Bottom));
            Assert.AreEqual(new Rect(150, 0, 500, 400), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.None, AlignmentX.Center, AlignmentY.Top));
            Assert.AreEqual(new Rect(150, 200, 500, 400), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.None, AlignmentX.Center, AlignmentY.Center));
            Assert.AreEqual(new Rect(150, 400, 500, 400), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.None, AlignmentX.Center, AlignmentY.Bottom));
            Assert.AreEqual(new Rect(300, 0, 500, 400), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.None, AlignmentX.Right, AlignmentY.Top));
            Assert.AreEqual(new Rect(300, 200, 500, 400), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.None, AlignmentX.Right, AlignmentY.Center));
            Assert.AreEqual(new Rect(300, 400, 500, 400), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.None, AlignmentX.Right, AlignmentY.Bottom));

            Assert.AreEqual(new Rect(0, 0, 800, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Fill, AlignmentX.Left, AlignmentY.Top));
            Assert.AreEqual(new Rect(0, 0, 800, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Fill, AlignmentX.Left, AlignmentY.Center));
            Assert.AreEqual(new Rect(0, 0, 800, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Fill, AlignmentX.Left, AlignmentY.Bottom));
            Assert.AreEqual(new Rect(0, 0, 800, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Fill, AlignmentX.Center, AlignmentY.Top));
            Assert.AreEqual(new Rect(0, 0, 800, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Fill, AlignmentX.Center, AlignmentY.Center));
            Assert.AreEqual(new Rect(0, 0, 800, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Fill, AlignmentX.Center, AlignmentY.Bottom));
            Assert.AreEqual(new Rect(0, 0, 800, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Fill, AlignmentX.Right, AlignmentY.Top));
            Assert.AreEqual(new Rect(0, 0, 800, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Fill, AlignmentX.Right, AlignmentY.Center));
            Assert.AreEqual(new Rect(0, 0, 800, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Fill, AlignmentX.Right, AlignmentY.Bottom));

            Assert.AreEqual(new Rect(0, 0, 800, 640), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Left, AlignmentY.Top));
            Assert.AreEqual(new Rect(0, 80, 800, 640), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Left, AlignmentY.Center));
            Assert.AreEqual(new Rect(0, 160, 800, 640), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Left, AlignmentY.Bottom));
            Assert.AreEqual(new Rect(0, 0, 800, 640), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Center, AlignmentY.Top));
            Assert.AreEqual(new Rect(0, 80, 800, 640), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Center, AlignmentY.Center));
            Assert.AreEqual(new Rect(0, 160, 800, 640), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Center, AlignmentY.Bottom));
            Assert.AreEqual(new Rect(0, 0, 800, 640), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Right, AlignmentY.Top));
            Assert.AreEqual(new Rect(0, 80, 800, 640), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Right, AlignmentY.Center));
            Assert.AreEqual(new Rect(0, 160, 800, 640), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Right, AlignmentY.Bottom));

            Assert.AreEqual(new Rect(0, 0, 1000, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Left, AlignmentY.Top));
            Assert.AreEqual(new Rect(0, 0, 1000, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Left, AlignmentY.Center));
            Assert.AreEqual(new Rect(0, 0, 1000, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Left, AlignmentY.Bottom));
            Assert.AreEqual(new Rect(-100, 0, 1000, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Center, AlignmentY.Top));
            Assert.AreEqual(new Rect(-100, 0, 1000, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Center, AlignmentY.Center));
            Assert.AreEqual(new Rect(-100, 0, 1000, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Center, AlignmentY.Bottom));
            Assert.AreEqual(new Rect(-200, 0, 1000, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Right, AlignmentY.Top));
            Assert.AreEqual(new Rect(-200, 0, 1000, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Right, AlignmentY.Center));
            Assert.AreEqual(new Rect(-200, 0, 1000, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Right, AlignmentY.Bottom));

            srcWidth = 400;
            srcHeight = 500;

            Assert.AreEqual(new Rect(0, 0, 640, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Left, AlignmentY.Top));
            Assert.AreEqual(new Rect(0, 0, 640, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Left, AlignmentY.Center));
            Assert.AreEqual(new Rect(0, 0, 640, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Left, AlignmentY.Bottom));
            Assert.AreEqual(new Rect(80, 0, 640, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Center, AlignmentY.Top));
            Assert.AreEqual(new Rect(80, 0, 640, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Center, AlignmentY.Center));
            Assert.AreEqual(new Rect(80, 0, 640, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Center, AlignmentY.Bottom));
            Assert.AreEqual(new Rect(160, 0, 640, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Right, AlignmentY.Top));
            Assert.AreEqual(new Rect(160, 0, 640, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Right, AlignmentY.Center));
            Assert.AreEqual(new Rect(160, 0, 640, 800), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.Uniform, AlignmentX.Right, AlignmentY.Bottom));

            Assert.AreEqual(new Rect(0, 0, 800, 1000), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Left, AlignmentY.Top));
            Assert.AreEqual(new Rect(0, -100, 800, 1000), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Left, AlignmentY.Center));
            Assert.AreEqual(new Rect(0, -200, 800, 1000), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Left, AlignmentY.Bottom));
            Assert.AreEqual(new Rect(0, 0, 800, 1000), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Center, AlignmentY.Top));
            Assert.AreEqual(new Rect(0, -100, 800, 1000), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Center, AlignmentY.Center));
            Assert.AreEqual(new Rect(0, -200, 800, 1000), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Center, AlignmentY.Bottom));
            Assert.AreEqual(new Rect(0, 0, 800, 1000), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Right, AlignmentY.Top));
            Assert.AreEqual(new Rect(0, -100, 800, 1000), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Right, AlignmentY.Center));
            Assert.AreEqual(new Rect(0, -200, 800, 1000), Utils.GetOptimumSize(srcWidth, srcHeight, destWidth, destHeight, Stretch.UniformToFill, AlignmentX.Right, AlignmentY.Bottom));
        }
    }
}