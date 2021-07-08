// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AdvancedCollectionViewPage : Page
    {
        public AdvancedCollectionViewPage()
        {
            this.InitializeComponent();
            Setup();
        }

        private void Setup()
        {
            // left list
            var oc = new ObservableCollection<Person>
            {
                new Person { Name = "Staff" },
                new Person { Name = "42" },
                new Person { Name = "Swan" },
                new Person { Name = "Orchid" },
                new Person { Name = "15" },
                new Person { Name = "Flame" },
                new Person { Name = "16" },
                new Person { Name = "Arrow" },
                new Person { Name = "Tempest" },
                new Person { Name = "23" },
                new Person { Name = "Pearl" },
                new Person { Name = "Hydra" },
                new Person { Name = "Lamp Post" },
                new Person { Name = "4" },
                new Person { Name = "Looking Glass" },
                new Person { Name = "8" },
            };

            LeftList.ItemsSource = oc;

            // right list
            var acv = new AdvancedCollectionView(oc);
            int nul;
            acv.Filter = x => !int.TryParse(((Person)x).Name, out nul);
            acv.SortDescriptions.Add(new SortDescription("Name", SortDirection.Ascending));

            RightList.ItemsSource = acv;

            // add button
            AddButton.Click += (sender, args) =>
            {
                if (!string.IsNullOrWhiteSpace(NewItemBox.Text))
                {
                    oc.Add(new Person { Name = NewItemBox.Text });
                }
            };
        }
    }
}