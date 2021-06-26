// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    public sealed partial class OrbitViewPage : Page, IXamlRenderListener
    {
        private Random _random = new Random();

        public ObservableCollection<DeviceItem> DeviceList { get; private set; } = new ObservableCollection<DeviceItem>();

        public OrbitViewPage()
        {
            this.InitializeComponent();

            DeviceList.Add(new DeviceItem() { Distance = 0.1, Label = "My Phone", Symbol = Symbol.CellPhone });
            Load();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            var people = control.FindChild("People") as OrbitView;
            if (people != null)
            {
                people.ItemClick += People_ItemClick;
            }

            var devices = control.FindChild("Devices") as OrbitView;
            if (devices != null)
            {
                devices.ItemsSource = DeviceList;
                devices.ItemClick += Devices_ItemClick;
            }
        }

        private void Load()
        {
            SampleController.Current.RegisterNewCommand("Add Device", AddDeviceClick);
        }

        private void AddDeviceClick(object sender, RoutedEventArgs e)
        {
            switch (_random.Next(3))
            {
                case 0:
                    DeviceList.Add(new DeviceItem() { Distance = _random.Next(1, 10) / 10f, Label = "Other Phone", Symbol = Symbol.CellPhone });
                    break;
                case 1:
                    DeviceList.Add(new DeviceItem() { Distance = _random.Next(1, 10) / 10f, Label = "Camera", Symbol = Symbol.Camera });
                    break;
                case 2:
                    DeviceList.Add(new DeviceItem() { Distance = _random.Next(1, 10) / 10f, Label = "TV", Symbol = Symbol.GoToStart });
                    break;
            }
        }

        private async void People_ItemClick(object sender, OrbitViewItemClickedEventArgs e)
        {
            await new MessageDialog("You clicked: " + (e.Item as OrbitViewDataItem)?.Label).ShowAsync();
        }

        private void Devices_ItemClick(object sender, OrbitViewItemClickedEventArgs e)
        {
            DeviceList.Remove(e.Item as DeviceItem);
        }
    }

#pragma warning disable SA1402 // File may only contain a single class
    public class DeviceItem : OrbitViewDataItem
#pragma warning restore SA1402 // File may only contain a single class
    {
        public Symbol Symbol { get; set; }
    }
}