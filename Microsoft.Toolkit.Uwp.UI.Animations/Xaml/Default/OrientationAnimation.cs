// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Windows.UI.Composition;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// An orientation animation working on the composition layer.
    /// </summary>
    public sealed class OrientationAnimation : ImplicitAnimation<string, Quaternion>
    {
        /// <inheritdoc/>
        protected override string ExplicitTarget => nameof(Visual.Orientation);

        /// <inheritdoc/>
        protected override (Quaternion?, Quaternion?) GetParsedValues()
        {
            return (To?.ToQuaternion(), From?.ToQuaternion());
        }
    }
}