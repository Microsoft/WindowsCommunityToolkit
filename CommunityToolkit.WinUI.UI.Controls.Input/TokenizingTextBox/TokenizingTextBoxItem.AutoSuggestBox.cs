// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
using Windows.System;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// A control that manages as the item logic for the <see cref="TokenizingTextBox"/> control.
    /// </summary>
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1124:Do not use regions", Justification = "Organization")]
    [TemplatePart(Name = PART_AutoSuggestBox, Type = typeof(AutoSuggestBox))] //// String case
    public partial class TokenizingTextBoxItem
    {
        private const string PART_AutoSuggestBox = "PART_AutoSuggestBox";

        private AutoSuggestBox _autoSuggestBox;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Tight Coupling with Parent for Selection control.")]
        internal TextBox _autoSuggestTextBox;

        /// <summary>
        /// Event raised when the 'Clear' Button is clicked.
        /// </summary>
        internal event TypedEventHandler<TokenizingTextBoxItem, RoutedEventArgs> AutoSuggestTextBoxLoaded;

        internal bool UseCharacterAsUser { get; set; }

        /// <summary>
        /// Gets a value indicating whether the textbox caret is in the first position. False otherwise
        /// </summary>
        private bool IsCaretAtStart => _autoSuggestTextBox?.SelectionStart == 0;

        /// <summary>
        /// Gets a value indicating whether the textbox caret is in the last position. False otherwise
        /// </summary>
        private bool IsCaretAtEnd => _autoSuggestTextBox?.SelectionStart == _autoSuggestTextBox?.Text.Length ||
                                     _autoSuggestTextBox?.SelectionStart + _autoSuggestTextBox?.SelectionLength == _autoSuggestTextBox?.Text.Length;

        /// <summary>
        /// Gets a value indicating whether all text in the text box is currently selected. False otherwise.
        /// </summary>
        private bool IsAllSelected => _autoSuggestTextBox?.SelectedText == _autoSuggestTextBox?.Text && !string.IsNullOrEmpty(_autoSuggestTextBox?.Text);

        /// <summary>
        /// Used to track if we're on the first character of the textbox while there is selected text
        /// </summary>
        private bool _isSelectedFocusOnFirstCharacter = false;

        /// <summary>
        /// Used to track if we're on the last character of the textbox while there is selected text
        /// </summary>
        private bool _isSelectedFocusOnLastCharacter = false;

        /// Called from <see cref="OnApplyTemplate"/>
        private void OnApplyTemplateAutoSuggestBox(AutoSuggestBox auto)
        {
            if (_autoSuggestBox != null)
            {
                _autoSuggestBox.Loaded -= OnASBLoaded;

                _autoSuggestBox.QuerySubmitted -= AutoSuggestBox_QuerySubmitted;
                _autoSuggestBox.SuggestionChosen -= AutoSuggestBox_SuggestionChosen;
                _autoSuggestBox.TextChanged -= AutoSuggestBox_TextChanged;
                _autoSuggestBox.PointerEntered -= AutoSuggestBox_PointerEntered;
                _autoSuggestBox.PointerExited -= AutoSuggestBox_PointerExited;
                _autoSuggestBox.PointerCanceled -= AutoSuggestBox_PointerExited;
                _autoSuggestBox.PointerCaptureLost -= AutoSuggestBox_PointerExited;
                _autoSuggestBox.GotFocus -= AutoSuggestBox_GotFocus;
                _autoSuggestBox.LostFocus -= AutoSuggestBox_LostFocus;

                // Remove any previous QueryIcon
                _autoSuggestBox.QueryIcon = null;
            }

            _autoSuggestBox = auto;

            if (_autoSuggestBox != null)
            {
                _autoSuggestBox.Loaded += OnASBLoaded;

                _autoSuggestBox.QuerySubmitted += AutoSuggestBox_QuerySubmitted;
                _autoSuggestBox.SuggestionChosen += AutoSuggestBox_SuggestionChosen;
                _autoSuggestBox.TextChanged += AutoSuggestBox_TextChanged;
                _autoSuggestBox.PointerEntered += AutoSuggestBox_PointerEntered;
                _autoSuggestBox.PointerExited += AutoSuggestBox_PointerExited;
                _autoSuggestBox.PointerCanceled += AutoSuggestBox_PointerExited;
                _autoSuggestBox.PointerCaptureLost += AutoSuggestBox_PointerExited;
                _autoSuggestBox.GotFocus += AutoSuggestBox_GotFocus;
                _autoSuggestBox.LostFocus += AutoSuggestBox_LostFocus;

                // Setup a binding to the QueryIcon of the Parent if we're the last box.
                if (Content is ITokenStringContainer str && str.IsLast)
                {
                    // Workaround for https://github.com/microsoft/microsoft-ui-xaml/issues/2568
                    if (Owner.QueryIcon is FontIconSource fis &&
                        fis.ReadLocalValue(FontIconSource.FontSizeProperty) == DependencyProperty.UnsetValue)
                    {
                        // This can be expensive, could we optimize?
                        // Also, this is changing the FontSize on the IconSource (which could be shared?)
                        fis.FontSize = Owner.TryFindResource("TokenizingTextBoxIconFontSize") as double? ?? 16;
                    }

                    var iconBinding = new Binding()
                    {
                        Source = Owner,
                        Path = new PropertyPath(nameof(Owner.QueryIcon)),
                        RelativeSource = new RelativeSource() { Mode = RelativeSourceMode.TemplatedParent }
                    };

                    var iconSourceElement = new IconSourceElement();

                    iconSourceElement.SetBinding(IconSourceElement.IconSourceProperty, iconBinding);

                    _autoSuggestBox.QueryIcon = iconSourceElement;
                }
            }
        }

        #region AutoSuggestBox
        private async void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            Owner.RaiseQuerySubmitted(sender, args);

            object chosenItem = null;
            if (args.ChosenSuggestion != null)
            {
                chosenItem = args.ChosenSuggestion;
            }
            else if (!string.IsNullOrWhiteSpace(args.QueryText))
            {
                chosenItem = args.QueryText;
            }

            if (chosenItem != null)
            {
                await Owner.AddTokenAsync(chosenItem); // TODO: Need to pass index?
                sender.Text = string.Empty;
                Owner.Text = string.Empty;
                sender.Focus(FocusState.Programmatic);
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            Owner.RaiseSuggestionChosen(sender, args);
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var t = sender.Text.Trim();

            Owner.Text = sender.Text; // Update parent text property

            // Override our programmatic manipulation as we're redirecting input for the user
            if (UseCharacterAsUser)
            {
                UseCharacterAsUser = false;

                args.Reason = AutoSuggestionBoxTextChangeReason.UserInput;
            }

            Owner.RaiseTextChanged(sender, args);

            // Look for Token Delimiters to create new tokens when text changes.
            if (!string.IsNullOrEmpty(Owner.TokenDelimiter) && t.Contains(Owner.TokenDelimiter))
            {
                bool lastDelimited = t[t.Length - 1] == Owner.TokenDelimiter[0];

                string[] tokens = t.Split(Owner.TokenDelimiter);
                int numberToProcess = lastDelimited ? tokens.Length : tokens.Length - 1;
                for (int position = 0; position < numberToProcess; position++)
                {
                    string token = tokens[position];
                    token = token.Trim();
                    if (token.Length > 0)
                    {
                        _ = Owner.AddTokenAsync(token); //// TODO: Pass Index?
                    }
                }

                if (lastDelimited)
                {
                    sender.Text = string.Empty;
                }
                else
                {
                    sender.Text = tokens[tokens.Length - 1];
                }
            }
        }
        #endregion

        #region Visual State Management for Parent
        private void AutoSuggestBox_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(Owner, TokenizingTextBox.PART_PointerOverState, true);
        }

        private void AutoSuggestBox_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(Owner, TokenizingTextBox.PART_NormalState, true);
        }

        private void AutoSuggestBox_LostFocus(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(Owner, TokenizingTextBox.PART_UnfocusedState, true);
        }

        private void AutoSuggestBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // Verify if the usual behavior of clearing token selection is required
            if (Owner.PauseTokenClearOnFocus == false && !TokenizingTextBox.IsShiftPressed)
            {
                // Clear any selected tokens
                Owner.DeselectAll();
            }

            Owner.PauseTokenClearOnFocus = false;

            VisualStateManager.GoToState(Owner, TokenizingTextBox.PART_FocusedState, true);
        }
        #endregion

        #region Inner TextBox
        private void OnASBLoaded(object sender, RoutedEventArgs e)
        {
            // Local function for Selection changed
            void AutoSuggestTextBox_SelectionChanged(object box, RoutedEventArgs args)
            {
                if (!(IsAllSelected || TokenizingTextBox.IsShiftPressed || Owner.IsClearingForClick))
                {
                    Owner.DeselectAllTokensAndText(this);
                }

                // Ensure flag is always reset
                Owner.IsClearingForClick = false;
            }

            // local function for clearing selection on interaction with text box
            async void AutoSuggestTextBox_TextChangingAsync(TextBox o, TextBoxTextChangingEventArgs args)
            {
                // remove any selected tokens.
                if (Owner.SelectedItems.Count > 1)
                {
                    await Owner.RemoveAllSelectedTokens();
                }
            }

            if (_autoSuggestTextBox != null)
            {
                _autoSuggestTextBox.PreviewKeyDown -= this.AutoSuggestTextBox_PreviewKeyDown;
                _autoSuggestTextBox.TextChanging -= AutoSuggestTextBox_TextChangingAsync;
                _autoSuggestTextBox.SelectionChanged -= AutoSuggestTextBox_SelectionChanged;
                _autoSuggestTextBox.SelectionChanging -= AutoSuggestTextBox_SelectionChanging;
            }

            _autoSuggestTextBox = _autoSuggestBox.FindDescendant<TextBox>() as TextBox;

            if (_autoSuggestTextBox != null)
            {
                _autoSuggestTextBox.PreviewKeyDown += this.AutoSuggestTextBox_PreviewKeyDown;
                _autoSuggestTextBox.TextChanging += AutoSuggestTextBox_TextChangingAsync;
                _autoSuggestTextBox.SelectionChanged += AutoSuggestTextBox_SelectionChanged;
                _autoSuggestTextBox.SelectionChanging += AutoSuggestTextBox_SelectionChanging;

                AutoSuggestTextBoxLoaded?.Invoke(this, e);
            }
        }

        private void AutoSuggestTextBox_SelectionChanging(TextBox sender, TextBoxSelectionChangingEventArgs args)
        {
            _isSelectedFocusOnFirstCharacter = args.SelectionLength > 0 && args.SelectionStart == 0 && _autoSuggestTextBox.SelectionStart > 0;
            _isSelectedFocusOnLastCharacter =
                //// see if we are NOW on the last character.
                //// test if the new selection includes the last character, and the current selection doesn't
                (args.SelectionStart + args.SelectionLength == _autoSuggestTextBox.Text.Length) &&
                (_autoSuggestTextBox.SelectionStart + _autoSuggestTextBox.SelectionLength != _autoSuggestTextBox.Text.Length);
        }

        private void AutoSuggestTextBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (IsCaretAtStart &&
                (e.Key == VirtualKey.Back ||
                 e.Key == VirtualKey.Left))
            {
                // if the back key is pressed and there is any selection in the text box then the text box can handle it
                if ((e.Key == VirtualKey.Left && _isSelectedFocusOnFirstCharacter) ||
                    _autoSuggestTextBox.SelectionLength == 0)
                {
                    if (Owner.SelectPreviousItem(this))
                    {
                        if (!TokenizingTextBox.IsShiftPressed)
                        {
                            // Clear any text box selection
                            _autoSuggestTextBox.SelectionLength = 0;
                        }

                        e.Handled = true;
                    }
                }
            }
            else if (IsCaretAtEnd && e.Key == VirtualKey.Right)
            {
                // if the back key is pressed and there is any selection in the text box then the text box can handle it
                if (_isSelectedFocusOnLastCharacter || _autoSuggestTextBox.SelectionLength == 0)
                {
                    if (Owner.SelectNextItem(this))
                    {
                        if (!TokenizingTextBox.IsShiftPressed)
                        {
                            // Clear any text box selection
                            _autoSuggestTextBox.SelectionLength = 0;
                        }

                        e.Handled = true;
                    }
                }
            }
            else if (e.Key == VirtualKey.A && Owner.IsControlPressed)
            {
                // Need to provide this shortcut from the textbox only, as ListViewBase will do it for us on token.
                Owner.SelectAllTokensAndText();
            }
        }
        #endregion
    }
}