// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.WinUI.SampleApp.Data;
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    public sealed partial class AdaptiveGridViewPage : IXamlRenderListener
    {
        private AdaptiveGridView _adaptiveGridViewControl;
        private PhotoDataItem[] _originalPhotos;
        private ObservableCollection<PhotoDataItem> _boundPhotos;

        public AdaptiveGridViewPage()
        {
            InitializeComponent();
        }

        public async void OnXamlRendered(FrameworkElement control)
        {
            _adaptiveGridViewControl = control.FindDescendant("AdaptiveGridViewControl") as AdaptiveGridView;
            if (_adaptiveGridViewControl != null)
            {
                var allPhotos = await new Data.PhotosDataSource().GetItemsAsync();
                _originalPhotos = allPhotos.ToArray();
                _boundPhotos = new ObservableCollection<PhotoDataItem>(_originalPhotos);
                _adaptiveGridViewControl.ItemsSource = _boundPhotos;
                _adaptiveGridViewControl.ItemClick -= AdaptiveGridViewControl_ItemClick;
                _adaptiveGridViewControl.ItemClick += AdaptiveGridViewControl_ItemClick;
                _adaptiveGridViewControl.SelectionChanged -= AdaptiveGridViewControl_SelectionChanged;
                _adaptiveGridViewControl.SelectionChanged += AdaptiveGridViewControl_SelectionChanged;
                NumberSlider.Minimum = 1;
                NumberSlider.Maximum = _originalPhotos.Length;
                NumberSlider.Value = _originalPhotos.Length;
                NumberSlider.ValueChanged -= OnNumberSliderValueChanged;
                NumberSlider.ValueChanged += OnNumberSliderValueChanged;
            }
        }

        private void OnNumberSliderValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var newCount = (int)e.NewValue;
            var currentCount = _boundPhotos.Count;
            if (currentCount < newCount)
            {
                for (var i = currentCount; i < newCount; i++)
                {
                    _boundPhotos.Add(_originalPhotos[i]);
                }
            }
            else if (currentCount > newCount)
            {
                for (var i = currentCount; i > newCount; i--)
                {
                    _boundPhotos.Remove(_originalPhotos[i - 1]);
                }
            }
        }

        private void AdaptiveGridViewControl_SelectionChanged(object sender, Microsoft.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            SelectedItemCountTextBlock.Text = _adaptiveGridViewControl.SelectedItems.Any()
                ? $"You have selected {_adaptiveGridViewControl.SelectedItems.Count} items."
                : "You haven't selected any items";
        }

        private async void AdaptiveGridViewControl_ItemClick(object sender, Microsoft.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                await new ContentDialog
                {
                    Title = "Item Clicked",
                    Content = $"You clicked {(e.ClickedItem as PhotoDataItem).Title}",
                    CloseButtonText = "Close",
                    XamlRoot = base.XamlRoot
                }.ShowAsync();
            }
        }
    }
}