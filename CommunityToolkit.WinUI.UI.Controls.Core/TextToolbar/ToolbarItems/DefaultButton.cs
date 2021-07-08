// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Controls.TextToolbarButtons
{
    /// <summary>
    /// Specifies a DefaultButton, modifies a Button Instance
    /// </summary>
    public partial class DefaultButton : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="IsVisible"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty =
            DependencyProperty.Register(nameof(IsVisible), typeof(bool), typeof(DefaultButton), new PropertyMetadata(true));

        /// <summary>
        /// Specifies the Type of DefaultButton in order to remove it.
        /// </summary>
        /// <param name="type">Type of Default Button</param>
        /// <returns>Removal Object</returns>
        public static DefaultButton OfType(ButtonType type)
        {
            return new DefaultButton { Type = type };
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var other = obj as DefaultButton;
            return other != null && other.Type == Type;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Type.GetHashCode();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Type.ToString();
        }

        private static void IsVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DefaultButton button && button.Button != null)
            {
                var model = button.Button as FrameworkElement;
                model.Visibility = button.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Toolbar Item is Visible
        /// </summary>
        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set { SetValue(IsVisibleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the type of Default Button to remove.
        /// </summary>
        public ButtonType Type { get; set; }

        /// <summary>
        /// Gets or sets the instance of button that is removed, in order to preserve any modifications when re-attaching to the Toolbar.
        /// </summary>
        internal IToolbarItem Button
        {
            get
            {
                return _button;
            }

            set
            {
                _button = value;
                if (_button != null)
                {
                    var element = _button as FrameworkElement;
                    element.Visibility = IsVisible ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        private IToolbarItem _button;
    }
}