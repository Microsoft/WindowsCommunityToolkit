// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Class used to provide helpers for design time
    /// </summary>
    public static class DesignTimeHelpers
    {
        private static Lazy<bool> designModeEnabled = new Lazy<bool>(InitializeDesignerMode);

        private static Lazy<bool> designMode2Enabled = new Lazy<bool>(InitializeDesignMode2);

        /// <summary>
        /// Gets a value indicating whether app is running in the Legacy Designer
        /// </summary>
        public static bool IsRunningInLegacyDesignerMode => DesignTimeHelpers.designModeEnabled.Value && !DesignTimeHelpers.designMode2Enabled.Value;

        /// <summary>
        /// Gets a value indicating whether app is running in the Enhanced Designer
        /// </summary>
        public static bool IsRunningInEnhancedDesignerMode => DesignTimeHelpers.designModeEnabled.Value && DesignTimeHelpers.designMode2Enabled.Value;

        /// <summary>
        /// Gets a value indicating whether app is not running in the Designer
        /// </summary>
        public static bool IsRunningInApplicationRuntimeMode => !DesignTimeHelpers.designModeEnabled.Value;

        // Private initializer
        private static bool InitializeDesignerMode()
        {
            return Windows.ApplicationModel.DesignMode.DesignModeEnabled;
        }

        /// <summary>
        /// Used to enable or disable user code inside a XAML designer that targets the Windows 10 Fall Creators Update SDK, or later.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Windows.ApplicationModel.DesignMode.DesignModeEnabled returns true when called from user code running inside any version of the XAML designer--regardless of which SDK version you target. This check is recommended for most users.
        /// </para><para>
        /// Starting with the Windows 10 Fall Creators Update, Visual Studio provides a new XAML designer that targets the Windows 10 Fall Creators Update and later.
        /// </para><para>
        /// Use Windows.ApplicationModel.DesignMode.DesignMode2Enabled to differentiate code that depends on functionality only enabled for a XAML designer that targets the Windows 10 Fall Creators Update SDK or later.
        /// </para>
        /// <para>
        /// More info here: https://docs.microsoft.com/en-us/uwp/api/Windows.ApplicationModel.DesignMode
        /// </para>
        /// </remarks>
        /// <returns>True if called from code running inside a XAML designer that targets the Windows 10 Fall Creators Update, or later; otherwise false.</returns>
        private static bool InitializeDesignMode2()
        {
            return Windows.ApplicationModel.DesignMode.DesignMode2Enabled;
        }
    }
}