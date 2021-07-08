// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.UI.Composition;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// A center point animation working on the composition layer.
    /// </summary>
    public sealed class CenterPointAnimation : ImplicitAnimation<string, Vector3>
    {
        /// <inheritdoc/>
        protected override string ExplicitTarget => nameof(Visual.CenterPoint);

        /// <inheritdoc/>
        protected override (Vector3?, Vector3?) GetParsedValues()
        {
            return (To?.ToVector3(), From?.ToVector3());
        }
    }
}