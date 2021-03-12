// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.SampleApp.Pages;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Animations;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;

namespace Microsoft.Toolkit.Uwp.SampleApp
{
    public sealed partial class Shell
    {
        private Sample _currentSample;
        private SampleCategory _selectedCategory;

        public CollectionViewSource SampleView { get; } = new CollectionViewSource();

        private Sample CurrentSample
        {
            get
            {
                return _currentSample;
            }

            set
            {
                _currentSample = value;
                _ = SetNavViewSelectionAsync();
            }
        }

        private async Task SetNavViewSelectionAsync()
        {
            if (_currentSample != null)
            {
                var category = await Samples.GetCategoryBySample(_currentSample);

                if ((NavView.MenuItemsSource as IEnumerable<SampleCategory>).Contains(category))
                {
                    NavView.SelectedItem = category;
                }
            }
            else
            {
                NavView.SelectedItem = null;
            }
        }

        private void HideSamplePicker()
        {
            SamplePickerGrid.Visibility = Visibility.Collapsed;
            _selectedCategory = null;

            _ = SetNavViewSelectionAsync();
        }

        private async void ShowSamplePicker(Sample[] samples = null, bool group = false)
        {
            if (samples == null && _currentSample != null)
            {
                var category = await Samples.GetCategoryBySample(_currentSample);
                if (category != null)
                {
                    samples = category.Samples;
                }
            }

            if (samples == null)
            {
                samples = (await Samples.GetCategoriesAsync()).FirstOrDefault()?.Samples;
            }

            if (samples == null)
            {
                return;
            }

            if (SamplePickerGrid.Visibility == Windows.UI.Xaml.Visibility.Visible &&
                SamplePickerGridView.ItemsSource is Sample[] currentSamples &&
                currentSamples.Count() == samples.Count() &&
                currentSamples.Except(samples).Count() == 0)
            {
                return;
            }

            SamplePickerGridView.ItemsSource = samples;

            var groups = samples.GroupBy(sample => sample.Subcategory);

            if (group && groups.Count() > 1)
            {
                SampleView.IsSourceGrouped = true;
                SampleView.Source = groups.OrderBy(g => g.Key);
            }
            else
            {
                SampleView.IsSourceGrouped = false;
                SampleView.Source = samples;
            }

            SamplePickerGridView.ItemsSource = SampleView.View;

            if (_currentSample != null && samples.Contains(_currentSample))
            {
                SamplePickerGridView.SelectedItem = _currentSample;
            }
            else
            {
                SamplePickerGridView.SelectedItem = null;
            }

            SamplePickerGrid.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void NavView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.InvokedItem is SampleCategory category)
            {
                if (SamplePickerGrid.Visibility != Visibility.Collapsed && _selectedCategory == category)
                {
                    // The NavView fires this event twice when the current selected item is clicked
                    // This makes sure the event get's processed correctly
                    var nop = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => HideSamplePicker());
                }
                else
                {
                    _selectedCategory = category;
                    ShowSamplePicker(category.Samples, true);

                    // Then Focus on Picker
                    dispatcherQueue.EnqueueAsync(() => SamplePickerGridView.Focus(FocusState.Keyboard));
                }
            }
        }

        private void SettingsTopNavPaneItem_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            // Can't get FooterMenuItems to work properly right now with ItemInvoked above, bug?
            // For now just hard-code an event.
            HideSamplePicker();
            if (NavigationFrame.CurrentSourcePageType != typeof(About))
            {
                NavigateToSample(null);
            }
        }

        private void ContentShadow_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            HideSamplePicker();
        }

        private void MoreInfoCanvas_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            HideMoreInfo();
        }

        private void SamplePickerGridView_ChoosingItemContainer(ListViewBase sender, ChoosingItemContainerEventArgs args)
        {
            if (args.ItemContainer != null)
            {
                return;
            }

            GridViewItem container = (GridViewItem)args.ItemContainer ?? new GridViewItem();

            container.Loaded -= ContainerItem_Loaded;
            container.PointerEntered -= ItemContainer_PointerEntered;
            container.PointerExited -= ItemContainer_PointerExited;

            container.Loaded += ContainerItem_Loaded;
            container.PointerEntered += ItemContainer_PointerEntered;
            container.PointerExited += ItemContainer_PointerExited;

            args.ItemContainer = container;
        }

        private void ContainerItem_Loaded(object sender, RoutedEventArgs e)
        {
            var itemsPanel = (ItemsWrapGrid)SamplePickerGridView.ItemsPanelRoot;
            var itemContainer = (GridViewItem)sender;
            itemContainer.Loaded -= this.ContainerItem_Loaded;

            var button = itemContainer.FindDescendant<Button>();
            if (button != null)
            {
                button.Click -= MoreInfoClicked;
                button.LostFocus -= MoreInfoLostFocus;
                button.Click += MoreInfoClicked;
                button.LostFocus += MoreInfoLostFocus;
            }

            var itemIndex = SamplePickerGridView.IndexFromContainer(itemContainer);

            var referenceIndex = itemsPanel.FirstVisibleIndex;

            if (SamplePickerGridView.SelectedIndex >= 0)
            {
                referenceIndex = SamplePickerGridView.SelectedIndex;
            }

            var relativeIndex = Math.Abs(itemIndex - referenceIndex);

            if (itemContainer.Content != CurrentSample && itemIndex >= 0 && itemIndex >= itemsPanel.FirstVisibleIndex && itemIndex <= itemsPanel.LastVisibleIndex)
            {
                var staggerDelay = TimeSpan.FromMilliseconds(relativeIndex * 30);

                VisualExtensions.SetNormalizedCenterPoint(itemContainer, "0.5");

                AnimationBuilder.Create()
                    .Opacity(from: 0, to: 1, delay: staggerDelay)
                    .Scale(from: 0.9, to: 1, delay: staggerDelay)
                    .Start(itemContainer);
            }
        }

        private void ItemContainer_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.FindDescendant<DropShadowPanel>() is FrameworkElement panel)
            {
                AnimationBuilder.Create().Opacity(0, duration: TimeSpan.FromMilliseconds(1200)).Start(panel);

                if (panel.Parent is UIElement parent)
                {
                    AnimationBuilder.Create().Scale(1, duration: TimeSpan.FromMilliseconds(1200)).Start(parent);
                }
            }
        }

        private void ItemContainer_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse &&
                (sender as FrameworkElement)?.FindDescendant<DropShadowPanel>() is FrameworkElement panel)
            {
                panel.Visibility = Visibility.Visible;

                AnimationBuilder.Create().Opacity(1, duration: TimeSpan.FromMilliseconds(600)).Start(panel);

                if (panel.Parent is UIElement parent)
                {
                    AnimationBuilder.Create().Scale(1.1, duration: TimeSpan.FromMilliseconds(600)).Start(parent);
                }
            }
        }

        private void MoreInfoClicked(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var sampleData = button.DataContext as Sample;

            var container = button.FindAscendant<GridViewItem>();
            if (container == null)
            {
                return;
            }

            InitMoreInfoContentContainer(container);
            MoreInfoContent.DataContext = sampleData;

            if (MoreInfoCanvas.Visibility == Visibility.Visible)
            {
                HideMoreInfo();
            }
            else
            {
                MoreInfoCanvas.Visibility = Visibility.Visible;
            }
        }

        private void MoreInfoLostFocus(object sender, RoutedEventArgs e)
        {
            HideMoreInfo();
        }

        private void InitMoreInfoContentContainer(GridViewItem container)
        {
            if (MoreInfoContent == null)
            {
                return;
            }

            var point = container.TransformToVisual(this).TransformPoint(new Windows.Foundation.Point(0, 0));
            var x = point.X - ((MoreInfoContent.Width - container.ActualWidth) / 2);
            var y = point.Y - ((MoreInfoContent.Height - container.ActualHeight) / 2);

            x = Math.Max(x, 10);
            x = Math.Min(x, ActualWidth - MoreInfoContent.Width - 10);

            y = Math.Max(y, 10);
            y = Math.Min(y, ActualHeight - MoreInfoContent.Height - 10);

            Canvas.SetLeft(MoreInfoContent, x);
            Canvas.SetTop(MoreInfoContent, y);

            var centerX = (point.X + (container.ActualWidth / 2)) - x;
            var centerY = (point.Y + (container.ActualHeight / 2)) - y;

            VisualExtensions.SetCenterPoint(MoreInfoContent, new Vector3((float)centerX, (float)centerY, 0).ToString());
        }

        private void HideMoreInfo()
        {
            if (MoreInfoImage != null && MoreInfoContent.DataContext != null)
            {
                ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("sample_icon", MoreInfoImage);
            }

            MoreInfoCanvas.Visibility = Visibility.Collapsed;

            if (MoreInfoImage != null && MoreInfoContent.DataContext != null)
            {
                var animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("sample_icon");
                animation.Configuration = new DirectConnectedAnimationConfiguration();

                _ = SamplePickerGridView.TryStartConnectedAnimationAsync(animation, MoreInfoContent.DataContext, "SampleIcon");
            }

            MoreInfoContent.DataContext = null;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            HideMoreInfo();
        }
    }
}
