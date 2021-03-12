// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp.SampleApp.Models;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    public sealed partial class ScrollViewerExtensionsPage : IXamlRenderListener
    {
        private ObservableCollection<Item> _items;

        public ScrollViewerExtensionsPage()
        {
            InitializeComponent();

            // Reset items when revisiting sample.
            _items = new ObservableCollection<Item>();

            for (var i = 0; i < 1000; i++)
            {
                _items.Add(new Item() { Title = "Item " + i });
            }
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            var listView = control.FindChild("listView") as ListView;
            if (listView != null)
            {
                listView.ItemsSource = _items;

                var shapesPanel = control.FindChild("shapesPanel") as StackPanel;
                if (shapesPanel != null)
                {
                    var listScrollViewer = listView.FindDescendant<ScrollViewer>();

                    listScrollViewer?.StartExpressionAnimation(shapesPanel, Axis.Y);
                }
            }
        }
    }
}