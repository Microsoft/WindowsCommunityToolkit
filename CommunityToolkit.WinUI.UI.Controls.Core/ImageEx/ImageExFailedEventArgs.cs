// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// A delegate for <see cref="ImageEx"/> failed.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void ImageExFailedEventHandler(object sender, ImageExFailedEventArgs e);

    /// <summary>
    /// Provides data for the <see cref="ImageEx"/> ImageFailed event.
    /// </summary>
    public class ImageExFailedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageExFailedEventArgs"/> class.
        /// </summary>
        /// <param name="errorException">exception that caused the error condition</param>
        public ImageExFailedEventArgs(Exception errorException)
        {
            ErrorException = errorException;
            ErrorMessage = ErrorException?.Message;
        }

        /// <summary>
        /// Gets the exception that caused the error condition.
        /// </summary>
        public Exception ErrorException { get; private set; }

        /// <summary>
        /// Gets the reason for the error condition.
        /// </summary>
        public string ErrorMessage { get; private set; }
    }
}