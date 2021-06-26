// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using Microsoft.Toolkit.Uwp.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Design
{
    internal class TabbedCommandBarItemMetadata : AttributeTableBuilder
    {
        public TabbedCommandBarItemMetadata()
            : base()
        {
            AddCallback(ControlTypes.TabbedCommandBarItem,
                b =>
                {
                    // TODO
                    // b.AddCustomAttributes(nameof(TabbedCommandBarItem.Header), new CategoryAttribute(Resources.CategoryCommon));
                    // b.AddCustomAttributes(nameof(TabbedCommandBarItem.Footer), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(TabbedCommandBarItem.IsContextual), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(TabbedCommandBarItem.OverflowButtonAlignment),
                        new CategoryAttribute(Resources.CategoryLayout),
                        new EditorBrowsableAttribute(EditorBrowsableState.Advanced)
                        );
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}