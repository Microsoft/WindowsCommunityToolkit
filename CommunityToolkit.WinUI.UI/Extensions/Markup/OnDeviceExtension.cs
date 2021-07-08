// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml.Markup;
using Windows.ApplicationModel;
using Windows.System.Profile;

namespace CommunityToolkit.WinUI.UI
{
    /// <summary>
    /// The OnDevice markup extension allows you to customize UI appearance on a per-DeviceFamily basis.
    /// </summary>
    /// <example>
    /// The next TextBlock will show the text 'Hello' on desktop computers, 'World' on Xbox and 'Hi' on all other devices
    /// <code>
    ///     <TextBlock Text="{ui:OnDevice Default=Hi, Desktop=Hello, Xbox=World}"
    ///                xmlns:ui="using:CommunityToolkit.WinUI.UI" />
    /// </code>
    /// </example>
    public class OnDeviceExtension : MarkupExtension
    {
        /// <summary>
        /// Gets the current device family.
        /// </summary>
        private string DeviceFamily { get; } = AnalyticsInfo.VersionInfo.DeviceFamily;

        /// <summary>
        /// Gets or sets the default value for this property
        /// </summary>
        public object Default { get; set; }

        /// <summary>
        /// Gets or sets a value for this property on Desktop
        /// </summary>
        public object Desktop { get; set; }

        /// <summary>
        /// Gets or sets a value for this property on Holographic
        /// </summary>
        public object Holographic { get; set; }

        /// <summary>
        /// Gets or sets a value for this property on IoT
        /// </summary>
        public object IoT { get; set; }

        /// <summary>
        /// Gets or sets a value for this property on Team
        /// </summary>
        public object Team { get; set; }

        /// <summary>
        /// Gets or sets a value for this property on Xbox
        /// </summary>
        public object Xbox { get; set; }

        /// <summary>
        /// Returns an object value for the current DeviceFamily, Default is used when it is not set.
        /// </summary>
        /// <returns>The object value to set on the property where the extension is applied.</returns>
        protected override object ProvideValue()
        {
            switch (DeviceFamily)
            {
                case "Windows.Desktop":
                    return this.Desktop ?? this.Default;
                case "Windows.Holographic":
                    return this.Holographic ?? this.Default;
                case "Windows.IoT":
                    return this.IoT ?? this.Default;
                case "Windows.Team":
                    return this.Team ?? this.Default;
                case "Windows.Xbox":
                    return this.Xbox ?? this.Default;
                default:
                    return this.Default;
            }
        }
    }
}