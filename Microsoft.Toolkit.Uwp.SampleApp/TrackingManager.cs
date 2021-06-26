// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Services.Store.Engagement;

namespace Microsoft.Toolkit.Uwp.SampleApp
{
    public static class TrackingManager
    {
        private static StoreServicesCustomEventLogger logger;

        static TrackingManager()
        {
            try
            {
                logger = StoreServicesCustomEventLogger.GetDefault();
            }
            catch
            {
                // Ignoring error
            }
        }

        public static void TrackException(Exception ex)
        {
            try
            {
                logger.Log($"exception - {ex.Message} - {ex.StackTrace}");
            }
            catch
            {
                // Ignore error
            }
        }

        public static void TrackEvent(string category, string action, string label = "", long value = 0)
        {
            try
            {
                logger.Log($"{category} - {action} - {label} - {value.ToString()}");
            }
            catch
            {
                // Ignore error
            }
        }

        public static void TrackPage(string pageName)
        {
            try
            {
                logger.Log($"pageView - {pageName}");
            }
            catch
            {
                // Ignore error
            }
        }
    }
}