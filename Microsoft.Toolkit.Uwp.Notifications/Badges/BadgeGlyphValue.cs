// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.Notifications
{
    /// <summary>
    /// The types of glyphs that can be placed on a badge.
    /// </summary>
    public enum BadgeGlyphValue
    {
        /// <summary>
        /// No glyph.  If there is a numeric badge, or a glyph currently on the badge,
        /// it will be removed.
        /// </summary>
        None = 0,

        /// <summary>
        /// A glyph representing application activity.
        /// </summary>
        Activity,

        /// <summary>
        /// A glyph representing an alert.
        /// </summary>
        Alert,

        /// <summary>
        /// A glyph representing an alarm.
        /// </summary>
        Alarm,

        /// <summary>
        /// A glyph representing availability status.
        /// </summary>
        Available,

        /// <summary>
        /// A glyph representing away status
        /// </summary>
        Away,

        /// <summary>
        /// A glyph representing busy status.
        /// </summary>
        Busy,

        /// <summary>
        /// A glyph representing that a new message is available.
        /// </summary>
        NewMessage,

        /// <summary>
        /// A glyph representing that media is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// A glyph representing that media is playing.
        /// </summary>
        Playing,

        /// <summary>
        /// A glyph representing unavailable status.
        /// </summary>
        Unavailable,

        /// <summary>
        /// A glyph representing an error.
        /// </summary>
        Error,

        /// <summary>
        /// A glyph representing attention status.
        /// </summary>
        Attention
    }
}