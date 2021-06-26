// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.Toolkit.Uwp.UI;
using System.Numerics;
using System.Globalization;

namespace UnitTests.Extensions
{
    [TestClass]
    public class Test_StringExtensions
    {
        [TestCategory("StringExtensions")]
        [TestMethod]
        [DataRow(0f)]
        [DataRow(3.14f)]
        public void Test_StringExtensions_ToVector2_XX(float x)
        {
            string text = x.ToString("R", CultureInfo.InvariantCulture);

            Vector2
                expected = new(x),
                result = text.ToVector2();

            Assert.AreEqual(expected, result);

            result = $"<{text}>".ToVector2();

            Assert.AreEqual(expected, result);
        }

        [TestCategory("StringExtensions")]
        [TestMethod]
        [DataRow(0f, 0f)]
        [DataRow(0f, 22f)]
        [DataRow(3.14f, 3.24724928f)]
        [DataRow(float.MaxValue / 2, 0.3248f)]
        public void Test_StringExtensions_ToVector2_XYZW(float x, float y)
        {
            string text = $"{x.ToString("R", CultureInfo.InvariantCulture)},{y.ToString("R", CultureInfo.InvariantCulture)}";

            Vector2
                expected = new(x, y),
                result = text.ToVector2();

            Assert.AreEqual(expected, result);

            result = text.Replace(",", ", ").ToVector2();

            Assert.AreEqual(expected, result);

            result = $"<{text}>".ToVector2();

            Assert.AreEqual(expected, result);
        }

        [TestCategory("StringExtensions")]
        [TestMethod]
        [DataRow("")]
        [DataRow("Hello")]
        [DataRow("1, 2, 3")]
        [DataRow("<1, 2, 3")]
        [DataRow("<1, 2, 3>")]
        [DataRow("<1, 2, 3, 4>")]
        [ExpectedException(typeof(FormatException))]
        public void Test_StringExtensions_ToVector2_Fail(string text)
        {
            _ = text.ToVector2();
        }

        [TestCategory("StringExtensions")]
        [TestMethod]
        [DataRow(0f)]
        [DataRow(3.14f)]
        public void Test_StringExtensions_ToVector3_XXX(float x)
        {
            string text = x.ToString("R", CultureInfo.InvariantCulture);

            Vector3
                expected = new(x),
                result = text.ToVector3();

            Assert.AreEqual(expected, result);

            result = $"<{text}>".ToVector3();

            Assert.AreEqual(expected, result);
        }

        [TestCategory("StringExtensions")]
        [TestMethod]
        [DataRow(0f, 0f, 0f)]
        [DataRow(0f, 0f, 22f)]
        [DataRow(3.14f, 6.55f, 3.24724928f)]
        [DataRow(float.MaxValue / 2, 22f, 0.3248f)]
        public void Test_StringExtensions_ToVector3_XYZW(float x, float y, float z)
        {
            string text = $"{x.ToString("R", CultureInfo.InvariantCulture)},{y.ToString("R", CultureInfo.InvariantCulture)},{z.ToString("R", CultureInfo.InvariantCulture)}";

            Vector3
                expected = new(x, y, z),
                result = text.ToVector3();

            Assert.AreEqual(expected, result);

            result = text.Replace(",", ", ").ToVector3();

            Assert.AreEqual(expected, result);

            result = $"<{text}>".ToVector3();

            Assert.AreEqual(expected, result);
        }

        [TestCategory("StringExtensions")]
        [TestMethod]
        [DataRow("")]
        [DataRow("Hello")]
        [DataRow("1, 2")]
        [DataRow("1, 2, 3, 99")]
        [DataRow("<1, 2>")]
        [DataRow("<1, 2, 3")]
        [DataRow("<1, 2, 3, 4>")]
        [ExpectedException(typeof(FormatException))]
        public void Test_StringExtensions_ToVector3_Fail(string text)
        {
            _ = text.ToVector3();
        }

