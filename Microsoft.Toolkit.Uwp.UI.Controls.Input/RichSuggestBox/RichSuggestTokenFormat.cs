// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;
using Windows.UI.Text;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// RichSuggestTokenFormat describes how a token should be formatted.
    /// </summary>
    public class RichSuggestTokenFormat
    {
        /// <summary>
        /// Gets or sets token foreground color.
        /// </summary>
        public Color Foreground { get; set; }

        /// <summary>
        /// Gets or sets token background color.
        /// </summary>
        public Color Background { get; set; }

        /// <summary>
        /// Gets or sets token italic style.
        /// </summary>
        public FormatEffect Italic { get; set; }

        /// <summary>
        /// Gets or sets token bold style.
        /// </summary>
        public FormatEffect Bold { get; set; }

        /// <summary>
        /// Gets or sets the degree to which the font is stretched, compared to the normal aspect ratio of the font.
        /// </summary>
        public FontStretch FontStretch { get; set; }

        /// <summary>
        /// Gets or sets the style of the font face, such as normal or italic.
        /// </summary>
        public FontStyle FontStyle { get; set; }

        /// <summary>
        /// Gets or sets whether characters are displayed with a horizontal line through the center.
        /// </summary>
        public FormatEffect Strikethrough { get; set; }

        /// <summary>
        /// Gets or sets whether characters are displayed as outlined characters.
        /// </summary>
        public FormatEffect Outline { get; set; }

        /// <summary>
        /// Gets or sets whether characters are displayed as subscript.
        /// </summary>
        public FormatEffect Subscript { get; set; }

        /// <summary>
        /// Gets or sets whether characters are displayed as superscript.
        /// </summary>
        public FormatEffect Superscript { get; set; }

        /// <summary>
        /// Gets or sets the font name.
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// Gets or sets the minimum font size at which kerning occurs.
        /// </summary>
        public float Kerning { get; set; }

        /// <summary>
        /// Gets or sets the font size.
        /// </summary>
        public float FontSize { get; set; }

        /// <summary>
        /// Gets or sets the font weight of the characters.
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// Gets or sets the amount of horizontal spacing between characters.
        /// </summary>
        public float Spacing { get; set; }

        /// <summary>
        /// Gets or sets the character offset relative to the baseline.
        /// </summary>
        public float Position { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RichSuggestTokenFormat"/> class.
        /// </summary>
        /// <param name="source">Source formatting to initialize from.</param>
        public RichSuggestTokenFormat(ITextCharacterFormat source)
        {
            Foreground = source.ForegroundColor;
            Background = source.BackgroundColor;
            Italic = source.Italic;
            Bold = source.Bold;
            FontStretch = source.FontStretch;
            FontStyle = source.FontStyle;
            Strikethrough = source.Strikethrough;
            Outline = source.Outline;
            Subscript = source.Subscript;
            Superscript = source.Superscript;
            FontName = source.Name;
            Kerning = source.Kerning;
            FontSize = source.Size;
            Weight = source.Weight;
            Spacing = source.Spacing;
            Position = source.Position;
        }
    }
}
