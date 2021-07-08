// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;
using CommunityToolkit.WinUI.SampleApp.Common;
using CommunityToolkit.WinUI.SampleApp.Data;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    public sealed partial class ListViewExtensionsPage : Page, IXamlRenderListener
    {
        public ListViewExtensionsPage()
        {
            this.InitializeComponent();
        }

        public ICommand SampleCommand => new DelegateCommand<PhotoDataItem>(OnExecuteSampleCommand);

        public async void OnXamlRendered(FrameworkElement control)
        {
            var sampleListView = control.FindChild("SampleListView") as ListView;

            if (sampleListView != null)
            {
                sampleListView.ItemsSource = await new Data.PhotosDataSource().GetItemsAsync();
            }

            // Transfer Data Context so we can access SampleCommand
            control.DataContext = this;
        }

        private async void OnExecuteSampleCommand(PhotoDataItem item)
        {
            await new ContentDialog
            {
                Title = "Item Clicked",
                Content = $"You clicked {item.Title} via the 'ListViewExtensions.Command' binding",
                CloseButtonText = "Close",
                XamlRoot = XamlRoot
            }.ShowAsync();
        }
    }
}