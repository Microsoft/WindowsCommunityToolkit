// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Enumeration;
using Windows.System;

namespace Microsoft.Toolkit.Uwp.Connectivity
{
    /// <summary>
    /// Context for the entire app. This is where all app wide variables are stored
    /// </summary>
    public class BluetoothLEHelper
    {
        /// <summary>
        /// AQS search string used to find bluetooth devices.
        /// </summary>
        private const string BluetoothLeDeviceWatcherAqs = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";

        /// <summary>
        /// We need to cache all DeviceInformation objects we get as they may
        /// get updated in the future. The update may make them eligible to be put on
        /// the displayed list.
        /// </summary>
        private readonly List<DeviceInformation> _unusedDevices = new List<DeviceInformation>();

        /// <summary>
        /// Reader/Writer lock for when we are updating the collection.
        /// </summary>
        private readonly ReaderWriterLockSlim _readerWriterLockSlim = new ReaderWriterLockSlim();

        /// <summary>
        /// Advertisement watcher used to find bluetooth devices.
        /// </summary>
        private BluetoothLEAdvertisementWatcher _advertisementWatcher;

        /// <summary>
        /// Device watcher used to find bluetooth devices
        /// </summary>
        private DeviceWatcher _deviceWatcher;

        /// <summary>
        /// The Bluetooth adapter.
        /// </summary>
        private BluetoothAdapter _adapter;

        /// <summary>
        /// Gets or sets which DispatcherQueue is used to dispatch UI updates.
        /// </summary>
        public DispatcherQueue DispatcherQueue { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="BluetoothLEHelper" /> class from being created.
        /// </summary>
        /// <param name="dispatcherQueue">The DispatcherQueue that should be used to dispatch UI updates, or null if this is being called from the UI thread.</param>
        private BluetoothLEHelper(DispatcherQueue dispatcherQueue = null)
        {
            DispatcherQueue = dispatcherQueue ?? DispatcherQueue.GetForCurrentThread();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Init();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }

        /// <summary>
        /// Gets the app context
        /// </summary>
        public static BluetoothLEHelper Context { get; } = new BluetoothLEHelper();

        /// <summary>
        /// Gets the list of available bluetooth devices
        /// </summary>
        public ObservableCollection<ObservableBluetoothLEDevice> BluetoothLeDevices { get; } = new ObservableCollection<ObservableBluetoothLEDevice>();

        /// <summary>
        /// Gets a value indicating whether app is currently enumerating
        /// </summary>
        public bool IsEnumerating
        {
            get
            {
                if (_advertisementWatcher == null)
                {
                    _advertisementWatcher = new BluetoothLEAdvertisementWatcher();
                }

                return _advertisementWatcher.Status == BluetoothLEAdvertisementWatcherStatus.Started;
            }
        }

        /// <summary>
        /// Gets a value indicating whether peripheral mode is supported by this device
        /// </summary>
        public bool IsPeripheralRoleSupported => _adapter.IsPeripheralRoleSupported;

        /// <summary>
        /// Gets a value indicating whether central role is supported by this device
        /// </summary>
        public bool IsCentralRoleSupported => _adapter.IsCentralRoleSupported;

        /// <summary>
        /// An event for when the enumeration is complete.
        /// </summary>
        public event EventHandler<EventArgs> EnumerationCompleted;

        /// <summary>
        /// Starts enumeration of bluetooth device
        /// </summary>
        public void StartEnumeration()
        {
            if (_advertisementWatcher?.Status == BluetoothLEAdvertisementWatcherStatus.Started || _deviceWatcher != null)
            {
                return;
            }

            // Additional properties we would like about the device.
            string[] requestedProperties =
            {
                "System.Devices.Aep.Category",
                "System.Devices.Aep.ContainerId",
                "System.Devices.Aep.DeviceAddress",
                "System.Devices.Aep.IsConnected",
                "System.Devices.Aep.IsPaired",
                "System.Devices.Aep.IsPresent",
                "System.Devices.Aep.ProtocolId",
                "System.Devices.Aep.Bluetooth.Le.IsConnectable",
                "System.Devices.Aep.SignalStrength"
            };

            // BT_Code: Currently Bluetooth APIs don't provide a selector to get ALL devices that are both paired and non-paired.
            _deviceWatcher = DeviceInformation.CreateWatcher(
                BluetoothLeDeviceWatcherAqs,
                requestedProperties,
                DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            _deviceWatcher.Added += DeviceWatcher_Added;
            _deviceWatcher.Updated += DeviceWatcher_Updated;
            _deviceWatcher.Removed += DeviceWatcher_Removed;
            _deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;

            _advertisementWatcher = new BluetoothLEAdvertisementWatcher();
            _advertisementWatcher.Received += AdvertisementWatcher_Received;

            BluetoothLeDevices.Clear();

            _deviceWatcher.Start();
            _advertisementWatcher.Start();
        }

        /// <summary>
        /// Stops enumeration of bluetooth device
        /// </summary>
        public void StopEnumeration()
        {
            if (_deviceWatcher != null)
            {
                _deviceWatcher.Added -= DeviceWatcher_Added;
                _deviceWatcher.Updated -= DeviceWatcher_Updated;
                _deviceWatcher.Removed -= DeviceWatcher_Removed;
                _deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;

                _deviceWatcher.Stop();
                _deviceWatcher = null;
            }

            if (_advertisementWatcher != null)
            {
                _advertisementWatcher.Received -= AdvertisementWatcher_Received;
                _advertisementWatcher.Stop();
                _advertisementWatcher = null;
            }
        }

        /// <summary>
        /// Initializes the app context.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task Init()
        {
            _adapter = await BluetoothAdapter.GetDefaultAsync();
        }

        /// <summary>
        /// Updates device metadata based on advertisement received
        /// </summary>
        /// <param name="sender">The Bluetooth LE Advertisement Watcher.</param>
        /// <param name="args">The advertisement.</param>
        private async void AdvertisementWatcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            await DispatcherQueue.EnqueueAsync(
                () =>
                {
                    if (_readerWriterLockSlim.TryEnterReadLock(TimeSpan.FromSeconds(1)))
                    {
                        foreach (var device in BluetoothLeDevices)
                        {
                            if (device.BluetoothAddressAsUlong == args.BluetoothAddress)
                            {
                                device.ServiceCount = args.Advertisement.ServiceUuids.Count();
                            }
                        }

                        _readerWriterLockSlim.ExitReadLock();
                    }
                }, DispatcherQueuePriority.Normal);
        }

        /// <summary>
        /// Callback when a new device is found
        /// </summary>
        /// <param name="sender">The device watcher.</param>
        /// <param name="deviceInfo">The update device information.</param>
        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            // Protect against race condition if the task runs after the app stopped the deviceWatcher.
            if (sender == _deviceWatcher)
            {
                await AddDeviceToList(deviceInfo);
            }
        }

