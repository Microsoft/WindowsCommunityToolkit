// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Defines how colors are represented.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public enum ColorRepresentation
    {
        /// <summary>
        /// Color is represented by hue, saturation, value and alpha channels.
        /// </summary>
        Hsva,

        /// <summary>
        /// Color is represented by red, green, blue and alpha channels.
        /// </summary>
        Rgba
    }
}