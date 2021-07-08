// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
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
            await new ContentDialog
            {
                Title = "Windows Community Toolkit Sample App",
                Content = "Image Opened",
                CloseButtonText = "Close",
                XamlRoot = XamlRoot
            }.ShowAsync();
        }
    }
}