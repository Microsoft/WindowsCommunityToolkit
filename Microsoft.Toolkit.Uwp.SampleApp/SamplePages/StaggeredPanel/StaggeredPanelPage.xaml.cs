// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StaggeredPanelPage : Page, IXamlRenderListener
    {
        public StaggeredPanelPage()
        {
            this.InitializeComponent();
        }

        public async void OnXamlRendered(FrameworkElement control)
        {
            var gridView = control.FindChild("GridView") as ItemsControl;

            if (gridView == null)
            {
                return;
            }

            var items = await new Data.PhotosDataSource().GetItemsAsync();
            gridView.ItemsSource = items
                .Select((p, i) => new
                {
                    Item = p,
                    Index = i + 1
                });
        }
    }
}
