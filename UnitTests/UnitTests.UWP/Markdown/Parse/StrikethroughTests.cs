// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.Common.Parsers.Markdown.Blocks;
using CommunityToolkit.Common.Parsers.Markdown.Inlines;

namespace UnitTests.Markdown.Parse
{
    [TestClass]
    public class StrikethroughTests : ParseTestBase
    {
#pragma warning disable CS0618 // Type or member is obsolete
        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Strikethrough_Simple()
        {
            AssertEqual("~~strike~~",
                new ParagraphBlock().AddChildren(
                    new StrikethroughTextInline().AddChildren(
                        new TextRunInline { Text = "strike" })));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Strikethrough_Inline()
        {
            AssertEqual("This is ~~strike~~ text",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "This is " },
                    new StrikethroughTextInline().AddChildren(
                        new TextRunInline { Text = "strike" }),
                    new TextRunInline { Text = " text" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Strikethrough_Negative_1()
        {
            AssertEqual(@"~~ strike~~",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "~~ strike~~" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Strikethrough_Negative_2()
        {
            AssertEqual(@"~~strike ~~",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "~~strike ~~" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Strikethrough_Negative_CannotBeEmpty()
        {
            AssertEqual(@"~~~~",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "~~~~" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Strikethrough_Escape_1()
        {
            AssertEqual(@"\~~strike~~",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "~~strike~~" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Strikethrough_Escape_2()
        {
            AssertEqual(@"~~strike\~~",
                new ParagraphBlock().AddChildren(
                    new StrikethroughTextInline().AddChildren(
                        new TextRunInline { Text = @"strike\" })));
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}