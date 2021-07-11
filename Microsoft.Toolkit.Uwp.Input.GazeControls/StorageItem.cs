// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace Microsoft.Toolkit.Uwp.Input.GazeControls
{
    internal class StorageItem : DependencyObject
    {
        public IStorageItem Item { get; }

        public StorageItem(IStorageItem item)
        {
            Item = item;
        }

        public bool IsFolder
        {
            get
            {
                return Item.IsOfType(StorageItemTypes.Folder);
            }
        }

        public string TextIcon
        {
            get
            {
                return IsFolder ? "\uF12B" : "\uE7C3";
            }
        }

        public string Name
        {
            get
            {
                return Item.Name;
            }
        }

        public string Path
        {
            get
            {
                return Item.Path;
            }
        }

        public BitmapImage Thumbnail
        {
            get
            {
                if (_thumbnail == null)
                {
                    if (_storageThumbnail == null)
                    {
                        return null;
                    }

                    _thumbnail = new BitmapImage();
                    _thumbnail.SetSource(_storageThumbnail);
                }

                return _thumbnail;
            }
        }

        private BitmapImage _thumbnail;

        private StorageItemThumbnail _storageThumbnail;

        public async Task GetThumbnailAsync()
        {
            var props = Item as IStorageItemProperties;
            _storageThumbnail = await props.GetThumbnailAsync(ThumbnailMode.DocumentsView);
        }

        internal static readonly DependencyProperty StorageThumbnailProperty =
            DependencyProperty.Register(
                "StorageThumbnail",
                typeof(StorageItemThumbnail),
                typeof(StorageItem),
                null);

        internal StorageItemThumbnail StorageThumbnail
        {
            get { return (StorageItemThumbnail)GetValue(StorageThumbnailProperty); }
            set { SetValue(StorageThumbnailProperty, value); }
        }

    }
}
