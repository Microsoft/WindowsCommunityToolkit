// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input.Inking;

namespace CommunityToolkit.WinUI.UI.Controls
{
    internal class CustomInkDrawingAttribute
    {
        public Color Color { get; set; }

        public bool FitToCurve { get; set; }

        public bool IgnorePressure { get; set; }

        public bool IgnoreTilt { get; set; }

        public Size Size { get; set; }

        public PenTipShape PenTip { get; set; }

        public Matrix3x2 PenTipTransform { get; set; }

        public bool DrawAsHighlighter { get; set; }
    }
}