// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Composition;

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A rotation in degrees animation working on the composition or layer.
    /// </summary>
    public sealed class RotationInDegreesAnimation : ImplicitAnimation<double?, double>
    {
        /// <inheritdoc/>
        protected override string ExplicitTarget => nameof(Visual.RotationAngleInDegrees);

        /// <inheritdoc/>
        protected override (double?, double?) GetParsedValues()
        {
            return (To, From);
        }
    }
}