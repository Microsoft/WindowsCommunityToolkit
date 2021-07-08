// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace UITests.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RangeSelectorTestPage : Page
    {
        public RangeSelectorTestPage()
        {
            this.InitializeComponent();
        }

        private void SubmitStepFrequency_Click(object sender, RoutedEventArgs e)
        {
            if(TryDoubleFromTextBox(inputStepFrequency, out var sf))
            {
                rangeSelector.StepFrequency = sf;
            }
        }

        private void SubmitMinimum_Click(object sender, RoutedEventArgs e)
        {
            if(TryDoubleFromTextBox(inputMinimum, out var min))
            {
                rangeSelector.Minimum = min;
            }
        }

        private void SubmitRangeStart_Click(object sender, RoutedEventArgs e)
        {
            if(TryDoubleFromTextBox(inputRangeStart, out var rStart))
            {
                rangeSelector.RangeStart = rStart;
            }
        }

        private void SubmitRangeEnd_Click(object sender, RoutedEventArgs e)
        {
            if(TryDoubleFromTextBox(inputRangeEnd, out var rEnd))
            {
                rangeSelector.RangeEnd = rEnd;
            }
        }

        private void SubmitMaximum_Click(object sender, RoutedEventArgs e)
        {
            if(TryDoubleFromTextBox(inputMaximum, out var max))
            {
                rangeSelector.Maximum = max;
            }
        }

        private void SubmitAll_Click(object sender, RoutedEventArgs e)
        {
            if(TryDoubleFromTextBox(inputStepFrequency, out var sf)
                && TryDoubleFromTextBox(inputMinimum, out var min)
                && TryDoubleFromTextBox(inputRangeStart, out var rStart)
                && TryDoubleFromTextBox(inputRangeEnd, out var rEnd)
                && TryDoubleFromTextBox(inputMaximum, out var max))
            {
                // This order is important.
                // TODO (2021.04.28) - document that this order for changing these props is the most predicable.
                rangeSelector.Minimum = min;
                rangeSelector.Maximum = max;
                rangeSelector.StepFrequency = sf;
                rangeSelector.RangeStart = rStart;
                rangeSelector.RangeEnd = rEnd;
            }
        }

        private bool TryDoubleFromTextBox(TextBox tbx, out double value)
        {
            if (tbx.Text is string s && double.TryParse(s, out value))
            {
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
}
