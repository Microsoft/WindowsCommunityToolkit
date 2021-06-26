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
    /// Defines a specific channel within a color representation.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public enum ColorChannel
    {
        /// <summary>
        /// Represents the alpha channel.
        /// </summary>
        Alpha,

        /// <summary>
        /// Represents the first color channel which is Red when RGB or Hue when HSV.
        /// </summary>
        Channel1,

        /// <summary>
        /// Represents the second color channel which is Green when RGB or Saturation when HSV.
        /// </summary>
        Channel2,

        /// <summary>
        /// Represents the third color channel which is Blue when RGB or Value when HSV.
        /// </summary>
        Channel3
    }
}