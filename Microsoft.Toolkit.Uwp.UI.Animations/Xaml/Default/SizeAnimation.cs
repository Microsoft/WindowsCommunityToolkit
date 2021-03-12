// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Windows.UI.Composition;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A size animation working on the composition or layer.
    /// </summary>
    public sealed class SizeAnimation : ImplicitAnimation<string, Vector2>
    {
        /// <inheritdoc/>
        protected override string ExplicitTarget => nameof(Visual.Size);

        /// <inheritdoc/>
        protected override (Vector2?, Vector2?) GetParsedValues()
        {
            return (To?.ToVector2(), From?.ToVector2());
        }
    }
}
