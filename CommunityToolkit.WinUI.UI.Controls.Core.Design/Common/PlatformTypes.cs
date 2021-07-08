// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.VisualStudio.DesignTools.Extensibility;
using Microsoft.VisualStudio.DesignTools.Extensibility.Metadata;

namespace CommunityToolkit.WinUI.Design.Types
{
    internal class PlatformTypes
    {
        public static readonly Type DependencyObject = typeof(DependencyObject);
        public static readonly Type UIElement = typeof(UIElement);
        public static readonly Type FrameworkElement = typeof(FrameworkElement);
        public static readonly Type Control = typeof(Control);
    }

    internal class XamlTypes
    {
        public static class FrameworkElement
        {
            public static readonly TypeIdentifier TypeId = new TypeIdentifier("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "FrameworkElement");
            public static readonly PropertyIdentifier MarginProperty = new PropertyIdentifier(TypeId, "Margin");
            public static readonly PropertyIdentifier HorizontalAlignmentProperty = new PropertyIdentifier(TypeId, "HorizontalAlignment");
            public static readonly PropertyIdentifier VerticalAlignmentProperty = new PropertyIdentifier(TypeId, "VerticalAlignment");
            public static readonly PropertyIdentifier HeightProperty = new PropertyIdentifier(TypeId, "Height");
            public static readonly PropertyIdentifier WidthProperty = new PropertyIdentifier(TypeId, "Width");
        }

        public static class Control
        {
            public static readonly TypeIdentifier TypeId = new TypeIdentifier("http://schemas.microsoft.com/winfx/2006/xaml/presentation", "Control");
            public static readonly PropertyIdentifier BackgroundProperty = new PropertyIdentifier(TypeId, "Background");
            public static readonly PropertyIdentifier BorderBrushProperty = new PropertyIdentifier(TypeId, "BorderBrush");
            public static readonly PropertyIdentifier BorderThicknessProperty = new PropertyIdentifier(TypeId, "BorderThickness");
        }
    }
}