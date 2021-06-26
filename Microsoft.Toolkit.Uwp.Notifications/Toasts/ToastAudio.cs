// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Toolkit.Uwp.Notifications
{
    /// <summary>
    /// Specify audio to be played when the Toast notification is received.
    /// </summary>
    public sealed class ToastAudio
    {
        /// <summary>
        /// Gets or sets the media file to play in place of the default sound.
        /// </summary>
        public Uri Src { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether sound should repeat as long as the Toast is shown; false to play only once (default).
        /// </summary>
        public bool Loop { get; set; } = Element_ToastAudio.DEFAULT_LOOP;

        /// <summary>
        /// Gets or sets a value indicating whether sound is muted; false to allow the Toast notification sound to play (default).
        /// </summary>
        public bool Silent { get; set; } = Element_ToastAudio.DEFAULT_SILENT;

        internal Element_ToastAudio ConvertToElement()
        {
            return new Element_ToastAudio()
            {
                Src = Src,
                Loop = Loop,
                Silent = Silent
            };
        }
    }
}