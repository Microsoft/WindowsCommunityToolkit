// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Toolkit.Uwp.Notifications.Adaptive
{
    internal static class AdaptiveHelper
    {
        internal static object ConvertToElement(object obj)
        {
            if (obj is AdaptiveText)
            {
                return (obj as AdaptiveText).ConvertToElement();
            }

            if (obj is AdaptiveImage)
            {
                return (obj as AdaptiveImage).ConvertToElement();
            }

            if (obj is AdaptiveGroup)
            {
                return (obj as AdaptiveGroup).ConvertToElement();
            }

            if (obj is AdaptiveSubgroup)
            {
                return (obj as AdaptiveSubgroup).ConvertToElement();
            }

            if (obj is AdaptiveProgressBar)
            {
                return (obj as AdaptiveProgressBar).ConvertToElement();
            }

            throw new NotImplementedException("Unknown object: " + obj.GetType());
        }
    }
}