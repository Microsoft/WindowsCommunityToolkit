// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CommunityToolkit.WinUI.Notifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Notifications
{
    [TestClass]
    public class TestAssertHelper
    {
        [TestMethod]
        public void TestAssertXmlElement()
        {
            AssertHelper.AssertXml("<tile>  <visual version='2'><text>Hello world</text></visual></tile>", "<tile><visual version=\"2\" > <text>Hello world</text></visual> </tile>");
        }

        [TestMethod]
        public void TestAssertXmlElement_002()
        {
            try
            {
                AssertHelper.AssertXml("<tile>  <visual version='2'><text>Hello world</text></visual></tile>", "<Tile><visual version=\"2\" > <text>Hello world</text></visual> </tile>");
            }
            catch
            {
                return;
            }

            Assert.Fail("tile element name was different, should have thrown exception");
        }

        [TestMethod]
        public void TestAssertXmlElement_003()
        {
            try
            {
                AssertHelper.AssertXml("<tile>  <visual version='2'><text>Hello world</text></visual></tile>", "<tile><visuals version=\"2\" > <text>Hello world</text></visual> </tile>");
            }
            catch
            {
                return;
            }

            Assert.Fail("visual element name was incorrect, should have thrown exception");
        }

        [TestMethod]
        public void TestAssertXmlElement_004()
        {
            try
            {
                AssertHelper.AssertXml("<tile>  <visual version='2'><text>Hello world</text></visual></tile>", "<tile><visual version=\"3\" > <text>Hello world</text></visual> </tile>");
            }
            catch
            {
                return;
            }

            Assert.Fail("visual version number was incorrect, should have thrown exception");
        }

        [TestMethod]
        public void TestAssertXmlElement_005()
        {
            try
            {
                AssertHelper.AssertXml("<tile>  <visual version='2'><text>Hello world</text></visual></tile>", "<Tile><visual version=\"2\" > <text>Hello world!</text></visual> </tile>");
            }
            catch
            {
                return;
            }

            Assert.Fail("text content was different, should have thrown exception");
        }

        [TestMethod]
        public void TestAssertXmlElement_006()
        {
            AssertHelper.AssertXml("<tile id='2' version='3'/>", "<tile version='3' id='2'/>");
        }

        [TestMethod]
        public void TestAssertXmlElement_006_1()
        {
            try
            {
                AssertHelper.AssertXml("<tile id='2' version='3'/>", "<tile version='45' id='2'/>");
            }
            catch
            {
                return;
            }

            Assert.Fail("Version number was incorrect, should have thrown exception.");
        }

        [TestMethod]
        public void TestAssertXmlElement_006_2()
        {
            try
            {
                AssertHelper.AssertXml("<tile id='2' version='3'/>", "<tile version='3' id='myId'/>");
            }
            catch
            {
                return;
            }

            Assert.Fail("ID number was incorrect, should have thrown exception.");
        }

        [TestMethod]
        public void TestAssertXmlElement_007()
        {
            AssertHelper.AssertXml("<tile></tile>", "<tile />");
        }

        [TestMethod]
        public void TestAssertXmlElement_008()
        {
            AssertHelper.AssertXml("<tile><visual/></tile>", "<tile><visual/></tile>");

            try
            {
                AssertHelper.AssertXml("<tile><visual/></tile>", "<tile/>");
            }
            catch
            {
                return;
            }

            Assert.Fail("Visual element was missing, should have thrown exception");
        }

        [TestMethod]
        public void TestAssertXmlElement_009()
        {
            AssertHelper.AssertXml("<tile><image/><text/></tile>", "<tile><image/><text/></tile>");

            try
            {
                AssertHelper.AssertXml("<tile><image/><text/></tile>", "<tile><text/><image/></tile>");
            }
            catch
            {
                return;
            }

            Assert.Fail("Child elements were different order, should have thrown exception");
        }

        [TestMethod]
        public void TestAssertXmlElement_010()
        {
            try
            {
                AssertHelper.AssertXml("<tile version='3' id='2'/>", "<tile version='3' id='5'/>");
            }
            catch
            {
                return;
            }

            Assert.Fail("id attribute value wasn't the same, should have thrown exception");
        }

        [TestMethod]
        public void TestAssertXmlElement_011()
        {
            try
            {
                AssertHelper.AssertXml("<tile id='2' version='3'/>", "<tile version='3'/>");
            }
            catch
            {
                return;
            }

            Assert.Fail("id attribute was missing, should have thrown exception");
        }
    }

#pragma warning disable SA1204 // Static elements should appear before instance elements
#pragma warning disable SA1402 // File may only contain a single type
    public static class AssertHelper
#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore SA1204 // Static elements should appear before instance elements
    {
        private class XmlElementHelper
        {
        }

        /*
        public static void AssertXml(string expected, string actual)
        {
            XmlDocument expectedDoc = new XmlDocument();
            expectedDoc.LoadXml(expected);

            XmlDocument actualDoc = new XmlDocument();
            actualDoc.LoadXml(actual);

            AssertXmlElement(expectedDoc.DocumentElement, actualDoc.DocumentElement);
        }

        private static void AssertXmlElement(XmlElement expected, XmlElement actual)
        {
            // If both null, good, done
            if (expected == null && actual == null)
                return;

            // If one is null and other isn't, bad
            if (expected == null)
                Assert.Fail("Expected XML element was null, while actual was initialized");

            if (actual == null)
                Assert.Fail("Actual XML element was null, while expected was initialized");

            // If name doesn't match
            Assert.AreEqual(expected.Name, actual.Name, "Element names did not match.");

            // If attribute count doesn't match
            Assert.AreEqual(expected.Attributes.Count, actual.Attributes.Count, "Element attributes counts didn't match");

            // Make sure attributes match (order does NOT matter)
            foreach (XmlAttribute expectedAttr in expected.Attributes)
            {
                var actualAttr = actual.Attributes.GetNamedItem(expectedAttr.Name);

                // If didn't find the attribute
                if (actualAttr == null)
                    Assert.Fail("Expected element to have attribute " + expectedAttr.Name + " but it didn't.");

                // Make sure value matches
                Assert.AreEqual(expectedAttr.Value, actualAttr.Value, $@"Attribute values for ""{expectedAttr.Name}"" didn't match.");
            }

            // Make sure children elements match (order DOES matter)

            // Obtain the child elements (ignore any comments, w
            XmlElement[] expectedChildren = expected.ChildNodes.OfType<XmlElement>().ToArray();
            XmlElement[] actualChildren = actual.ChildNodes.OfType<XmlElement>().ToArray();

            Assert.AreEqual(expectedChildren.Length, actualChildren.Length, "Number of child elements did not match.");

            // If no elements, compare inner text
            if (expectedChildren.Length == 0)
            {
                Assert.AreEqual(expected.InnerText, actual.InnerText, "Inner text did not match.");
            }

            // Otherwise compare elements
            else
            {
                for (int i = 0; i < expectedChildren.Length; i++)
                {
                    AssertXmlElement(expectedChildren[i], actualChildren[i]);
                }
            }
        }
        */

        public static void AssertToast(string expected, ToastContent toast)
        {
            AssertHelper.AssertXml(expected.ToLower(), toast.GetContent().ToLower());

#if WINDOWS_UWP
            // For WinRT, we'll test the XmlDocument method too, make sure it works.
            AssertHelper.AssertXml(expected, toast.GetXml().GetXml());
#endif
        }

        public static void AssertTile(string expected, TileContent tile)
        {
            AssertHelper.AssertXml(expected.ToLower(), tile.GetContent().ToLower());

#if WINDOWS_UWP
            // For WinRT, we'll test the XmlDocument method too, make sure it works.
            AssertHelper.AssertXml(expected, tile.GetXml().GetXml());
#endif
        }

        public static void AssertXml(string expected, string actual)
        {
            MyXmlElement expectedEl = ParseXml(expected);
            MyXmlElement actualEl = ParseXml(actual);

            AssertXmlElement(expectedEl, actualEl);
        }

        private static string AttributesToString(IEnumerable<MyXmlAttribute> attributes)
        {
            return string.Join(", ", attributes.Select(i => i.Name));
        }

        private static void AssertXmlElement(MyXmlElement expected, MyXmlElement actual)
        {
            // If both null, good, done
            if (expected == null && actual == null)
            {
                return;
            }

            // If one is null and other isn't, bad
            if (expected == null)
            {
                Assert.Fail("Expected XML element was null, while actual was initialized");
            }

            if (actual == null)
            {
                Assert.Fail("Actual XML element was null, while expected was initialized");
            }

            // If name doesn't match
            Assert.AreEqual(expected.Name.ToLower(), actual.Name.ToLower(), "Element names did not match.");

            // If attribute count doesn't match
            Assert.AreEqual(expected.Attributes.Count, actual.Attributes.Count, $"Different number of attributes on <{expected.Name}>\n\nExpected: " + AttributesToString(expected.Attributes) + "\nActual: " + AttributesToString(actual.Attributes));

            // Make sure attributes match (order does NOT matter)
            foreach (MyXmlAttribute expectedAttr in expected.Attributes)
            {
                var actualAttr = actual.Attributes.FirstOrDefault(i => i.Name.Equals(expectedAttr.Name));

                // If didn't find the attribute
                if (actualAttr == null)
                {
                    Assert.Fail("Expected element to have attribute " + expectedAttr.Name + " but it didn't.");
                }

                // Make sure value matches
                Assert.AreEqual(expectedAttr.Value.ToLower(), actualAttr.Value.ToLower(), $@"Attribute values for ""{expectedAttr.Name}"" didn't match.");
            }

            // Make sure children elements match (order DOES matter)

            // Obtain the child elements (ignore any comments, w
            MyXmlElement[] expectedChildren = expected.ChildNodes.ToArray();
            MyXmlElement[] actualChildren = actual.ChildNodes.ToArray();

            Assert.AreEqual(expectedChildren.Length, actualChildren.Length, "Number of child elements did not match.");

            // Compare elements
            for (int i = 0; i < expectedChildren.Length; i++)
            {
                AssertXmlElement(expectedChildren[i], actualChildren[i]);
            }
        }

        private class MyXmlElement
        {
            public string Name { get; set; }

            public List<MyXmlElement> ChildNodes { get; private set; } = new List<MyXmlElement>();

            public List<MyXmlAttribute> Attributes { get; private set; } = new List<MyXmlAttribute>();
        }

        private class MyXmlAttribute
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }

        private static MyXmlElement ParseXml(string xml)
        {
            XmlReader reader = XmlReader.Create(new StringReader(xml));

            MyXmlElement documentElement = new MyXmlElement();

            reader.Read();

            while (true)
            {
                if (reader.ReadState == ReadState.EndOfFile)
                {
                    break;
                }

                if (reader.ReadState == ReadState.Error)
                {
                    throw new Exception("ReadState was Error");
                }

                if (reader.NodeType == XmlNodeType.Element)
                {
                    PopulateElement(reader, documentElement);
                    ParseXml(reader, documentElement);
                    break;
                }

                reader.Read();
            }

            return documentElement;
        }

        private static void PopulateElement(XmlReader reader, MyXmlElement into)
        {
            into.Name = reader.Name;

            int attrCount = reader.AttributeCount;
            for (int i = 0; i < attrCount; i++)
            {
                reader.MoveToNextAttribute();

                into.Attributes.Add(new MyXmlAttribute()
                {
                    Name = reader.Name,
                    Value = reader.Value
                });
            }
        }

        private static void ParseXml(XmlReader reader, MyXmlElement intoElement)
        {
            if (!reader.Read())
            {
                return;
            }

            while (true)
            {
                switch (reader.NodeType)
                {
                    // Found child
                    case XmlNodeType.Element:
                    case XmlNodeType.Text:
                        MyXmlElement child = new MyXmlElement();
                        PopulateElement(reader, child);
                        ParseXml(reader, child);
                        intoElement.ChildNodes.Add(child);
                        break;

                    // All done
                    case XmlNodeType.EndElement:
                        return;
                }

                if (!reader.Read())
                {
                    return;
                }
            }
        }
    }
}