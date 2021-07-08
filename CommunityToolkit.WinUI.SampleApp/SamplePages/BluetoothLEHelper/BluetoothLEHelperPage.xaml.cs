// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.Connectivity;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    public sealed partial class BluetoothLEHelperPage : Page
    {
        private BluetoothLEHelper bluetoothLEHelper = BluetoothLEHelper.Context;

        public BluetoothLEHelperPage()
        {
            this.InitializeComponent();
            bluetoothLEHelper.EnumerationCompleted += BluetoothLEHelper_EnumerationCompleted;
        }

        private async void BluetoothLEHelper_EnumerationCompleted(object sender, EventArgs e)
        {
            await DispatcherQueue.EnqueueAsync(
                () =>
            {
                bluetoothLEHelper.StopEnumeration();
                BtEnumeration.Content = "Start Enumerating";
            }, DispatcherQueuePriority.Normal);
        }

        private void Enumeration_Click(object sender, RoutedEventArgs e)
        {
            if (!bluetoothLEHelper.IsEnumerating)
            {
                bluetoothLEHelper.StartEnumeration();
                BtEnumeration.Content = "Stop Enumerating";
            }
            else
            {
                bluetoothLEHelper.StopEnumeration();
                BtEnumeration.Content = "Start Enumerating";
            }
        }

        private async void LVDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TbDeviceName.Text = "No device selected";
            TbDeviceBtAddr.Text = "No device selected";

            CBServices.Visibility = Visibility.Collapsed;
            CBServices.ItemsSource = null;

            if (e.AddedItems.Count > 0)
            {
                ObservableBluetoothLEDevice device = e.AddedItems[0] as ObservableBluetoothLEDevice;

                if (device != null)
                {
                    TbDeviceName.Text = "Device Name: " + device.Name;
                    TbDeviceBtAddr.Text = "Device Address: " + device.BluetoothAddressAsString;

                    // Make sure the Bluetooth capability is set else this will fail
                    bluetoothLEHelper.StopEnumeration();
                    await device.ConnectAsync();
                    CBServices.ItemsSource = device.Services;
                    CBServices.Visibility = Visibility.Visible;
                }
            }
        }

        private void CBServices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CBCharacteristic.ItemsSource = null;
            CBCharacteristic.Visibility = Visibility.Collapsed;

            if (e.AddedItems.Count > 0)
            {
                ObservableGattDeviceService service = e.AddedItems[0] as ObservableGattDeviceService;

                if (service != null)
                {
                    CBCharacteristic.ItemsSource = service.Characteristics;
                    CBCharacteristic.Visibility = Visibility.Visible;
                }
            }
        }

        private void CBCharacteristic_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                ObservableGattCharacteristics characteristic = e.AddedItems[0] as ObservableGattCharacteristics;

                if (characteristic.Characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read))
                {
                    BtReadCharValue.Visibility = Visibility.Visible;
                    TBCharValue.Text = string.Empty;
                }
                else
                {
                    TBCharValue.Text = "This characteristic can not be read because the read property is not set";
                }
            }
            else
            {
                BtReadCharValue.Visibility = Visibility.Collapsed;
                TBCharValue.Text = string.Empty;
            }
        }

        private async void ReadCharValue_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            button.IsEnabled = false;

            ObservableGattCharacteristics characteristic = CBCharacteristic.SelectedItem as ObservableGattCharacteristics;

            if (characteristic != null)
            {
                TBCharValue.Text = await characteristic.ReadValueAsync();
            }

            button.IsEnabled = true;
        }
    }
}