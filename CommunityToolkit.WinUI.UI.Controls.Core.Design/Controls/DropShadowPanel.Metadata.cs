// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using CommunityToolkit.WinUI.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Features;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;
using Microsoft.VisualStudio.DesignTools.Extensibility.Model;
using Microsoft.VisualStudio.DesignTools.Extensibility.PropertyEditing;

namespace CommunityToolkit.WinUI.UI.Controls.Design

{
    internal class DropShadowPanelDefaults : DefaultInitializer
    {
        public override void InitializeDefaults(ModelItem item)
        {
            item.Properties[nameof(DropShadowPanel.BlurRadius)].SetValue(5d);
        }
    }

    internal class DropShadowPanelMetadata : AttributeTableBuilder
    {
        public DropShadowPanelMetadata()
            : base()
        {
            AddCallback(ControlTypes.DropShadowPanel,
                b =>
                {
                    b.AddCustomAttributes(new FeatureAttribute(typeof(DropShadowPanelDefaults)));

                    b.AddCustomAttributes(nameof(DropShadowPanel.BlurRadius),
                        new PropertyOrderAttribute(PropertyOrder.Early),
                        new CategoryAttribute(Resources.CategoryDropShadow)
                        );
                    b.AddCustomAttributes(nameof(DropShadowPanel.ShadowOpacity),
                       new PropertyOrderAttribute(PropertyOrder.Early),
                       new CategoryAttribute(Resources.CategoryDropShadow)
                       );
                    b.AddCustomAttributes(nameof(DropShadowPanel.Color),
                        new CategoryAttribute(Resources.CategoryDropShadow)
                        );
                    b.AddCustomAttributes(nameof(DropShadowPanel.OffsetX),
                       new PropertyOrderAttribute(PropertyOrder.Late),
                        new CategoryAttribute(Resources.CategoryDropShadow)
                        );
                    b.AddCustomAttributes(nameof(DropShadowPanel.OffsetY),
                       new PropertyOrderAttribute(PropertyOrder.Late),
                       new CategoryAttribute(Resources.CategoryDropShadow)
                       );
                    b.AddCustomAttributes(nameof(DropShadowPanel.OffsetZ),
                       new PropertyOrderAttribute(PropertyOrder.Late),
                       new CategoryAttribute(Resources.CategoryDropShadow)
                       );

                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}