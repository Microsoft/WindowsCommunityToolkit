// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Controls.TextToolbarButtons;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarFormats;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarFormats.RichText;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Toolbar for Editing Text attached to a RichEditBox
    /// </summary>
    public partial class TextToolbar
    {
        /// <summary>
        /// Identifies the <see cref="Editor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EditorProperty =
            DependencyProperty.Register(nameof(Editor), typeof(RichEditBox), typeof(TextToolbar), new PropertyMetadata(null, OnEditorChanged));

        /// <summary>
        /// Identifies the <see cref="Formatter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FormatterProperty =
            DependencyProperty.Register(nameof(Formatter), typeof(Formatter), typeof(TextToolbar), new PropertyMetadata(new RichTextFormatter(), OnFormatterChanged));

        /// <summary>
        /// Identifies the <see cref="DefaultButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultButtonsProperty =
            DependencyProperty.Register(nameof(DefaultButtons), typeof(ButtonMap), typeof(TextToolbar), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CustomButtons"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomButtonsProperty =
            DependencyProperty.Register(nameof(CustomButtons), typeof(ButtonMap), typeof(TextToolbar), new PropertyMetadata(null, OnButtonMapChanged));

        /// <summary>
        /// Identifies the <see cref="ButtonModifications"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultButtonModificationsProperty =
            DependencyProperty.Register(nameof(ButtonModifications), typeof(DefaultButtonModificationList), typeof(TextToolbar), new PropertyMetadata(null, OnDefaultButtonModificationsChanged));

        /// <summary>
        /// Identifies the <see cref="UseURIChecker"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UseURICheckerProperty =
            DependencyProperty.Register(nameof(UseURIChecker), typeof(bool), typeof(TextToolbar), new PropertyMetadata(true));

        /// <summary>
        /// Gets or sets the RichEditBox to Attach to, this is required for any formatting to work.
        /// </summary>
        public RichEditBox Editor
        {
            get { return (RichEditBox)GetValue(EditorProperty); }
            set { SetValue(EditorProperty, value); }
        }

        /// <summary>
        /// Gets or sets the formatter instance which is used to format the text, using the buttons and shortcuts.
        /// </summary>
        public Formatter Formatter
        {
            get { return (Formatter)GetValue(FormatterProperty); }
            set { SetValue(FormatterProperty, value); }
        }

        /// <summary>
        /// Gets the default buttons for this format.
        /// </summary>
        public ButtonMap DefaultButtons
        {
            get { return (ButtonMap)GetValue(DefaultButtonsProperty); }
            private set { SetValue(DefaultButtonsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a list of buttons to add on top of the Default Button set.
        /// </summary>
        public ButtonMap CustomButtons
        {
            get { return (ButtonMap)GetValue(CustomButtonsProperty); }
            set { SetValue(CustomButtonsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a list of Default buttons to Modify.
        /// </summary>
        public DefaultButtonModificationList ButtonModifications
        {
            get { return (DefaultButtonModificationList)GetValue(DefaultButtonModificationsProperty); }
            set { SetValue(DefaultButtonModificationsProperty, value); }
        }

        /// <summary>
        /// Gets the last key pressed using the Editor.
        /// </summary>
        public VirtualKey LastKeyPress { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to enable use of URI Checker for Link Creator. This allows you to verify Absolute URIs, before creating the Link.
        /// </summary>
        public bool UseURIChecker
        {
            get { return (bool)GetValue(UseURICheckerProperty); }
            set { SetValue(UseURICheckerProperty, value); }
        }

        internal static bool InDesignMode
        {
            get { return Windows.ApplicationModel.DesignMode.DesignModeEnabled; }
        }
    }
}