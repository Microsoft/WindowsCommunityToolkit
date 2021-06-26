// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WIN32

using System.Runtime.InteropServices;

namespace Microsoft.Toolkit.Uwp.Notifications
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SIZE
    {
        internal int X;
        internal int Y;

        internal SIZE(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}

#endif