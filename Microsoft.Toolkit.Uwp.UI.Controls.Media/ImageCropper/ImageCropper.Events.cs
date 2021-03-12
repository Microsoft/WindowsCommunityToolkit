// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// The <see cref="ImageCropper"/> control allows user to crop image freely.
    /// </summary>
    public partial class ImageCropper
    {
        private void ImageCropperThumb_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            var changed = false;
            var diffPos = default(Point);
            if (e.Key == VirtualKey.Left)
            {
                diffPos.X--;
                var upKeyState = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Up);
                var downKeyState = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Down);
                if (upKeyState == CoreVirtualKeyStates.Down)
                {
                    diffPos.Y--;
                }

                if (downKeyState == CoreVirtualKeyStates.Down)
                {
                    diffPos.Y++;
                }

                changed = true;
            }
            else if (e.Key == VirtualKey.Right)
            {
                diffPos.X++;
                var upKeyState = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Up);
                var downKeyState = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Down);
                if (upKeyState == CoreVirtualKeyStates.Down)
                {
                    diffPos.Y--;
                }

                if (downKeyState == CoreVirtualKeyStates.Down)
                {
                    diffPos.Y++;
                }

                changed = true;
            }
            else if (e.Key == VirtualKey.Up)
            {
                diffPos.Y--;
                var leftKeyState = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Left);
                var rightKeyState = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Right);
                if (leftKeyState == CoreVirtualKeyStates.Down)
                {
                    diffPos.X--;
                }

                if (rightKeyState == CoreVirtualKeyStates.Down)
                {
                    diffPos.X++;
                }

                changed = true;
            }
            else if (e.Key == VirtualKey.Down)
            {
                diffPos.Y++;
                var leftKeyState = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Left);
                var rightKeyState = Window.Current.CoreWindow.GetAsyncKeyState(VirtualKey.Right);
                if (leftKeyState == CoreVirtualKeyStates.Down)
                {
                    diffPos.X--;
                }

                if (rightKeyState == CoreVirtualKeyStates.Down)
                {
                    diffPos.X++;
                }

                changed = true;
            }

            if (changed)
            {
                var imageCropperThumb = (ImageCropperThumb)sender;
                UpdateCroppedRect(imageCropperThumb.Position, diffPos);
            }
        }

        private void ImageCropperThumb_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            var selectedRect = new Point(_startX, _startY).ToRect(new Point(_endX, _endY));
            var croppedRect = _inverseImageTransform.TransformBounds(selectedRect);
            if (croppedRect.Width > MinCropSize.Width && croppedRect.Height > MinCropSize.Height)
            {
                croppedRect.Intersect(_restrictedCropRect);
                _currentCroppedRect = croppedRect;
            }

            UpdateImageLayout(true);
        }

        private void ImageCropperThumb_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var selectedRect = new Point(_startX, _startY).ToRect(new Point(_endX, _endY));
            var croppedRect = _inverseImageTransform.TransformBounds(selectedRect);
            if (croppedRect.Width > MinCropSize.Width && croppedRect.Height > MinCropSize.Height)
            {
                croppedRect.Intersect(_restrictedCropRect);
                _currentCroppedRect = croppedRect;
            }

            UpdateImageLayout(true);
        }

        private void ImageCropperThumb_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var imageCropperThumb = (ImageCropperThumb)sender;
            var currentPointerPosition = new Point(
                imageCropperThumb.X + e.Position.X + e.Delta.Translation.X - (imageCropperThumb.ActualWidth / 2),
                imageCropperThumb.Y + e.Position.Y + e.Delta.Translation.Y - (imageCropperThumb.ActualHeight / 2));
            var safePosition = GetSafePoint(_restrictedSelectRect, currentPointerPosition);
            var safeDiffPoint = new Point(safePosition.X - imageCropperThumb.X, safePosition.Y - imageCropperThumb.Y);
            UpdateCroppedRect(imageCropperThumb.Position, safeDiffPoint);
        }

        private void SourceImage_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var offsetX = -e.Delta.Translation.X;
            var offsetY = -e.Delta.Translation.Y;
            if (offsetX > 0)
            {
                offsetX = Math.Min(offsetX, _restrictedSelectRect.X + _restrictedSelectRect.Width - _endX);
            }
            else
            {
                offsetX = Math.Max(offsetX, _restrictedSelectRect.X - _startX);
            }

            if (offsetY > 0)
            {
                offsetY = Math.Min(offsetY, _restrictedSelectRect.Y + _restrictedSelectRect.Height - _endY);
            }
            else
            {
                offsetY = Math.Max(offsetY, _restrictedSelectRect.Y - _startY);
            }

            var selectedRect = new Point(_startX, _startY).ToRect(new Point(_endX, _endY));
            selectedRect.X += offsetX;
            selectedRect.Y += offsetY;
            var croppedRect = _inverseImageTransform.TransformBounds(selectedRect);
            croppedRect.Intersect(_restrictedCropRect);
            _currentCroppedRect = croppedRect;
            UpdateImageLayout();
        }

        private void ImageCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Source == null)
            {
                return;
            }

            UpdateImageLayout();
            UpdateMaskArea();
        }
    }
}