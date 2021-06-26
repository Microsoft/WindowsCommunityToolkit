// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Features;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;
using Microsoft.VisualStudio.DesignTools.Extensibility.Model;

namespace Microsoft.Toolkit.Uwp.UI.Controls.Design
{
    internal class RadialGaugeDefaults : DefaultInitializer
    {
        public override void InitializeDefaults(ModelItem item)
        {
            item.Properties[nameof(RadialGauge.Value)].SetValue(0d);
        }
    }

    internal class RadialGaugeMetadata : AttributeTableBuilder
    {
        public RadialGaugeMetadata()
            : base()
        {
            AddCallback(ControlTypes.RadialGauge,
                b =>
                {
                    b.AddCustomAttributes(new FeatureAttribute(typeof(RadialGaugeDefaults)));
                    b.AddCustomAttributes(nameof(RadialGauge.Minimum), new CategoryAttribute(Properties.Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(RadialGauge.Maximum), new CategoryAttribute(Properties.Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(RadialGauge.StepSize), new CategoryAttribute(Properties.Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(RadialGauge.IsInteractive), new CategoryAttribute(Properties.Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(RadialGauge.ScaleWidth), new CategoryAttribute(Properties.Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(RadialGauge.NeedleBrush), new CategoryAttribute(Properties.Resources.CategoryBrush));
                    b.AddCustomAttributes(nameof(RadialGauge.Value), new CategoryAttribute(Properties.Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(RadialGauge.Unit), new CategoryAttribute(Properties.Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(RadialGauge.TrailBrush), new CategoryAttribute(Properties.Resources.CategoryBrush));
                    b.AddCustomAttributes(nameof(RadialGauge.ScaleBrush), new CategoryAttribute(Properties.Resources.CategoryBrush));
                    b.AddCustomAttributes(nameof(RadialGauge.TickBrush), new CategoryAttribute(Properties.Resources.CategoryBrush));
                    b.AddCustomAttributes(nameof(RadialGauge.ValueStringFormat), new CategoryAttribute(Properties.Resources.CategoryCommon));
                    b.AddCustomAttributes(nameof(RadialGauge.TickSpacing), new CategoryAttribute(Properties.Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(RadialGauge.NeedleLength), new CategoryAttribute(Properties.Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(RadialGauge.NeedleWidth), new CategoryAttribute(Properties.Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(RadialGauge.ScalePadding), new CategoryAttribute(Properties.Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(RadialGauge.ScaleTickWidth), new CategoryAttribute(Properties.Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(RadialGauge.TickWidth), new CategoryAttribute(Properties.Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(RadialGauge.TickLength), new CategoryAttribute(Properties.Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(RadialGauge.MinAngle), new CategoryAttribute(Properties.Resources.CategoryAppearance));
                    b.AddCustomAttributes(nameof(RadialGauge.MaxAngle), new CategoryAttribute(Properties.Resources.CategoryAppearance));
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Toolkit, false));
                }
            );
        }
    }
}