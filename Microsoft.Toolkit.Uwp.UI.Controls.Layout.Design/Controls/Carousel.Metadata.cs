// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using Microsoft.Toolkit.Uwp.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Design
{
    internal class CarouselMetadata : AttributeTableBuilder
    {
        public CarouselMetadata()
            : base()
        {
            AddCallback(ControlTypes.Carousel,
                b =>
                {
                    b.AddCustomAttributes(nameof(Carousel.SelectedItem), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(Carousel.SelectedIndex), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(Carousel.TransitionDuration), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(Carousel.ItemDepth), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(Carousel.EasingFunction), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(Carousel.ItemMargin), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(Carousel.InvertPositive), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(Carousel.ItemRotationX), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(Carousel.ItemRotationY), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(Carousel.ItemRotationZ), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(Carousel.Orientation), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}