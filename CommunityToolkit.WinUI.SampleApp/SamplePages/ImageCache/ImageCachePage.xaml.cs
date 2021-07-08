// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.SampleApp.Data;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    /// <summary>
    /// Page shows how ImageCache is used
    /// </summary>
    public sealed partial class ImageCachePage : IXamlRenderListener
    {
        private ObservableCollection<PhotoDataItem> _photoItems;
        private ListView photoList;

        public ImageCachePage()
        {
            InitializeComponent();
            Load();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            photoList = control.FindChild("PhotoList") as ListView;
        }

        private async void Load()
        {
            SampleController.Current.RegisterNewCommand("PreCache photos", PreCache_Tapped);

            SampleController.Current.RegisterNewCommand("PreCache photos in memory", PreCacheInMemory_Tapped);

            SampleController.Current.RegisterNewCommand("Load photos", LoadImages_Tapped);

            SampleController.Current.RegisterNewCommand("Clear cache", ClearCache_Tapped);

            await LoadDataAsync();
        }

        private async Task PreCacheImages(bool loadInMemory = false)
        {
            BusyIndicator.IsActive = true;

            ImageCache.Instance.MaxMemoryCacheCount = loadInMemory ? 200 : 0;

            DateTime dtStart = DateTime.Now;

            foreach (var item in _photoItems)
            {
                await ImageCache.Instance.PreCacheAsync(new Uri(item.Thumbnail), false, loadInMemory);
            }

            Message.Text = $"Preloading {_photoItems.Count} photo took {DateTime.Now.Subtract(dtStart).TotalSeconds} seconds";

            BusyIndicator.IsActive = false;
        }

        private async Task LoadDataAsync()
        {
            var source = new PhotosDataSource();
            _photoItems = await source.GetItemsAsync(true);
        }

        private async void PreCache_Tapped(object sender, RoutedEventArgs e)
        {
            await PreCacheImages(false);
        }

        private async void PreCacheInMemory_Tapped(object sender, RoutedEventArgs e)
        {
            await PreCacheImages(true);
        }

        private void LoadImages_Tapped(object sender, RoutedEventArgs e)
        {
            if (photoList != null)
            {
                photoList.ItemsSource = _photoItems;
            }
        }

        private async void ClearCache_Tapped(object sender, RoutedEventArgs e)
        {
            BusyIndicator.IsActive = true;

            GC.Collect(); // Force GC to free file locks
            await ImageCache.Instance.ClearAsync();

            if (photoList != null)
            {
                photoList.ItemsSource = null;
            }

            Message.Text = "Cache cleared";

            BusyIndicator.IsActive = false;
        }
    }
}