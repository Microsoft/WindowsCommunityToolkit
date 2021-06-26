// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using Microsoft.Toolkit.Uwp.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Design
{
    internal class RotatorTileMetadata : AttributeTableBuilder
    {
        public RotatorTileMetadata()
            : base()
        {
            AddCallback(ControlTypes.RotatorTile,
                b =>
                {
                    b.AddCustomAttributes(nameof(RotatorTile.ExtraRandomDuration), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(RotatorTile.RotationDelay), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(RotatorTile.ItemsSource), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(RotatorTile.ItemTemplate),
                        new CategoryAttribute(Resources.CategoryAppearance),
                        new EditorBrowsableAttribute(EditorBrowsableState.Advanced)
                        );
                    b.AddCustomAttributes(nameof(RotatorTile.RotateDirection), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(RotatorTile.CurrentItem), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}