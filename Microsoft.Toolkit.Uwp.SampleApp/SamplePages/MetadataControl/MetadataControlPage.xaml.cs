// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp.SampleApp.Common;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// A page that shows how to use the MetadataControl
    /// </summary>
    public sealed partial class MetadataControlPage : Page, IXamlRenderListener
    {
        private static readonly string[] Labels = "Lorem ipsum dolor sit amet consectetur adipiscing elit".Split(" ");

        private readonly Random _random;
        private readonly ObservableCollection<MetadataItem> _units;
        private readonly DelegateCommand<object> _command;
        private MetadataControl _metadataControl;

        public MetadataControlPage()
        {
            _random = new Random();
            _units = new ObservableCollection<MetadataItem>();
            _command = new DelegateCommand<object>(OnExecuteCommand);
            InitializeComponent();
            Setup();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            _metadataControl = control.FindChild("metadataControl") as MetadataControl;
            if (_metadataControl != null)
            {
                _metadataControl.Items = _units;
            }
        }

        private void Setup()
        {
            SampleController.Current.RegisterNewCommand("Add label", (sender, args) =>
            {
                _units.Add(new MetadataItem { Label = GetRandomLabel() });
            });

            SampleController.Current.RegisterNewCommand("Add command", (sender, args) =>
            {
                var label = GetRandomLabel();
                _units.Add(new MetadataItem
                {
                    Label = label,
                    Command = _command,
                    CommandParameter = label,
                });
            });

            SampleController.Current.RegisterNewCommand("Clear", (sender, args) =>
            {
                _units.Clear();
            });
        }

        private string GetRandomLabel() => Labels[_random.Next(Labels.Length)];

        private async void OnExecuteCommand(object obj)
        {
            var dialog = new ContentDialog
            {
                Title = "Command invoked",
                Content = $"Command parameter: {obj}",
                CloseButtonText = "OK"
            };

            await dialog.ShowAsync();
        }
    }
}