        [TestCategory("StringExtensions")]
        [TestMethod]
        [DataRow(0f)]
        [DataRow(3.14f)]
        public void Test_StringExtensions_ToVector4_XXXX(float x)
        {
            string text = x.ToString("R", CultureInfo.InvariantCulture);

            Vector4
                expected = new(x),
                result = text.ToVector4();

            Assert.AreEqual(expected, result);

            result = $"<{text}>".ToVector4();

            Assert.AreEqual(expected, result);
        }

        [TestCategory("StringExtensions")]
        [TestMethod]
        [DataRow(0f, 0f, 0f, 0f)]
        [DataRow(0f, 0f, 22f, 6.89f)]
        [DataRow(3.14f, 6.55f, 3838f, 3.24724928f)]
        [DataRow(float.MaxValue / 2, float.Epsilon, 22f, 0.3248f)]
        public void Test_StringExtensions_ToVector4_XYZW(float x, float y, float z, float w)
        {
            string text = $"{x.ToString("R", CultureInfo.InvariantCulture)},{y.ToString("R", CultureInfo.InvariantCulture)},{z.ToString("R", CultureInfo.InvariantCulture)},{w.ToString("R", CultureInfo.InvariantCulture)}";

            Vector4
                expected = new(x, y, z, w),
                result = text.ToVector4();

            // Test the "1,2,3,4" format
            Assert.AreEqual(expected, result);

            result = text.Replace(",", ", ").ToVector4();

            // Test the "1, 2, 3, 4" format
            Assert.AreEqual(expected, result);

            // Test the "<1, 2, 3, 4>" format
            result = $"<{text}>".ToVector4();

            Assert.AreEqual(expected, result);
        }

        [TestCategory("StringExtensions")]
        [TestMethod]
        [DataRow("")]
        [DataRow("Hello")]
        [DataRow("1, 2")]
        [DataRow("1, 2, 3")]
        [DataRow("1, 2, 3, 99, 100")]
        [DataRow("<1, 2, 3>")]
        [DataRow("<1, 2, 3, 4")]
        [DataRow("<1, 2, 3, 4, 5>")]
        [ExpectedException(typeof(FormatException))]
        public void Test_StringExtensions_ToVector4_Fail(string text)
        {
            _ = text.ToVector4();
        }

        [TestCategory("StringExtensions")]
        [TestMethod]
        [DataRow(0f, 0f, 0f, 0f)]
        [DataRow(0f, 0f, 22f, 6.89f)]
        [DataRow(3.14f, 6.55f, 3838f, 3.24724928f)]
        [DataRow(float.MaxValue / 2, float.Epsilon, 22f, 0.3248f)]
        public void Test_StringExtensions_ToQuaternion_XYZW(float x, float y, float z, float w)
        {
            string text = $"{x.ToString("R", CultureInfo.InvariantCulture)},{y.ToString("R", CultureInfo.InvariantCulture)},{z.ToString("R", CultureInfo.InvariantCulture)},{w.ToString("R", CultureInfo.InvariantCulture)}";

            Quaternion
                expected = new(x, y, z, w),
                result = text.ToQuaternion();

            // Test the "1,2,3,4" format
            Assert.AreEqual(expected, result);

            result = text.Replace(",", ", ").ToQuaternion();

            // Test the "1, 2, 3, 4" format
            Assert.AreEqual(expected, result);

            // Test the "<1, 2, 3, 4>" format
            result = $"<{text}>".ToQuaternion();

            Assert.AreEqual(expected, result);
        }

        [TestCategory("StringExtensions")]
        [TestMethod]
        [DataRow("")]
        [DataRow("Hello")]
        [DataRow("1, 2")]
        [DataRow("1, 2, 3")]
        [DataRow("1, 2, 3, 99, 100")]
        [DataRow("<1, 2, 3>")]
        [DataRow("<1, 2, 3, 4")]
        [DataRow("<1, 2, 3, 4, 5>")]
        [ExpectedException(typeof(FormatException))]
        public void Test_StringExtensions_ToQuaternion_Fail(string text)
        {
            _ = text.ToQuaternion();
        }
    }
}