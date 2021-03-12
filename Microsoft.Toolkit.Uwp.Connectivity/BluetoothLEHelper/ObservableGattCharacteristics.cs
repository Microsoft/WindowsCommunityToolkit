// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.System;

namespace Microsoft.Toolkit.Uwp.Connectivity
{
    /// <summary>
    /// Wrapper around <see cref="GattCharacteristic"/>  to make it easier to use
    /// </summary>
    public class ObservableGattCharacteristics : INotifyPropertyChanged
    {
        /// <summary>
        /// Enum used to determine how the <see cref="Value" /> should be displayed
        /// </summary>
        public enum DisplayTypes
        {
            /// <summary>
            /// Not set
            /// </summary>
            NotSet,

            /// <summary>
            /// Bool
            /// </summary>
            Bool,

            /// <summary>
            /// Decimal
            /// </summary>
            Decimal,

            /// <summary>
            /// Hexadecimal
            /// </summary>
            Hex,

            /// <summary>
            /// UTF8
            /// </summary>
            Utf8,

            /// <summary>
            /// UTF16
            /// </summary>
            Utf16,

            /// <summary>
            /// Unsupported
            /// </summary>
            Unsupported
        }

        /// <summary>
        /// Source for <see cref="Characteristic"/>
        /// </summary>
        private GattCharacteristic _characteristic;

        /// <summary>
        /// A byte array representation of the characteristic value
        /// </summary>
        private byte[] _data;

        /// <summary>
        /// Source for <see cref="IsIndicateSet"/>
        /// </summary>
        private bool _isIndicateSet;

        /// <summary>
        /// Source for <see cref="IsNotifySet"/>
        /// </summary>
        private bool _isNotifySet;

        /// <summary>
        /// Raw buffer of this value of this characteristic
        /// </summary>
        private IBuffer _rawData;

        /// <summary>
        /// Source for <see cref="DisplayType"/>
        /// </summary>
        private DisplayTypes _displayType = DisplayTypes.NotSet;

        /// <summary>
        /// Source for <see cref="Name"/>
        /// </summary>
        private string _name;

        /// <summary>
        /// Source for <see cref="Parent"/>
        /// </summary>
        private ObservableGattDeviceService _parent;

        /// <summary>
        /// Source for <see cref="UUID"/>
        /// </summary>
        private string _uuid;

        /// <summary>
        /// Source for <see cref="Value"/>
        /// </summary>
        private string _value;

