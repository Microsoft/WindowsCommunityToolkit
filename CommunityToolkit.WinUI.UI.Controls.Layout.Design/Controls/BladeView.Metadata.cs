// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using CommunityToolkit.WinUI.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;
using Microsoft.VisualStudio.DesignTools.Extensibility.PropertyEditing;

namespace CommunityToolkit.WinUI.UI.Controls.Design
{

    internal class BladeViewMetadata : AttributeTableBuilder
    {
        public BladeViewMetadata()
            : base()
        {
            AddCallback(ControlTypes.BladeView,
                b =>
                {
                    b.AddCustomAttributes(nameof(BladeView.ActiveBlades),
                        new CategoryAttribute(Resources.CategoryCommon)
                        );
                    b.AddCustomAttributes(nameof(BladeView.BladeMode),
                        new CategoryAttribute(Resources.CategoryCommon)
                        );
                    b.AddCustomAttributes(nameof(BladeView.AutoCollapseCountThreshold),
                        new CategoryAttribute(Resources.CategoryCommon)
                        );
                    b.AddCustomAttributes(nameof(BladeView.Items),
                        new PropertyOrderAttribute(PropertyOrder.Early),
                        new CategoryAttribute(Resources.CategoryCommon),
                        //The following is necessary because this is a collection of an abstract type, so we help
                        //the designer with populating supported types that can be added to the collection
                        new NewItemTypesAttribute(ControlTypes.BladeItem),
                        new AlternateContentPropertyAttribute()
                    );
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}