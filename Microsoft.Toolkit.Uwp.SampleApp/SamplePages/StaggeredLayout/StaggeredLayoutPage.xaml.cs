// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.UI.Xaml.Controls;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StaggeredLayoutPage : Page, IXamlRenderListener
    {
        private ObservableCollection<Item> _items = new ObservableCollection<Item>();
        private Random _random;

        public StaggeredLayoutPage()
        {
            this.InitializeComponent();

            _random = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < _random.Next(1000, 5000); i++)
            {
                var item = new Item { Index = i, Width = _random.Next(50, 250), Height = _random.Next(50, 250), Color = Color.FromArgb(255, (byte)_random.Next(0, 255), (byte)_random.Next(0, 255), (byte)_random.Next(0, 255)) };
                _items.Add(item);
            }
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            var repeater = control.FindDescendant("StaggeredRepeater") as ItemsRepeater;

            if (repeater != null)
            {
                repeater.ItemsSource = _items;
            }
        }

        private class Item
        {
            public int Index { get; internal set; }

            public int Width { get; internal set; }

            public int Height { get; internal set; }

            public Color Color { get; internal set; }
        }
    }
}
