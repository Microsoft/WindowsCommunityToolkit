// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.Helpers;
using CommunityToolkit.WinUI.UI.Controls.ColorPickerConverters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Controls.Primitives
{
    /// <summary>
    /// A slider that represents a single color channel for use in the <see cref="ColorPicker"/>.
    /// </summary>
    public partial class ColorPickerSlider : Slider
    {
        // TODO Combine this with the ColorPicker field or make a property
        internal Color CheckerBackgroundColor { get; set; } = Color.FromArgb(0x19, 0x80, 0x80, 0x80); // Overridden later

        private Size oldSize = Size.Empty;
        private Size measuredSize = Size.Empty;
        private Size cachedSize = Size.Empty;

        /***************************************************************************************
         *
         * Constructor/Destructor
         *
         ***************************************************************************************/

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPickerSlider"/> class.
        /// </summary>
        public ColorPickerSlider()
            : base()
        {
            this.DefaultStyleKey = typeof(ColorPickerSlider);
        }

        /***************************************************************************************
         *
         * Methods
         *
         ***************************************************************************************/

        /// <summary>
        /// Update the slider's Foreground and Background brushes based on the current slider state and color.
        /// </summary>
        /// <remarks>
        /// Manually refreshes the background gradient of the slider.
        /// This is callable separately for performance reasons.
        /// </remarks>
        public void UpdateColors()
        {
            HsvColor hsvColor = this.HsvColor;

            // Calculate and set the background
            this.UpdateBackground(hsvColor);

            // Calculate and set the foreground ensuring contrast with the background
            Color rgbColor = ColorHelper.FromHsv(hsvColor.H, hsvColor.S, hsvColor.V, hsvColor.A);
            Color selectedRgbColor;
            double sliderPercent = this.Value / (this.Maximum - this.Minimum);

            if (this.ColorRepresentation == ColorRepresentation.Hsva)
            {
                if (this.IsAlphaMaxForced &&
                    this.ColorChannel != ColorChannel.Alpha)
                {
                    hsvColor = new HsvColor()
                    {
                        H = hsvColor.H,
                        S = hsvColor.S,
                        V = hsvColor.V,
                        A = 1.0
                    };
                }

                switch (this.ColorChannel)
                {
                    case ColorChannel.Channel1:
                    {
                        var channelValue = Math.Clamp(sliderPercent * 360.0, 0.0, 360.0);

                        hsvColor = new HsvColor()
                        {
                            H = channelValue,
                            S = this.IsSaturationValueMaxForced ? 1.0 : hsvColor.S,
                            V = this.IsSaturationValueMaxForced ? 1.0 : hsvColor.V,
                            A = hsvColor.A
                        };
                        break;
                    }

                    case ColorChannel.Channel2:
                    {
                        var channelValue = Math.Clamp(sliderPercent * 1.0, 0.0, 1.0);

                        hsvColor = new HsvColor()
                        {
                            H = hsvColor.H,
                            S = channelValue,
                            V = this.IsSaturationValueMaxForced ? 1.0 : hsvColor.V,
                            A = hsvColor.A
                        };
                        break;
                    }

                    case ColorChannel.Channel3:
                    {
                        var channelValue = Math.Clamp(sliderPercent * 1.0, 0.0, 1.0);

                        hsvColor = new HsvColor()
                        {
                            H = hsvColor.H,
                            S = this.IsSaturationValueMaxForced ? 1.0 : hsvColor.S,
                            V = channelValue,
                            A = hsvColor.A
                        };
                        break;
                    }
                }

                selectedRgbColor = ColorHelper.FromHsv(
                    hsvColor.H,
                    hsvColor.S,
                    hsvColor.V,
                    hsvColor.A);
            }
            else
            {
                if (this.IsAlphaMaxForced &&
                    this.ColorChannel != ColorChannel.Alpha)
                {
                    rgbColor = new Color()
                    {
                        R = rgbColor.R,
                        G = rgbColor.G,
                        B = rgbColor.B,
                        A = 255
                    };
                }

                byte channelValue = Convert.ToByte(Math.Clamp(sliderPercent * 255.0, 0.0, 255.0));

                switch (this.ColorChannel)
                {
                    case ColorChannel.Channel1:
                        rgbColor = new Color()
                        {
                            R = channelValue,
                            G = rgbColor.G,
                            B = rgbColor.B,
                            A = rgbColor.A
                        };
                        break;
                    case ColorChannel.Channel2:
                        rgbColor = new Color()
                        {
                            R = rgbColor.R,
                            G = channelValue,
                            B = rgbColor.B,
                            A = rgbColor.A
                        };
                        break;
                    case ColorChannel.Channel3:
                        rgbColor = new Color()
                        {
                            R = rgbColor.R,
                            G = rgbColor.G,
                            B = channelValue,
                            A = rgbColor.A
                        };
                        break;
                }

                selectedRgbColor = rgbColor;
            }

            var converter = new ContrastBrushConverter();
            this.Foreground = converter.Convert(selectedRgbColor, typeof(Brush), this.DefaultForeground, null) as Brush;

            return;
        }

        /// <summary>
        /// Generates a new background image for the color channel slider and applies it.
        /// </summary>
        private async void UpdateBackground(HsvColor color)
        {
            /* Updates may be requested when sliders are not in the visual tree.
             * For first-time load this is handled by the Loaded event.
             * However, after that problems may arise, consider the following case:
             *
             *   (1) Backgrounds are drawn normally the first time on Loaded.
             *       Actual height/width are available.
             *   (2) The palette tab is selected which has no sliders
             *   (3) The picker flyout is closed
             *   (4) Externally the color is changed
             *       The color change will trigger slider background updates but
             *       with the flyout closed, actual height/width are zero.
             *       No zero size bitmap can be generated.
             *   (5) The picker flyout is re-opened by the user and the default
             *       last-opened tab will be viewed: palette.
             *       No loaded events will be fired for sliders. The color change
             *       event was already handled in (4). The sliders will never
             *       be updated.
             *
             * In this case the sliders become out of sync with the Color because there is no way
             * to tell when they actually come into view. To work around this, force a re-render of
             * the background with the last size of the slider. This last size will be when it was
             * last loaded or updated.
             *
             * In the future additional consideration may be required for SizeChanged of the control.
             * This work-around will also cause issues if display scaling changes in the special
             * case where cached sizes are required.
             */
            var width = Convert.ToInt32(this.ActualWidth);
            var height = Convert.ToInt32(this.ActualHeight);

            if (width == 0 || height == 0)
            {
                // Attempt to use the last size if it was available
                if (this.cachedSize.IsEmpty == false)
                {
                    width = Convert.ToInt32(this.cachedSize.Width);
                    height = Convert.ToInt32(this.cachedSize.Height);
                }
            }
            else
            {
                this.cachedSize = new Size(width, height);
            }

            var bitmap = await ColorPickerRenderingHelpers.CreateChannelBitmapAsync(
                width,
                height,
                this.Orientation,
                this.ColorRepresentation,
                this.ColorChannel,
                color,
                this.CheckerBackgroundColor,
                this.IsAlphaMaxForced,
                this.IsSaturationValueMaxForced);

            if (bitmap != null)
            {
                this.Background = await ColorPickerRenderingHelpers.BitmapToBrushAsync(bitmap, width, height);
            }

            return;
        }

        /// <summary>
        /// Measures the size in layout required for child elements and determines a size for the
        /// FrameworkElement-derived class.
        /// </summary>
        /// <remarks>
        ///
        /// Slider has some critical bugs:
        ///
        ///  * https://github.com/microsoft/microsoft-ui-xaml/issues/477
        ///  * https://social.msdn.microsoft.com/Forums/sqlserver/en-US/0d3a2e64-d192-4250-b583-508a02bd75e1/uwp-bug-crash-layoutcycleexception-because-of-slider-under-certain-circumstances?forum=wpdevelop
        ///
        /// </remarks>
        /// <param name="availableSize">The available size that this element can give to child elements.
        /// Infinity can be specified as a value to indicate that the element will size to whatever content
        /// is available.</param>
        /// <returns>The size that this element determines it needs during layout,
        /// based on its calculations of child element sizes.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (!Size.Equals(oldSize, availableSize))
            {
                measuredSize = base.MeasureOverride(availableSize);
                oldSize = availableSize;
            }

            return measuredSize;
        }

        private void OnDependencyPropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            if (object.ReferenceEquals(args.Property, ColorProperty))
            {
                // Sync with HSV (which is primary)
                this.HsvColor = this.Color.ToHsv();
            }

            if (this.IsAutoUpdatingEnabled)
            {
                this.UpdateColors();
            }

            return;
        }
    }
}