        /// <summary>
        /// Executes when a device is updated
        /// </summary>
        /// <param name="sender">The device watcher.</param>
        /// <param name="deviceInfoUpdate">The update device information.</param>
        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            if (sender == _deviceWatcher)
            {
                ObservableBluetoothLEDevice device = null;

                device = BluetoothLeDevices.FirstOrDefault(i => i.DeviceInfo.Id == deviceInfoUpdate.Id);

                if (device != null)
                {
                    await device.UpdateAsync(deviceInfoUpdate);
                }

                if (device == null)
                {
                    if (_readerWriterLockSlim.TryEnterWriteLock(TimeSpan.FromSeconds(2)))
                    {
                        var unusedDevice = _unusedDevices.FirstOrDefault(i => i.Id == deviceInfoUpdate.Id);

                        if (unusedDevice != null)
                        {
                            _unusedDevices.Remove(unusedDevice);
                            unusedDevice.Update(deviceInfoUpdate);
                        }

                        _readerWriterLockSlim.ExitWriteLock();

                        // The update to the unknown device means we should move it to the Bluetooth LE Device collection.
                        if (unusedDevice != null)
                        {
                            await AddDeviceToList(unusedDevice);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Executes when a device is removed from enumeration
        /// </summary>
        /// <param name="sender">The device watcher.</param>
        /// <param name="deviceInfoUpdate">An update of the device.</param>
        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            // Protect against race condition if the task runs after the app stopped the deviceWatcher.
            if (sender == _deviceWatcher)
            {
                await DispatcherQueue.EnqueueAsync(
                    () =>
                    {
                        if (_readerWriterLockSlim.TryEnterWriteLock(TimeSpan.FromSeconds(1)))
                        {
                            var device = BluetoothLeDevices.FirstOrDefault(i => i.DeviceInfo.Id == deviceInfoUpdate.Id);
                            BluetoothLeDevices.Remove(device);

                            var unusedDevice = _unusedDevices.FirstOrDefault(i => i.Id == deviceInfoUpdate.Id);
                            _unusedDevices?.Remove(unusedDevice);

                            _readerWriterLockSlim.ExitWriteLock();
                        }
                    }, DispatcherQueuePriority.Normal);
            }
        }

        /// <summary>
        /// An event for when the enumeration is completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        {
            if (sender == _deviceWatcher)
            {
                StopEnumeration();

                EnumerationCompleted?.Invoke(sender, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Adds the new or updated device to the displayed or unused list
        /// </summary>
        /// <param name="deviceInfo">The device to add</param>
        /// <returns>The task being used to add a device to a list</returns>
        private async Task AddDeviceToList(DeviceInformation deviceInfo)
        {
            // Make sure device name isn't blank or already present in the list.
            if (!string.IsNullOrEmpty(deviceInfo?.Name))
            {
                var device = new ObservableBluetoothLEDevice(deviceInfo, DispatcherQueue);
                var connectable = (device.DeviceInfo.Properties.Keys.Contains("System.Devices.Aep.Bluetooth.Le.IsConnectable") &&
                                        (bool)device.DeviceInfo.Properties["System.Devices.Aep.Bluetooth.Le.IsConnectable"]) ||
                                        (device.DeviceInfo.Properties.Keys.Contains("System.Devices.Aep.IsConnected") &&
                                        (bool)device.DeviceInfo.Properties["System.Devices.Aep.IsConnected"]);

                if (connectable)
                {
                    await DispatcherQueue.EnqueueAsync(
                        () =>
                        {
                            if (_readerWriterLockSlim.TryEnterWriteLock(TimeSpan.FromSeconds(1)))
                            {
                                if (!BluetoothLeDevices.Contains(device))
                                {
                                    BluetoothLeDevices.Add(device);
                                }

                                _readerWriterLockSlim.ExitWriteLock();
                            }
                        }, DispatcherQueuePriority.Normal);

                    return;
                }
            }

            if (_readerWriterLockSlim.TryEnterWriteLock(TimeSpan.FromSeconds(1)))
            {
                _unusedDevices.Add(deviceInfo);
                _readerWriterLockSlim.ExitWriteLock();
            }
        }
    }
}