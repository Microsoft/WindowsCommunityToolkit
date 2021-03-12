// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A custom <see cref="Vector2"/> animation.
    /// </summary>
    public sealed class Vector2Animation : CustomAnimation<string, Vector2>
    {
        /// <inheritdoc/>
        protected override (Vector2?, Vector2?) GetParsedValues()
        {
            return (To?.ToVector2(), From?.ToVector2());
        }
    }
}
