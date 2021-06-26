// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;

#pragma warning disable CS0419

namespace Microsoft.Toolkit.Uwp.UI.Animations
{
    /// <summary>
    /// A custom <see cref="Color"/> animation.
    /// </summary>
    public sealed class ColorAnimation : CustomAnimation<Color?, Color>
    {
        /// <inheritdoc/>
        protected override (Color?, Color?) GetParsedValues()
        {
            return (To, From);
        }
    }
}