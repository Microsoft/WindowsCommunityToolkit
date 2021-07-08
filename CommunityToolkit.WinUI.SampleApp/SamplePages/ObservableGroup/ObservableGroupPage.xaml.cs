// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using CommunityToolkit.Common.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    /// <summary>
    /// The sample page for the observable group collections.
    /// </summary>
    public sealed partial class ObservableGroupPage : Page
    {
        private readonly ObservableGroupedCollection<string, Person> _contactsSource;

        public ObservableGroupPage()
        {
            var contacts = new[]
            {
                new Person { Name = "Staff" },
                new Person { Name = "Swan" },
                new Person { Name = "Orchid" },
                new Person { Name = "Flame" },
                new Person { Name = "Arrow" },
                new Person { Name = "Tempest" },
                new Person { Name = "Pearl" },
                new Person { Name = "Hydra" },
                new Person { Name = "Lamp Post" },
                new Person { Name = "Looking Glass" },
            };
            var grouped = contacts.GroupBy(GetGroupName).OrderBy(g => g.Key);
            _contactsSource = new ObservableGroupedCollection<string, Person>(grouped);
            Contacts = new ReadOnlyObservableGroupedCollection<string, Person>(_contactsSource);

            InitializeComponent();
        }

        public ReadOnlyObservableGroupedCollection<string, Person> Contacts { get; }

        private static string GetGroupName(Person person) => person.Name.First().ToString().ToUpper();

        private void OnContactsListViewSelectionChanged(object sender, SelectionChangedEventArgs e) => RemoveContact.IsEnabled = ContactsListView.SelectedItem is Person;

        private void OnRemoveButtonClick(object sender, RoutedEventArgs e)
        {
            var selectedContact = (Person)ContactsListView.SelectedItem;
            var selectedGroupName = GetGroupName(selectedContact);
            var selectedGroup = _contactsSource.FirstOrDefault(group => group.Key == selectedGroupName);
            if (selectedGroup != null)
            {
                selectedGroup.Remove(selectedContact);
                if (!selectedGroup.Any())
                {
                    // The group is empty. We can remove it.
                    _contactsSource.Remove(selectedGroup);
                }
            }
        }

        private bool CanAddContact() => !string.IsNullOrEmpty(NewContact.Text.Trim());

        private void AddNewContact()
        {
            var newContact = new Person
            {
                Name = NewContact.Text.Trim(),
            };

            var groupName = GetGroupName(newContact);
            var targetGroup = _contactsSource.FirstOrDefault(group => group.Key == groupName);
            if (targetGroup is null)
            {
                _contactsSource.Add(new ObservableGroup<string, Person>(groupName, new[] { newContact }));
            }
            else
            {
                targetGroup.Add(newContact);
            }

            NewContact.Text = string.Empty;
        }

        private void OnAddContactClick(object sender, RoutedEventArgs e) => AddNewContact();

        private void OnNewContactTextChanged(object sender, TextChangedEventArgs e) => AddContact.IsEnabled = CanAddContact();

        private void OnNewContactKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter && CanAddContact())
            {
                AddNewContact();
            }
        }
    }
}