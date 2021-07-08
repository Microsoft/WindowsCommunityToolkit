// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.Common.Parsers.Markdown.Blocks;
using CommunityToolkit.Common.Parsers.Markdown.Inlines;

namespace UnitTests.Markdown.Parse
{
    [TestClass]
    public class ImageInlineTests : ParseTestBase
    {
#pragma warning disable CS0618 // Type or member is obsolete
        [TestMethod]
        [TestCategory("Parse - inline")]
        public void ImageInline_WithWidth()
        {
            AssertEqual(
                "![SVG logo](https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg =1)",
                new ParagraphBlock().AddChildren(
                    new ImageInline
                    {
                        Url = "https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg",
                        Text = "![SVG logo](https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg =1)",
                        Tooltip = "SVG logo",
                        ImageWidth = 1,
                        ImageHeight = 0,
                        RenderUrl = "https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg",
                        ReferenceId = string.Empty
                    }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void ImageInline_WithHeight()
        {
            AssertEqual(
                "![SVG logo](https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg =x1)",
                new ParagraphBlock().AddChildren(
                    new ImageInline
                    {
                        Url = "https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg",
                        Text = "![SVG logo](https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg =x1)",
                        Tooltip = "SVG logo",
                        ImageWidth = 0,
                        ImageHeight = 1,
                        RenderUrl = "https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg",
                        ReferenceId = string.Empty
                    }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void ImageInline_WithWidthAndHeight()
        {
            AssertEqual(
                "![SVG logo](https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg =128x64)",
                new ParagraphBlock().AddChildren(
                    new ImageInline
                    {
                        Url = "https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg",
                        Tooltip = "SVG logo",
                        Text = "![SVG logo](https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg =128x64)",
                        ImageWidth = 128,
                        ImageHeight = 64,
                        RenderUrl = "https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg",
                        ReferenceId = string.Empty
                    }));
        }

        [TestMethod]
        [TestCategory("Parse - inline")]
        public void ImageInline_ParseEncodedUrl()
        {
            AssertEqual(
                "![SVG logo](https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg%20=32)",
                new ParagraphBlock().AddChildren(
                    new ImageInline
                    {
                        Url = "https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg%20=32",
                        Tooltip = "SVG logo",
                        Text = "![SVG logo](https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg%20=32)",
                        ImageWidth = 0,
                        ImageHeight = 0,
                        RenderUrl = "https://upload.wikimedia.org/wikipedia/commons/0/02/SVG_logo.svg%20=32",
                        ReferenceId = string.Empty
                    }));
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}