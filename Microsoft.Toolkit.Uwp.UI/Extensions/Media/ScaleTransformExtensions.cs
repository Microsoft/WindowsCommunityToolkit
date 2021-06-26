// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Extension methods for <see cref="ScaleTransform"/>.
    /// </summary>
    public static class ScaleTransformExtensions
    {
        /// <summary>
        /// Gets the matrix that represents this transform.
        /// Implements WPF's SkewTransform.Value.
        /// </summary>
        /// <param name="transform">Extended SkewTransform.</param>
        /// <returns>Matrix representing transform.</returns>
        public static Matrix GetMatrix(this ScaleTransform transform)
        {
            return Matrix.Identity.ScaleAt(transform.ScaleX, transform.ScaleY, transform.CenterX, transform.CenterY);
        }
    }
}