// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

#nullable enable

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A <see cref="KeyFrame{TValue,TKeyFrame}"/> type for <see cref="Vector2"/> animations.
    /// </summary>
    public sealed class Vector2KeyFrame : KeyFrame<string, Vector2>
    {
        /// <inheritdoc/>
        protected override Vector2 GetParsedValue()
        {
            return Value!.ToVector2();
        }
    }
}
