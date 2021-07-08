// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Foundation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.WinUI;

namespace UnitTests.Extensions
{
    [TestClass]
    public class Test_PointExtensions
    {
        [TestCategory("PointExtensions")]
        [TestMethod]
        [DataRow(0d, 0d, 0d, 0d)]
        [DataRow(0d, 0d, 22d, 6.89d)]
        [DataRow(3.14d, 6.55f, 3838d, 3.24724928d)]
        [DataRow(double.MaxValue / 2, double.Epsilon, 22d, 0.3248d)]
        public void Test_PointExtensions_ToRect_FromWidthHeight(double width, double height, double x, double y)
        {
            Point p = new(x, y);
            Rect
                a = p.ToRect(width, height),
                b = new(x, y, width, height);

            Assert.AreEqual(a, b);
        }

        [TestCategory("SizeExtensions")]
        [TestMethod]
        [DataRow(0d, 0d, 0d, 0d)]
        [DataRow(0d, 0d, 22d, 6.89d)]
        [DataRow(3.14d, 6.55f, 3838d, 3.24724928d)]
        [DataRow(double.MaxValue / 2, double.Epsilon, 22, 0.3248d)]
        public void Test_PointExtensions_ToRect_FromPoint(double width, double height, double x, double y)
        {
            Point
                p1 = new(x, y),
                p2 = new(x + width, y + height);
            Rect
                a = p1.ToRect(p2),
                b = new(p1, p2);

            Assert.AreEqual(a, b);
        }

        [TestCategory("SizeExtensions")]
        [TestMethod]
        [DataRow(0d, 0d, 0d, 0d)]
        [DataRow(0d, 0d, 22d, 6.89d)]
        [DataRow(3.14d, 6.55f, 3838d, 3.24724928d)]
        [DataRow(double.MaxValue / 2, double.Epsilon, 22, 0.3248d)]
        public void Test_PointExtensions_ToRect_FromSize(double width, double height, double x, double y)
        {
            Point p = new(x, y);
            Size s = new(width, height);
            Rect
                a = p.ToRect(s),
                b = new(p, s);

            Assert.AreEqual(a, b);
        }
    }
}