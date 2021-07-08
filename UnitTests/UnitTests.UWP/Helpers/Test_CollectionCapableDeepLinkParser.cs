// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Tests
{
    using System;
    using System.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using UnitTests;
    using CommunityToolkit.WinUI.Helpers;

    [TestClass]
    public class Test_CollectionCapableDeepLinkParser
    {
        private const string SAMPLELINK = @"myapp://MainPage/Options?option1=value1&option2=value2&option3=value3&option2=value4";
        private static readonly Uri SAMPLELINKURI = new Uri(SAMPLELINK);
        private static readonly DeepLinkParser _stringParser = new TestCollectionCapableDeepLinkParser(SAMPLELINK);
        private static readonly DeepLinkParser _uriParser = new TestCollectionCapableDeepLinkParser(SAMPLELINKURI);

        [TestMethod]
        public void Test_DeepLinkCollection_string_RootValue()
        {
            Assert.AreEqual("MainPage/Options", _stringParser.Root);
        }

        [TestMethod]
        public void Test_DeepLinkCollection_string_PullOptions()
        {
            Assert.AreEqual("value1", _stringParser["option1"]);
            Assert.AreEqual("value2,value4", _stringParser["option2"]);
            Assert.AreEqual("value3", _stringParser["option3"]);
        }

        [TestMethod]
        public void Test_DeepLinkCollection_string_OptionNotFound()
        {
            try
            {
                var nonexistentvalue = _stringParser["nonexistentoption"];

                Assert.Fail("Should have thrown KeyNotFoundException");
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
            }
            catch
            {
                Assert.Fail("Should have thrown KeyNotFoundException");
            }
        }

        [TestMethod]
        public void Test_DeepLinkCollection_string_null_empty_whitespace()
        {
            try
            {
                string s = null;
                var p = new TestCollectionCapableDeepLinkParser(s);

                Assert.Fail("Should have thrown ArgumentNullException for null string");
            }
            catch (ArgumentNullException aex)
            {
                Debug.WriteLine(aex.ToString());
            }
            catch
            {
                Assert.Fail("Should have thrown ArgumentNullException for null string");
            }

            try
            {
                string s = string.Empty;
                var p = new TestCollectionCapableDeepLinkParser(s);

                Assert.Fail("Should have thrown ArgumentNullException for empty string");
            }
            catch (ArgumentNullException aex)
            {
                Debug.WriteLine(aex.ToString());
            }
            catch
            {
                Assert.Fail("Should have thrown ArgumentNullException for empty string");
            }

            try
            {
                string s = "   ";
                var p = new TestCollectionCapableDeepLinkParser(s);

                Assert.Fail("Should have thrown ArgumentNullException for whitespace-only string");
            }
            catch (ArgumentNullException aex)
            {
                Debug.WriteLine(aex.ToString());
            }
            catch
            {
                Assert.Fail("Should have thrown ArgumentNullException for whitespace-only string");
            }
        }

        [TestMethod]
        public void Test_DeepLinkCollection_uri_RootValue()
        {
            Assert.AreEqual("MainPage/Options", _uriParser.Root);
        }

        [TestMethod]
        public void Test_DeepLinkCollection_uri_PullOptions()
        {
            Assert.AreEqual("value1", _uriParser["option1"]);
            Assert.AreEqual("value2,value4", _uriParser["option2"]);
            Assert.AreEqual("value3", _uriParser["option3"]);
        }

        [TestMethod]
        public void Test_DeepLinkCollection_uri_OptionNotFound()
        {
            try
            {
                var nonexistentvalue = _uriParser["nonexistentoption"];

                Assert.Fail("Should have thrown KeyNotFoundException");
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
            }
            catch
            {
                Assert.Fail("Should have thrown KeyNotFoundException");
            }
        }

        [TestMethod]
        public void Test_DeepLinkCollection_uri_null()
        {
            try
            {
                Uri s = null;
                var p = new TestCollectionCapableDeepLinkParser(s);

                Assert.Fail("Should have thrown ArgumentNullException for null string");
            }
            catch (ArgumentNullException aex)
            {
                Debug.WriteLine(aex.ToString());
            }
            catch
            {
                Assert.Fail("Should have thrown ArgumentNullException for null string");
            }
        }
    }
}