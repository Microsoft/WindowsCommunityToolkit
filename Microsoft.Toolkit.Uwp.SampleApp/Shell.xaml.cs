// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.SampleApp.Pages;
using Microsoft.Toolkit.Uwp.UI;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Toolkit.Uwp.SampleApp
{
    public sealed partial class Shell
    {
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public static Shell Current { get; private set; }

        public Shell()
        {
            InitializeComponent();
            Current = this;
        }

        /// <summary>
        /// Navigates to a Sample via a deep link.
        /// </summary>
        /// <param name="deepLink">The deep link. Specified as protocol://[collectionName]?sample=[sampleName]</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task NavigateToSampleAsync(string deepLink)
        {
            var parser = DeepLinkParser.Create(deepLink);
            var targetSample = await Samples.GetSampleByName(parser["sample"]);
            if (targetSample != null)
            {
                NavigateToSample(targetSample);
            }
        }

        /// <summary>
        /// Navigates to a Sample
        /// </summary>
        /// <param name="sample">Sample to navigate to</param>
        public void NavigateToSample(Sample sample)
        {
            if (sample == null)
            {
                NavigationFrame.Navigate(typeof(About), null, new SuppressNavigationTransitionInfo());
            }
            else
            {
                NavigationFrame.Navigate(typeof(SampleController), sample);
                TrackingManager.TrackEvent("sample", "navigation", sample.Name);
            }
        }

        /// <summary>
        /// Set app title
        /// </summary>
        /// <param name="title">Title to set</param>
        public void SetAppTitle(string title)
        {
            ApplicationViewExtensions.SetTitle(this, title);
        }

        /// <summary>
        /// Attach a ScrollViewer to Parallax hosting the background image
        /// </summary>
        /// <param name="viewer">The ScrollViewer</param>
        public void AttachScroll(ScrollViewer viewer)
        {
            Parallax.Source = viewer;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            NavigationFrame.Navigated += NavigationFrameOnNavigated;
            NavView.BackRequested += NavView_BackRequested;

            // Get list of samples
            var sampleCategories = await Samples.GetCategoriesAsync();
            NavView.MenuItemsSource = sampleCategories;

            SetAppTitle(string.Empty);
            NavigateToSample(null);

            if (!string.IsNullOrWhiteSpace(e?.Parameter?.ToString()))
            {
                var parser = DeepLinkParser.Create(e.Parameter.ToString());
                var targetSample = await Sample.FindAsync(parser.Root, parser["sample"]);
                if (targetSample != null)
                {
                    NavigateToSample(targetSample);
                }
            }

            // NavView goes into a weird size on load unless the window size changes
            // Needs a gentle push to update layout
            NavView.Loaded += (s, args) => NavView.InvalidateMeasure();
        }

        private void NavView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            if (NavigationFrame.CanGoBack)
            {
                NavigationFrame.GoBack();
            }
        }

        private void NavigationFrameOnNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            NavView.IsBackEnabled = NavigationFrame.CanGoBack;

            CurrentSample = navigationEventArgs.Parameter as Sample;
        }

        private void SamplePickerGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            HideSamplePicker();
            NavigateToSample(e.ClickedItem as Sample);
        }
    }
}