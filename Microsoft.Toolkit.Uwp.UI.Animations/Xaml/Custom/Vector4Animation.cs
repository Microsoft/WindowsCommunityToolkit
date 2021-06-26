// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A custom <see cref="Vector4"/> animation.
    /// </summary>
    public sealed class Vector4Animation : CustomAnimation<string, Vector4>
    {
        /// <inheritdoc/>
        protected override (Vector4?, Vector4?) GetParsedValues()
        {
            return (To?.ToVector4(), From?.ToVector4());
        }
    }
}