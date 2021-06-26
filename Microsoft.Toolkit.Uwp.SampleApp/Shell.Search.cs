// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Microsoft.Toolkit.Uwp.SampleApp
{
    public sealed partial class Shell
    {
        internal void StartSearch(string startingText = null)
        {
            object focusedElement;

            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Xaml.UIElement", "XamlRoot") && XamlRoot != null)
            {
                focusedElement = FocusManager.GetFocusedElement(XamlRoot);
            }
            else
            {
                focusedElement = FocusManager.GetFocusedElement();
            }

            if (focusedElement == SearchBox.FindDescendant<TextBox>())
            {
                return;
            }

            SearchBox.Focus(FocusState.Keyboard);

            var currentSearchText = SearchBox.Text;

            SearchBox.Text = string.Empty;

            if (!string.IsNullOrWhiteSpace(startingText))
            {
                SearchBox.Text = startingText;
            }
            else
            {
                SearchBox.Text = currentSearchText;
            }
        }

        private async void UpdateSearchSuggestions()
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                HideSamplePicker();
                return;
            }

            var samples = (await Samples.FindSample(SearchBox.Text)).OrderBy(s => s.Name).ToArray();
            if (samples.Count() > 0)
            {
                ShowSamplePicker(samples);
            }
            else
            {
                HideSamplePicker();
            }
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            UpdateSearchSuggestions();
        }

        private void SearchBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Down && SamplePickerGrid.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                // If we try and navigate down out of the textbox (and there's search results), go to the search results.
                dispatcherQueue.EnqueueAsync(() => SamplePickerGridView.Focus(FocusState.Keyboard));
            }
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            UpdateSearchSuggestions();
        }
    }
}