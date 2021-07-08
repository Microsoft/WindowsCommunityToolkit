// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Foundation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.WinUI;

namespace UnitTests.Extensions
{
    [TestClass]
    public class Test_SizeExtensions
    {
        [TestCategory("SizeExtensions")]
        [TestMethod]
        [DataRow(0d, 0d)]
        [DataRow(3.14d, 6.55d)]
        [DataRow(double.MaxValue / 2, double.Epsilon)]
        public void Test_SizeExtensions_ToRect_FromSize(double width, double height)
        {
            Size s = new(width, height);
            Rect
                a = s.ToRect(),
                b = new(0, 0, s.Width, s.Height);

            Assert.AreEqual(a, b);
        }

        [TestCategory("SizeExtensions")]
        [TestMethod]
        [DataRow(0d, 0d, 0d, 0d)]
        [DataRow(0d, 0d, 22d, 6.89d)]
        [DataRow(3.14d, 6.55f, 3838d, 3.24724928d)]
        [DataRow(double.MaxValue / 2, double.Epsilon, 22, 0.3248d)]
        public void Test_SizeExtensions_ToRect_FromSizeAndPosition(double width, double height, double x, double y)
        {
            Size s = new(width, height);
            Rect
                a = s.ToRect(x, y),
                b = new(x, y, s.Width, s.Height);

            Assert.AreEqual(a, b);
        }

        [TestCategory("SizeExtensions")]
        [TestMethod]
        [DataRow(0d, 0d, 0d, 0d)]
        [DataRow(0d, 0d, 22d, 6.89d)]
        [DataRow(3.14d, 6.55f, 3838d, 3.24724928d)]
        [DataRow(double.MaxValue / 2, double.Epsilon, 22d, 0.3248d)]
        public void Test_SizeExtensions_ToRect_FromSizeAndPoint(double width, double height, double x, double y)
        {
            Point p = new(x, y);
            Size s = new(width, height);
            Rect
                a = s.ToRect(p),
                b = new(p, s);

            Assert.AreEqual(a, b);
        }
    }
}