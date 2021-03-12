// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;
using Microsoft.Toolkit.Uwp.SampleApp.Common;
using Microsoft.Toolkit.Uwp.SampleApp.Data;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
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
            await new MessageDialog($"You clicked {item.Title} via the 'ListViewExtensions.Command' binding", "Item Clicked").ShowAsync();
        }
    }
}