        /// <summary>
        /// Gets or sets which DispatcherQueue is used to dispatch UI updates.
        /// </summary>
        public DispatcherQueue DispatcherQueue { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableGattCharacteristics"/> class.
        /// </summary>
        /// <param name="characteristic">The characteristic.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="dispatcherQueue">The DispatcherQueue that should be used to dispatch UI updates, or null if this is being called from the UI thread.</param>
        public ObservableGattCharacteristics(GattCharacteristic characteristic, ObservableGattDeviceService parent, DispatcherQueue dispatcherQueue = null)
        {
            DispatcherQueue = dispatcherQueue ?? DispatcherQueue.GetForCurrentThread();

            Characteristic = characteristic;
            Parent = parent;
            Name = GattUuidsService.ConvertUuidToName(Characteristic.Uuid);
            UUID = Characteristic.Uuid.ToString();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            ReadValueAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            characteristic.ValueChanged += Characteristic_ValueChanged;
        }

        /// <summary>
        /// Gets or sets the characteristic this class wraps
        /// </summary>
        public GattCharacteristic Characteristic
        {
            get
            {
                return _characteristic;
            }

            set
            {
                if (_characteristic != value)
                {
                    _characteristic = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether indicate is set
        /// </summary>
        public bool IsIndicateSet
        {
            get
            {
                return _isIndicateSet;
            }

            private set
            {
                if (_isIndicateSet != value)
                {
                    _isIndicateSet = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether notify is set
        /// </summary>
        public bool IsNotifySet
        {
            get
            {
                return _isNotifySet;
            }

            private set
            {
                if (_isNotifySet != value)
                {
                    _isNotifySet = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent service of this characteristic
        /// </summary>
        public ObservableGattDeviceService Parent
        {
            get
            {
                return _parent;
            }

            set
            {
                if (_parent != value)
                {
                    _parent = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of this characteristic
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the UUID of this characteristic
        /// </summary>
        public string UUID
        {
            get
            {
                return _uuid;
            }

            set
            {
                if (_uuid != value)
                {
                    _uuid = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the value of this characteristic
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }

            private set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets how this characteristic's value should be displayed
        /// </summary>
        public DisplayTypes DisplayType
        {
            get
            {
                return _displayType;
            }

            set
            {
                if (_displayType != value)
                {
                    _displayType = value;
                    SetValue();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Event to notify when this object has changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Reads the value of the Characteristic
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task<string> ReadValueAsync()
        {
            var result = await Characteristic.ReadValueAsync(BluetoothCacheMode.Uncached);

            if (result.Status == GattCommunicationStatus.Success)
            {
                SetValue(result.Value);
            }
            else if (result.Status == GattCommunicationStatus.ProtocolError)
            {
                Value = result.ProtocolError.GetErrorString();
            }
            else
            {
                Value = "Unreachable";
            }

            return Value;
        }

        /// <summary>
        /// Set's the indicate descriptor
        /// </summary>
        /// <returns>Set indicate task</returns>
        public async Task<bool> SetIndicateAsync()
        {
            if (IsIndicateSet)
            {
                return IsIndicateSet;
            }

            var result = await
                Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.Indicate);

            if (result == GattCommunicationStatus.Success)
            {
                IsIndicateSet = true;
            }
            else if (result == GattCommunicationStatus.ProtocolError)
            {
                IsIndicateSet = false;
            }
            else if (result == GattCommunicationStatus.Unreachable)
            {
                IsIndicateSet = false;
            }

            return IsIndicateSet;
        }

        /// <summary>
        /// Unset the indicate descriptor
        /// </summary>
        /// <returns>Unset indicate task</returns>
        public async Task<bool> StopIndicateAsync()
        {
            if (IsIndicateSet == false)
            {
                return !IsIndicateSet;
            }

            var result = await
                Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.None);

            if (result == GattCommunicationStatus.Success)
            {
                IsIndicateSet = false;
            }
            else if (result == GattCommunicationStatus.ProtocolError)
            {
                IsIndicateSet = true;
            }
            else if (result == GattCommunicationStatus.Unreachable)
            {
                IsIndicateSet = true;
            }

            return !IsIndicateSet;
        }

        /// <summary>
        /// Sets the notify characteristic
        /// </summary>
        /// <returns>Set notify task</returns>
        public async Task<bool> SetNotifyAsync()
        {
            if (IsNotifySet)
            {
                return IsNotifySet;
            }

            var result = await
                Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.Notify);

            if (result == GattCommunicationStatus.Success)
            {
                IsNotifySet = true;
            }
            else if (result == GattCommunicationStatus.ProtocolError)
            {
                IsNotifySet = false;
            }
            else if (result == GattCommunicationStatus.Unreachable)
            {
                IsNotifySet = false;
            }

            return IsNotifySet;
        }

        /// <summary>
        /// Unsets the notify descriptor
        /// </summary>
        /// <returns>Unset notify task</returns>
        public async Task<bool> StopNotifyAsync()
        {
            if (IsNotifySet == false)
            {
                return IsNotifySet;
            }

            var result = await
                Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                    GattClientCharacteristicConfigurationDescriptorValue.None);

            if (result == GattCommunicationStatus.Success)
            {
                IsNotifySet = false;
            }
            else if (result == GattCommunicationStatus.ProtocolError)
            {
                IsNotifySet = true;
            }
            else if (result == GattCommunicationStatus.Unreachable)
            {
                IsNotifySet = true;
            }

            return !IsNotifySet;
        }

        /// <summary>
        /// Property changed event invoker
        /// </summary>
        /// <param name="propertyName">name of the property that changed</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// When the Characteristics value changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GattValueChangedEventArgs"/> instance containing the event data.</param>
        private async void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            await DispatcherQueue.EnqueueAsync(() => { SetValue(args.CharacteristicValue); }, DispatcherQueuePriority.Normal);
        }

        /// <summary>
        /// helper function that copies the raw data into byte array
        /// </summary>
        /// <param name="buffer">The raw input buffer</param>
        private void SetValue(IBuffer buffer)
        {
            _rawData = buffer;
            CryptographicBuffer.CopyToByteArray(_rawData, out _data);

            SetValue();
        }

        /// <summary>
        /// Sets the value of this characteristic based on the display type
        /// </summary>
        private void SetValue()
        {
            if (_data == null)
            {
                Value = "NULL";
                return;
            }

            GattPresentationFormat format = null;

            if (Characteristic.PresentationFormats.Any())
            {
                format = Characteristic.PresentationFormats[0];
            }

            // Determine what to set our DisplayType to
            if (format == null && DisplayType == DisplayTypes.NotSet)
            {
                if (Name == "DeviceName")
                {
                    // All devices have DeviceName so this is a special case.
                    DisplayType = DisplayTypes.Utf8;
                }
                else
                {
                    var isString = true;
                    var buffer = GattConvert.ToUTF8String(_rawData);

                    // if buffer is only 1 char or 2 char with 0 at end then let's assume it's hex
                    if (buffer.Length == 1)
                    {
                        isString = false;
                    }
                    else if (buffer.Length == 2 && buffer[1] == 0)
                    {
                        isString = false;
                    }
                    else
                    {
                        foreach (var b in buffer)
                        {
                            // if within the reasonable range of used characters and not null, let's assume it's a UTF8 string by default, else hex
                            if ((b < ' ' || b > '~') && b != 0)
                            {
                                isString = false;
                                break;
                            }
                        }
                    }

                    DisplayType = isString ? DisplayTypes.Utf8 : DisplayTypes.Hex;
                }
            }
            else if (format != null && DisplayType == DisplayTypes.NotSet)
            {
                if (format.FormatType == GattPresentationFormatTypes.Boolean ||
                    format.FormatType == GattPresentationFormatTypes.Bit2 ||
                    format.FormatType == GattPresentationFormatTypes.Nibble ||
                    format.FormatType == GattPresentationFormatTypes.UInt8 ||
                    format.FormatType == GattPresentationFormatTypes.UInt12 ||
                    format.FormatType == GattPresentationFormatTypes.UInt16 ||
                    format.FormatType == GattPresentationFormatTypes.UInt24 ||
                    format.FormatType == GattPresentationFormatTypes.UInt32 ||
                    format.FormatType == GattPresentationFormatTypes.UInt48 ||
                    format.FormatType == GattPresentationFormatTypes.UInt64 ||
                    format.FormatType == GattPresentationFormatTypes.SInt8 ||
                    format.FormatType == GattPresentationFormatTypes.SInt12 ||
                    format.FormatType == GattPresentationFormatTypes.SInt16 ||
                    format.FormatType == GattPresentationFormatTypes.SInt24 ||
                    format.FormatType == GattPresentationFormatTypes.SInt32)
                {
                    DisplayType = DisplayTypes.Decimal;
                }
                else if (format.FormatType == GattPresentationFormatTypes.Utf8)
                {
                    DisplayType = DisplayTypes.Utf8;
                }
                else if (format.FormatType == GattPresentationFormatTypes.Utf16)
                {
                    DisplayType = DisplayTypes.Utf16;
                }
                else if (format.FormatType == GattPresentationFormatTypes.UInt128 ||
                         format.FormatType == GattPresentationFormatTypes.SInt128 ||
                         format.FormatType == GattPresentationFormatTypes.DUInt16 ||
                         format.FormatType == GattPresentationFormatTypes.SInt64 ||
                         format.FormatType == GattPresentationFormatTypes.Struct ||
                         format.FormatType == GattPresentationFormatTypes.Float ||
                         format.FormatType == GattPresentationFormatTypes.Float32 ||
                         format.FormatType == GattPresentationFormatTypes.Float64)
                {
                    DisplayType = DisplayTypes.Unsupported;
                }
                else
                {
                    DisplayType = DisplayTypes.Unsupported;
                }
            }

            // Decode the value into the right display type
            if (DisplayType == DisplayTypes.Hex || DisplayType == DisplayTypes.Unsupported)
            {
                Value = GattConvert.ToHexString(_rawData);
            }
            else if (DisplayType == DisplayTypes.Decimal)
            {
                Value = GattConvert.ToInt32(_rawData).ToString();
            }
            else if (DisplayType == DisplayTypes.Utf8)
            {
                Value = GattConvert.ToUTF8String(_rawData);
            }
            else if (DisplayType == DisplayTypes.Utf16)
            {
                Value = GattConvert.ToUTF16String(_rawData);
            }
        }
    }
}