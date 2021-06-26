// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using Microsoft.Toolkit.Uwp.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Design
{
    internal class ListDetailsViewMetadata : AttributeTableBuilder
    {
        public ListDetailsViewMetadata()
            : base()
        {
            AddCallback(ControlTypes.ListDetailsView,
                b =>
                {
                    b.AddCustomAttributes(nameof(ListDetailsView.SelectedItem), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(ListDetailsView.DetailsTemplate),
                        new CategoryAttribute(Resources.CategoryAppearance),
                        new EditorBrowsableAttribute(EditorBrowsableState.Advanced)
                    );
                    b.AddCustomAttributes(nameof(ListDetailsView.ListPaneBackground), new CategoryAttribute(Resources.CategoryBrush));
                    b.AddCustomAttributes(nameof(ListDetailsView.ListHeader), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(ListDetailsView.ListHeaderTemplate),
                        new CategoryAttribute(Resources.CategoryAppearance),
                        new EditorBrowsableAttribute(EditorBrowsableState.Advanced)
                    );
                    b.AddCustomAttributes(nameof(ListDetailsView.ListPaneWidth), new CategoryAttribute(Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(ListDetailsView.NoSelectionContent), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(ListDetailsView.NoSelectionContentTemplate),
                        new CategoryAttribute(Resources.CategoryCommon),
                        new EditorBrowsableAttribute(EditorBrowsableState.Advanced)
                    );
                    b.AddCustomAttributes(nameof(ListDetailsView.ViewState), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(ListDetailsView.ListPaneCommandBar), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(ListDetailsView.DetailsPaneCommandBar), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}