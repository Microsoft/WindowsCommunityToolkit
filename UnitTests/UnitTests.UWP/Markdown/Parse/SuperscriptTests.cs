// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.Common.Parsers.Markdown.Blocks;
using CommunityToolkit.Common.Parsers.Markdown.Inlines;

namespace UnitTests.Markdown.Parse
{
    [TestClass]
    public class SuperscriptTests : ParseTestBase
    {
#pragma warning disable CS0618 // Type or member is obsolete
        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Superscript_Simple()
        {
            AssertEqual("Using the carot sign ^will create exponentials",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "Using the carot sign " },
                    new SuperscriptTextInline().AddChildren(
                        new TextRunInline { Text = "will" }),
                    new TextRunInline { Text = " create exponentials" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Superscript_Tag()
        {
            AssertEqual(
                "This is <sup>tag</sup> create test",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "This is " },
                    new SuperscriptTextInline().AddChildren(
                        new TextRunInline { Text = "tag" }),
                    new TextRunInline { Text = " create test" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Superscript_Nested()
        {
            AssertEqual("A^B^C",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "A" },
                    new SuperscriptTextInline().AddChildren(
                        new TextRunInline { Text = "B" },
                        new SuperscriptTextInline().AddChildren(
                            new TextRunInline { Text = "C" }))));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Superscript_WithParentheses()
        {
            // The text to superscript can be enclosed in brackets.
            AssertEqual("This is a sentence^(This is a note in superscript).",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "This is a sentence" },
                    new SuperscriptTextInline().AddChildren(
                        new TextRunInline { Text = "This is a note in superscript" }),
                    new TextRunInline { Text = "." }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Superscript_StartOfLine()
        {
            // Start of the line is okay.
            AssertEqual("^Test",
                new ParagraphBlock().AddChildren(
                    new SuperscriptTextInline().AddChildren(
                        new TextRunInline { Text = "Test" })));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Superscript_TwoInARow()
        {
            AssertEqual("^a ^b",
                new ParagraphBlock().AddChildren(
                    new SuperscriptTextInline().AddChildren(
                        new TextRunInline { Text = "a" }),
                    new TextRunInline { Text = " " },
                    new SuperscriptTextInline().AddChildren(
                        new TextRunInline { Text = "b" })));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Superscript_Escape()
        {
            // Escaped caret.
            AssertEqual(@"\^Test",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "^Test" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Superscript_Negative()
        {
            AssertEqual("Using the carot sign ^ incorrectly",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "Using the carot sign ^ incorrectly" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Superscript_Negative_WithParentheses()
        {
            AssertEqual("Using the carot sign ^ (incorrectly)",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "Using the carot sign ^ (incorrectly)" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Superscript_Negative_WithParentheses_2()
        {
            AssertEqual("Using the carot sign ^(incorrectly",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "Using the carot sign ^(incorrectly" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Superscript_Negative_WithParentheses_3()
        {
            AssertEqual("Using the carot sign ^(across\r\n\r\nparagraphs)",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "Using the carot sign ^(across" }),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "paragraphs)" }));
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}