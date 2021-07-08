// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.Common.Parsers.Markdown.Blocks;
using CommunityToolkit.Common.Parsers.Markdown.Inlines;

namespace UnitTests.Markdown.Parse
{
    [TestClass]
    public class HorizontalRuleTests : ParseTestBase
    {
#pragma warning disable CS0618 // Type or member is obsolete
        [TestMethod]
        [TestCategory("Parse - block")]
        public void HorizontalRule_Simple()
        {
            AssertEqual("***",
                new HorizontalRuleBlock());
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void HorizontalRule_StarsAndSpaces()
        {
            AssertEqual("* * * * *",
                new HorizontalRuleBlock());
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void HorizontalRule_Alt()
        {
            AssertEqual("---",
                new HorizontalRuleBlock());
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void HorizontalRule_Alt_BeforeAfter()
        {
            AssertEqual(CollapseWhitespace(@"
                before

                ---
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" }),
                new HorizontalRuleBlock(),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "after" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void HorizontalRule_Alt2()
        {
            AssertEqual(CollapseWhitespace(@"
                before
                ___
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" }),
                new HorizontalRuleBlock(),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "after" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void HorizontalRule_BeforeAfter()
        {
            // Text on other lines is okay.
            AssertEqual(CollapseWhitespace(@"
                before
                *****
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" }),
                new HorizontalRuleBlock(),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "after" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void HorizontalRule_Negative()
        {
            // Text on the same line is not.
            AssertEqual(CollapseWhitespace(@"
                before
                ****d
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before ****d after" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void HorizontalRule_Negative_FourStars()
        {
            // Also, must be at least 3 stars.
            AssertEqual(CollapseWhitespace(@"
                before
                **
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before ** after" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void HorizontalRule_DoubleLine()
        {
            AssertEqual(
                CollapseWhitespace(@"
                ---
                ---
                "),
                new HorizontalRuleBlock(),
                new HorizontalRuleBlock());
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}