// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.SampleApp.SamplePages.ConnectedAnimations.Pages;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConnectedAnimationsPage : Page
    {
        public ConnectedAnimationsPage()
        {
            this.InitializeComponent();
            RootFrame.Navigate(typeof(FirstPage));
        }

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            BackButton.Visibility = RootFrame.CanGoBack ? Visibility.Visible : Visibility.Collapsed;
        }

        private void RootFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.SourcePageType == RootFrame.SourcePageType)
            {
                e.Cancel = true;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            RootFrame.GoBack(new SuppressNavigationTransitionInfo());
        }
    }
}