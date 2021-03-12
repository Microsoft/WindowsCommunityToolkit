// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <inheritdoc cref="TextBoxExtensions"/>
    public static partial class TextBoxExtensions
    {
        private const string DefaultPlaceHolder = "_";
        private const char EscapeChar = '\\';
        private static readonly KeyValuePair<char, string> AlphaCharacterRepresentation = new KeyValuePair<char, string>('a', "[A-Za-z]");
        private static readonly KeyValuePair<char, string> NumericCharacterRepresentation = new KeyValuePair<char, string>('9', "[0-9]");
        private static readonly KeyValuePair<char, string> AlphaNumericRepresentation = new KeyValuePair<char, string>('*', "[A-Za-z0-9]");

        private static void InitTextBoxMask(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as TextBox;

            if (textbox == null)
            {
                return;
            }

            textbox.SelectionChanged -= Textbox_SelectionChanged;
            textbox.TextChanging -= Textbox_TextChanging;
            textbox.Paste -= Textbox_Paste;
            textbox.Loaded -= Textbox_Loaded;
            textbox.GotFocus -= Textbox_GotFocus_Mask;
            textbox.Loaded += Textbox_Loaded;
        }

        private static void Textbox_Loaded(object sender, RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;

            // In case no value is provided, use it as normal textbox
            var mask = textbox.GetValue(MaskProperty) as string;
            if (string.IsNullOrWhiteSpace(mask))
            {
                return;
            }

            var placeHolderValue = textbox.GetValue(MaskPlaceholderProperty) as string;
            if (string.IsNullOrEmpty(placeHolderValue))
            {
                throw new ArgumentException("PlaceHolder can't be null or empty");
            }

            var escapedChars = new List<int>();

            var builder = new StringBuilder(mask);
            for (int i = 0; i < builder.Length - 1; i++)
            {
                if (builder[i] == EscapeChar)
                {
                    escapedChars.Add(i);
                    builder.Remove(i, 1);
                }
            }

            var escapedMask = builder.ToString();

            textbox.SetValue(EscapedCharacterIndicesProperty, escapedChars);
            textbox.SetValue(EscapedMaskProperty, escapedMask);

            var placeHolder = placeHolderValue[0];

            var representationDictionary = new Dictionary<char, string>();
            representationDictionary.Add(AlphaCharacterRepresentation.Key, AlphaCharacterRepresentation.Value);
            representationDictionary.Add(NumericCharacterRepresentation.Key, NumericCharacterRepresentation.Value);
            representationDictionary.Add(AlphaNumericRepresentation.Key, AlphaNumericRepresentation.Value);

            var customDictionaryValue = textbox.GetValue(CustomMaskProperty) as string;
            if (!string.IsNullOrWhiteSpace(customDictionaryValue))
            {
                var customRoles = customDictionaryValue.Split(',');
                foreach (var role in customRoles)
                {
                    var roleValues = role.Split(':');
                    if (roleValues.Length != 2)
                    {
                        throw new ArgumentException("Invalid CustomMask property");
                    }

                    var keyValue = roleValues[0];
                    var value = roleValues[1];
                    char key;

                    // an exception should be throw if the regex is not valid
                    Regex.Match(string.Empty, value);
                    if (!char.TryParse(keyValue, out key))
                    {
                        throw new ArgumentException("Invalid CustomMask property, please validate the mask key");
                    }

                    representationDictionary.Add(key, value);
                }
            }

            textbox.SetValue(RepresentationDictionaryProperty, representationDictionary);

            var displayTextBuilder = new StringBuilder(escapedMask);
            for (int i = 0; i < displayTextBuilder.Length; i++)
            {
                if (escapedChars.Contains(i))
                {
                    continue;
                }

                foreach (var key in representationDictionary.Keys)
                {
                    if (displayTextBuilder[i] == key)
                    {
                        displayTextBuilder[i] = placeHolder;
                    }
                }
            }

            var displayText = displayTextBuilder.ToString();

            if (string.IsNullOrEmpty(textbox.Text))
            {
                textbox.Text = displayText;
            }
            else
            {
                var textboxInitialValue = textbox.Text;
                textbox.Text = displayText;
                int oldSelectionStart = (int)textbox.GetValue(OldSelectionStartProperty);
                SetTextBoxValue(textboxInitialValue, textbox, escapedMask, escapedChars, representationDictionary, placeHolder, oldSelectionStart);
            }

            textbox.TextChanging += Textbox_TextChanging;
            textbox.SelectionChanged += Textbox_SelectionChanged;
            textbox.Paste += Textbox_Paste;
            textbox.GotFocus += Textbox_GotFocus_Mask;
            textbox.SetValue(OldTextProperty, textbox.Text);
            textbox.SetValue(DefaultDisplayTextProperty, displayText);
            textbox.SelectionStart = 0;
        }

        private static void Textbox_GotFocus_Mask(object sender, RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;
            var mask = textbox?.GetValue(MaskProperty) as string;
            var placeHolderValue = textbox?.GetValue(MaskPlaceholderProperty) as string;
            var representationDictionary = textbox?.GetValue(RepresentationDictionaryProperty) as Dictionary<char, string>;
            if (string.IsNullOrWhiteSpace(mask) ||
                representationDictionary == null ||
                string.IsNullOrEmpty(placeHolderValue))
            {
                return;
            }

            var placeHolder = placeHolderValue[0];

            // if the textbox got focus and the textbox is empty (contains only mask) set the textbox cursor at the beginning to simulate normal TextBox behavior if it is empty.
            // if the textbox has value set the cursor to the first empty mask character
            var textboxText = textbox.Text;
            for (int i = 0; i < textboxText.Length; i++)
            {
                if (placeHolder == textboxText[i])
                {
                    textbox.SelectionStart = i;
                    break;
                }
            }
        }

        private static async void Textbox_Paste(object sender, TextControlPasteEventArgs e)
        {
            e.Handled = true;
            DataPackageView dataPackageView = Clipboard.GetContent();
            if (!dataPackageView.Contains(StandardDataFormats.Text))
            {
                return;
            }

            var pasteText = await dataPackageView.GetTextAsync();
            if (string.IsNullOrWhiteSpace(pasteText))
            {
                return;
            }

            var textbox = (TextBox)sender;
            var mask = textbox.GetValue(MaskProperty) as string;
            var representationDictionary = textbox?.GetValue(RepresentationDictionaryProperty) as Dictionary<char, string>;
            var placeHolderValue = textbox.GetValue(MaskPlaceholderProperty) as string;
            if (string.IsNullOrWhiteSpace(mask) ||
            representationDictionary == null ||
            string.IsNullOrEmpty(placeHolderValue))
            {
                return;
            }

            var escapedMask = textbox.GetValue(EscapedMaskProperty) as string;
            var escapedChars = textbox.GetValue(EscapedCharacterIndicesProperty) as List<int>;

            // to update the textbox text without triggering TextChanging text
            int oldSelectionStart = (int)textbox.GetValue(OldSelectionStartProperty);
            textbox.TextChanging -= Textbox_TextChanging;
            SetTextBoxValue(pasteText, textbox, escapedMask, escapedChars, representationDictionary, placeHolderValue[0], oldSelectionStart);
            textbox.SetValue(OldTextProperty, textbox.Text);
            textbox.TextChanging += Textbox_TextChanging;
        }

        private static void SetTextBoxValue(
            string newValue,
            TextBox textbox,
            string mask,
            List<int> escapedChars,
            Dictionary<char, string> representationDictionary,
            char placeholder,
            int oldSelectionStart)
        {
            var maxLength = (newValue.Length + oldSelectionStart) < mask.Length ? (newValue.Length + oldSelectionStart) : mask.Length;
            var textArray = textbox.Text.ToCharArray();

            for (int i = oldSelectionStart; i < maxLength; i++)
            {
                var maskChar = mask[i];
                var selectedChar = newValue[i - oldSelectionStart];

                // If dynamic character a,9,* or custom
                if (representationDictionary.ContainsKey(maskChar) && !escapedChars.Contains(i))
                {
                    var pattern = representationDictionary[maskChar];
                    if (Regex.IsMatch(selectedChar.ToString(), pattern))
                    {
                        textArray[i] = selectedChar;
                    }
                    else
                    {
                        textArray[i] = placeholder;
                    }
                }
            }

            textbox.Text = new string(textArray);
            textbox.SelectionStart = maxLength;
        }

        private static void Textbox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;
            textbox.SetValue(OldSelectionStartProperty, textbox.SelectionStart);
            textbox.SetValue(OldSelectionLengthProperty, textbox.SelectionLength);
        }

        private static void Textbox_TextChanging(TextBox textbox, TextBoxTextChangingEventArgs args)
        {
            var escapedMask = textbox.GetValue(EscapedMaskProperty) as string;
            var escapedChars = textbox.GetValue(EscapedCharacterIndicesProperty) as List<int>;

            var representationDictionary = textbox.GetValue(RepresentationDictionaryProperty) as Dictionary<char, string>;
            var placeHolderValue = textbox?.GetValue(MaskPlaceholderProperty) as string;
            var oldText = textbox.GetValue(OldTextProperty) as string;
            var oldSelectionStart = (int)textbox.GetValue(OldSelectionStartProperty);
            var oldSelectionLength = (int)textbox.GetValue(OldSelectionLengthProperty);
            if (string.IsNullOrWhiteSpace(escapedMask) ||
                representationDictionary == null ||
                string.IsNullOrEmpty(placeHolderValue) ||
                oldText == null)
            {
                return;
            }

            var placeHolder = placeHolderValue[0];
            var isDeleteOrBackspace = false;
            var deleteBackspaceIndex = 0;

            // Delete or backspace is triggered
            // if the new length is less than or equal the old text - the old selection length then a delete or backspace is triggered with or without selection and no characters is added
            if (textbox.Text.Length < oldText.Length
                && textbox.Text.Length <= oldText.Length - oldSelectionLength)
            {
                isDeleteOrBackspace = true;
                if (oldSelectionLength == 0)
                {
                    // backspace else delete
                    if (oldSelectionStart != textbox.SelectionStart)
                    {
                        deleteBackspaceIndex++;
                    }
                }
            }

            // case adding data at the end of the textbox
            if (oldSelectionStart >= oldText.Length && !isDeleteOrBackspace)
            {
                textbox.Text = textbox.Text.Substring(0, oldText.Length);
                if (oldText.Length >= 0)
                {
                    textbox.SelectionStart = oldText.Length;
                }

                return;
            }

            var textArray = oldText.ToCharArray();

            // detect if backspace or delete is triggered to handle the right removed character
            var newSelectionIndex = oldSelectionStart - deleteBackspaceIndex;

            // check if single selection
            var isSingleSelection = oldSelectionLength != 0 && oldSelectionLength != 1;

            // for handling single key click add +1 to match length for selection = 1
            var singleOrMultiSelectionIndex = oldSelectionLength == 0 ? oldSelectionLength + 1 : oldSelectionLength;

            // Case change due to Text property is assigned a value (Ex Textbox.Text="value")
            if (textbox.SelectionStart == 0 && textbox.FocusState == FocusState.Unfocused)
            {
                var displayText = textbox.GetValue(DefaultDisplayTextProperty) as string ?? string.Empty;
                if (string.IsNullOrEmpty(textbox.Text))
                {
                    textbox.Text = displayText;
                    return;
                }
                else
                {
                    var textboxInitialValue = textbox.Text;
                    textbox.Text = displayText;
                    SetTextBoxValue(textboxInitialValue, textbox, escapedMask, escapedChars, representationDictionary, placeHolderValue[0], 0);
                    textbox.SetValue(OldTextProperty, textbox.Text);
                    return;
                }
            }

            if (!isDeleteOrBackspace)
            {
                // In case the change happened due to user input
                var selectedChar = textbox.SelectionStart > 0 ?
                                    textbox.Text[textbox.SelectionStart - 1] :
                                    placeHolder;

                var maskChar = escapedMask[newSelectionIndex];

                // If dynamic character a,9,* or custom
                if (representationDictionary.ContainsKey(maskChar) && !escapedChars.Contains(newSelectionIndex))
                {
                    var pattern = representationDictionary[maskChar];
                    if (Regex.IsMatch(selectedChar.ToString(), pattern))
                    {
                        textArray[newSelectionIndex] = selectedChar;

                        // updating text box new index
                        newSelectionIndex++;
                    }

                    // character doesn't match the pattern get the old character
                    else
                    {
                        // if single press don't change
                        if (oldSelectionLength == 0)
                        {
                            textArray[newSelectionIndex] = oldText[newSelectionIndex];
                        }

                        // if change in selection reset to default place holder instead of keeping the old valid to be clear for the user
                        else
                        {
                            textArray[newSelectionIndex] = placeHolder;
                        }
                    }
                }

                // if fixed character
                else
                {
                    textArray[newSelectionIndex] = oldText[newSelectionIndex];

                    // updating text box new index
                    newSelectionIndex++;
                }
            }

            if (isSingleSelection || isDeleteOrBackspace)
            {
                for (int i = newSelectionIndex;
                    i < (oldSelectionStart - deleteBackspaceIndex + singleOrMultiSelectionIndex);
                    i++)
                {
                    var maskChar = escapedMask[i];

                    // If dynamic character a,9,* or custom
                    if (representationDictionary.ContainsKey(maskChar) && !escapedChars.Contains(i))
                    {
                        textArray[i] = placeHolder;
                    }

                    // if fixed character
                    else
                    {
                        textArray[i] = oldText[i];
                    }
                }
            }

            textbox.Text = new string(textArray);
            textbox.SetValue(OldTextProperty, textbox.Text);
            textbox.SelectionStart = isDeleteOrBackspace ? newSelectionIndex : GetSelectionStart(escapedMask, escapedChars, newSelectionIndex, representationDictionary);
        }

        private static int GetSelectionStart(string mask, List<int> escapedChars, int selectionIndex, Dictionary<char, string> representationDictionary)
        {
            for (int i = selectionIndex; i < mask.Length; i++)
            {
                var maskChar = mask[i];

                // If dynamic character a,9,* or custom
                if (representationDictionary.ContainsKey(maskChar) && !escapedChars.Contains(i))
                {
                    return i;
                }
            }

            return selectionIndex;
        }
    }
}
