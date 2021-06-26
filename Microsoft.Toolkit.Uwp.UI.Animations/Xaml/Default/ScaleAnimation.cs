// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Windows.UI.Composition;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A scale animation working on the composition or layer.
    /// </summary>
    public sealed class ScaleAnimation : ImplicitAnimation<string, Vector3>
    {
        /// <inheritdoc/>
        protected override string ExplicitTarget => nameof(Visual.Scale);

        /// <inheritdoc/>
        protected override (Vector3?, Vector3?) GetParsedValues()
        {
            return (To?.ToVector3(), From?.ToVector3());
        }
    }
}