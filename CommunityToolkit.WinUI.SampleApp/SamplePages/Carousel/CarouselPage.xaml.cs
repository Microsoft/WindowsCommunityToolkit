// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    /// <summary>
    /// A page that shows how to use the Carousel control.
    /// </summary>
    public sealed partial class CarouselPage : Page, IXamlRenderListener
    {
        private Carousel carouselControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="CarouselPage"/> class.
        /// </summary>
        public CarouselPage()
        {
            InitializeComponent();
        }

        public async void OnXamlRendered(FrameworkElement control)
        {
            carouselControl = control.FindDescendant("CarouselControl") as Carousel;
            carouselControl.ItemsSource = await new Data.PhotosDataSource().GetItemsAsync();
        }
    }
}