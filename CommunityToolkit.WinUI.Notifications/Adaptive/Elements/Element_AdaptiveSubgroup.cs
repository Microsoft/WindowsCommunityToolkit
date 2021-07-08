// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace CommunityToolkit.WinUI.Notifications.Adaptive.Elements
{
    [NotificationXmlElement("subgroup")]
    internal sealed class Element_AdaptiveSubgroup : IElementWithDescendants
    {
        internal const AdaptiveSubgroupTextStacking DEFAULT_TEXT_STACKING = AdaptiveSubgroupTextStacking.Default;

        [NotificationXmlAttribute("hint-textStacking", DEFAULT_TEXT_STACKING)]
        public AdaptiveSubgroupTextStacking TextStacking { get; set; } = DEFAULT_TEXT_STACKING;

        private int? _weight;

        [NotificationXmlAttribute("hint-weight")]
        public int? Weight
        {
            get
            {
                return _weight;
            }

            set
            {
                CheckWeight(value);

                _weight = value;
            }
        }

        internal static void CheckWeight(int? weight)
        {
            if (weight != null && weight.Value < 1)
            {
                throw new ArgumentOutOfRangeException("Weight must be between 1 and int.MaxValue, inclusive (or null)");
            }
        }

        public IList<IElement_AdaptiveSubgroupChild> Children { get; private set; } = new List<IElement_AdaptiveSubgroupChild>();

        public IEnumerable<object> Descendants()
        {
            foreach (IElement_AdaptiveSubgroupChild child in Children)
            {
                // Return each child (we know there's no further descendants)
                yield return child;
            }
        }
    }

    internal interface IElement_AdaptiveSubgroupChild
    {
    }
}