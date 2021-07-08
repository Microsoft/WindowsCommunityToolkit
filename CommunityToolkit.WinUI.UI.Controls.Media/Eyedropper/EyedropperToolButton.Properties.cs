// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// The <see cref="EyedropperToolButton"/> control helps use <see cref="Eyedropper"/> in view.
    /// </summary>
    public partial class EyedropperToolButton
    {
        /// <summary>
        /// Identifies the <see cref="Color"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register(nameof(Color), typeof(Color), typeof(EyedropperToolButton), new PropertyMetadata(default(Color)));

        /// <summary>
        /// Identifies the <see cref="EyedropperEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EyedropperEnabledProperty =
            DependencyProperty.Register(nameof(EyedropperEnabled), typeof(bool), typeof(EyedropperToolButton), new PropertyMetadata(false, OnEyedropperEnabledChanged));

        /// <summary>
        /// Identifies the <see cref="EyedropperStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EyedropperStyleProperty =
            DependencyProperty.Register(nameof(EyedropperStyle), typeof(Style), typeof(EyedropperToolButton), new PropertyMetadata(default(Style), OnEyedropperStyleChanged));

        /// <summary>
        /// Identifies the <see cref="TargetElement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetElementProperty =
            DependencyProperty.Register(nameof(TargetElement), typeof(FrameworkElement), typeof(EyedropperToolButton), new PropertyMetadata(default(FrameworkElement), OnTargetElementChanged));

        /// <summary>
        /// Gets the current color value.
        /// </summary>
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            private set => SetValue(ColorProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether eyedropper is opened.
        /// </summary>
        public bool EyedropperEnabled
        {
            get => (bool)GetValue(EyedropperEnabledProperty);
            set => SetValue(EyedropperEnabledProperty, value);
        }

        /// <summary>
        /// Gets or sets a value for the style to use for the eyedropper.
        /// </summary>
        public Style EyedropperStyle
        {
            get => (Style)GetValue(EyedropperStyleProperty);
            set => SetValue(EyedropperStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the working target element of the eyedropper.
        /// </summary>
        public FrameworkElement TargetElement
        {
            get => (FrameworkElement)GetValue(TargetElementProperty);
            set => SetValue(TargetElementProperty, value);
        }

        private static void OnEyedropperEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EyedropperToolButton eyedropperToolButton)
            {
                if (eyedropperToolButton.EyedropperEnabled)
                {
                    VisualStateManager.GoToState(eyedropperToolButton, eyedropperToolButton.IsPointerOver ? EyedropperEnabledPointerOverState : EyedropperEnabledState, true);
                    if (eyedropperToolButton.XamlRoot != null)
                    {
                        eyedropperToolButton._eyedropper.XamlRoot = eyedropperToolButton.XamlRoot;
                    }

                    eyedropperToolButton._eyedropper.Open().ConfigureAwait(false);
                }
                else
                {
                    VisualStateManager.GoToState(eyedropperToolButton, eyedropperToolButton.IsPointerOver ? PointerOverState : NormalState, true);
                    eyedropperToolButton._eyedropper.Close();
                }
            }
        }

        private static void OnEyedropperStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EyedropperToolButton eyedropperToolButton)
            {
                eyedropperToolButton._eyedropper.Style = eyedropperToolButton.EyedropperStyle;
            }
        }

        private static void OnTargetElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EyedropperToolButton eyedropperToolButton)
            {
                eyedropperToolButton.UnhookTargetElementEvents(e.OldValue as FrameworkElement);
                eyedropperToolButton.HookUpTargetElementEvents(e.NewValue as FrameworkElement);
            }
        }

        private void UnhookTargetElementEvents(FrameworkElement target)
        {
            if (target != null)
            {
                target.SizeChanged -= Target_SizeChanged;
                target.PointerEntered -= Target_PointerEntered;
            }
        }

        private void HookUpTargetElementEvents(FrameworkElement target)
        {
            if (target != null)
            {
                target.SizeChanged += Target_SizeChanged;
                target.PointerEntered += Target_PointerEntered;
            }
        }

        private async void Target_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            await UpdateEyedropperWorkAreaAsync();
        }

        private async void Target_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            await UpdateEyedropperWorkAreaAsync();
        }
    }
}