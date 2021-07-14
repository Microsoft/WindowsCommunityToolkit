// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// The RichSuggestBox control extends <see cref="RichEditBox"/> control that suggests and embeds custom data in a rich document.
    /// </summary>
    public partial class RichSuggestBox
    {
        private static bool IsElementOnScreen(UIElement element, double offsetX = 0, double offsetY = 0)
        {
            var toWindow = element.TransformToVisual(Window.Current.Content);
            var windowBounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var elementBounds = new Rect(offsetX, offsetY, element.ActualSize.X, element.ActualSize.Y);
            elementBounds = toWindow.TransformBounds(elementBounds);
            elementBounds.X += windowBounds.X;
            elementBounds.Y += windowBounds.Y;
            var displayInfo = DisplayInformation.GetForCurrentView();
            var scaleFactor = displayInfo.RawPixelsPerViewPixel;
            var displayHeight = displayInfo.ScreenHeightInRawPixels;
            return elementBounds.Top * scaleFactor >= 0 && elementBounds.Bottom * scaleFactor <= displayHeight;
        }

        private static bool IsElementInsideWindow(UIElement element, double offsetX = 0, double offsetY = 0)
        {
            var toWindow = element.TransformToVisual(Window.Current.Content);
            var windowBounds = ApplicationView.GetForCurrentView().VisibleBounds;
            windowBounds = new Rect(0, 0, windowBounds.Width, windowBounds.Height);
            var elementBounds = new Rect(offsetX, offsetY, element.ActualSize.X, element.ActualSize.Y);
            elementBounds = toWindow.TransformBounds(elementBounds);
            elementBounds.Intersect(windowBounds);
            return elementBounds.Height >= element.ActualSize.Y;
        }

        private static string EnforcePrefixesRequirements(string value)
        {
            return string.IsNullOrEmpty(value) ? string.Empty : string.Concat(value.Where(char.IsPunctuation));
        }

        private static void ApplyTokenFormat(ITextCharacterFormat target, RichSuggestTokenFormat format)
        {
            target.BackgroundColor = format.Background;
            target.ForegroundColor = format.Foreground;
            target.Bold = format.Bold;
            target.Italic = format.Italic;
            target.FontStretch = format.FontStretch;
            target.FontStyle = format.FontStyle;
            target.Name = format.FontName;
            target.Kerning = format.Kerning;
            target.Strikethrough = format.Strikethrough;
            target.Size = format.FontSize;
            target.Outline = format.Outline;
            target.Weight = format.Weight;
            target.Spacing = format.Spacing;
            target.Subscript = format.Subscript;
            target.Superscript = format.Superscript;
            target.Position = format.Position;
        }

        private static void ForEachLinkInDocument(ITextDocument document, Action<ITextRange> action)
        {
            var range = document.GetRange(0, 0);
            range.SetIndex(TextRangeUnit.Character, -1, false);

            // Handle link at the very end of the document where GetIndex fails to detect
            range.Expand(TextRangeUnit.Link);
            if (!string.IsNullOrEmpty(range.Link))
            {
                action?.Invoke(range);
            }

            var nextIndex = range.GetIndex(TextRangeUnit.Link);
            while (nextIndex != 0 && nextIndex != 1)
            {
                range.Move(TextRangeUnit.Link, -1);

                var linkRange = range.GetClone();
                linkRange.Expand(TextRangeUnit.Link);

                // Adjacent links have the same index. Manually check each link with Collapse and Expand.
                var previousStart = linkRange.StartPosition;
                var hasAdjacentToken = true;
                while (hasAdjacentToken)
                {
                    action?.Invoke(linkRange);

                    linkRange.Collapse(false);
                    linkRange.Expand(TextRangeUnit.Link);
                    hasAdjacentToken = !string.IsNullOrEmpty(linkRange.Link) && linkRange.StartPosition != previousStart;
                    previousStart = linkRange.StartPosition;
                }

                nextIndex = range.GetIndex(TextRangeUnit.Link);
            }
        }

        private static void CancelIfNotDisposed(CancellationTokenSource cancellationSource)
        {
            if (cancellationSource == null || cancellationSource.IsCancellationRequested)
            {
                return;
            }

            try
            {
                cancellationSource.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // pass
            }
        }
    }
}
