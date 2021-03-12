// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    public sealed partial class ItemsReorderAnimationPage : Page, IXamlRenderListener
    {
        private GridView imageView;

        public ItemsReorderAnimationPage()
        {
            InitializeComponent();
        }

        public async void OnXamlRendered(FrameworkElement control)
        {
            imageView = control.FindChild("ImageView") as GridView;

            if (imageView != null)
            {
                imageView.ItemsSource = await new Data.PhotosDataSource().GetItemsAsync();
            }
        }
    }
}
