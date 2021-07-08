// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Rect = Windows.Foundation.Rect;

namespace CommunityToolkit.WinUI.UI
{
    /// <summary>
    /// Provides a set of extensions to the <see cref="Rect"/> struct.
    /// </summary>
    public static class RectExtensions
    {
        /// <summary>
        /// Implement WPF's <c>Rect.Transform(Matrix)</c> logic.
        /// </summary>
        /// <param name="rectangle">The rectangle to transform.</param>
        /// <param name="matrix">The matrix to use to transform the rectangle.
        /// </param>
        /// <returns>The transformed rectangle.</returns>
        [Pure]
        public static Rect Transform(this Rect rectangle, Matrix matrix)
        {
            Point leftTop = matrix.Transform(new Point(rectangle.Left, rectangle.Top));
            Point rightTop = matrix.Transform(new Point(rectangle.Right, rectangle.Top));
            Point leftBottom = matrix.Transform(new Point(rectangle.Left, rectangle.Bottom));
            Point rightBottom = matrix.Transform(new Point(rectangle.Right, rectangle.Bottom));

            double left = Math.Min(Math.Min(leftTop.X, rightTop.X), Math.Min(leftBottom.X, rightBottom.X));
            double top = Math.Min(Math.Min(leftTop.Y, rightTop.Y), Math.Min(leftBottom.Y, rightBottom.Y));
            double right = Math.Max(Math.Max(leftTop.X, rightTop.X), Math.Max(leftBottom.X, rightBottom.X));
            double bottom = Math.Max(Math.Max(leftTop.Y, rightTop.Y), Math.Max(leftBottom.Y, rightBottom.Y));

            return new (left, top, right - left, bottom - top);
        }
    }
}