// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A custom scalar animation.
    /// </summary>
    public sealed class ScalarAnimation : CustomAnimation<double?, double>
    {
        /// <inheritdoc/>
        protected override (double?, double?) GetParsedValues()
        {
            return (To, From);
        }
    }
}