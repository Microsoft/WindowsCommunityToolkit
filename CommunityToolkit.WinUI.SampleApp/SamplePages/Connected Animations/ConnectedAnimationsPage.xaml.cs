// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.SampleApp.SamplePages.ConnectedAnimations.Pages;
using CommunityToolkit.WinUI.UI.Animations;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
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