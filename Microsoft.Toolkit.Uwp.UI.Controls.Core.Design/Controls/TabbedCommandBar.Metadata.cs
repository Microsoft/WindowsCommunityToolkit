// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using Microsoft.Toolkit.Uwp.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Design
{
    internal class TabbedCommandBarMetadata : AttributeTableBuilder
    {
        public TabbedCommandBarMetadata()
            : base()
        {
            AddCallback(ControlTypes.TabbedCommandBar,
                b =>
                {
                    b.AddCustomAttributes(nameof(TabbedCommandBar.CollapsedState), new CategoryAttribute(Resources.CategoryAppearance));
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}