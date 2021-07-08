// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using CommunityToolkit.WinUI.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace CommunityToolkit.WinUI.UI.Controls.Design
{
    internal class CustomDialogMetadata : AttributeTableBuilder
    {
        public CustomDialogMetadata()
            : base()
        {
            AddCallback(ControlTypes.AdaptiveGridView,
                b =>
                {
                    b.AddCustomAttributes(nameof(AdaptiveGridView.DesiredWidth),
                        new CategoryAttribute(Resources.CategoryCommon)
                        );
                    b.AddCustomAttributes(nameof(AdaptiveGridView.ItemHeight),
                        new CategoryAttribute(Resources.CategoryCommon)
                        );
                    b.AddCustomAttributes(nameof(AdaptiveGridView.OneRowModeEnabled),
                       new CategoryAttribute(Resources.CategoryCommon)
                       );
                    b.AddCustomAttributes(nameof(AdaptiveGridView.StretchContentForSingleRow),
                        new CategoryAttribute(Resources.CategoryCommon)
                        );
                    b.AddCustomAttributes(nameof(AdaptiveGridView.ItemClickCommand),
                        new EditorBrowsableAttribute(EditorBrowsableState.Advanced),
                        new CategoryAttribute(Resources.CategoryCommon)
                        );

                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}