// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WIN32

using System;
using System.Runtime.InteropServices;

namespace Microsoft.Toolkit.Uwp.Notifications
{
    [ComImport]
    [Guid("43826d1e-e718-42ee-bc55-a1e261c37bfe")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IShellItem
    {
        void BindToHandler(
            IntPtr pbc,
            IntPtr bhid,
            IntPtr riid,
            [Out] out IntPtr ppv);

        void GetParent(
            [Out] out IShellItem ppsi);

        void GetDisplayName(
            int sigdnName,
            [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszName);

        void GetAttributes(
            int sfgaoMask,
            [Out] out int psfgaoAttribs);

        void Compare(
            IShellItem psi,
            int hint,
            [Out] out int piOrder);
    }
}

#endif