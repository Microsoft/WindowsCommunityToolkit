// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarButtons;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Toolbar for Editing Text attached to a RichEditBox
    /// </summary>
    [TemplatePart(Name = RootControl, Type = typeof(CommandBar))]
    public partial class TextToolbar : Control
    {
        internal const string RootControl = "Root";
        internal const string BoldElement = "Bold";
        internal const string ItalicsElement = "Italics";
        internal const string StrikethoughElement = "Strikethrough";
        internal const string LinkElement = "Link";
        internal const string ListElement = "List";
        internal const string OrderedElement = "OrderedList";

        /// <summary>
        /// Initializes a new instance of the <see cref="TextToolbar"/> class.
        /// </summary>
        public TextToolbar()
        {
            DefaultStyleKey = typeof(TextToolbar);

            CustomButtons = new ButtonMap();
            ButtonModifications = new DefaultButtonModificationList();

            if (!InDesignMode)
            {
                KeyEventHandler = new KeyEventHandler(Editor_KeyDown);
            }
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            if (Formatter == null)
            {
                throw new InvalidOperationException("No formatter specified.");
            }
            else
            {
                BuildBar();
            }

            base.OnApplyTemplate();
        }
    }
}