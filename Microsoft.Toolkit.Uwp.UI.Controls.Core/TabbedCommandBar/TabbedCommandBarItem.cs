// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// A <see cref="CommandBar"/> to be displayed in a <see cref="TabbedCommandBar"/>
    /// </summary>
    [TemplatePart(Name = "PrimaryItemsControl", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "MoreButton", Type = typeof(Button))]
    public class TabbedCommandBarItem : CommandBar
    {
        private ItemsControl _primaryItemsControl;
        private Button _moreButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="TabbedCommandBarItem"/> class.
        /// </summary>
        public TabbedCommandBarItem()
        {
            DefaultStyleKey = typeof(TabbedCommandBarItem);
        }

        /// <summary>
        /// Identifies the <see cref="Header"/> property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            nameof(Header),
            typeof(object),
            typeof(TabbedCommandBarItem),
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or sets the text or <see cref="UIElement"/> to display in the header of this TabbedCommandBar tab.
        /// </summary>
        public object Header
        {
            get => (object)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsContextual"/> property.
        /// </summary>
        public static readonly DependencyProperty IsContextualProperty = DependencyProperty.Register(
            nameof(IsContextual),
            typeof(bool),
            typeof(TabbedCommandBarItem),
            new PropertyMetadata(false));

        /// <summary>
        /// Gets or sets a value indicating whether this tab is contextual.
        /// </summary>
        public bool IsContextual
        {
            get => (bool)GetValue(IsContextualProperty);
            set => SetValue(IsContextualProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="OverflowButtonAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty OverflowButtonAlignmentProperty = DependencyProperty.Register(
            nameof(OverflowButtonAlignment),
            typeof(HorizontalAlignment),
            typeof(TabbedCommandBarItem),
            new PropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// Gets or sets a value indicating the alignment of the command overflow button.
        /// </summary>
        public HorizontalAlignment OverflowButtonAlignment
        {
            get => (HorizontalAlignment)GetValue(OverflowButtonAlignmentProperty);
            set => SetValue(OverflowButtonAlignmentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CommandAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty CommandAlignmentProperty = DependencyProperty.Register(
            nameof(CommandAlignment),
            typeof(HorizontalAlignment),
            typeof(TabbedCommandBarItem),
            new PropertyMetadata(HorizontalAlignment.Stretch));

        /// <summary>
        /// Gets or sets a value indicating the alignment of the commands in the <see cref="TabbedCommandBarItem"/>.
        /// </summary>
        public HorizontalAlignment CommandAlignment
        {
            get => (HorizontalAlignment)GetValue(CommandAlignmentProperty);
            set => SetValue(CommandAlignmentProperty, value);
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _primaryItemsControl = GetTemplateChild("PrimaryItemsControl") as ItemsControl;
            if (_primaryItemsControl != null)
            {
                _primaryItemsControl.HorizontalAlignment = CommandAlignment;
                RegisterPropertyChangedCallback(CommandAlignmentProperty, (sender, dp) =>
                {
                    _primaryItemsControl.HorizontalAlignment = (HorizontalAlignment)sender.GetValue(dp);
                });
            }

            _moreButton = GetTemplateChild("MoreButton") as Button;
            if (_moreButton != null)
            {
                _moreButton.HorizontalAlignment = OverflowButtonAlignment;
                RegisterPropertyChangedCallback(OverflowButtonAlignmentProperty, (sender, dp) =>
                {
                    _moreButton.HorizontalAlignment = (HorizontalAlignment)sender.GetValue(dp);
                });
            }
        }
    }
}