// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CommunityToolkit.Common.Parsers.Markdown.Blocks;
using CommunityToolkit.Common.Parsers.Markdown.Inlines;
using CommunityToolkit.Common.Parsers.Markdown;

namespace UnitTests.Markdown.Parse
{
    [TestClass]
    public class HeaderTests : ParseTestBase
    {
#pragma warning disable CS0618 // Type or member is obsolete
        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_1()
        {
            AssertEqual("#Header 1",
                new HeaderBlock { HeaderLevel = 1 }.AddChildren(
                    new TextRunInline { Text = "Header 1" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_1_Alt()
        {
            AssertEqual(CollapseWhitespace(@"
                Header 1
                ="),
                new HeaderBlock { HeaderLevel = 1 }.AddChildren(
                    new TextRunInline { Text = "Header 1" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_1_Alt_WithHashes()
        {
            AssertEqual(CollapseWhitespace(@"
                Header 1##
                ="),
                new HeaderBlock { HeaderLevel = 1 }.AddChildren(
                    new TextRunInline { Text = "Header 1##" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_2()
        {
            AssertEqual("##Header 2",
                new HeaderBlock { HeaderLevel = 2 }.AddChildren(
                    new TextRunInline { Text = "Header 2" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_2_Alt()
        {
            /***
             * Note: This is a tricky scenario because Header 2's
             * can be easily confused with thematic breaks, see specs:
             * https://spec.commonmark.org/0.29/#setext-headings
             * https://spec.commonmark.org/0.29/#thematic-breaks
             ***/

            // Note: trailing spaces on the second line are okay.
            AssertEqual(CollapseWhitespace(@"
                Header 2
                --  "),
                new HeaderBlock { HeaderLevel = 2 }.AddChildren(
                    new TextRunInline { Text = "Header 2" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_2_Alt_NotHorizontalRule()
        {
            /***
             * Note: This is a tricky scenario because Header 2's
             * can be easily confused with thematic breaks, see specs:
             * https://spec.commonmark.org/0.29/#setext-headings
             * https://spec.commonmark.org/0.29/#thematic-breaks
             ***/

            // Note: trailing spaces on the second line are okay.
            AssertEqual(
                CollapseWhitespace(@"
                    Header 2
                    ---  "),
                new HeaderBlock { HeaderLevel = 2 }.AddChildren(
                    new TextRunInline { Text = "Header 2" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_3()
        {
            AssertEqual("###Header 3",
                new HeaderBlock { HeaderLevel = 3 }.AddChildren(
                    new TextRunInline { Text = "Header 3" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_4()
        {
            AssertEqual("####Header 4",
                new HeaderBlock { HeaderLevel = 4 }.AddChildren(
                    new TextRunInline { Text = "Header 4" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_5()
        {
            AssertEqual("#####Header 5",
                new HeaderBlock { HeaderLevel = 5 }.AddChildren(
                    new TextRunInline { Text = "Header 5" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_6()
        {
            AssertEqual("######Header 6",
                new HeaderBlock { HeaderLevel = 6 }.AddChildren(
                    new TextRunInline { Text = "Header 6" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_6_WithTrailingHashSymbols()
        {
            AssertEqual("###### Header 6 ######",
                new HeaderBlock { HeaderLevel = 6 }.AddChildren(
                    new TextRunInline { Text = " Header 6 " }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_7()
        {
            AssertEqual("#######Header 6",
                new HeaderBlock { HeaderLevel = 6 }.AddChildren(
                    new TextRunInline { Text = "#Header 6" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_1_Empty()
        {
            AssertEqual("#",
                new HeaderBlock { HeaderLevel = 1, Inlines = new List<MarkdownInline>() });
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_Hashes()
        {
            AssertEqual("## # # ##",
                new HeaderBlock { HeaderLevel = 2 }.AddChildren(
                    new TextRunInline { Text = " # # " }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_6_Empty()
        {
            AssertEqual("#######",
                new HeaderBlock { HeaderLevel = 6, Inlines = new List<MarkdownInline>() });
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_1_Inline()
        {
            AssertEqual(CollapseWhitespace(@"
                before
                #Header
                after"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "before" }),
                new HeaderBlock { HeaderLevel = 1 }.AddChildren(
                    new TextRunInline { Text = "Header" }),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "after" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_Negative_RogueCharacter()
        {
            // The second line after a heading must be all === or all ---
            AssertEqual(CollapseWhitespace(@"
                Header 1
                =f"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "Header 1 =f" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Header_Negative_ExtraSpace()
        {
            // The second line after a heading must not start with a space
            AssertEqual(CollapseWhitespace(@"
                Header 1
                  ="),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "Header 1   =" }));
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}