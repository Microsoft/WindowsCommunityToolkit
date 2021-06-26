// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SystemInformationPage : Page
    {
        // To get application's name:
        public string ApplicationName => SystemInformation.Instance.ApplicationName;

        // To get application's version:
        public string ApplicationVersion => SystemInformation.Instance.ApplicationVersion.ToFormattedString();

        // To get the most preferred language by the user:
        public CultureInfo Culture => SystemInformation.Instance.Culture;

        // To get operating system
        public string OperatingSystem => SystemInformation.Instance.OperatingSystem;

        // To get used processor architecture
        public ProcessorArchitecture OperatingSystemArchitecture => SystemInformation.Instance.OperatingSystemArchitecture;

        // To get operating system version
        public OSVersion OperatingSystemVersion => SystemInformation.Instance.OperatingSystemVersion;

        // To get device family
        public string DeviceFamily => SystemInformation.Instance.DeviceFamily;

        // To get device model
        public string DeviceModel => SystemInformation.Instance.DeviceModel;

        // To get device manufacturer
        public string DeviceManufacturer => SystemInformation.Instance.DeviceManufacturer;

        // To get available memory in MB
        public float AvailableMemory => SystemInformation.Instance.AvailableMemory;

        // To get if the app is being used for the first time since it was installed.
        public string IsFirstUse => SystemInformation.Instance.IsFirstRun.ToString();

        // To get if the app is being used for the first time since being upgraded from an older version.
        public string IsAppUpdated => SystemInformation.Instance.IsAppUpdated.ToString();

        // To get the first version installed
        public string FirstVersionInstalled => SystemInformation.Instance.FirstVersionInstalled.ToFormattedString();

        // To get the previous version installed
        public string PreviousVersionInstalled => SystemInformation.Instance.PreviousVersionInstalled.ToFormattedString();

        // To get the first time the app was launched
        public string FirstUseTime => SystemInformation.Instance.FirstUseTime.ToString(Culture.DateTimeFormat);

        // To get the time the app was launched
        public string LaunchTime => SystemInformation.Instance.LaunchTime.ToString(Culture.DateTimeFormat);

        // To get the time the app was previously launched, not including this instance
        public string LastLaunchTime => SystemInformation.Instance.LastLaunchTime.ToString(Culture.DateTimeFormat);

        // To get the time the launch count was reset, not including this instance
        public string LastResetTime
        {
            get { return (string)GetValue(LastResetTimeProperty); }
            set { SetValue(LastResetTimeProperty, value); }
        }

        public static readonly DependencyProperty LastResetTimeProperty =
            DependencyProperty.Register(nameof(LastResetTime), typeof(string), typeof(SystemInformationPage), new PropertyMetadata(string.Empty));

        // To get the number of times the app has been launched since the last reset.
        public long LaunchCount
        {
            get { return (long)GetValue(LaunchCountProperty); }
            set { SetValue(LaunchCountProperty, value); }
        }

        public static readonly DependencyProperty LaunchCountProperty =
            DependencyProperty.Register(nameof(LaunchCount), typeof(long), typeof(SystemInformationPage), new PropertyMetadata(0));

        // To get the number of times the app has been launched.
        public long TotalLaunchCount => SystemInformation.Instance.TotalLaunchCount;

        // To get how long the app has been running
        public string AppUptime => SystemInformation.Instance.AppUptime.ToString();

        public SystemInformationPage()
        {
            InitializeComponent();

            RefreshProperties();

            SampleController.Current.RegisterNewCommand("Reset launch count", (sender, args) =>
            {
                SystemInformation.Instance.ResetLaunchCount();
                RefreshProperties();
            });
        }

        private void RefreshProperties()
        {
            LaunchCount = SystemInformation.Instance.LaunchCount;
            LastResetTime = SystemInformation.Instance.LastResetTime.ToString(Culture.DateTimeFormat);
        }
    }
}