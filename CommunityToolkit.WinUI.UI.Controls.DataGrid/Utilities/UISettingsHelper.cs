// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.ViewManagement;

namespace CommunityToolkit.WinUI.UI.Controls.Utilities
{
    /// <summary>
    /// Helper class for accessing UISettings properties.
    /// </summary>
    internal static class UISettingsHelper
    {
        private static UISettings _uiSettings = null;

        internal static bool AreSettingsEnablingAnimations
        {
            get
            {
                if (_uiSettings == null)
                {
                    _uiSettings = new UISettings();
                }

                return _uiSettings.AnimationsEnabled;
            }
        }

        internal static bool AreSettingsAutoHidingScrollBars
        {
            get
            {
                // TODO: Use UISettings public API once available
                return true;
            }
        }
    }
}