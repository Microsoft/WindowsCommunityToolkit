// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.Notifications.Adaptive.Elements;

namespace Microsoft.Toolkit.Uwp.Notifications
{
    /// <summary>
    /// Groups semantically identify that the content in the group must either be displayed as a whole, or not displayed if it cannot fit. Groups also allow creating multiple columns. Supported on Tiles since RTM. Supported on Toasts since Anniversary Update.
    /// </summary>
    public sealed class AdaptiveGroup : ITileBindingContentAdaptiveChild, IAdaptiveChild, IToastBindingGenericChild
    {
        /// <summary>
        /// Gets the only valid children of groups are <see cref="AdaptiveSubgroup"/>.
        /// Each subgroup is displayed as a separate vertical column. Note that you must
        /// include at least one subgroup in your group, otherwise an <see cref="InvalidOperationException"/>
        /// will be thrown when you try to retrieve the XML for the notification.
        /// </summary>
        public IList<AdaptiveSubgroup> Children { get; private set; } = new List<AdaptiveSubgroup>();

        internal Element_AdaptiveGroup ConvertToElement()
        {
            if (Children.Count == 0)
            {
                throw new InvalidOperationException("Groups must have at least one child subgroup. The Children property had zero items in it.");
            }

            Element_AdaptiveGroup group = new Element_AdaptiveGroup();

            foreach (var subgroup in Children)
            {
                group.Children.Add(subgroup.ConvertToElement());
            }

            return group;
        }
    }
}