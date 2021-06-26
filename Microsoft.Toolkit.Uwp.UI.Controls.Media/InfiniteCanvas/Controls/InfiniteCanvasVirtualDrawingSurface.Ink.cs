// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Input.Inking;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// The virtual Drawing surface renderer used to render the ink and text. This control is used as part of the <see cref="InfiniteCanvas"/>
    /// </summary>
    public partial class InfiniteCanvasVirtualDrawingSurface
    {
        internal List<InkStroke> ExportInkStrokes()
        {
            return _drawableList.OfType<InkDrawable>().SelectMany(id => id.Strokes).ToList();
        }

        internal void Erase(Point point, Rect viewPort, float zoomFactor)
        {
            const int tolerance = 5;
            float toleranceWithZoom = tolerance;
            if (zoomFactor > 1)
            {
                toleranceWithZoom /= zoomFactor;
            }

            for (var i = _visibleList.Count - 1; i >= 0; i--)
            {
                var drawable = _visibleList[i];
                if (drawable is InkDrawable inkDrawable && drawable.Bounds.Contains(point))
                {
                    foreach (var stroke in inkDrawable.Strokes)
                    {
                        if (stroke.BoundingRect.Contains(point))
                        {
                            foreach (var inkPoint in stroke.GetInkPoints())
                            {
                                if (Math.Abs(point.X - inkPoint.Position.X) < toleranceWithZoom && Math.Abs(point.Y - inkPoint.Position.Y) < toleranceWithZoom)
                                {
                                    var toRemove = _visibleList.ElementAt(i);
                                    ExecuteEraseInk(toRemove);
                                    ReDraw(viewPort, zoomFactor);

                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}