// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Common.Parsers.Markdown.Blocks;
using CommunityToolkit.Common.Parsers.Markdown.Inlines;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Markdown.Parse
{
    [TestClass]
    public class SubscriptTests : ParseTestBase
    {
#pragma warning disable CS0618 // Type or member is obsolete
        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Superscript_Tag()
        {
            AssertEqual(
                "This is <sub>tag</sub> create test",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "This is " },
                    new SubscriptTextInline().AddChildren(
                        new TextRunInline { Text = "tag" }),
                    new TextRunInline { Text = " create test" }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void Superscript_TagWithSup()
        {
            AssertEqual(
                "This is <sub>**tag**</sub> create test",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "This is " },
                    new SubscriptTextInline().AddChildren(
                        new BoldTextInline().AddChildren(
                        new TextRunInline { Text = "tag" })),
                    new TextRunInline { Text = " create test" }));
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}