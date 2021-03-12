// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// Sample page demonstrating view extensions
    /// </summary>
    public sealed partial class ViewExtensionsPage : Page
    {
        public ViewExtensionsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            ApplicationViewExtensions.SetTitle(this, string.Empty);

            var lightGreyBrush = (Color)Application.Current.Resources["Grey-04"];
            var brandColor = (Color)Application.Current.Resources["Brand-Color"];

            TitleBarExtensions.SetButtonBackgroundColor(this, brandColor);
            TitleBarExtensions.SetButtonForegroundColor(this, lightGreyBrush);
            TitleBarExtensions.SetBackgroundColor(this, brandColor);
            TitleBarExtensions.SetForegroundColor(this, lightGreyBrush);
        }
    }
}
