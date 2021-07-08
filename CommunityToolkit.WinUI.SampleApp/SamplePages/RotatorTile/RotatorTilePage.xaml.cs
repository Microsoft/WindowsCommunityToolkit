// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.SampleApp.Data;
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    public sealed partial class RotatorTilePage : IXamlRenderListener
    {
        private ObservableCollection<PhotoDataItem> _pictures;

        public RotatorTilePage()
        {
            InitializeComponent();
            Load();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            var tile1 = control.FindChild("Tile1") as RotatorTile;

            if (tile1 != null)
            {
                tile1.ItemsSource = _pictures;
            }

            var tile2 = control.FindChild("Tile2") as RotatorTile;

            if (tile2 != null)
            {
                tile2.ItemsSource = _pictures;
            }

            var tile3 = control.FindChild("Tile3") as RotatorTile;

            if (tile3 != null)
            {
                tile3.ItemsSource = _pictures;
            }

            var tile4 = control.FindChild("Tile4") as RotatorTile;

            if (tile4 != null)
            {
                tile4.ItemsSource = _pictures;
            }
        }

        private async void Load()
        {
            _pictures = await new Data.PhotosDataSource().GetItemsAsync(true);
        }
    }
}