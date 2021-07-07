// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Events for <see cref="ContentSizer"/>.
    /// </summary>
    public partial class ContentSizer
    {
        // If no values specified, setup our default behaviors.
        private void ContentSizer_Loaded(object sender, RoutedEventArgs e)
        {
            // Adding Grip to Grid Splitter
            if (Content == null)
            {
                // TODO: Make Converter to put in XAML?
                Content =
                    ResizeDirection == ContentResizeDirection.Vertical ? GripperBarVertical : GripperBarHorizontal;
            }

            if (TargetControl == null)
            {
                TargetControl = this.FindAscendant<FrameworkElement>();
            }
        }

        private void ContentSizer_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (ResizeDirection == ContentResizeDirection.Vertical)
            {
                if (e.Key == Windows.System.VirtualKey.Left)
                {
                    HorizontalMove(-GripperKeyboardChange);
                }
                else if (e.Key == Windows.System.VirtualKey.Right)
                {
                    HorizontalMove(GripperKeyboardChange);
                }
            }
            else
            {
                if (e.Key == Windows.System.VirtualKey.Up)
                {
                    VerticalMove(-GripperKeyboardChange);
                }
                else if (e.Key == Windows.System.VirtualKey.Down)
                {
                    VerticalMove(GripperKeyboardChange);
                }
            }
        }

        /// <inheritdoc />
        protected override void OnManipulationDelta(ManipulationDeltaRoutedEventArgs e)
        {
            var horizontalChange = e.Delta.Translation.X;
            var verticalChange = e.Delta.Translation.Y;

            if (ResizeDirection == ContentResizeDirection.Vertical)
            {
                if (HorizontalMove(horizontalChange))
                {
                    return;
                }
            }
            else if (ResizeDirection == ContentResizeDirection.Horizontal)
            {
                if (VerticalMove(verticalChange))
                {
                    return;
                }
            }

            base.OnManipulationDelta(e);
        }

        private bool VerticalMove(double verticalChange)
        {
            if (TargetControl == null)
            {
                return true;
            }

            if (!IsValidHeight(TargetControl, verticalChange))
            {
                return true;
            }

            // TODO: This only works if splitter is on top and making things grow down
            // Do we need our ContentResizeDirection to be 4 way? Maybe 'Auto' would check the horizontal/vertical alignment of the target???
            TargetControl.Height += verticalChange;

            return false;
        }

        private bool HorizontalMove(double horizontalChange)
        {
            if (TargetControl == null)
            {
                return true;
            }

            if (!IsValidWidth(TargetControl, horizontalChange))
            {
                return true;
            }

            // TODO: This only works if splitter is on left and making things grow right...
            TargetControl.Width += horizontalChange;

            return false;
        }

        private bool IsValidHeight(FrameworkElement target, double verticalChange)
        {
            var newHeight = target.ActualHeight + verticalChange;

            var minHeight = target.MinHeight;
            if (newHeight < 0 || (!double.IsNaN(minHeight) && newHeight < minHeight))
            {
                return false;
            }

            var maxHeight = target.MaxHeight;
            if (!double.IsNaN(maxHeight) && newHeight > maxHeight)
            {
                return false;
            }

            if (newHeight <= ActualHeight)
            {
                return false;
            }

            return true;
        }

        private bool IsValidWidth(FrameworkElement target, double horizontalChange)
        {
            var newWidth = target.ActualWidth + horizontalChange;

            var minWidth = target.MinWidth;
            if (newWidth < 0 || (!double.IsNaN(minWidth) && newWidth < minWidth))
            {
                return false;
            }

            var maxWidth = target.MaxWidth;
            if (!double.IsNaN(maxWidth) && newWidth > maxWidth)
            {
                return false;
            }

            if (newWidth <= ActualWidth)
            {
                return false;
            }

            return true;
        }
    }
}
