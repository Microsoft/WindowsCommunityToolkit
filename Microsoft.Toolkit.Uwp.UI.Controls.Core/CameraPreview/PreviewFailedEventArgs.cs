// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// PreviewFailed Event Args
    /// </summary>
    public class PreviewFailedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets error information about failure
        /// </summary>
        public string Error { get; internal set; }
    }
}