// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Connectivity
{
    /// <summary>
    /// This enum assists in finding a string representation of a BT SIG assigned value for UUIDS
    /// Reference: https://developer.bluetooth.org/gatt/services/Pages/ServicesHome.aspx
    /// Reference: https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicsHome.aspx
    /// Reference: https://developer.bluetooth.org/gatt/descriptors/Pages/DescriptorsHomePage.aspx
    /// </summary>
    public enum GattNativeUuid : ushort
    {
        /// <summary>
        /// None.
        /// </summary>
        None = 0,

        /// <summary>
        /// The alert notification service.
        /// </summary>
        AlertNotificationService = 0x1811,

        /// <summary>
        /// The automation IO.
        /// </summary>
        AutomationIO = 0x1815,

        /// <summary>
        /// The battery service.
        /// </summary>
        BatteryService = 0x180F,

        /// <summary>
        /// The blood pressure.
        /// </summary>
        BloodPressure = 0x1810,

        /// <summary>
        /// The body composition.
        /// </summary>
        BodyComposition = 0x181B,

        /// <summary>
        /// The bond management.
        /// </summary>
        BondManagement = 0x181E,

        /// <summary>
        /// The continuous glucose monitoring.
        /// </summary>
        ContinuousGlucoseMonitoring = 0x181F,

        /// <summary>
        /// The current time service.
        /// </summary>
        CurrentTimeService = 0x1805,

        /// <summary>
        /// The cycling power.
        /// </summary>
        CyclingPower = 0x1818,

        /// <summary>
        /// The cycling speed and cadence.
        /// </summary>
        CyclingSpeedAndCadence = 0x1816,

        /// <summary>
        /// The device information.
        /// </summary>
        DeviceInformation = 0x180A,

        /// <summary>
        /// The environmental sensing.
        /// </summary>
        EnvironmentalSensing = 0x181A,

        /// <summary>
        /// The generic access.
        /// </summary>
        GenericAccess = 0x1800,

        /// <summary>
        /// The generic attribute.
        /// </summary>
        GenericAttribute = 0x1801,

        /// <summary>
        /// The glucose.
        /// </summary>
        Glucose = 0x1808,

        /// <summary>
        /// The health thermometer.
        /// </summary>
        HealthThermometer = 0x1809,

        /// <summary>
        /// The heart rate.
        /// </summary>
        HeartRate = 0x180D,

        /// <summary>
        /// The Http proxy.
        /// </summary>
        HttpProxy = 0x1823,

        /// <summary>
        /// The human interface device.
        /// </summary>
        HumanInterfaceDevice = 0x1812,

        /// <summary>
        /// The immediate alert.
        /// </summary>
        ImmediateAlert = 0x1802,

        /// <summary>
        /// The indoor positioning.
        /// </summary>
        IndoorPositioning = 0x1821,

        /// <summary>
        /// The internet protocol support.
        /// </summary>
        InternetProtocolSupport = 0x1820,

        /// <summary>
        /// The link loss.
        /// </summary>
        LinkLoss = 0x1803,

        /// <summary>
        /// The location and navigation.
        /// </summary>
        LocationAndNavigation = 0x1819,

        /// <summary>
        /// The next DST change service.
        /// </summary>
        NextDSTChangeService = 0x1807,

        /// <summary>
        /// The object transfer.
        /// </summary>
        ObjectTransfer = 0x1825,

        /// <summary>
        /// The phone alert status service.
        /// </summary>
        PhoneAlertStatusService = 0x180E,

        /// <summary>
        /// The pulse oximeter.
        /// </summary>
        PulseOximeter = 0x1822,

        /// <summary>
        /// The reference time update service.
        /// </summary>
        ReferenceTimeUpdateService = 0x1806,

        /// <summary>
        /// The running speed and cadence.
        /// </summary>
        RunningSpeedAndCadence = 0x1814,

        /// <summary>
        /// The scan parameters.
        /// </summary>
        ScanParameters = 0x1813,

        /// <summary>
        /// The transport discovery.
        /// </summary>
        TransportDiscovery = 0x1824,

        /// <summary>
        /// The tx power.
        /// </summary>
        TxPower = 0x1804,

        /// <summary>
        /// The user data.
        /// </summary>
        UserData = 0x181C,

        /// <summary>
        /// The weight scale.
        /// </summary>
        WeightScale = 0x181D,

        /// <summary>
        /// The alert category identifier.
        /// </summary>
        AlertCategoryID = 0x2A43,

        /// <summary>
        /// The alert category identifier bit mask.
        /// </summary>
        AlertCategoryIDBitMask = 0x2A42,

        /// <summary>
        /// The alert level.
        /// </summary>
        AlertLevel = 0x2A06,

        /// <summary>
        /// The alert notification control point.
        /// </summary>
        AlertNotificationControlPoint = 0x2A44,

        /// <summary>
        /// The alert status.
        /// </summary>
        AlertStatus = 0x2A3F,

        /// <summary>
        /// The appearance.
        /// </summary>
        Appearance = 0x2A01,

        /// <summary>
        /// The battery level.
        /// </summary>
        BatteryLevel = 0x2A19,

        /// <summary>
        /// The blood pressure feature.
        /// </summary>
        BloodPressureFeature = 0x2A49,

        /// <summary>
        /// The blood pressure measurement.
        /// </summary>
        BloodPressureMeasurement = 0x2A35,

        /// <summary>
        /// The body sensor location.
        /// </summary>
        BodySensorLocation = 0x2A38,

        /// <summary>
        /// The boot keyboard input report.
        /// </summary>
        BootKeyboardInputReport = 0x2A22,

        /// <summary>
        /// The boot keyboard output report.
        /// </summary>
        BootKeyboardOutputReport = 0x2A32,

        /// <summary>
        /// The boot mouse input report.
        /// </summary>
        BootMouseInputReport = 0x2A33,

        /// <summary>
        /// The CSC feature.
        /// </summary>
        CSCFeature = 0x2A5C,

        /// <summary>
        /// The CSC measurement.
        /// </summary>
        CSCMeasurement = 0x2A5B,

        /// <summary>
        /// The current time.
        /// </summary>
        CurrentTime = 0x2A2B,

        /// <summary>
        /// The date time.
        /// </summary>
        DateTime = 0x2A08,

        /// <summary>
        /// The day date time.
        /// </summary>
        DayDateTime = 0x2A0A,

        /// <summary>
        /// The day of week.
        /// </summary>
        DayOfWeek = 0x2A09,

        /// <summary>
        /// The device name.
        /// </summary>
        DeviceName = 0x2A00,

        /// <summary>
        /// The DST offset.
        /// </summary>
        DSTOffset = 0x2A0D,

        /// <summary>
        /// The exact time 256.
        /// </summary>
        ExactTime256 = 0x2A0C,

        /// <summary>
        /// The firmware revision string
        /// </summary>
        FirmwareRevisionString = 0x2A26,

        /// <summary>
        /// The glucose feature
        /// </summary>
        GlucoseFeature = 0x2A51,

        /// <summary>
        /// The glucose measurement
        /// </summary>
        GlucoseMeasurement = 0x2A18,

        /// <summary>
        /// The glucose measurement context
        /// </summary>
        GlucoseMeasurementContext = 0x2A34,

        /// <summary>
        /// The hardware revision string
        /// </summary>
        HardwareRevisionString = 0x2A27,

        /// <summary>
        /// The heart rate control point
        /// </summary>
        HeartRateControlPoint = 0x2A39,

        /// <summary>
        /// The heart rate measurement
        /// </summary>
        HeartRateMeasurement = 0x2A37,

        /// <summary>
        /// The hid control point
        /// </summary>
        HIDControlPoint = 0x2A4C,

        /// <summary>
        /// The hid information
        /// </summary>
        HIDInformation = 0x2A4A,

        /// <summary>
        /// The iee e11073 20601 regulatory certification data list
        /// </summary>
        IEEE11073_20601RegulatoryCertificationDataList = 0x2A2A,

        /// <summary>
        /// The intermediate cuff pressure
        /// </summary>
        IntermediateCuffPressure = 0x2A36,

        /// <summary>
        /// The intermediate temperature
        /// </summary>
        IntermediateTemperature = 0x2A1E,

        /// <summary>
        /// The local time information
        /// </summary>
        LocalTimeInformation = 0x2A0F,

        /// <summary>
        /// The manufacturer name string
        /// </summary>
        ManufacturerNameString = 0x2A29,

        /// <summary>
        /// The measurement interval
        /// </summary>
        MeasurementInterval = 0x2A21,

        /// <summary>
        /// The model number string
        /// </summary>
        ModelNumberString = 0x2A24,

        /// <summary>
        /// The new alert
        /// </summary>
        NewAlert = 0x2A46,

        /// <summary>
        /// The peripheral preferred connection parameters
        /// </summary>
        PeripheralPreferredConnectionParameters = 0x2A04,

        /// <summary>
        /// The peripheral privacy flag
        /// </summary>
        PeripheralPrivacyFlag = 0x2A02,

        /// <summary>
        /// The pn pid
        /// </summary>
        PnPID = 0x2A50,

        /// <summary>
        /// The protocol mode
        /// </summary>
        ProtocolMode = 0x2A4E,

        /// <summary>
        /// The reconnection address
        /// </summary>
        ReconnectionAddress = 0x2A03,

        /// <summary>
        /// The record access control point
        /// </summary>
        RecordAccessControlPoint = 0x2A52,

        /// <summary>
        /// The reference time information
        /// </summary>
        ReferenceTimeInformation = 0x2A14,

        /// <summary>
        /// The report
        /// </summary>
        Report = 0x2A4D,

        /// <summary>
        /// The report map
        /// </summary>
        ReportMap = 0x2A4B,

        /// <summary>
        /// The ringer control point
        /// </summary>
        RingerControlPoint = 0x2A40,

        /// <summary>
        /// The ringer setting
        /// </summary>
        RingerSetting = 0x2A41,

        /// <summary>
        /// The RSC feature
        /// </summary>
        RSCFeature = 0x2A54,

        /// <summary>
        /// The RSC measurement
        /// </summary>
        RSCMeasurement = 0x2A53,

        /// <summary>
        /// The sc control point
        /// </summary>
        SCControlPoint = 0x2A55,

        /// <summary>
        /// The scan interval window
        /// </summary>
        ScanIntervalWindow = 0x2A4F,

        /// <summary>
        /// The scan refresh
        /// </summary>
        ScanRefresh = 0x2A31,

        /// <summary>
        /// The sensor location
        /// </summary>
        SensorLocation = 0x2A5D,

        /// <summary>
        /// The serial number string
        /// </summary>
        SerialNumberString = 0x2A25,

        /// <summary>
        /// The service changed
        /// </summary>
        ServiceChanged = 0x2A05,

        /// <summary>
        /// The software revision string
        /// </summary>
        SoftwareRevisionString = 0x2A28,

        /// <summary>
        /// The supported new alert category
        /// </summary>
        SupportedNewAlertCategory = 0x2A47,

        /// <summary>
        /// The supported unread alert category
        /// </summary>
        SupportedUnreadAlertCategory = 0x2A48,

        /// <summary>
        /// The system identifier
        /// </summary>
        SystemID = 0x2A23,

        /// <summary>
        /// The temperature measurement
        /// </summary>
        TemperatureMeasurement = 0x2A1C,

        /// <summary>
        /// The temperature type
        /// </summary>
        TemperatureType = 0x2A1D,

        /// <summary>
        /// The time accuracy
        /// </summary>
        TimeAccuracy = 0x2A12,

        /// <summary>
        /// The time source
        /// </summary>
        TimeSource = 0x2A13,

        /// <summary>
        /// The time update control point
        /// </summary>
        TimeUpdateControlPoint = 0x2A16,

        /// <summary>
        /// The time update state
        /// </summary>
        TimeUpdateState = 0x2A17,

        /// <summary>
        /// The time with DST
        /// </summary>
        TimeWithDST = 0x2A11,

        /// <summary>
        /// The time zone
        /// </summary>
        TimeZone = 0x2A0E,

        /// <summary>
        /// The tx power level
        /// </summary>
        TxPowerLevel = 0x2A07,

        /// <summary>
        /// The unread alert status
        /// </summary>
        UnreadAlertStatus = 0x2A45,

        /// <summary>
        /// The aggregate input
        /// </summary>
        AggregateInput = 0x2A5A,

        /// <summary>
        /// The analog input
        /// </summary>
        AnalogInput = 0x2A58,

        /// <summary>
        /// The analog output
        /// </summary>
        AnalogOutput = 0x2A59,

        /// <summary>
        /// The cycling power control point
        /// </summary>
        CyclingPowerControlPoint = 0x2A66,

        /// <summary>
        /// The cycling power feature
        /// </summary>
        CyclingPowerFeature = 0x2A65,

        /// <summary>
        /// The cycling power measurement
        /// </summary>
        CyclingPowerMeasurement = 0x2A63,

        /// <summary>
        /// The cycling power vector
        /// </summary>
        CyclingPowerVector = 0x2A64,

        /// <summary>
        /// The digital input
        /// </summary>
        DigitalInput = 0x2A56,

        /// <summary>
        /// The digital output
        /// </summary>
        DigitalOutput = 0x2A57,

        /// <summary>
        /// The exact time100
        /// </summary>
        ExactTime100 = 0x2A0B,

        /// <summary>
        /// The ln control point
        /// </summary>
        LNControlPoint = 0x2A6B,

        /// <summary>
        /// The ln feature
        /// </summary>
        LNFeature = 0x2A6A,

        /// <summary>
        /// The location and speed
        /// </summary>
        LocationAndSpeed = 0x2A67,

        /// <summary>
        /// The navigation
        /// </summary>
        Navigation = 0x2A68,

        /// <summary>
        /// The network availability
        /// </summary>
        NetworkAvailability = 0x2A3E,

        /// <summary>
        /// The position quality
        /// </summary>
        PositionQuality = 0x2A69,

        /// <summary>
        /// The scientific temperature in Celsius
        /// </summary>
        ScientificTemperatureInCelsius = 0x2A3C,

        /// <summary>
        /// The secondary time zone
        /// </summary>
        SecondaryTimeZone = 0x2A10,

        /// <summary>
        /// The string
        /// </summary>
        String = 0x2A3D,

        /// <summary>
        /// The temperature in Celsius
        /// </summary>
        TemperatureInCelsius = 0x2A1F,

        /// <summary>
        /// The temperature in Fahrenheit
        /// </summary>
        TemperatureInFahrenheit = 0x2A20,

        /// <summary>
        /// The time broadcast
        /// </summary>
        TimeBroadcast = 0x2A15,

        /// <summary>
        /// The battery level state
        /// </summary>
        BatteryLevelState = 0x2A1B,

        /// <summary>
        /// The battery power state
        /// </summary>
        BatteryPowerState = 0x2A1A,

        /// <summary>
        /// The pulse oximetry continuous measurement
        /// </summary>
        PulseOximetryContinuousMeasurement = 0x2A5F,

        /// <summary>
        /// The pulse oximetry control point
        /// </summary>
        PulseOximetryControlPoint = 0x2A62,

        /// <summary>
        /// The pulse oximetry features
        /// </summary>
        PulseOximetryFeatures = 0x2A61,

        /// <summary>
        /// The pulse oximetry pulsatile event
        /// </summary>
        PulseOximetryPulsatileEvent = 0x2A60,

        /// <summary>
        /// The simple key state
        /// </summary>
        SimpleKeyState = 0xFFE1,

        /// <summary>
        /// The characteristic extended properties
        /// </summary>
        CharacteristicExtendedProperties = 0x2900,

        /// <summary>
        /// The characteristic user description
        /// </summary>
        CharacteristicUserDescription = 0x2901,

        /// <summary>
        /// The client characteristic configuration
        /// </summary>
        ClientCharacteristicConfiguration = 0x2902,

        /// <summary>
        /// The server characteristic configuration
        /// </summary>
        ServerCharacteristicConfiguration = 0x2903,

        /// <summary>
        /// The characteristic presentation format
        /// </summary>
        CharacteristicPresentationFormat = 0x2904,

        /// <summary>
        /// The characteristic aggregate format
        /// </summary>
        CharacteristicAggregateFormat = 0x2905,

        /// <summary>
        /// The valid range
        /// </summary>
        ValidRange = 0x2906,

        /// <summary>
        /// The external report reference
        /// </summary>
        ExternalReportReference = 0x2907,

        /// <summary>
        /// The report reference
        /// </summary>
        ReportReference = 0x2908
    }
}