// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace CommunityToolkit.WinUI.Connectivity
{
    /// <summary>
    /// Wrapper around <see cref="GattDeviceService" /> to make it easier to use
    /// </summary>
    public class ObservableGattDeviceService : INotifyPropertyChanged
    {
        /// <summary>
        /// Source for <see cref="Characteristics" />
        /// </summary>
        private ObservableCollection<ObservableGattCharacteristics> _characteristics =
            new ObservableCollection<ObservableGattCharacteristics>();

        /// <summary>
        /// Source for <see cref="Name" />
        /// </summary>
        private string _name;

        /// <summary>
        /// Source for <see cref="Service" />
        /// </summary>
        private GattDeviceService _service;

        /// <summary>
        /// Source for <see cref="UUID" />
        /// </summary>
        private string _uuid;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableGattDeviceService" /> class.
        /// </summary>
        /// <param name="service">The service this class wraps</param>
        public ObservableGattDeviceService(GattDeviceService service)
        {
            Service = service;
            Name = GattUuidsService.ConvertUuidToName(service.Uuid);
            UUID = Service.Uuid.ToString();
            _ = PopulateAllCharacteristicsAsync();
        }

        /// <summary>
        /// Gets the service this class wraps
        /// </summary>
        public GattDeviceService Service
        {
            get
            {
                return _service;
            }

            private set
            {
                if (_service != value)
                {
                    _service = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets all the characteristics of this service
        /// </summary>
        /// <value>The characteristics.</value>
        public ObservableCollection<ObservableGattCharacteristics> Characteristics
        {
            get
            {
                return _characteristics;
            }

            private set
            {
                if (_characteristics != value)
                {
                    _characteristics = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the name of this service
        /// </summary>
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
        /// Gets the UUID of this service
        /// </summary>
        public string UUID
        {
            get
            {
                return _uuid;
            }

            private set
            {
                if (_uuid != value)
                {
                    _uuid = value;
                    OnPropertyChanged();
                }
            }
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
        /// Populate the characteristics in <see cref="Characteristics"/>.
        /// </summary>
        /// <returns>The status of the communication with the GATT device.</returns>
        private async Task<GattCommunicationStatus> PopulateAllCharacteristicsAsync()
        {
            var tokenSource = new CancellationTokenSource(5000);
            var getCharacteristicsTask = await Task.Run(
                () => Service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached),
                tokenSource.Token);

            GattCharacteristicsResult result = null;
            result = await getCharacteristicsTask;

            if (result.Status == GattCommunicationStatus.Success)
            {
                foreach (var gattCharacteristic in result.Characteristics)
                {
                    Characteristics.Add(new ObservableGattCharacteristics(gattCharacteristic, this));
                }
            }

            return result.Status;
        }
    }
}