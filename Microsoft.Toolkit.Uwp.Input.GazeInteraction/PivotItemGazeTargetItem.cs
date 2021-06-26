// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    internal class PivotItemGazeTargetItem : GazeTargetItem
    {
        internal PivotItemGazeTargetItem(UIElement element)
            : base(element)
        {
        }

        internal override void Invoke()
        {
            var headerItem = (PivotHeaderItem)TargetElement;
            var headerPanel = (PivotHeaderPanel)VisualTreeHelper.GetParent(headerItem);
            int index = headerPanel.Children.IndexOf(headerItem);

            DependencyObject walker = headerPanel;
            Pivot pivot;
            do
            {
                walker = VisualTreeHelper.GetParent(walker);
                pivot = walker as Pivot;
            }
            while (pivot == null);

            pivot.SelectedIndex = index;
        }
    }
}