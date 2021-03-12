// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

#nullable enable

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <inheritdoc cref="TextBoxExtensions"/>
    public static partial class TextBoxExtensions
    {
        /// <summary>
        /// The <see cref="SurfaceDialOptions"/> instance containing properties to configure the Surface Dial support for a <see cref="TextBox"/>.
        /// </summary>
        public static readonly DependencyProperty SurfaceDialOptionsProperty = DependencyProperty.RegisterAttached(
            nameof(SurfaceDialOptions),
            typeof(SurfaceDialOptions),
            typeof(TextBoxExtensions),
            new PropertyMetadata(null, OnSurfaceDialOptionsPropertyChanged));

        /// <summary>
        /// Gets the value for <see cref="SurfaceDialOptionsProperty"/>.
        /// </summary>
        /// <param name="textBox">The target <see cref="TextBox"/> control.</param>
        /// <returns>The value of <see cref="SurfaceDialOptionsProperty"/> for <paramref name="textBox"/>.</returns>
        public static SurfaceDialOptions? GetSurfaceDialOptions(TextBox textBox)
        {
            return (SurfaceDialOptions?)textBox.GetValue(SurfaceDialOptionsProperty);
        }

        /// <summary>
        /// Sets the value for <see cref="SurfaceDialOptionsProperty"/>.
        /// </summary>
        /// <param name="textBox">The target <see cref="TextBox"/> control.</param>
        /// <param name="value">The value to set the property to.</param>
        public static void SetSurfaceDialOptions(TextBox textBox, SurfaceDialOptions? value)
        {
            textBox.SetValue(SurfaceDialOptionsProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether this attached proeprty is supported.
        /// </summary>
        public static bool IsSurfaceDialOptionsSupported
        {
            get => RadialController.IsSupported();
        }

        /// <summary>
        /// The Surface Dial controller instance itself.
        /// </summary>
        private static RadialController? _controller;

        /// <summary>
        /// A default menu item that will be used for this to function.
        /// It will automatically be cleaned up when you move away from the <see cref="TextBox"/>, and created on Focus.
        /// </summary>
        private static RadialControllerMenuItem? _stepTextMenuItem;

        /// <summary>
        /// The textbox itself needed to reference the current <see cref="TextBox"/> that is being modified.
        /// </summary>
        private static TextBox? _textBox;

        /// <summary>
        /// Gets or sets the controller for the Surface Dial.
        /// The <see cref="RadialController"/> can be set from your app logic in case you use Surface Dial in other custom cases than on a <see cref="TextBox"/>.
        /// This helper class will do everything for you, but if you want to control the Menu Items and/or wish to use the same Surface Dial instance
        /// This is the property for the static controller so you can access it if needed.
        /// </summary>
        public static RadialController Controller
        {
            get
            {
                return _controller ??= RadialController.CreateForCurrentView();
            }
            set => _controller = value;
        }

        /// <summary>
        /// This function gets called every time there is a rotational change on the connected Surface Dial while a Surface Dial enabled <see cref="TextBox"/> is in focus.
        /// This function ensures that the <see cref="TextBox"/> stays within the set range between MinValue and MaxValue while rotating the Surface Dial.
        /// It defaults the content of the <see cref="TextBox"/> to 0.0 if a non-numerical value is detected.
        /// </summary>
        /// <param name="sender">The <see cref="RadialController"/> being used.</param>
        /// <param name="args">The arguments of the changed event.</param>
        private static void Controller_RotationChanged(RadialController sender, RadialControllerRotationChangedEventArgs args)
        {
            if (_textBox is null)
            {
                return;
            }

            string text = _textBox.Text;
            SurfaceDialOptions? options = GetSurfaceDialOptions(_textBox) ?? SurfaceDialOptions.Default;

            if (double.TryParse(text, out double number))
            {
                // We only care about the sign of RotationDeltaInDegrees to determine if we're going up/down
                // The value is controlled by the StepValue independent of when we should call the rotation changed event.
                number += Math.Sign(args.RotationDeltaInDegrees) * options.StepValue;

                if (options.EnableMinMaxValue)
                {
                    number = Math.Clamp(number, options.MinValue, options.MaxValue);
                }
            }
            else
            {
                number = 0.0d;
            }

            _textBox.Text = number.ToString("0.00");
        }

        /// <summary>
        /// Sets up the events needed for the current <see cref="TextBox"/> so it can trigger on <see cref="UIElement.GotFocus"/> and <see cref="UIElement.LostFocus"/>.
        /// </summary>
        /// <param name="d">The <see cref="DependencyObject"/> we are dealing with, like a <see cref="TextBox"/>.</param>
        /// <param name="e">The arguments of the changed event.</param>
        private static void OnSurfaceDialOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!IsSurfaceDialOptionsSupported)
            {
                return;
            }

            if (d is not TextBox textBox)
            {
                return;
            }

            // Initialize our RadialController once.
            _controller ??= RadialController.CreateForCurrentView();

            textBox.GotFocus -= TextBox_GotFocus_SurfaceDial;
            textBox.LostFocus -= TextBox_LostFocus_SurfaceDial;

            if (e.NewValue is not null)
            {
                textBox.GotFocus += TextBox_GotFocus_SurfaceDial;
                textBox.LostFocus += TextBox_LostFocus_SurfaceDial;
            }
        }

        /// <summary>
        /// When the focus of the <see cref="TextBox"/> is lost, ensure we clean up the events and Surface Dial menu item.
        /// </summary>
        /// <param name="sender">The <see cref="TextBox"/> in being affected.</param>
        /// <param name="e">The event arguments.</param>
        private static void TextBox_LostFocus_SurfaceDial(object sender, RoutedEventArgs e)
        {
            if (_textBox is null ||
                _controller is null)
            {
                return;
            }

            SurfaceDialOptions? options = GetSurfaceDialOptions(_textBox) ?? SurfaceDialOptions.Default;

            if (_stepTextMenuItem is not null)
            {
                _controller.Menu.Items.Remove(_stepTextMenuItem);
            }

            _controller.RotationChanged -= Controller_RotationChanged;

            if (options.EnableTapToNextControl)
            {
                _controller.ButtonClicked -= Controller_ButtonClicked;
            }

            _textBox = null;
        }

        /// <summary>
        /// When a Surface Dial TextBox gets focus, ensure the proper events are setup, and connect the Surface Dial itself.
        /// </summary>
        /// <param name="sender">The TextBox in being affected.</param>
        /// <param name="e">The event arguments.</param>
        private static void TextBox_GotFocus_SurfaceDial(object sender, RoutedEventArgs e)
        {
            _textBox = sender as TextBox;

            if (_textBox is null ||
                _controller is null)
            {
                return;
            }

            if (!IsSurfaceDialOptionsSupported)
            {
                return;
            }

            _controller.RotationChanged -= Controller_RotationChanged;
            _controller.ButtonClicked -= Controller_ButtonClicked;

            SurfaceDialOptions? options = GetSurfaceDialOptions(_textBox) ?? SurfaceDialOptions.Default;

            _stepTextMenuItem ??= RadialControllerMenuItem.CreateFromKnownIcon("Step Text Box", options.Icon);

            _controller.Menu.Items.Add(_stepTextMenuItem);
            _controller.Menu.SelectMenuItem(_stepTextMenuItem);

            _controller.UseAutomaticHapticFeedback = options.EnableHapticFeedback;
            _controller.RotationResolutionInDegrees = options.RotationResolutionInDegrees;
            _controller.RotationChanged += Controller_RotationChanged;

            if (options.EnableTapToNextControl)
            {
                _controller.ButtonClicked += Controller_ButtonClicked;
            }
        }

        /// <summary>
        /// If the TapToNext flag is enabled, this function will try to take the focus to the next focusable element.
        /// </summary>
        /// <param name="sender">The RadialController being used.</param>
        /// <param name="args">The arguments of the changed event.</param>
        private static void Controller_ButtonClicked(RadialController sender, RadialControllerButtonClickedEventArgs args)
        {
            FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
        }
    }
}
