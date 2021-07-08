// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommunityToolkit.Common.Parsers.Markdown.Inlines;
using CommunityToolkit.Common.Parsers.Markdown.Blocks;

namespace UnitTests.Markdown.Parse
{
#pragma warning disable CS0618 // Type or member is obsolete
    [TestClass]
    public class ParagraphTests : ParseTestBase
    {
        [TestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_Empty()
        {
            AssertEqual("", new MarkdownBlock[0]);
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_NoLineBreak()
        {
            // A line break in the markup does not translate to a line break in the resulting formatted text.
            AssertEqual(CollapseWhitespace(@"
                line 1
                line 2"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 1 line 2" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_NoLineBreak_OneSpace()
        {
            // A line break in the markup does not translate to a line break in the resulting formatted text.
            var space = " ";
            AssertEqual(CollapseWhitespace($@"
                line 1{space}
                line 2"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 1  line 2" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_LineBreak()
        {
            // Two spaces at the end of the line results in a line break.
            var spaces = "  ";
            AssertEqual(CollapseWhitespace($@"
                line 1{spaces}
                line 2 with *italic{spaces}
                formatting*"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 1\r\nline 2 with " },
                    new ItalicTextInline().AddChildren(
                        new TextRunInline { Text = "italic\r\nformatting" })));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_LineBreak_ThreeSpaces()
        {
            // Three spaces at the end of the line also results in a line break.
            var spaces = "   ";
            AssertEqual(CollapseWhitespace($@"
                line 1{spaces}
                line 2"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 1 \r\nline 2" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_NewParagraph()
        {
            // An empty line starts a new paragraph.
            AssertEqual(CollapseWhitespace(@"
                line 1

                line 2"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 1" }),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 2" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_NewParagraph_Whitespace()
        {
            // A line that contains only whitespace starts a new paragraph.
            var spaces = "      ";
            AssertEqual(CollapseWhitespace($@"
                line 1
                {spaces}
                line 2"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 1" }),
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "line 2" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_NoSpaceCompression()
        {
            // Multiple spaces are not collapsed; this is handled by the renderer.
            AssertEqual("one      two",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = "one      two" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_TextEscaping()
        {
            AssertEqual(@"\~\`\!\@\#\$\%\^\&\*\(\)\_\+\-\=\{\}\|\[\]\\\:\""\;\'\<\>\?\,\.\/\a\A\1",
                new ParagraphBlock().AddChildren(
                    new TextRunInline { Text = @"~`!\@#\$\%^&*()_+-\={}|[]\:\""\;\'<>\?\,./\a\A\1" }));
        }

        [TestMethod]
        [TestCategory("Parse - block")]
        public void Paragraph_Entities()
        {
            AssertEqual(CollapseWhitespace(@"
                &quot;
                &amp;
                &apos;
                &lt;
                &gt;
                &nbsp;
                &#160;
                &iexcl;
                &cent;
                &pound;
                &curren;
                &yen;
                &brvbar;
                &sect;
                &uml;
                &copy;
                &ordf;
                &laquo;
                &not;
                &shy;
                &reg;
                &macr;
                &deg;
                &plusmn;
                &sup2;
                &sup3;
                &acute;
                &micro;
                &para;
                &middot;
                &cedil;
                &sup1;
                &ordm;
                &raquo;
                &frac14;
                &frac12;
                &frac34;
                &iquest;
                &Agrave;
                &Aacute;
                &Acirc;
                &Atilde;
                &Auml;
                &Aring;
                &AElig;
                &Ccedil;
                &Egrave;
                &Eacute;
                &Ecirc;
                &Euml;
                &Igrave;
                &Iacute;
                &Icirc;
                &Iuml;
                &ETH;
                &Ntilde;
                &Ograve;
                &Oacute;
                &Ocirc;
                &Otilde;
                &Ouml;
                &times;
                &Oslash;
                &Ugrave;
                &Uacute;
                &Ucirc;
                &Uuml;
                &Yacute;
                &THORN;
                &szlig;
                &agrave;
                &aacute;
                &acirc;
                &atilde;
                &auml;
                &aring;
                &aelig;
                &ccedil;
                &egrave;
                &eacute;
                &ecirc;
                &euml;
                &igrave;
                &iacute;
                &icirc;
                &iuml;
                &eth;
                &ntilde;
                &ograve;
                &oacute;
                &ocirc;
                &otilde;
                &ouml;
                &divide;
                &oslash;
                &ugrave;
                &uacute;
                &ucirc;
                &uuml;
                &yacute;
                &thorn;
                &yuml;
                &OElig;
                &oelig;
                &Scaron;
                &scaron;
                &Yuml;
                &fnof;
                &circ;
                &tilde;
                &Alpha;
                &Beta;
                &Gamma;
                &Delta;
                &Epsilon;
                &Zeta;
                &Eta;
                &Theta;
                &Iota;
                &Kappa;
                &Lambda;
                &Mu;
                &Nu;
                &Xi;
                &Omicron;
                &Pi;
                &Rho;
                &Sigma;
                &Tau;
                &Upsilon;
                &Phi;
                &Chi;
                &Psi;
                &Omega;
                &alpha;
                &beta;
                &gamma;
                &delta;
                &epsilon;
                &zeta;
                &eta;
                &theta;
                &iota;
                &kappa;
                &lambda;
                &mu;
                &nu;
                &xi;
                &omicron;
                &pi;
                &rho;
                &sigmaf;
                &sigma;
                &tau;
                &upsilon;
                &phi;
                &chi;
                &psi;
                &omega;
                &thetasym;
                &upsih;
                &piv;
                &ensp;
                &emsp;
                &thinsp;
                &zwnj;
                &zwj;
                &lrm;
                &rlm;
                &ndash;
                &mdash;
                &lsquo;
                &rsquo;
                &sbquo;
                &ldquo;
                &rdquo;
                &bdquo;
                &dagger;
                &Dagger;
                &bull;
                &hellip;
                &permil;
                &prime;
                &Prime;
                &lsaquo;
                &rsaquo;
                &oline;
                &frasl;
                &euro;
                &image;
                &weierp;
                &real;
                &trade;
                &alefsym;
                &larr;
                &uarr;
                &rarr;
                &darr;
                &harr;
                &crarr;
                &lArr;
                &uArr;
                &rArr;
                &dArr;
                &hArr;
                &forall;
                &part;
                &exist;
                &empty;
                &nabla;
                &isin;
                &notin;
                &ni;
                &prod;
                &sum;
                &minus;
                &lowast;
                &radic;
                &prop;
                &infin;
                &ang;
                &and;
                &or;
                &cap;
                &cup;
                &int;
                &there4;
                &sim;
                &cong;
                &asymp;
                &ne;
                &equiv;
                &le;
                &ge;
                &sub;
                &sup;
                &nsub;
                &sube;
                &supe;
                &oplus;
                &otimes;
                &perp;
                &sdot;
                &lceil;
                &rceil;
                &lfloor;
                &rfloor;
                &lang;
                &rang;
                &loz;
                &spades;
                &clubs;
                &hearts;
                &diams;"),
                new ParagraphBlock().AddChildren(
                    new TextRunInline
                    {
                        Text = @""" & ' < >     ¡ ¢ £ ¤ ¥ ¦ § ¨ © ª « ¬ ­ ® ¯ ° ± ² ³ ´ µ ¶ · ¸ ¹ º » ¼ ½ ¾ " +
                    "¿ À Á Â Ã Ä Å Æ Ç È É Ê Ë Ì Í Î Ï Ð Ñ Ò Ó Ô Õ Ö × Ø Ù Ú Û Ü Ý Þ ß à á â ã ä å æ ç è é ê ë ì í î ï ð ñ ò ó ô " +
                    "õ ö ÷ ø ù ú û ü ý þ ÿ Œ œ Š š Ÿ ƒ ˆ ˜ Α Β Γ Δ Ε Ζ Η Θ Ι Κ Λ Μ Ν Ξ Ο Π Ρ Σ Τ Υ Φ Χ Ψ Ω α β γ δ ε ζ η θ ι κ λ " +
                    "μ ν ξ ο π ρ ς σ τ υ φ χ ψ ω ϑ ϒ ϖ       ‌ ‍ ‎ ‏ – — ‘ ’ ‚ “ ” „ † ‡ • … ‰ ′ ″ ‹ › ‾ ⁄ € ℑ ℘ ℜ ™ ℵ ← ↑ → ↓ ↔ ↵ ⇐ " +
                    "⇑ ⇒ ⇓ ⇔ ∀ ∂ ∃ ∅ ∇ ∈ ∉ ∋ ∏ ∑ − ∗ √ ∝ ∞ ∠ ∧ ∨ ∩ ∪ ∫ ∴ ∼ ≅ ≈ ≠ ≡ ≤ ≥ ⊂ ⊃ ⊄ ⊆ ⊇ ⊕ ⊗ ⊥ ⋅ ⌈ ⌉ ⌊ ⌋ 〈 〉 ◊ ♠ ♣ ♥ ♦"
                    }));
        }
#pragma warning restore CS0618 // Type or member is obsolete
    }
}