// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Options for how to calculate the layout of <see cref="Windows.UI.Xaml.Controls.WrapGrid"/> items.
    /// </summary>
    public enum StretchChild
    {
        /// <summary>
        /// Don't apply any additional stretching logic
        /// </summary>
        None,

        /// <summary>
        /// Make the last child stretch to fill the available space
        /// </summary>
        Last
    }
}