// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using CommunityToolkit.WinUI.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace CommunityToolkit.WinUI.UI.Controls.Design
{

    internal class BladeItemMetadata : AttributeTableBuilder
    {
        public BladeItemMetadata()
            : base()
        {
            AddCallback(ControlTypes.BladeItem,
                b =>
                {
                    b.AddCustomAttributes(nameof(BladeItem.TitleBarVisibility),
                        new CategoryAttribute(Resources.CategoryCommon)
                        );
                    b.AddCustomAttributes(nameof(BladeItem.IsOpen),
                        new CategoryAttribute(Resources.CategoryCommon)
                        );
                    b.AddCustomAttributes(nameof(BladeItem.TitleBarBackground),
                        new CategoryAttribute(Resources.CategoryBrush)
                        );
                    b.AddCustomAttributes(nameof(BladeItem.CloseButtonBackground),
                        new CategoryAttribute(Resources.CategoryBrush)
                        );
                    b.AddCustomAttributes(nameof(BladeItem.CloseButtonForeground),
                        new CategoryAttribute(Resources.CategoryBrush)
                        );

                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}