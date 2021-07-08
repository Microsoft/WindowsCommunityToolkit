// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests.Converters
{
    [TestClass]
    public class Test_StringFormatConverter
    {
        private static readonly object NullString = null;
        private static readonly object NotEmptyString = "Hello, world";
        private static readonly DateTime Date = DateTime.Now;

        [TestCategory("Converters")]
        [TestMethod]
        public void WhenValueIsNull_ThenReturnNull()
        {
            var converter = new StringFormatConverter();
            var result = converter.Convert(NullString, typeof(string), NullString, "en-us");
            Assert.IsNull(result);
        }

        [TestCategory("Converters")]
        [TestMethod]
        public void WhenValueExistsAndParameterIsNull_ThenReturnValue()
        {
            var converter = new StringFormatConverter();
            var result = converter.Convert(NotEmptyString, typeof(string), NullString, "en-us");
            Assert.AreEqual(NotEmptyString, result);
        }

        [TestCategory("Converters")]
        [TestMethod]
        public void WhenParameterIsTimeFormat_ThenReturnValueOfTimeFormat()
        {
            var converter = new StringFormatConverter();
            var result = converter.Convert(Date, typeof(string), "{0:HH:mm}", "en-us");
            Assert.AreEqual(Date.ToString("HH:mm"), result);
        }

        [TestCategory("Converters")]
        [TestMethod]
        public void WhenParameterIsInvalidFormat_ThenReturnValue()
        {
            var converter = new StringFormatConverter();
            var result = converter.Convert(Date, typeof(string), "{1:}", "en-us");
            Assert.AreEqual(Date, result);
        }

        [TestCategory("Converters")]
        [TestMethod]
        public void WhenParameterIsNotAString_ThenReturnValue()
        {
            var converter = new StringFormatConverter();
            var result = converter.Convert(NotEmptyString, typeof(string), 172, "en-us");
            Assert.AreEqual(NotEmptyString, result);
        }
    }
}