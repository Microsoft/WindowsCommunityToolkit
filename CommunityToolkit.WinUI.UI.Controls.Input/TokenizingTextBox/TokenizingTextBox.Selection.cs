// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.DataTransfer;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Methods related to Selection of items in the <see cref="TokenizingTextBox"/>.
    /// </summary>
    public partial class TokenizingTextBox
    {
        private enum MoveDirection
        {
            Next,
            Previous
        }

        /// <summary>
        /// Adjust the selected item and range based on keyboard input.
        /// This is used to override the listview behaviors for up/down arrow manipulation vs left/right for a horizontal control
        /// </summary>
        /// <param name="direction">direction to move the selection</param>
        /// <returns>True if the focus was moved, false otherwise</returns>
        private bool MoveFocusAndSelection(MoveDirection direction)
        {
            bool retVal = false;
            var currentContainerItem = GetCurrentContainerItem();

            if (currentContainerItem != null)
            {
                var currentItem = ItemFromContainer(currentContainerItem);
                var previousIndex = Items.IndexOf(currentItem);
                var index = previousIndex;

                if (direction == MoveDirection.Previous)
                {
                    if (previousIndex > 0)
                    {
                        index -= 1;
                    }
                    else
                    {
                        if (TabNavigateBackOnArrow)
                        {
                            FocusManager.TryMoveFocus(FocusNavigationDirection.Previous, new FindNextElementOptions
                            {
                                SearchRoot = XamlRoot.Content
                            });
                        }

                        retVal = true;
                    }
                }
                else if (direction == MoveDirection.Next)
                {
                    if (previousIndex < Items.Count - 1)
                    {
                        index += 1;
                    }
                }

                // Only do stuff if the index is actually changing
                if (index != previousIndex)
                {
                    var newItem = ContainerFromIndex(index) as TokenizingTextBoxItem;

                    // Check for the new item being a text control.
                    // this must happen before focus is set to avoid seeing the caret
                    // jump in come cases
                    if (Items[index] is ITokenStringContainer && !IsShiftPressed)
                    {
                        newItem._autoSuggestTextBox.SelectionLength = 0;
                        newItem._autoSuggestTextBox.SelectionStart = direction == MoveDirection.Next
                            ? 0
                            : newItem._autoSuggestTextBox.Text.Length;
                    }

                    newItem.Focus(FocusState.Keyboard);

                    // if no control keys are selected then the selection also becomes just this item
                    if (IsShiftPressed)
                    {
                        // What we do here depends on where the selection started
                        // if the previous item is between the start and new position then we add the new item to the selected range
                        // if the new item is between the start and the previous position then we remove the previous position
                        int newDistance = Math.Abs(SelectedIndex - index);
                        int oldDistance = Math.Abs(SelectedIndex - previousIndex);

                        if (newDistance > oldDistance)
                        {
                            SelectedItems.Add(Items[index]);
                        }
                        else
                        {
                            SelectedItems.Remove(Items[previousIndex]);
                        }
                    }
                    else if (!IsControlPressed)
                    {
                        SelectedIndex = index;

                        // This looks like a bug in the underlying ListViewBase control.
                        // Might need to be reviewed if the base behavior is fixed
                        // When two consecutive items are selected and the navigation moves between them,
                        // the first time that happens the old focused item is not unselected
                        if (SelectedItems.Count > 1)
                        {
                            SelectedItems.Clear();
                            SelectedIndex = index;
                        }
                    }

                    retVal = true;
                }
            }

            return retVal;
        }

        private TokenizingTextBoxItem GetCurrentContainerItem()
        {
            if (XamlRoot != null)
            {
                return FocusManager.GetFocusedElement(XamlRoot) as TokenizingTextBoxItem;
            }
            else
            {
                return FocusManager.GetFocusedElement() as TokenizingTextBoxItem;
            }
        }

        internal void SelectAllTokensAndText()
        {
            _ = DispatcherQueue.EnqueueAsync(
                () =>
                {
                    this.SelectAllSafe();

                    // need to synchronize the select all and the focus behavior on the text box
                    // because there is no way to identify that the focus has been set from this point
                    // to avoid instantly clearing the selection of tokens
                    PauseTokenClearOnFocus = true;

                    foreach (var item in Items)
                    {
                        if (item is ITokenStringContainer)
                        {
                            // grab any selected text
                            var pretoken = ContainerFromItem(item) as TokenizingTextBoxItem;
                            pretoken._autoSuggestTextBox.SelectionStart = 0;
                            pretoken._autoSuggestTextBox.SelectionLength = pretoken._autoSuggestTextBox.Text.Length;
                        }
                    }

                    (ContainerFromIndex(Items.Count - 1) as TokenizingTextBoxItem).Focus(FocusState.Programmatic);
                }, Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal);
        }

        internal void DeselectAllTokensAndText(TokenizingTextBoxItem ignoreItem = null)
        {
            this.DeselectAll();
            ClearAllTextSelections(ignoreItem);
        }

        private void ClearAllTextSelections(TokenizingTextBoxItem ignoreItem)
        {
            // Clear any selection in the text box
            foreach (var item in Items)
            {
                if (item is ITokenStringContainer)
                {
                    var container = ContainerFromItem(item) as TokenizingTextBoxItem;

                    if (container != ignoreItem)
                    {
                        container._autoSuggestTextBox.SelectionLength = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Select the previous item in the list, if one is available. Called when moving from textbox to token.
        /// </summary>
        /// <param name="item">identifies the current item</param>
        /// <returns>a value indicating whether the previous item was successfully selected</returns>
        internal bool SelectPreviousItem(TokenizingTextBoxItem item)
        {
            return SelectNewItem(item, -1, i => i > 0);
        }

        /// <summary>
        /// Select the next item in the list, if one is available. Called when moving from textbox to token.
        /// </summary>
        /// <param name="item">identifies the current item</param>
        /// <returns>a value indicating whether the next item was successfully selected, false if nothing was changed</returns>
        internal bool SelectNextItem(TokenizingTextBoxItem item)
        {
            return SelectNewItem(item, 1, i => i < Items.Count - 1);
        }

        private bool SelectNewItem(TokenizingTextBoxItem item, int increment, Func<int, bool> testFunc)
        {
            bool returnVal = false;

            // find the item in the list
            var currentIndex = IndexFromContainer(item);

            // Select previous token item (if there is one).
            if (testFunc(currentIndex))
            {
                var newItem = ContainerFromItem(Items[currentIndex + increment]) as ListViewItem;
                newItem.Focus(FocusState.Keyboard);
                SelectedItems.Add(Items[currentIndex + increment]);
                returnVal = true;
            }

            return returnVal;
        }

        private async void TokenizingTextBoxItem_ClearAllAction(TokenizingTextBoxItem sender, RoutedEventArgs args)
        {
            // find the first item selected
            int newSelectedIndex = -1;

            if (SelectedRanges.Count > 0)
            {
                newSelectedIndex = SelectedRanges[0].FirstIndex - 1;
            }

            await RemoveAllSelectedTokens();

            SelectedIndex = newSelectedIndex;

            if (newSelectedIndex == -1)
            {
                newSelectedIndex = Items.Count - 1;
            }

            // focus the item prior to the first selected item
            (ContainerFromIndex(newSelectedIndex) as TokenizingTextBoxItem).Focus(FocusState.Keyboard);
        }

        private async void TokenizingTextBoxItem_ClearClicked(TokenizingTextBoxItem sender, RoutedEventArgs args)
        {
            await RemoveTokenAsync(sender);
        }

        /// <summary>
        /// Remove any tokens that are in the selected list, except for the last text box or the currently selected item
        /// </summary>
        /// <returns>async task</returns>
        internal async Task RemoveAllSelectedTokens()
        {
            var currentContainerItem = GetCurrentContainerItem();

            while (SelectedItems.Count > 0)
            {
                var container = ContainerFromItem(SelectedItems[0]) as TokenizingTextBoxItem;

                if (IndexFromContainer(container) != Items.Count - 1)
                {
                    // if its a text box, remove any selected text, and if its then empty remove the container, unless its focused
                    if (SelectedItems[0] is ITokenStringContainer)
                    {
                        var asb = container._autoSuggestTextBox;

                        // grab any selected text
                        var tempStr = asb.SelectionStart == 0
                            ? string.Empty
                            : asb.Text.Substring(
                                0,
                                asb.SelectionStart);
                        tempStr +=
                            asb.SelectionStart +
                            asb.SelectionLength < asb.Text.Length
                                ? asb.Text.Substring(
                                    asb.SelectionStart +
                                    asb.SelectionLength)
                                : string.Empty;

                        if (tempStr.Length == 0)
                        {
                            // Need to be careful not to remove the last item in the list
                            await RemoveTokenAsync(container);
                        }
                        else
                        {
                            asb.Text = tempStr;
                        }
                    }
                    else
                    {
                        // if the item is a token just remove it.
                        await RemoveTokenAsync(container);
                    }
                }
                else
                {
                    if (SelectedItems.Count == 1)
                    {
                        // at this point we have one selection and its the default textbox.
                        // stop the iteration here
                        break;
                    }
                }
            }
        }

        private void CopySelectedToClipboard()
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;

            var tokenString = PrepareSelectionForClipboard();

            if (!string.IsNullOrEmpty(tokenString))
            {
                dataPackage.SetText(tokenString);
                Clipboard.SetContent(dataPackage);
            }
        }

        private string PrepareSelectionForClipboard()
        {
            string tokenString = string.Empty;
            bool addSeparator = false;

            // Copy all items if none selected (and no text selected)
            foreach (var item in SelectedItems.Count > 0 ? SelectedItems : Items)
            {
                if (addSeparator)
                {
                    tokenString += TokenDelimiter;
                }
                else
                {
                    addSeparator = true;
                }

                if (item is ITokenStringContainer)
                {
                    // grab any selected text
                    var pretoken = ContainerFromItem(item) as TokenizingTextBoxItem;
                    tokenString += pretoken._autoSuggestTextBox.Text.Substring(
                        pretoken._autoSuggestTextBox.SelectionStart,
                        pretoken._autoSuggestTextBox.SelectionLength);
                }
                else
                {
                    tokenString += item.ToString();
                }
            }

            return tokenString;
        }
    }
}