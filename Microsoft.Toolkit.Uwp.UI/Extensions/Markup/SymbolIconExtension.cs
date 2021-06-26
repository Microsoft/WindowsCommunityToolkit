// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Custom <see cref="MarkupExtension"/> which can provide symbol-based <see cref="FontIcon"/> values.
    /// </summary>
    [MarkupExtensionReturnType(ReturnType = typeof(FontIcon))]
    public class SymbolIconExtension : TextIconExtension
    {
        /// <summary>
        /// Gets or sets the <see cref="Windows.UI.Xaml.Controls.Symbol"/> value representing the icon to display.
        /// </summary>
        public Symbol Symbol { get; set; }

        /// <inheritdoc/>
        protected override object ProvideValue()
        {
            var fontIcon = new FontIcon
            {
                Glyph = unchecked((char)Symbol).ToString(),
                FontFamily = SegoeMDL2AssetsFontFamily,
                FontWeight = FontWeight,
                FontStyle = FontStyle,
                IsTextScaleFactorEnabled = IsTextScaleFactorEnabled,
                MirroredWhenRightToLeft = MirroredWhenRightToLeft
            };

            if (FontSize > 0)
            {
                fontIcon.FontSize = FontSize;
            }

            if (Foreground != null)
            {
                fontIcon.Foreground = Foreground;
            }

            return fontIcon;
        }
    }
}