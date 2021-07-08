// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.Common.Parsers.Markdown.Blocks;
using CommunityToolkit.Common.Parsers.Markdown.Inlines;

namespace UnitTests.Markdown.Parse
{
    [TestClass]
    public class BoldTests : ParseTestBase
    {
#pragma warning disable CS0618 // Type or member is obsolete
        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Bold_Simple()
        {
            AssertEqual("**bold**", new ParagraphBlock().AddChildren(
                new BoldTextInline().AddChildren(
                    new TextRunInline { Text = "bold" })));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Bold_Simple_Alt()
        {
            AssertEqual("__bold__", new ParagraphBlock().AddChildren(
                new BoldTextInline().AddChildren(
                    new TextRunInline { Text = "bold" })));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Bold_Inline()
        {
            AssertEqual("This is **bold** text", new ParagraphBlock().AddChildren(
                new TextRunInline { Text = "This is " },
                new BoldTextInline().AddChildren(
                    new TextRunInline { Text = "bold" }),
                new TextRunInline { Text = " text" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Bold_Inline_Alt()
        {
            AssertEqual("This is __bold__ text", new ParagraphBlock().AddChildren(
                new TextRunInline { Text = "This is " },
                new BoldTextInline().AddChildren(
                    new TextRunInline { Text = "bold" }),
                new TextRunInline { Text = " text" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Bold_Inside_Word()
        {
            AssertEqual("before**middle**end", new ParagraphBlock().AddChildren(
                new TextRunInline { Text = "before" },
                new BoldTextInline().AddChildren(
                    new TextRunInline { Text = "middle" }),
                new TextRunInline { Text = "end" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Bold_Negative_1()
        {
            AssertEqual("before** middle **end", new ParagraphBlock().AddChildren(
                new TextRunInline { Text = "before** middle **end" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Bold_Negative_2()
        {
            AssertEqual("before** middle**end", new ParagraphBlock().AddChildren(
                new TextRunInline { Text = "before** middle**end" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Bold_Negative_CannotBeEmpty()
        {
            AssertEqual("before ****** after",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before ****** after" }));
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}