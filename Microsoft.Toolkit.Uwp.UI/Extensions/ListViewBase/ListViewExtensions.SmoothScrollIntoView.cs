// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Smooth scroll the list to bring specified item into view
    /// </summary>
    public static partial class ListViewExtensions
    {
        /// <summary>
        /// Smooth scrolling the list to bring the specified index into view
        /// </summary>
        /// <param name="listViewBase">List to scroll</param>
        /// <param name="index">The index to bring into view. Index can be negative.</param>
        /// <param name="itemPlacement">Set the item placement after scrolling</param>
        /// <param name="disableAnimation">Set true to disable animation</param>
        /// <param name="scrollIfVisible">Set false to disable scrolling when the corresponding item is in view</param>
        /// <param name="additionalHorizontalOffset">Adds additional horizontal offset</param>
        /// <param name="additionalVerticalOffset">Adds additional vertical offset</param>
        /// <returns>Note: Even though this return <see cref="Task"/>, it will not wait until the scrolling completes</returns>
        public static async Task SmoothScrollIntoViewWithIndexAsync(this ListViewBase listViewBase, int index, ScrollItemPlacement itemPlacement = ScrollItemPlacement.Default, bool disableAnimation = false, bool scrollIfVisible = true, int additionalHorizontalOffset = 0, int additionalVerticalOffset = 0)
        {
            if (index > (listViewBase.Items.Count - 1))
            {
                index = listViewBase.Items.Count - 1;
            }

            if (index < -listViewBase.Items.Count)
            {
                index = -listViewBase.Items.Count;
            }

            index = (index < 0) ? (index + listViewBase.Items.Count) : index;

            bool isVirtualizing = default;
            double previousXOffset = default, previousYOffset = default;

            var scrollViewer = listViewBase.FindDescendant<ScrollViewer>();
            var selectorItem = listViewBase.ContainerFromIndex(index) as SelectorItem;

            // If selectorItem is null then the panel is virtualized.
            // So in order to get the container of the item we need to scroll to that item first and then use ContainerFromIndex
            if (selectorItem == null)
            {
                isVirtualizing = true;

                previousXOffset = scrollViewer.HorizontalOffset;
                previousYOffset = scrollViewer.VerticalOffset;

                var tcs = new TaskCompletionSource<object>();

                void ViewChanged(object obj, ScrollViewerViewChangedEventArgs args) => tcs.TrySetResult(result: null);

                try
                {
                    scrollViewer.ViewChanged += ViewChanged;
                    listViewBase.ScrollIntoView(listViewBase.Items[index], ScrollIntoViewAlignment.Leading);
                    await tcs.Task;
                }
                finally
                {
                    scrollViewer.ViewChanged -= ViewChanged;
                }

                selectorItem = (SelectorItem)listViewBase.ContainerFromIndex(index);
            }

            var transform = selectorItem.TransformToVisual((UIElement)scrollViewer.Content);
            var position = transform.TransformPoint(new Point(0, 0));

            // Scrolling back to previous position
            if (isVirtualizing)
            {
                var tcs = new TaskCompletionSource<object>();

                void ViewChanged(object obj, ScrollViewerViewChangedEventArgs args) => tcs.TrySetResult(result: null);

                try
                {
                    scrollViewer.ViewChanged += ViewChanged;
                    scrollViewer.ChangeView(previousXOffset, previousYOffset, zoomFactor: null, disableAnimation: true);
                    await tcs.Task;
                }
                finally
                {
                    scrollViewer.ViewChanged -= ViewChanged;
                }
            }

            var listViewBaseWidth = listViewBase.ActualWidth;
            var selectorItemWidth = selectorItem.ActualWidth;
            var listViewBaseHeight = listViewBase.ActualHeight;
            var selectorItemHeight = selectorItem.ActualHeight;

            previousXOffset = scrollViewer.HorizontalOffset;
            previousYOffset = scrollViewer.VerticalOffset;

            var minXPosition = position.X - listViewBaseWidth + selectorItemWidth;
            var minYPosition = position.Y - listViewBaseHeight + selectorItemHeight;

            var maxXPosition = position.X;
            var maxYPosition = position.Y;

            double finalXPosition, finalYPosition;

            // If the Item is in view and scrollIfVisible is false then we don't need to scroll
            if (!scrollIfVisible && (previousXOffset <= maxXPosition && previousXOffset >= minXPosition) && (previousYOffset <= maxYPosition && previousYOffset >= minYPosition))
            {
                finalXPosition = previousXOffset;
                finalYPosition = previousYOffset;
            }
            else
            {
                switch (itemPlacement)
                {
                    case ScrollItemPlacement.Default:
                        if (previousXOffset <= maxXPosition && previousXOffset >= minXPosition)
                        {
                            finalXPosition = previousXOffset + additionalHorizontalOffset;
                        }
                        else if (Math.Abs(previousXOffset - minXPosition) < Math.Abs(previousXOffset - maxXPosition))
                        {
                            finalXPosition = minXPosition + additionalHorizontalOffset;
                        }
                        else
                        {
                            finalXPosition = maxXPosition + additionalHorizontalOffset;
                        }

                        if (previousYOffset <= maxYPosition && previousYOffset >= minYPosition)
                        {
                            finalYPosition = previousYOffset + additionalVerticalOffset;
                        }
                        else if (Math.Abs(previousYOffset - minYPosition) < Math.Abs(previousYOffset - maxYPosition))
                        {
                            finalYPosition = minYPosition + additionalVerticalOffset;
                        }
                        else
                        {
                            finalYPosition = maxYPosition + additionalVerticalOffset;
                        }

                        break;

                    case ScrollItemPlacement.Left:
                        finalXPosition = maxXPosition + additionalHorizontalOffset;
                        finalYPosition = previousYOffset + additionalVerticalOffset;
                        break;

                    case ScrollItemPlacement.Top:
                        finalXPosition = previousXOffset + additionalHorizontalOffset;
                        finalYPosition = maxYPosition + additionalVerticalOffset;
                        break;

                    case ScrollItemPlacement.Center:
                        var centreX = (listViewBaseWidth - selectorItemWidth) / 2.0;
                        var centreY = (listViewBaseHeight - selectorItemHeight) / 2.0;
                        finalXPosition = maxXPosition - centreX + additionalHorizontalOffset;
                        finalYPosition = maxYPosition - centreY + additionalVerticalOffset;
                        break;

                    case ScrollItemPlacement.Right:
                        finalXPosition = minXPosition + additionalHorizontalOffset;
                        finalYPosition = previousYOffset + additionalVerticalOffset;
                        break;

                    case ScrollItemPlacement.Bottom:
                        finalXPosition = previousXOffset + additionalHorizontalOffset;
                        finalYPosition = minYPosition + additionalVerticalOffset;
                        break;

                    default:
                        finalXPosition = previousXOffset + additionalHorizontalOffset;
                        finalYPosition = previousYOffset + additionalVerticalOffset;
                        break;
                }
            }

            scrollViewer.ChangeView(finalXPosition, finalYPosition, zoomFactor: null, disableAnimation);
        }

        /// <summary>
        /// Smooth scrolling the list to bring the specified data item into view
        /// </summary>
        /// <param name="listViewBase">List to scroll</param>
        /// <param name="item">The data item to bring into view</param>
        /// <param name="itemPlacement">Set the item placement after scrolling</param>
        /// <param name="disableAnimation">Set true to disable animation</param>
        /// <param name="scrollIfVisibile">Set true to disable scrolling when the corresponding item is in view</param>
        /// <param name="additionalHorizontalOffset">Adds additional horizontal offset</param>
        /// <param name="additionalVerticalOffset">Adds additional vertical offset</param>
        /// <returns>Note: Even though this return <see cref="Task"/>, it will not wait until the scrolling completes</returns>
        public static async Task SmoothScrollIntoViewWithItemAsync(this ListViewBase listViewBase, object item, ScrollItemPlacement itemPlacement = ScrollItemPlacement.Default, bool disableAnimation = false, bool scrollIfVisibile = true, int additionalHorizontalOffset = 0, int additionalVerticalOffset = 0)
        {
            await SmoothScrollIntoViewWithIndexAsync(listViewBase, listViewBase.Items.IndexOf(item), itemPlacement, disableAnimation, scrollIfVisibile, additionalHorizontalOffset, additionalVerticalOffset);
        }
    }
}
