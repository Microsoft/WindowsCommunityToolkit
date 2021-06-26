// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.Toolkit.Uwp.Notifications.Adaptive.Elements;

namespace Microsoft.Toolkit.Uwp.Notifications
{
    /// <summary>
    /// Subgroups are vertical columns that can contain text and images. Supported on Tiles since RTM. Supported on Toasts since Anniversary Update.
    /// </summary>
    public sealed class AdaptiveSubgroup
    {
        /// <summary>
        /// Gets a list of Children. <see cref="AdaptiveText"/> and <see cref="AdaptiveImage"/> are valid children of subgroups.
        /// </summary>
        public IList<IAdaptiveSubgroupChild> Children { get; private set; } = new List<IAdaptiveSubgroupChild>();

        private int? _hintWeight;

        /// <summary>
        /// Gets or sets the width of this subgroup column by specifying the weight, relative to the other subgroups.
        /// </summary>
        public int? HintWeight
        {
            get
            {
                return _hintWeight;
            }

            set
            {
                Element_AdaptiveSubgroup.CheckWeight(value);

                _hintWeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of this subgroup's content.
        /// </summary>
        public AdaptiveSubgroupTextStacking HintTextStacking { get; set; } = Element_AdaptiveSubgroup.DEFAULT_TEXT_STACKING;

        internal Element_AdaptiveSubgroup ConvertToElement()
        {
            var subgroup = new Element_AdaptiveSubgroup()
            {
                Weight = HintWeight,
                TextStacking = HintTextStacking
            };

            foreach (var child in Children)
            {
                subgroup.Children.Add(ConvertToSubgroupChildElement(child));
            }

            return subgroup;
        }

        private static IElement_AdaptiveSubgroupChild ConvertToSubgroupChildElement(IAdaptiveSubgroupChild child)
        {
            if (child is AdaptiveText)
            {
                return (child as AdaptiveText).ConvertToElement();
            }

            if (child is AdaptiveImage)
            {
                return (child as AdaptiveImage).ConvertToElement();
            }

            throw new NotImplementedException("Unknown child: " + child.GetType());
        }
    }
}