// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using Microsoft.Toolkit.Uwp.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Design
{
    internal class WrapPanelMetadata : AttributeTableBuilder
    {
        public WrapPanelMetadata()
            : base()
        {
            AddCallback(ControlTypes.WrapPanel,
                b =>
                {
                    b.AddCustomAttributes(nameof(WrapPanel.Orientation),
                        new CategoryAttribute(Resources.CategoryLayout)
                    );
                    b.AddCustomAttributes(nameof(WrapPanel.HorizontalSpacing),
                        new CategoryAttribute(Resources.CategoryLayout)
                    );
                    b.AddCustomAttributes(nameof(WrapPanel.VerticalSpacing),
                        new CategoryAttribute(Resources.CategoryLayout)
                    );
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}