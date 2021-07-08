// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Common.Parsers.Markdown.Render;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.UI.Controls.Markdown.Render
{
    /// <summary>
    /// The Context of the Current Document Rendering.
    /// </summary>
    public class UIElementCollectionRenderContext : RenderContext
    {
        internal UIElementCollectionRenderContext(UIElementCollection blockUIElementCollection)
        {
            BlockUIElementCollection = blockUIElementCollection;
        }

        internal UIElementCollectionRenderContext(UIElementCollection blockUIElementCollection, IRenderContext context)
            : this(blockUIElementCollection)
        {
            TrimLeadingWhitespace = context.TrimLeadingWhitespace;
            Parent = context.Parent;

            if (context is RenderContext localcontext)
            {
                Foreground = localcontext.Foreground;
                OverrideForeground = localcontext.OverrideForeground;
            }
        }

        /// <summary>
        /// Gets or sets the list to add to.
        /// </summary>
        public UIElementCollection BlockUIElementCollection { get; set; }
    }
}