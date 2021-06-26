// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using Microsoft.Toolkit.Uwp.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Design
{
    internal class ExpanderMetadata : AttributeTableBuilder
    {
        public ExpanderMetadata()
            : base()
        {
            AddCallback(ControlTypes.Expander,
                b =>
                {
                    b.AddCustomAttributes(nameof(Expander.Header), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(Expander.HeaderTemplate),
                        new CategoryAttribute(Resources.CategoryCommon),
                        new EditorBrowsableAttribute(EditorBrowsableState.Advanced)
                        );
                    b.AddCustomAttributes(nameof(Expander.IsExpanded), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(Expander.ExpandDirection), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}