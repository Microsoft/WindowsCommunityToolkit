// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml.Media;

namespace CommunityToolkit.WinUI.UI
{
    /// <summary>
    /// Provides a set of extensions to the <see cref="o:Microsoft.UI.Xaml.Media.Matrix"/> struct.
    /// </summary>
    public static class MatrixExtensions
    {
        /// <summary>
        /// Implements WPF's <c>Matrix.HasInverse</c> logic.
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <returns>True if matrix has an inverse.</returns>
        public static bool HasInverse(this Matrix matrix)
        {
            return ((matrix.M11 * matrix.M22) - (matrix.M12 * matrix.M21)) != 0;
        }

        /// <summary>
        /// Multiply this matrix to the given matrix and return the result.
        /// </summary>
        /// <param name="matrix1">Initial matrix.</param>
        /// <param name="matrix2">Matrix to multiply by.</param>
        /// <returns>Multiplied Matrix</returns>
        public static Matrix Multiply(this Matrix matrix1, Matrix matrix2)
        {
            return new(
                (matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21),
                (matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22),
                (matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21),
                (matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22),
                (matrix1.OffsetX * matrix2.M11) + (matrix1.OffsetY * matrix2.M21) + matrix2.OffsetX,
                (matrix1.OffsetX * matrix2.M12) + (matrix1.OffsetY * matrix2.M22) + matrix2.OffsetY);
        }

        /// <summary>
        /// Rounds the non-offset elements of a matrix to avoid issues due to floating point imprecision and returns the result.
        /// </summary>
        /// <param name="matrix">The matrix to round.</param>
        /// <param name="decimalsAfterRound">The number of decimals after the round.</param>
        /// <returns>The rounded matrix.</returns>
        public static Matrix Round(this Matrix matrix, int decimalsAfterRound)
        {
            return new(
                Math.Round(matrix.M11, decimalsAfterRound),
                Math.Round(matrix.M12, decimalsAfterRound),
                Math.Round(matrix.M21, decimalsAfterRound),
                Math.Round(matrix.M22, decimalsAfterRound),
                matrix.OffsetX,
                matrix.OffsetY);
        }

        /// <summary>
        /// Applies a rotation of the specified angle about the origin of this Matrix structure and returns the result.
        /// </summary>
        /// <param name="matrix">Matrix to extend.</param>
        /// <param name="angle">The angle of rotation in degrees.</param>
        /// <returns>Rotated Matrix.</returns>
        public static Matrix Rotate(this Matrix matrix, double angle)
        {
            return matrix.Multiply(CreateRotationRadians((angle % 360) * (Math.PI / 180.0)));
        }

        /// <summary>
        /// Rotates this matrix about the specified point and returns the new result.
        /// </summary>
        /// <param name="matrix">Matrix to extend.</param>
        /// <param name="angle">The angle of rotation in degrees.</param>
        /// <param name="centerX">The x-coordinate of the point about which to rotate this matrix.</param>
        /// <param name="centerY">The y-coordinate of the point about which to rotate this matrix.</param>
        /// <returns>Rotated Matrix.</returns>
        public static Matrix RotateAt(this Matrix matrix, double angle, double centerX, double centerY)
        {
            return matrix.Multiply(CreateRotationRadians((angle % 360) * (Math.PI / 180.0), centerX, centerY));
        }

        /// <summary>
        /// Appends the specified scale vector to this Matrix structure and returns the result.
        /// </summary>
        /// <param name="matrix">Matrix to extend.</param>
        /// <param name="scaleX">The value by which to scale this Matrix along the x-axis.</param>
        /// <param name="scaleY">The value by which to scale this Matrix along the y-axis.</param>
        /// <returns>Scaled Matrix.</returns>
        public static Matrix Scale(this Matrix matrix, double scaleX, double scaleY)
        {
            return matrix.Multiply(CreateScaling(scaleX, scaleY));
        }

        /// <summary>
        /// Scales this Matrix by the specified amount about the specified point and returns the result.
        /// </summary>
        /// <param name="matrix">Matrix to extend.</param>
        /// <param name="scaleX">The value by which to scale this Matrix along the x-axis.</param>
        /// <param name="scaleY">The value by which to scale this Matrix along the y-axis.</param>
        /// <param name="centerX">The x-coordinate of the scale operation's center point.</param>
        /// <param name="centerY">The y-coordinate of the scale operation's center point.</param>
        /// <returns>Scaled Matrix.</returns>
        public static Matrix ScaleAt(this Matrix matrix, double scaleX, double scaleY, double centerX, double centerY)
        {
            return matrix.Multiply(CreateScaling(scaleX, scaleY, centerX, centerY));
        }

        /// <summary>
        /// Appends a skew of the specified degrees in the x and y dimensions to this Matrix structure and returns the result.
        /// </summary>
        /// <param name="matrix">Matrix to extend.</param>
        /// <param name="skewX">The angle in the x dimension by which to skew this Matrix.</param>
        /// <param name="skewY">The angle in the y dimension by which to skew this Matrix.</param>
        /// <returns>Skewed Matrix.</returns>
        public static Matrix Skew(this Matrix matrix, double skewX, double skewY)
        {
            return matrix.Multiply(CreateSkewRadians((skewX % 360) * (Math.PI / 180.0), (skewY % 360) * (Math.PI / 180.0)));
        }

        /// <summary>
        /// Translates the matrix by the given amount and returns the result.
        /// </summary>
        /// <param name="matrix">Matrix to extend.</param>
        /// <param name="offsetX">The offset in the x dimension.</param>
        /// <param name="offsetY">The offset in the y dimension.</param>
        /// <returns>Translated Matrix.</returns>
        public static Matrix Translate(this Matrix matrix, double offsetX, double offsetY)
        {
            return new(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.OffsetX + offsetX, matrix.OffsetY + offsetY);
        }

        private static Matrix CreateRotationRadians(double angle)
        {
            return CreateRotationRadians(angle, 0, 0);
        }

        private static Matrix CreateRotationRadians(double angle, double centerX, double centerY)
        {
            var sin = Math.Sin(angle);
            var cos = Math.Cos(angle);
            var dx = (centerX * (1.0 - cos)) + (centerY * sin);
            var dy = (centerY * (1.0 - cos)) - (centerX * sin);

            return new(cos, sin, -sin, cos, dx, dy);
        }

        private static Matrix CreateScaling(double scaleX, double scaleY)
        {
            return new(scaleX, 0, 0, scaleY, 0, 0);
        }

        private static Matrix CreateScaling(double scaleX, double scaleY, double centerX, double centerY)
        {
            return new(scaleX, 0, 0, scaleY, centerX - (scaleX * centerX), centerY - (scaleY * centerY));
        }

        private static Matrix CreateSkewRadians(double skewX, double skewY)
        {
            return new(1.0, Math.Tan(skewY), Math.Tan(skewX), 1.0, 0.0, 0.0);
        }
    }
}