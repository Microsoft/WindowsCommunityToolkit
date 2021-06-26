// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using Microsoft.Toolkit.Uwp.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Design
{
    internal class TextToolbarMetadata : AttributeTableBuilder
    {
        public TextToolbarMetadata()
            : base()
        {
            AddCallback(ControlTypes.TextToolbar,
                b =>
                {
                    b.AddCustomAttributes(nameof(TextToolbar.Editor), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(TextToolbar.Formatter), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(TextToolbar.DefaultButtons), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(TextToolbar.CustomButtons), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(TextToolbar.ButtonModifications), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}