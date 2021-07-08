// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// A delegate for <see cref="ImageEx"/> opened.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ImageExOpenedEventHandler(object sender, ImageExOpenedEventArgs e);

    /// <summary>
    /// Provides data for the <see cref="ImageEx"/> ImageOpened event.
    /// </summary>
    public class ImageExOpenedEventArgs : EventArgs
    {
    }
}