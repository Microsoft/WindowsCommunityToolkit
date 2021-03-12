// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Text;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// Page that shows how to use StringExtensions
    /// </summary>
    public sealed partial class StringExtensionsPage : Page
    {
        public StringExtensionsPage()
        {
            this.InitializeComponent();
            ValidateCurrentText();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateCurrentText();
        }

        private void ValidateCurrentText()
        {
            IsValidEmailResult.Text = InputTextBox.Text.IsEmail().ToString();
            IsValidEmailResult.FontWeight = InputTextBox.Text.IsEmail() ? FontWeights.Bold : FontWeights.Normal;

            IsValidNumberResult.Text = InputTextBox.Text.IsNumeric().ToString();
            IsValidNumberResult.FontWeight = InputTextBox.Text.IsNumeric() ? FontWeights.Bold : FontWeights.Normal;

            IsValidDecimalResult.Text = InputTextBox.Text.IsDecimal().ToString();
            IsValidDecimalResult.FontWeight = InputTextBox.Text.IsDecimal() ? FontWeights.Bold : FontWeights.Normal;

            IsValidStringResult.Text = InputTextBox.Text.IsCharacterString().ToString();
            IsValidStringResult.FontWeight = InputTextBox.Text.IsCharacterString() ? FontWeights.Bold : FontWeights.Normal;

            IsValidPhoneNumberResult.Text = InputTextBox.Text.IsPhoneNumber().ToString();
            IsValidPhoneNumberResult.FontWeight = InputTextBox.Text.IsPhoneNumber() ? FontWeights.Bold : FontWeights.Normal;
        }
    }
}
