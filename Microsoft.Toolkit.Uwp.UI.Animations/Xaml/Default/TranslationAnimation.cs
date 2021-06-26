// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A translation animation working on the composition or layer.
    /// </summary>
    public sealed class TranslationAnimation : ImplicitAnimation<string, Vector3>
    {
        /// <inheritdoc/>
        protected override string ExplicitTarget => "Translation";

        /// <inheritdoc/>
        protected override (Vector3?, Vector3?) GetParsedValues()
        {
            return (To?.ToVector3(), From?.ToVector3());
        }
    }
}