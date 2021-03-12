// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Toolkit.Uwp.SampleApp.Data;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
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
                _adaptiveGridViewControl.ItemClick += AdaptiveGridViewControl_ItemClick;
                _adaptiveGridViewControl.SelectionChanged += AdaptiveGridViewControl_SelectionChanged;
                NumberSlider.Minimum = 1;
                NumberSlider.Maximum = _originalPhotos.Length;
                NumberSlider.Value = _originalPhotos.Length;
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

        private void AdaptiveGridViewControl_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
        {
            SelectedItemCountTextBlock.Text = _adaptiveGridViewControl.SelectedItems.Any()
                ? $"You have selected {_adaptiveGridViewControl.SelectedItems.Count} items."
                : "You haven't selected any items";
        }

        private async void AdaptiveGridViewControl_ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
        {
            if (e.ClickedItem != null)
            {
                await new MessageDialog($"You clicked {(e.ClickedItem as PhotoDataItem).Title}", "Item Clicked").ShowAsync();
            }
        }
    }
}
