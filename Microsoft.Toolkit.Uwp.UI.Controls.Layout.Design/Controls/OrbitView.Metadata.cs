// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using Microsoft.Toolkit.Uwp.UI.Controls.Design.Properties;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Design
{
    internal class OrbitViewMetadata : AttributeTableBuilder
    {
        public OrbitViewMetadata()
            : base()
        {
            AddCallback(ControlTypes.OrbitView,
                b =>
                {
                    b.AddCustomAttributes(nameof(OrbitView.OrbitsEnabled), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(OrbitView.IsItemClickEnabled), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(OrbitView.AnchorsEnabled), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(OrbitView.MinItemSize), new CategoryAttribute(Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(OrbitView.MaxItemSize), new CategoryAttribute(Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(OrbitView.AnchorColor), new CategoryAttribute(Resources.CategoryBrush));
                    b.AddCustomAttributes(nameof(OrbitView.OrbitColor), new CategoryAttribute(Resources.CategoryBrush));
                    b.AddCustomAttributes(nameof(OrbitView.OrbitDashArray), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(OrbitView.AnchorThickness), new CategoryAttribute(Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(OrbitView.OrbitThickness), new CategoryAttribute(Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(OrbitView.CenterContent), new CategoryAttribute(Resources.CategoryCommon));
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}