// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.Foundation.Collections;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    // Basic filter which performs no input filtering -- easy to
    // use as a default filter.
    internal class NullFilter : IGazeFilter
    {
        public virtual GazeFilterArgs Update(GazeFilterArgs args)
        {
            return args;
        }

        public virtual void LoadSettings(ValueSet settings)
        {
        }
    }
}