// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    public sealed partial class ImageExLazyLoadingControl
    {
        public ImageExLazyLoadingControl()
        {
            InitializeComponent();
        }

        public event EventHandler CloseButtonClick;

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseButtonClick?.Invoke(this, EventArgs.Empty);
        }

        private async void ImageEx_ImageExOpened(object sender, ImageExOpenedEventArgs e)
        {
            await new MessageDialog("Image Opened").ShowAsync();
        }
    }
}