// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    public sealed partial class TokenizingTextBoxPage : Page, IXamlRenderListener
    {
        //// TODO: We should use images here.
        private readonly List<SampleEmailDataType> _emailSamples = new List<SampleEmailDataType>()
        {
            new SampleEmailDataType() { FirstName = "Marcus", FamilyName = "Perryman" },
            new SampleEmailDataType() { FirstName = "Michael", FamilyName = "Hawker" },
            new SampleEmailDataType() { FirstName = "Matt", FamilyName = "Lacey" },
            new SampleEmailDataType() { FirstName = "Alexandre", FamilyName = "Chohfi" },
            new SampleEmailDataType() { FirstName = "Filip", FamilyName = "Wallberg" },
            new SampleEmailDataType() { FirstName = "Shane", FamilyName = "Weaver" },
            new SampleEmailDataType() { FirstName = "Vincent", FamilyName = "Gromfeld" },
            new SampleEmailDataType() { FirstName = "Sergio", FamilyName = "Pedri" },
            new SampleEmailDataType() { FirstName = "Alex", FamilyName = "Wilber" },
            new SampleEmailDataType() { FirstName = "Allan", FamilyName = "Deyoung" },
            new SampleEmailDataType() { FirstName = "Adele", FamilyName = "Vance" },
            new SampleEmailDataType() { FirstName = "Grady", FamilyName = "Archie" },
            new SampleEmailDataType() { FirstName = "Megan", FamilyName = "Bowen" },
            new SampleEmailDataType() { FirstName = "Ben", FamilyName = "Walters" },
            new SampleEmailDataType() { FirstName = "Debra", FamilyName = "Berger" },
            new SampleEmailDataType() { FirstName = "Emily", FamilyName = "Braun" },
            new SampleEmailDataType() { FirstName = "Christine", FamilyName = "Cline" },
            new SampleEmailDataType() { FirstName = "Enrico", FamilyName = "Catteneo" },
            new SampleEmailDataType() { FirstName = "Davit", FamilyName = "Badalyan" },
            new SampleEmailDataType() { FirstName = "Diego", FamilyName = "Siciliani" },
            new SampleEmailDataType() { FirstName = "Raul", FamilyName = "Razo" },
            new SampleEmailDataType() { FirstName = "Miriam", FamilyName = "Graham" },
            new SampleEmailDataType() { FirstName = "Lynne", FamilyName = "Robbins" },
            new SampleEmailDataType() { FirstName = "Lydia", FamilyName = "Holloway" },
            new SampleEmailDataType() { FirstName = "Nestor", FamilyName = "Wilke" },
            new SampleEmailDataType() { FirstName = "Patti", FamilyName = "Fernandez" },
            new SampleEmailDataType() { FirstName = "Pradeep", FamilyName = "Gupta" },
            new SampleEmailDataType() { FirstName = "Joni", FamilyName = "Sherman" },
            new SampleEmailDataType() { FirstName = "Isaiah", FamilyName = "Langer" },
            new SampleEmailDataType() { FirstName = "Irvin", FamilyName = "Sayers" },
        };

        // TODO: Setup ACV for this collection as well.
        private readonly List<SampleDataType> _samples = new List<SampleDataType>()
        {
            new SampleDataType() { Text = "Account", Icon = Symbol.Account },
            new SampleDataType() { Text = "Add Friend", Icon = Symbol.AddFriend },
            new SampleDataType() { Text = "Attach", Icon = Symbol.Attach },
            new SampleDataType() { Text = "Attach Camera", Icon = Symbol.AttachCamera },
            new SampleDataType() { Text = "Audio", Icon = Symbol.Audio },
            new SampleDataType() { Text = "Block Contact", Icon = Symbol.BlockContact },
            new SampleDataType() { Text = "Calculator", Icon = Symbol.Calculator },
            new SampleDataType() { Text = "Calendar", Icon = Symbol.Calendar },
            new SampleDataType() { Text = "Camera", Icon = Symbol.Camera },
            new SampleDataType() { Text = "Contact", Icon = Symbol.Contact },
            new SampleDataType() { Text = "Favorite", Icon = Symbol.Favorite },
            new SampleDataType() { Text = "Link", Icon = Symbol.Link },
            new SampleDataType() { Text = "Mail", Icon = Symbol.Mail },
            new SampleDataType() { Text = "Map", Icon = Symbol.Map },
            new SampleDataType() { Text = "Phone", Icon = Symbol.Phone },
            new SampleDataType() { Text = "Pin", Icon = Symbol.Pin },
            new SampleDataType() { Text = "Rotate", Icon = Symbol.Rotate },
            new SampleDataType() { Text = "Rotate Camera", Icon = Symbol.RotateCamera },
            new SampleDataType() { Text = "Send", Icon = Symbol.Send },
            new SampleDataType() { Text = "Tags", Icon = Symbol.Tag },
            new SampleDataType() { Text = "UnFavorite", Icon = Symbol.UnFavorite },
            new SampleDataType() { Text = "UnPin", Icon = Symbol.UnPin },
            new SampleDataType() { Text = "Zoom", Icon = Symbol.Zoom },
            new SampleDataType() { Text = "ZoomIn", Icon = Symbol.ZoomIn },
            new SampleDataType() { Text = "ZoomOut", Icon = Symbol.ZoomOut },
        };

        private TokenizingTextBox _ttb;
        private TokenizingTextBox _ttbEmail;
        private ListView _ttbEmailSuggestions;

        private AdvancedCollectionView _acv;
        private AdvancedCollectionView _acvEmail;

        private ObservableCollection<SampleEmailDataType> _selectedEmails;

        public TokenizingTextBoxPage()
        {
            InitializeComponent();

            _acv = new AdvancedCollectionView(_samples, false);
            _acvEmail = new AdvancedCollectionView(_emailSamples, false);

            _acv.SortDescriptions.Add(new SortDescription(nameof(SampleDataType.Text), SortDirection.Ascending));
            _acvEmail.SortDescriptions.Add(new SortDescription(nameof(SampleEmailDataType.DisplayName), SortDirection.Ascending));

            Loaded += (sender, e) => { this.OnXamlRendered(this); };

            // Add the buttons
            SampleController.Current.RegisterNewCommand("Clear Tokens", ClearButtonClick);
            SampleController.Current.RegisterNewCommand("Show Email Items", ShowEmailSelectedClick);
            SampleController.Current.RegisterNewCommand("Show Email Selection", ShowSelectedTextClick);
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            _selectedEmails = new ObservableCollection<SampleEmailDataType>();

            if (_ttb != null)
            {
                _ttb.TokenItemAdded -= TokenItemAdded;
                _ttb.TokenItemRemoving -= TokenItemRemoved;
                _ttb.TextChanged -= TextChanged;
                _ttb.TokenItemAdding -= TokenItemCreating;
            }

            if (control.FindChild("TokenBox") is TokenizingTextBox ttb)
            {
                _ttb = ttb;
                _ttb.TokenItemAdded += TokenItemAdded;
                _ttb.TokenItemRemoving += TokenItemRemoved;
                _ttb.TextChanged += TextChanged;
                _ttb.TokenItemAdding += TokenItemCreating;

                _acv.Filter = item => !_ttb.Items.Contains(item) && (item as SampleDataType).Text.Contains(_ttb.Text, StringComparison.CurrentCultureIgnoreCase);

                _ttb.SuggestedItemsSource = _acv;
            }

            // For the Email Selection control
            if (_ttbEmail != null)
            {
                _ttbEmail.ItemClick -= EmailTokenItemClick;
                _ttbEmail.TokenItemAdding -= EmailTokenItemAdding;
                _ttbEmail.TokenItemAdded -= EmailTokenItemAdded;
                _ttbEmail.TokenItemRemoved -= EmailTokenItemRemoved;
                _ttbEmail.TextChanged -= EmailTextChanged;
                _ttbEmail.PreviewKeyDown -= EmailPreviewKeyDown;
            }

            if (control.FindChild("TokenBoxEmail") is TokenizingTextBox ttbEmail)
            {
                _ttbEmail = ttbEmail;

                _ttbEmail.ItemsSource = _selectedEmails;
                _ttbEmail.ItemClick += EmailTokenItemClick;
                _ttbEmail.TokenItemAdding += EmailTokenItemAdding;
                _ttbEmail.TokenItemAdded += EmailTokenItemAdded;
                _ttbEmail.TokenItemRemoved += EmailTokenItemRemoved;
                _ttbEmail.TextChanged += EmailTextChanged;
                _ttbEmail.PreviewKeyDown += EmailPreviewKeyDown;

                _acvEmail.Filter = item => !_ttbEmail.Items.Contains(item) && (item as SampleEmailDataType).DisplayName.Contains(_ttbEmail.Text, StringComparison.CurrentCultureIgnoreCase);
            }

            if (_ttbEmailSuggestions != null)
            {
                _ttbEmailSuggestions.ItemClick -= EmailList_ItemClick;
                _ttbEmailSuggestions.PreviewKeyDown -= EmailList_PreviewKeyDown;
            }

            if (control.FindChild("EmailList") is ListView ttbList)
            {
                _ttbEmailSuggestions = ttbList;

                _ttbEmailSuggestions.ItemClick += EmailList_ItemClick;
                _ttbEmailSuggestions.PreviewKeyDown += EmailList_PreviewKeyDown;

                _ttbEmailSuggestions.ItemsSource = _acvEmail;
            }
        }

        private async void EmailTokenItemClick(object sender, ItemClickEventArgs e)
        {
            var dialog = new ContentDialog
            {
                Title = "Clicked Item",
                Content = $"email address {(e.ClickedItem as SampleEmailDataType)?.EmailAddress} clicked",
                CloseButtonText = "Close",
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }

        private void TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent() && args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                _acv.RefreshFilter();
            }
        }

        private void TokenItemCreating(object sender, TokenItemAddingEventArgs e)
        {
            // Take the user's text and convert it to our data type (if we have a matching one).
            e.Item = _samples.FirstOrDefault((item) => item.Text.Contains(e.TokenText, StringComparison.CurrentCultureIgnoreCase));

            // Otherwise, create a new version of our data type
            if (e.Item == null)
            {
                e.Item = new SampleDataType()
                {
                    Text = e.TokenText,
                    Icon = Symbol.OutlineStar
                };
            }
        }

        private void TokenItemAdded(TokenizingTextBox sender, object data)
        {
            // TODO: Add InApp Notification?
            if (data is SampleDataType sample)
            {
                Debug.WriteLine("Added Token: " + sample.Text);
            }
            else
            {
                Debug.WriteLine("Added Token: " + data);
            }
        }

        private void TokenItemRemoved(TokenizingTextBox sender, TokenItemRemovingEventArgs args)
        {
            if (args.Item is SampleDataType sample)
            {
                Debug.WriteLine("Removed Token: " + sample.Text);
            }
            else
            {
                Debug.WriteLine("Removed Token: " + args.Item);
            }
        }

        private void EmailTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.CheckCurrent() && args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                _acvEmail.RefreshFilter();
            }
        }

        private void EmailTokenItemAdding(TokenizingTextBox sender, TokenItemAddingEventArgs args)
        {
            // Search our list for a matching person
            foreach (var person in _emailSamples)
            {
                if (args.TokenText.Contains(person.EmailAddress) ||
                    args.TokenText.Contains(person.DisplayName, StringComparison.CurrentCultureIgnoreCase))
                {
                    args.Item = person;
                    return;
                }
            }

            // Otherwise don't create a token.
            args.Cancel = true;
        }

        private void EmailTokenItemAdded(TokenizingTextBox sender, object args)
        {
            if (args is SampleEmailDataType sample)
            {
                Debug.WriteLine("Added Email: " + sample.DisplayName);
            }
            else
            {
                Debug.WriteLine("Added Token: " + args);
            }

            _acvEmail.RefreshFilter();
        }

        private void EmailTokenItemRemoved(TokenizingTextBox sender, object args)
        {
            if (args is SampleEmailDataType sample)
            {
                Debug.WriteLine("Removed Email: " + sample.DisplayName);
            }
            else
            {
                Debug.WriteLine("Removed Token: " + args);
            }

            _acvEmail.RefreshFilter();
        }

        private void EmailList_ItemClick(object sender, ItemClickEventArgs e)
        {
            // TODO: not sure how this is getting to be null, need to make simple repro and file platform issue?
            if (e.ClickedItem != null && e.ClickedItem is SampleEmailDataType email)
            {
                _ttbEmail.Text = string.Empty; // Clear current text

                _ttbEmail.AddTokenItem(email); // Insert new token with picked item to current text location

                _acvEmail.RefreshFilter();

                _ttbEmail.Focus(FocusState.Programmatic); // Give focus back to type another filter
            }
        }

        private async void ClearButtonClick(object sender, RoutedEventArgs e)
        {
            await _ttbEmail.ClearAsync();
            _acvEmail.RefreshFilter();

            await _ttb.ClearAsync();
        }

        private async void ShowEmailSelectedClick(object sender, RoutedEventArgs e)
        {
            // Grab the list of items and identify which ones are free text, which ones are tokens
            string message = string.Empty;

            foreach (var item in _ttbEmail.Items)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    message += "\r\n";
                }

                message += item is ITokenStringContainer ? "Unrslvd: " : "Token  : ";
                var textVal = item.ToString();

                message += string.IsNullOrEmpty(textVal) ? "<empty>" : textVal;
            }

            var dialog = new ContentDialog
            {
                Title = "Item List with type",
                Content = message,
                CloseButtonText = "Close",
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }

        private async void ShowSelectedTextClick(object sender, RoutedEventArgs e)
        {
            // Grab the list of items and identify which ones are free text, which ones are tokens
            string message = _ttbEmail.SelectedTokenText;

            if (_ttbEmail.SelectedItems.Count == 0)
            {
                message = "<Nothing Selected>";
            }

            var dialog = new ContentDialog
            {
                Title = "Selected Tokens as Text",
                Content = message,
                CloseButtonText = "Close",
                XamlRoot = XamlRoot
            };
            await dialog.ShowAsync();
        }

        // Move to Email Suggest ListView list when we keydown from the TTB
        private void EmailPreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Down && _ttbEmailSuggestions != null)
            {
                e.Handled = true;

                _ttbEmailSuggestions.SelectedIndex = 0;

                _ttbEmailSuggestions.Focus(FocusState.Programmatic);
            }
        }

        private void EmailList_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Up &&
                _ttbEmailSuggestions != null && _ttbEmail != null &&
                _ttbEmailSuggestions.SelectedIndex == 0)
            {
                e.Handled = true;

                _ttbEmail.Focus(FocusState.Programmatic); // Give focus back to type another filter
            }
        }
    }
}