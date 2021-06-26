// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Provides attached properties for interacting with the <see cref="Windows.UI.ViewManagement.ApplicationView"/> on a window (app view).
    /// </summary>
    public static class ApplicationViewExtensions
    {
        /// <summary>
        /// Gets <see cref="string"/> for <see cref="Windows.UI.ViewManagement.ApplicationView.Title"/>
        /// </summary>
        /// <param name="page">The <see cref="Page"/></param>
        /// <returns><see cref="string"/></returns>
        public static string GetTitle(Page page)
        {
            var applicationView = GetApplicationView();

            return applicationView?.Title ?? string.Empty;
        }

        /// <summary>
        /// Sets <see cref="string"/> to <see cref="Windows.UI.ViewManagement.ApplicationView.Title"/>
        /// </summary>
        /// <param name="page">The <see cref="Page"/></param>
        /// <param name="value"><see cref="string"/></param>
        public static void SetTitle(Page page, string value)
        {
            var applicationView = GetApplicationView();
            if (applicationView != null)
            {
                applicationView.Title = value;
            }
        }

        /// <summary>
        /// Gets <see cref="bool"/> CoreApplicationView.TitleBar.ExtendViewIntoTitleBar
        /// </summary>
        /// <param name="page">The <see cref="Page"/></param>
        /// <returns><see cref="string"/></returns>
        public static bool GetExtendViewIntoTitleBar(Page page)
        {
            var applicationView = GetCoreApplicationView();

            return applicationView?.TitleBar?.ExtendViewIntoTitleBar ?? false;
        }

        /// <summary>
        /// Sets <see cref="bool"/> to CoreApplicationView.TitleBar.ExtendViewIntoTitleBar
        /// </summary>
        /// <param name="page">The <see cref="Page"/></param>
        /// <param name="value"><see cref="bool"/></param>
        public static void SetExtendViewIntoTitleBar(Page page, bool value)
        {
            var applicationView = GetCoreApplicationView();
            if (applicationView != null && applicationView.TitleBar != null)
            {
                applicationView.TitleBar.ExtendViewIntoTitleBar = value;
            }
        }

        /// <summary>
        /// Gets <see cref="AppViewBackButtonVisibility"/> for <see cref="SystemNavigationManager.AppViewBackButtonVisibility"/>
        /// </summary>
        /// <param name="page">The <see cref="Page"/></param>
        /// <returns><see cref="string"/></returns>
        public static AppViewBackButtonVisibility GetBackButtonVisibility(Page page)
        {
            var systemNavigationManager = GetSystemNavigationManager();

            return systemNavigationManager?.AppViewBackButtonVisibility ?? AppViewBackButtonVisibility.Collapsed;
        }

        /// <summary>
        /// Sets <see cref="AppViewBackButtonVisibility"/> to <see cref="SystemNavigationManager.AppViewBackButtonVisibility"/>
        /// </summary>
        /// <param name="page">The <see cref="Page"/></param>
        /// <param name="value"><see cref="AppViewBackButtonVisibility"/></param>
        public static void SetBackButtonVisibility(Page page, AppViewBackButtonVisibility value)
        {
            var systemNavigationManager = GetSystemNavigationManager();

            if (systemNavigationManager != null)
            {
                systemNavigationManager.AppViewBackButtonVisibility = value;
            }
        }

        private static SystemNavigationManager GetSystemNavigationManager()
        {
            return SystemNavigationManager.GetForCurrentView();
        }

        private static CoreApplicationView GetCoreApplicationView()
        {
            return CoreApplication.GetCurrentView();
        }

        private static Windows.UI.ViewManagement.ApplicationView GetApplicationView()
        {
            return Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
        }
    }
}