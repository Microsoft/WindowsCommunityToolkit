// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    [NotificationXmlElement("audio")]
    internal sealed class Element_ToastAudio
    {
        internal const bool DEFAULT_LOOP = false;
        internal const bool DEFAULT_SILENT = false;

        /// <summary>
        /// Gets or sets the media file to play in place of the default sound. This can either be a ms-winsoundevent value, or a custom ms-appx:/// or ms-appdata:/// file, or null for the default sound.
        /// </summary>
        [NotificationXmlAttribute("src")]
        public Uri Src { get; set; }

        [NotificationXmlAttribute("loop", DEFAULT_LOOP)]
        public bool Loop { get; set; } = DEFAULT_LOOP;

        /// <summary>
        /// Gets or sets a value indicating whether the sound is muted; false to allow the Toast notification sound to play.
        /// </summary>
        [NotificationXmlAttribute("silent", DEFAULT_SILENT)]
        public bool Silent { get; set; } = DEFAULT_SILENT;
    }
}