// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    public sealed partial class LoadingPage : IXamlRenderListener
    {
        private Loading loadingControl;
        private ContentControl loadingContentControl;
        private ResourceDictionary resources;

        public LoadingPage()
        {
            InitializeComponent();
            Load();
        }

        public async void OnXamlRendered(FrameworkElement control)
        {
            loadingControl = control.FindDescendant("LoadingControl") as Loading;
            loadingContentControl = control.FindChild("LoadingContentControl") as ContentControl;
            resources = control.Resources;

            if (control.FindChild("AdaptiveGridViewControl") is AdaptiveGridView gridView)
            {
                gridView.ItemsSource = await new Data.PhotosDataSource().GetItemsAsync();
            }
        }

        private void Load()
        {
            SampleController.Current.RegisterNewCommand("Loading control with wait ring", async (sender, args) =>
            {
                if (loadingContentControl != null)
                {
                    loadingContentControl.ContentTemplate = resources["WaitListTemplate"] as DataTemplate;
                    await ShowLoadingDialogAsync();
                }
            });

            SampleController.Current.RegisterNewCommand("Loading control with progressbar", async (sender, args) =>
            {
                if (loadingContentControl != null)
                {
                    loadingContentControl.ContentTemplate = resources["ProgressBarTemplate"] as DataTemplate;
                    await ShowLoadingDialogAsync();
                }
            });

            SampleController.Current.RegisterNewCommand("Loading control with logo", async (sender, args) =>
            {
                if (loadingContentControl != null)
                {
                    loadingContentControl.ContentTemplate = resources["LogoTemplate"] as DataTemplate;
                    await ShowLoadingDialogAsync();
                }
            });
        }

        private async Task ShowLoadingDialogAsync()
        {
            loadingControl.IsLoading = true;
            await Task.Delay(5000);
            loadingControl.IsLoading = false;
        }
    }
}