// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml.Media;

namespace CommunityToolkit.WinUI.UI
{
    /// <summary>
    /// Extension methods for <see cref="SkewTransform"/>.
    /// </summary>
    public static class SkewTransformExtensions
    {
        /// <summary>
        /// Gets the matrix that represents this transform.
        /// Implements WPF's SkewTransform.Value.
        /// </summary>
        /// <param name="transform">Extended SkewTranform.</param>
        /// <returns>Matrix representing transform.</returns>
        public static Matrix GetMatrix(this SkewTransform transform)
        {
            Matrix matrix = Matrix.Identity;

            var angleX = transform.AngleX;
            var angleY = transform.AngleY;
            var centerX = transform.CenterX;
            var centerY = transform.CenterY;

            bool hasCenter = centerX != 0 || centerY != 0;

            if (hasCenter)
            {
                // If we have a center, translate matrix before/after skewing.
                matrix = matrix.Translate(-centerX, -centerY);
                matrix = matrix.Skew(angleX, angleY);
                matrix = matrix.Translate(centerX, centerY);
            }
            else
            {
                matrix = matrix.Skew(angleX, angleY);
            }

            return matrix;
        }
    }
}