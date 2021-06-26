// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    public sealed partial class ObjectStoragePage
    {
        private readonly IObjectStorageHelper localStorageHelper = new LocalObjectStorageHelper(new SystemSerializer());

        public ObjectStoragePage()
        {
            InitializeComponent();
        }

        private void ReadButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(KeyTextBox.Text))
            {
                return;
            }

            // Read from local storage
            if (localStorageHelper.KeyExists(KeyTextBox.Text))
            {
                ContentTextBox.Text = localStorageHelper.Read<string>(KeyTextBox.Text);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(KeyTextBox.Text))
            {
                return;
            }

            if (string.IsNullOrEmpty(ContentTextBox.Text))
            {
                return;
            }

            // Save into local storage
            localStorageHelper.Save(KeyTextBox.Text, ContentTextBox.Text);
        }
    }
}