// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Toolkit.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Extensions
{
    /// <summary>
    /// TextBoxRegex allows text validation using a regular expression.
    /// </summary>
    /// <remarks>
    /// If <see cref="ValidationMode"> is set to Normal then IsValid will be set according to whether the regex is valid.</see>
    /// If <see cref="ValidationMode"> is set to Forced then IsValid will be set according to whether the regex is valid, when TextBox lose focus and in case the TextBox is invalid clear its value. </see>
    /// If <see cref="ValidationMode"> is set to Dynamic then IsValid will be set according to whether the regex is valid. If the newest character is invalid, the input will be canceled.</see>
    /// </remarks>
    public partial class TextBoxRegex
    {
        private static void TextBoxRegexPropertyOnChange(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox == null)
            {
                return;
            }

            textBox.LostFocus -= TextBox_LostFocus;
            textBox.BeforeTextChanging -= TextBox_BeforeTextChanging;
            textBox.LostFocus += TextBox_LostFocus;
            textBox.BeforeTextChanging += TextBox_BeforeTextChanging;
        }

        private static void TextBox_BeforeTextChanging(TextBox textBox, TextBoxBeforeTextChangingEventArgs args)
        {
            var validationMode = (ValidationMode)textBox.GetValue(ValidationModeProperty);
            var validationType = (ValidationType)textBox.GetValue(ValidationTypeProperty);
            var (valid, successful) = ValidateTextBox(textBox, args.NewText, validationMode != ValidationMode.Normal);
            if (successful &&
                !valid &&
                validationMode == ValidationMode.Dynamic &&
                validationType != ValidationType.Email &&
                validationType != ValidationType.PhoneNumber &&
                args.NewText != string.Empty)
            {
                args.Cancel = true;
            }
        }

        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            ValidateTextBox(textBox, textBox.Text);
        }

        private static (bool valid, bool successful) ValidateTextBox(TextBox textBox, string newText, bool force = true)
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
                        return (false, false);
                    }

                    regexMatch = Regex.IsMatch(newText, regex);
                    break;
                case ValidationType.Decimal:
                    regexMatch = newText.IsDecimal();
                    break;
                case ValidationType.Email:
                    regexMatch = newText.IsEmail();
                    break;
                case ValidationType.Number:
                    regexMatch = newText.IsNumeric();
                    break;
                case ValidationType.PhoneNumber:
                    regexMatch = newText.IsPhoneNumber();
                    break;
                case ValidationType.Characters:
                    regexMatch = newText.IsCharacterString();
                    break;
            }

            var isValid = (bool)textBox.GetValue(IsValidProperty);
            if (regexMatch == false && force && newText != string.Empty)
            {
                var validationModel = (ValidationMode)textBox.GetValue(ValidationModeProperty);
                if (validationModel == ValidationMode.Forced)
                {
                    if (textBox.Text == newText)
                    {
                        // occurs when focus is lost
                        textBox.Text = string.Empty;
                    }
                    else
                    {
                        // only set IsValidProperty to false, when the property is true
                        if (isValid)
                        {
                            textBox.SetValue(IsValidProperty, regexMatch);
                        }
                    }
                }
            }
            else
            {
                // only set IsValidProperty when the property is not equal to regexMatch
                if (isValid != regexMatch)
                {
                    textBox.SetValue(IsValidProperty, regexMatch);
                }
            }

            return (regexMatch, true);
        }
    }
}
