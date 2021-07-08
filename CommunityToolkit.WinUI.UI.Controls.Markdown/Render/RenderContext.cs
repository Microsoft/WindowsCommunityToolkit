// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Common.Parsers.Markdown.Render;
using Microsoft.UI.Xaml.Media;

namespace CommunityToolkit.WinUI.UI.Controls.Markdown.Render
{
    /// <summary>
    /// The Context of the Current Position
    /// </summary>
    public abstract class RenderContext : IRenderContext
    {
        /// <summary>
        /// Gets or sets the Foreground of the Current Context.
        /// </summary>
        public Brush Foreground { get; set; }

        /// <inheritdoc/>
        public bool TrimLeadingWhitespace { get; set; }

        /// <inheritdoc/>
        public object Parent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to override the Foreground Property.
        /// </summary>
        public bool OverrideForeground { get; set; }

        /// <inheritdoc/>
        public IRenderContext Clone()
        {
            return (IRenderContext)MemberwiseClone();
        }
    }
}