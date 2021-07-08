// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// WrapPanel is a panel that position child control vertically or horizontally based on the orientation and when max width/ max height is received a new row(in case of horizontal) or column (in case of vertical) is created to fit new controls.
    /// </summary>
    public partial class WrapPanel
    {
        [global::System.Diagnostics.DebuggerDisplay("U = {U} V = {V}")]
        private struct UvMeasure
        {
            internal static UvMeasure Zero => default;

            internal double U { get; set; }

            internal double V { get; set; }

            public UvMeasure(Orientation orientation, Size size)
                : this(orientation, size.Width, size.Height)
            {
            }

            public UvMeasure(Orientation orientation, double width, double height)
            {
                if (orientation == Orientation.Horizontal)
                {
                    U = width;
                    V = height;
                }
                else
                {
                    U = height;
                    V = width;
                }
            }

            public UvMeasure Add(double u, double v)
                => new UvMeasure { U = U + u, V = V + v };

            public UvMeasure Add(UvMeasure measure)
                => Add(measure.U, measure.V);

            public Size ToSize(Orientation orientation)
                => orientation == Orientation.Horizontal ? new Size(U, V) : new Size(V, U);
        }

        private struct UvRect
        {
            public UvMeasure Position { get; set; }

            public UvMeasure Size { get; set; }

            public Rect ToRect(Orientation orientation) => orientation switch
            {
                Orientation.Vertical => new Rect(Position.V, Position.U, Size.V, Size.U),
                Orientation.Horizontal => new Rect(Position.U, Position.V, Size.U, Size.V),
                _ => ThrowArgumentException()
            };

            private static Rect ThrowArgumentException() => throw new ArgumentException("The input orientation is not valid.");
        }

        private struct Row
        {
            public Row(List<UvRect> childrenRects, UvMeasure size)
            {
                ChildrenRects = childrenRects;
                Size = size;
            }

            public List<UvRect> ChildrenRects { get; }

            public UvMeasure Size { get; set; }

            public UvRect Rect => ChildrenRects.Count > 0 ?
                new UvRect { Position = ChildrenRects[0].Position, Size = Size } :
                new UvRect { Position = UvMeasure.Zero, Size = Size };

            public void Add(UvMeasure position, UvMeasure size)
            {
                ChildrenRects.Add(new UvRect { Position = position, Size = size });
                Size = new UvMeasure
                {
                    U = position.U + size.U,
                    V = Math.Max(Size.V, size.V),
                };
            }
        }
    }
}