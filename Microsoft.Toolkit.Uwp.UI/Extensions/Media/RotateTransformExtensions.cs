// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Extension methods for <see cref="RotateTransform"/>.
    /// </summary>
    public static class RotateTransformExtensions
    {
        /// <summary>
        /// Gets the matrix that represents this transform.
        /// Implements WPF's SkewTransform.Value.
        /// </summary>
        /// <param name="transform">Extended SkewTranform.</param>
        /// <returns>Matrix representing transform.</returns>
        public static Matrix GetMatrix(this RotateTransform transform)
        {
            return Matrix.Identity.RotateAt(transform.Angle, transform.CenterX, transform.CenterY);
        }
    }
}