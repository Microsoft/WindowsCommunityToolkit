// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using Microsoft.Toolkit.Uwp.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Design
{
    internal class GridSplitterMetadata : AttributeTableBuilder
    {
        public GridSplitterMetadata()
            : base()
        {
            AddCallback(ControlTypes.GridSplitter,
                b =>
                {
                    b.AddCustomAttributes(nameof(GridSplitter.Element), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(GridSplitter.ResizeDirection), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(GridSplitter.ResizeBehavior), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(GridSplitter.GripperForeground), new CategoryAttribute(Resources.CategoryBrush));
                    b.AddCustomAttributes(nameof(GridSplitter.ParentLevel), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(GridSplitter.GripperCursor), new CategoryAttribute(Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(GridSplitter.GripperCustomCursorResource),
                        new CategoryAttribute(Resources.CategoryAppearance),
                        new EditorBrowsableAttribute(EditorBrowsableState.Advanced)
                    );
                    b.AddCustomAttributes(nameof(GridSplitter.CursorBehavior), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}