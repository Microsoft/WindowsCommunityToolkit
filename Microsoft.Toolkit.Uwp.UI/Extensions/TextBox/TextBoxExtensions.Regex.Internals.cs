// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <inheritdoc cref="TextBoxExtensions"/>
    public static partial class TextBoxExtensions
    {
        private static void TextBoxRegexPropertyOnChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
            {
                return;
            }

            ValidateTextBox(textBox, false);

            textBox.Loaded -= TextBox_Loaded;
            textBox.LostFocus -= TextBox_LostFocus;
            textBox.TextChanged -= TextBox_TextChanged;
            textBox.Loaded += TextBox_Loaded;
            textBox.LostFocus += TextBox_LostFocus;
            textBox.TextChanged += TextBox_TextChanged;
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var validationMode = (ValidationMode)textBox.GetValue(ValidationModeProperty);
            ValidateTextBox(textBox, validationMode == ValidationMode.Dynamic);
        }

        private static void TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            ValidateTextBox(textBox);
        }

        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            ValidateTextBox(textBox);
        }

        private static void ValidateTextBox(TextBox textBox, bool force = true)
        {
            var validationType = (ValidationType)textBox.GetValue(ValidationTypeProperty);
            string regex;
            bool regexMatch;
            switch (validationType)
            {
                default:
                    regex = textBox.GetValue(RegexProperty) as string;
                    if (string.IsNullOrWhiteSpace(regex))
                    {
                        Debug.WriteLine("Regex property can't be null or empty when custom mode is selected");
                        return;
                    }

                    regexMatch = Regex.IsMatch(textBox.Text, regex);
                    break;
                case ValidationType.Decimal:
                    regexMatch = textBox.Text.IsDecimal();
                    break;
                case ValidationType.Email:
                    regexMatch = textBox.Text.IsEmail();
                    break;
                case ValidationType.Number:
                    regexMatch = textBox.Text.IsNumeric();
                    break;
                case ValidationType.PhoneNumber:
                    regexMatch = textBox.Text.IsPhoneNumber();
                    break;
                case ValidationType.Characters:
                    regexMatch = textBox.Text.IsCharacterString();
                    break;
            }

            if (!regexMatch && force)
            {
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    var validationModel = (ValidationMode)textBox.GetValue(ValidationModeProperty);
                    if (validationModel == ValidationMode.Forced)
                    {
                        textBox.Text = string.Empty;
                    }
                    else if (validationType != ValidationType.Email && validationType != ValidationType.PhoneNumber)
                    {
                        if (validationModel == ValidationMode.Dynamic)
                        {
                            int selectionStart = textBox.SelectionStart == 0 ? textBox.SelectionStart : textBox.SelectionStart - 1;
                            textBox.Text = textBox.Text.Remove(selectionStart, 1);
                            textBox.SelectionStart = selectionStart;
                        }
                    }
                }
            }

            textBox.SetValue(IsValidProperty, regexMatch);
        }
    }
}