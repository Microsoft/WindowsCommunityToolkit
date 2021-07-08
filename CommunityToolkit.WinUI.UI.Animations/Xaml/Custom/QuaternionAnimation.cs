// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// A custom <see cref="Quaternion"/> animation.
    /// </summary>
    public sealed class QuaternionAnimation : CustomAnimation<string, Quaternion>
    {
        /// <inheritdoc/>
        protected override (Quaternion?, Quaternion?) GetParsedValues()
        {
            return (To?.ToQuaternion(), From?.ToQuaternion());
        }
    }
}