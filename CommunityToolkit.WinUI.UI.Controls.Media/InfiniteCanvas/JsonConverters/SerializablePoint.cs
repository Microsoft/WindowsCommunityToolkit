// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Foundation;

namespace CommunityToolkit.WinUI.UI.Controls
{
    internal class SerializablePoint
    {
        public Point Position { get; set; }

        public float Pressure { get; set; }

        public float TiltX { get; set; }

        public float TiltY { get; set; }

        public ulong Timestamp { get; set; }
    }
}