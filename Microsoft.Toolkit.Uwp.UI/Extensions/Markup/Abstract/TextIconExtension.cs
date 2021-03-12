// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Text;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// An abstract <see cref="MarkupExtension"/> which to produce text-based icons.
    /// </summary>
    public abstract class TextIconExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the size of the icon to display.
        /// </summary>
        public double FontSize { get; set; }

        [ThreadStatic]
        private static FontFamily segoeMDL2AssetsFontFamily;

        /// <summary>
        /// Gets the reusable "Segoe MDL2 Assets" <see cref="FontFamily"/> instance.
        /// </summary>
        protected static FontFamily SegoeMDL2AssetsFontFamily
        {
            get => segoeMDL2AssetsFontFamily ??= new FontFamily("Segoe MDL2 Assets");
        }

        /// <summary>
        /// Gets or sets the thickness of the icon glyph.
        /// </summary>
        public FontWeight FontWeight { get; set; } = FontWeights.Normal;

        /// <summary>
        /// Gets or sets the font style for the icon glyph.
        /// </summary>
        public FontStyle FontStyle { get; set; } = FontStyle.Normal;

        /// <summary>
        /// Gets or sets the foreground <see cref="Brush"/> for the icon.
        /// </summary>
        public Brush Foreground { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether automatic text enlargement, to reflect the system text size setting, is enabled.
        /// </summary>
        public bool IsTextScaleFactorEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the icon is mirrored when the flow direction is right to left.
        /// </summary>
        public bool MirroredWhenRightToLeft { get; set; }
    }
}
