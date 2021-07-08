// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WIN32

using System;
using System.Runtime.InteropServices;

namespace CommunityToolkit.WinUI.Notifications
{
    [ComImport]
    [Guid("DE25675A-72DE-44b4-9373-05170450C140")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IApplicationResolver
    {
        void GetAppIDForShortcut(
            IntPtr psi,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszAppID);

        void GetAppIDForShortcutObject(
            IntPtr psl,
            IntPtr psi,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszAppID);

        void GetAppIDForWindow(
            int hwnd,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszAppID,
            [Out] out bool pfPinningPrevented,
            [Out] out bool pfExplicitAppID,
            [Out] out bool pfEmbeddedShortcutValid);

        /// <summary>
        /// Main way to obtain app ID for any process. Calls GetShortcutPathOrAppIdFromPid
        /// </summary>
        void GetAppIDForProcess(
            uint dwProcessID,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszAppID,
            [Out] out bool pfPinningPrevented,
            [Out] out bool pfExplicitAppID,
            [Out] out bool pfEmbeddedShortcutValid);

        void GetShortcutForProcess(
            uint dwProcessID,
            [Out] out IntPtr ppsi);

        void GetBestShortcutForAppID(
            string pszAppID,
            [Out] out IShellItem ppsi);

        void GetBestShortcutAndAppIDForAppPath(
            string pszAppPath,
            [Out] out IntPtr ppsi,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszAppID);

        void CanPinApp(IntPtr psi);

        void CanPinAppShortcut(
            IntPtr psl,
            IntPtr psi);

        void GetRelaunchProperties(
            int hwnd,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszAppID,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszCmdLine,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszIconResource,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszDisplayNameResource,
            [Out] bool pfPinnable);

        void GenerateShortcutFromWindowProperties(
            int hwnd,
            [Out] out IntPtr ppsi);

        void GenerateShortcutFromItemProperties(
            IntPtr psi2,
            [Out] out IntPtr ppsi);
    }
}

#endif