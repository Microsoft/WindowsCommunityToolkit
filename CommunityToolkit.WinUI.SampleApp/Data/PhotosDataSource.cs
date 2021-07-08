// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;
using Windows.ApplicationModel;

namespace CommunityToolkit.WinUI.SampleApp.Data
{
    [Bindable]
    public class PhotosDataSource
    {
        private static ObservableCollection<PhotoDataItem> _photos;
        private static ObservableCollection<IEnumerable<PhotoDataItem>> _groupedPhotos;
        private static bool _isOnlineCached;

        public async Task<ObservableCollection<PhotoDataItem>> GetItemsAsync(bool online = false, int maxCount = -1)
        {
            CheckCacheState(online);

            if (_photos == null)
            {
                await LoadAsync(online, maxCount);
            }

            return _photos;
        }

        public async Task<ObservableCollection<IEnumerable<PhotoDataItem>>> GetGroupedItemsAsync(bool online = false, int maxCount = -1)
        {
            CheckCacheState(online);

            if (_groupedPhotos == null)
            {
                await LoadAsync(online, maxCount);
            }

            return _groupedPhotos;
        }

        private static async Task LoadAsync(bool online, int maxCount)
        {
            _isOnlineCached = online;
            _photos = new ObservableCollection<PhotoDataItem>();
            _groupedPhotos = new ObservableCollection<IEnumerable<PhotoDataItem>>();

            foreach (var item in await GetPhotosAsync(online))
            {
                _photos.Add(item);

                if (maxCount != -1)
                {
                    maxCount--;

                    if (maxCount == 0)
                    {
                        break;
                    }
                }
            }

            foreach (var group in _photos.GroupBy(x => x.Category))
            {
                _groupedPhotos.Add(group);
            }
        }

        private static async Task<IEnumerable<PhotoDataItem>> GetPhotosAsync(bool online)
        {
            var prefix = online ? "Online" : string.Empty;
            return Parse(await File.ReadAllTextAsync(Path.Combine(Package.Current.InstalledLocation.Path, $"Assets/Photos/{prefix}Photos.json")));
        }

        private static IEnumerable<PhotoDataItem> Parse(string jsonData)
        {
            return JsonSerializer.Deserialize<IList<PhotoDataItem>>(jsonData);
        }

        private static void CheckCacheState(bool online)
        {
            if (_isOnlineCached != online)
            {
                _photos = null;
                _groupedPhotos = null;
            }
        }
    }
}