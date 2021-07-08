// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// A custom <see cref="Vector3"/> animation.
    /// </summary>
    public sealed class Vector3Animation : CustomAnimation<string, Vector3>
    {
        /// <inheritdoc/>
        protected override (Vector3?, Vector3?) GetParsedValues()
        {
            return (To?.ToVector3(), From?.ToVector3());
        }
    }
}