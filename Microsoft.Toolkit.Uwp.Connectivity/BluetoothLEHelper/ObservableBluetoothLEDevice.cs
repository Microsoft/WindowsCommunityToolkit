// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.System;
using Windows.UI.Xaml.Media.Imaging;

namespace Microsoft.Toolkit.Uwp.Connectivity
{
    /// <summary>
    /// Wrapper around <see cref="BluetoothLEDevice" /> to make it bindable.
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    /// <seealso cref="System.IEquatable{ObservableBluetoothLEDevice}" />
    public class ObservableBluetoothLEDevice : INotifyPropertyChanged, IEquatable<ObservableBluetoothLEDevice>
    {
        /// <summary>
        /// Compares RSSI values between ObservableBluetoothLEDevice. Sorts based on closest to furthest where 0 is unknown
        /// and is sorted as furthest away
        /// </summary>
        public class RSSIComparer : IComparer
        {
            /// <summary>
            /// Compares two <see cref="ObservableBluetoothLEDevice"/> and returns a value indicating
            /// whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <param name="x">First object to compare</param>
            /// <param name="y">Second object to compare</param>
            /// <returns>Returns 0 if equal</returns>
            public int Compare(object x, object y)
            {
                ObservableBluetoothLEDevice a = x as ObservableBluetoothLEDevice;
                ObservableBluetoothLEDevice b = y as ObservableBluetoothLEDevice;

                if (a == null || b == null)
                {
                    throw new InvalidOperationException("Compared objects are not ObservableBluetoothLEDevice");
                }

                // If they're equal
                if (a.RSSI == b.RSSI)
                {
                    return 0;
                }

                // RSSI == 0 means we don't know it. Always make that the end.
                if (b.RSSI == 0)
                {
                    return -1;
                }

                if (a.RSSI < b.RSSI || a.RSSI == 0)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Source for <see cref="BluetoothLEDevice" />
        /// </summary>
        private BluetoothLEDevice _bluetoothLeDevice;

        /// <summary>
        /// Source for <see cref="DeviceInfo" />
        /// </summary>
        private DeviceInformation _deviceInfo;

        /// <summary>
        /// Source for <see cref="ErrorText" />
        /// </summary>
        private string _errorText;

        /// <summary>
        /// Source for <see cref="Glyph" />
        /// </summary>
        private BitmapImage _glyph;

        /// <summary>
        /// Source for <see cref="IsConnected" />
        /// </summary>
        private bool _isConnected;

        /// <summary>
        /// Source for <see cref="IsPaired" />
        /// </summary>
        private bool _isPaired;

        /// <summary>
        /// Source for <see cref="Name" />
        /// </summary>
        private string _name;

        /// <summary>
        /// result of finding all the services
        /// </summary>
        private GattDeviceServicesResult _result;

        /// <summary>
        /// Queue to store the last 10 observed RSSI values
        /// </summary>
        private Queue<int> _rssiValue = new Queue<int>(10);

        /// <summary>
        /// Source for <see cref="RSSI"/>
        /// </summary>
        private int _rssi;

        /// <summary>
        /// Source for <see cref="ServiceCount" />
        /// </summary>
        private int _serviceCount;

        /// <summary>
        /// Source for <see cref="Services" />
        /// </summary>
        private ObservableCollection<ObservableGattDeviceService> _services =
            new ObservableCollection<ObservableGattDeviceService>();

        /// <summary>
        /// Gets or sets which DispatcherQueue is used to dispatch UI updates.
        /// </summary>
        public DispatcherQueue DispatcherQueue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableBluetoothLEDevice"/> class.
        /// </summary>
        /// <param name="deviceInfo">The device information.</param>
        /// <param name="dispatcherQueue">The DispatcherQueue that should be used to dispatch UI updates for this BluetoothLE Device, or null if this is being called from the UI thread.</param>
        public ObservableBluetoothLEDevice(DeviceInformation deviceInfo, DispatcherQueue dispatcherQueue = null)
        {
            DeviceInfo = deviceInfo;
            Name = DeviceInfo.Name;

            IsPaired = DeviceInfo.Pairing.IsPaired;

            DispatcherQueue = dispatcherQueue ?? DispatcherQueue.GetForCurrentThread();

            LoadGlyph();

            this.PropertyChanged += ObservableBluetoothLEDevice_PropertyChanged;
        }

        /// <summary>
        /// Gets the bluetooth device this class wraps
        /// </summary>
        /// <value>The bluetooth le device.</value>
        public BluetoothLEDevice BluetoothLEDevice
        {
            get
            {
                return _bluetoothLeDevice;
            }

            private set
            {
                _bluetoothLeDevice = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the glyph of this bluetooth device
        /// </summary>
        /// <value>The glyph.</value>
        public BitmapImage Glyph
        {
            get
            {
                return _glyph;
            }

            set
            {
                _glyph = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the device information for the device this class wraps
        /// </summary>
        /// <value>The device information.</value>
        public DeviceInformation DeviceInfo
        {
            get
            {
                return _deviceInfo;
            }

            private set
            {
                _deviceInfo = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this device is connected
        /// </summary>
        /// <value><c>true</c> if this instance is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }

            private set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this device is paired
        /// </summary>
        /// <value><c>true</c> if this instance is paired; otherwise, <c>false</c>.</value>
        public bool IsPaired
        {
            get
            {
                return _isPaired;
            }

            private set
            {
                if (_isPaired != value)
                {
                    _isPaired = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the services this device supports
        /// </summary>
        /// <value>The services.</value>
        public ObservableCollection<ObservableGattDeviceService> Services
        {
            get
            {
                return _services;
            }

            private set
            {
                _services = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the number of services this device has
        /// </summary>
        /// <value>The service count.</value>
        public int ServiceCount
        {
            get
            {
                return _serviceCount;
            }

            set
            {
                if (_serviceCount != value)
                {
                    _serviceCount = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the name of this device
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return _name;
            }

            private set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the error text when connecting to this device fails
        /// </summary>
        /// <value>The error text.</value>
        public string ErrorText
        {
            get
            {
                return _errorText;
            }

            private set
            {
                _errorText = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the RSSI value of this device
        /// </summary>
        public int RSSI
        {
            get
            {
                return _rssi;
            }

            private set
            {
                if (_rssiValue.Count >= 10)
                {
                    _rssiValue.Dequeue();
                }

                _rssiValue.Enqueue(value);

                int newValue = (int)Math.Round(_rssiValue.Average(), 0);

                if (_rssi != newValue)
                {
                    _rssi = newValue;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the bluetooth address of this device as a string
        /// </summary>
        /// <value>The bluetooth address as string.</value>
        public string BluetoothAddressAsString => DeviceInfo.Properties["System.Devices.Aep.DeviceAddress"]?.ToString();

        /// <summary>
        /// Gets the bluetooth address of this device
        /// </summary>
        /// <value>The bluetooth address as ulong.</value>
        public ulong BluetoothAddressAsUlong => Convert.ToUInt64(
            BluetoothAddressAsString.Replace(":", string.Empty),
            16);

        /// <summary>
        /// Compares this device to other bluetooth devices by checking the id
        /// </summary>
        /// <param name="other">The device to compare with.</param>
        /// <returns>true for equal</returns>
        bool IEquatable<ObservableBluetoothLEDevice>.Equals(ObservableBluetoothLEDevice other)
        {
            return other?.DeviceInfo.Id != null && DeviceInfo.Id == other.DeviceInfo.Id;
        }

        private void ObservableBluetoothLEDevice_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "DeviceInfo")
            {
                if (DeviceInfo.Properties.ContainsKey("System.Devices.Aep.SignalStrength") && DeviceInfo.Properties["System.Devices.Aep.SignalStrength"] != null)
                {
                    RSSI = (int)DeviceInfo.Properties["System.Devices.Aep.SignalStrength"];
                }
            }
        }

        /// <summary>
        /// ConnectAsync to this bluetooth device
        /// </summary>
        /// <returns>Connection task</returns>
        /// <exception cref="Exception">Throws Exception when no permission to access device</exception>
        public async Task ConnectAsync()
        {
            await DispatcherQueue.EnqueueAsync(
                async () =>
            {
                if (BluetoothLEDevice == null)
                {
                    BluetoothLEDevice = await BluetoothLEDevice.FromIdAsync(DeviceInfo.Id);

                    if (BluetoothLEDevice == null)
                    {
                        throw new Exception("Connection error, no permission to access device");
                    }
                }

                BluetoothLEDevice.ConnectionStatusChanged += BluetoothLEDevice_ConnectionStatusChanged;
                BluetoothLEDevice.NameChanged += BluetoothLEDevice_NameChanged;

                IsPaired = DeviceInfo.Pairing.IsPaired;
                IsConnected = BluetoothLEDevice.ConnectionStatus == BluetoothConnectionStatus.Connected;
                Name = BluetoothLEDevice.Name;

                // Get all the services for this device
                var getGattServicesAsyncTokenSource = new CancellationTokenSource(5000);
                var getGattServicesAsyncTask = await
                    Task.Run(
                        () => BluetoothLEDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached),
                        getGattServicesAsyncTokenSource.Token);

                _result = await getGattServicesAsyncTask;

                if (_result.Status == GattCommunicationStatus.Success)
                {
                    // In case we connected before, clear the service list and recreate it
                    Services.Clear();

                    foreach (var service in _result.Services)
                    {
                        Services.Add(new ObservableGattDeviceService(service));
                    }

                    ServiceCount = Services.Count;
                }
                else
                {
                    if (_result.ProtocolError != null)
                    {
                        throw new Exception(_result.ProtocolError.GetErrorString());
                    }
                }
            }, DispatcherQueuePriority.Normal);
        }

        /// <summary>
        /// Does the in application pairing
        /// </summary>
        /// <returns>Task.</returns>
        /// <exception cref="Exception">The status of the pairing.</exception>
        public async Task DoInAppPairingAsync()
        {
            var result = await DeviceInfo.Pairing.PairAsync();

            if (result.Status != DevicePairingResultStatus.Paired ||
                result.Status != DevicePairingResultStatus.AlreadyPaired)
            {
                throw new Exception(result.Status.ToString());
            }
        }

        /// <summary>
        /// Updates this device's deviceInformation
        /// </summary>
        /// <param name="deviceUpdate">The device information which has been updated.</param>
        /// <returns>The task of the update.</returns>
        public async Task UpdateAsync(DeviceInformationUpdate deviceUpdate)
        {
            await DispatcherQueue.EnqueueAsync(
                () =>
                {
                    DeviceInfo.Update(deviceUpdate);
                    Name = DeviceInfo.Name;

                    IsPaired = DeviceInfo.Pairing.IsPaired;

                    LoadGlyph();
                    OnPropertyChanged("DeviceInfo");
                }, DispatcherQueuePriority.Normal);
        }

        /// <summary>
        /// Overrides the ToString function to return the name of the device
        /// </summary>
        /// <returns>Name of this characteristic</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Event to notify when this object has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Property changed event invoker
        /// </summary>
        /// <param name="propertyName">name of the property that changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Executes when the name of this devices changes
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        private async void BluetoothLEDevice_NameChanged(BluetoothLEDevice sender, object args)
        {
            await DispatcherQueue.EnqueueAsync(() => { Name = BluetoothLEDevice.Name; }, DispatcherQueuePriority.Normal);
        }

        /// <summary>
        /// Executes when the connection state changes
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The arguments.</param>
        private async void BluetoothLEDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
        {
            await DispatcherQueue.EnqueueAsync(
                () =>
                {
                    IsPaired = DeviceInfo.Pairing.IsPaired;
                    IsConnected = BluetoothLEDevice.ConnectionStatus == BluetoothConnectionStatus.Connected;
                }, DispatcherQueuePriority.Normal);
        }

        /// <summary>
        /// Load the glyph for this device
        /// </summary>
        private async void LoadGlyph()
        {
            await DispatcherQueue.EnqueueAsync(
                async () =>
                {
                    var deviceThumbnail = await DeviceInfo.GetGlyphThumbnailAsync();
                    var glyphBitmapImage = new BitmapImage();
                    await glyphBitmapImage.SetSourceAsync(deviceThumbnail);
                    Glyph = glyphBitmapImage;
                }, DispatcherQueuePriority.Normal);
        }
    }
}
