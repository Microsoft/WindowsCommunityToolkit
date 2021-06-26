// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Media.Geometry
{
    /// <summary>
    /// Class containing collection of useful methods for various types
    /// </summary>
    public static class Utils
    {
        // Constant values

        // Smallest double value such that 1.0 + DoubleEpsilon != 1.0
        internal const double DoubleEpsilon = 2.2250738585072014E-308;

        // Smallest float value such that 1.0f + FloatMin != 1.0f
        internal const float FloatMin = 1.175494351E-38F;

        /// <summary>
        /// Returns whether or not two doubles are "close".
        /// </summary>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        /// <returns>
        /// bool - the result of the AreClose comparison.
        /// </returns>
        public static bool IsCloseTo(this double value1, double value2)
        {
            // In case they are Infinities or NaN (then epsilon check does not work)
            if ((double.IsInfinity(value1) &&
                 double.IsInfinity(value2)) ||
                (double.IsNaN(value1) && double.IsNaN(value2)))
            {
                return true;
            }

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DoubleEpsilon
            var eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DoubleEpsilon;
            var delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        /// <summary>
        /// Returns whether or not the first double is less than the second double.
        /// </summary>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        /// <returns>
        /// bool - the result of the LessThan comparison.
        /// </returns>
        public static bool IsLessThan(this double value1, double value2)
        {
            return (value1 < value2) && !value1.IsCloseTo(value2);
        }

        /// <summary>
        /// Returns whether or not the first double is greater than the second double.
        /// </summary>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        /// <returns>
        /// bool - the result of the GreaterThan comparison.
        /// </returns>
        public static bool IsGreaterThan(this double value1, double value2)
        {
            return (value1 > value2) && !value1.IsCloseTo(value2);
        }

        /// <summary>
        /// Returns whether or not the double is "close" to 1.  Same as AreClose(double, 1),
        /// but this is faster.
        /// </summary>
        /// <param name="value"> The double to compare to 1. </param>
        /// <returns>
        /// bool - the result of the AreClose comparison.
        /// </returns>
        public static bool IsOne(this double value)
        {
            return Math.Abs(value - 1.0d) < 10.0d * DoubleEpsilon;
        }

        /// <summary>
        /// IsZero - Returns whether or not the double is "close" to 0.  Same as AreClose(double, 0),
        /// but this is faster.
        /// </summary>
        /// <param name="value"> The double to compare to 0. </param>
        /// <returns>
        /// bool - the result of the AreClose comparison.
        /// </returns>
        public static bool IsZero(this double value)
        {
            return Math.Abs(value) < 10.0d * DoubleEpsilon;
        }

        /// <summary>
        /// Returns whether or not two floats are "close".
        /// </summary>
        /// <param name="value1"> The first float to compare. </param>
        /// <param name="value2"> The second float to compare. </param>
        /// <returns>
        /// bool - the result of the AreClose comparison.
        /// </returns>
        public static bool IsCloseTo(this float value1, float value2)
        {
            // In case they are Infinities or NaN (then epsilon check does not work)
            if ((float.IsInfinity(value1) &&
                 float.IsInfinity(value2)) ||
                (float.IsNaN(value1) && float.IsNaN(value2)))
            {
                return true;
            }

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < FloatMin
            var eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0f) * FloatMin;
            var delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        /// <summary>
        /// Returns whether or not the first float is less than the second float.
        /// </summary>
        /// <param name="value1"> The first float to compare. </param>
        /// <param name="value2"> The second float to compare. </param>
        /// <returns>
        /// bool - the result of the LessThan comparison.
        /// </returns>
        public static bool IsLessThan(this float value1, float value2)
        {
            return (value1 < value2) && !value1.IsCloseTo(value2);
        }

        /// <summary>
        /// Returns whether or not the first float is greater than the second float.
        /// </summary>
        /// <param name="value1"> The first float to compare. </param>
        /// <param name="value2"> The second float to compare. </param>
        /// <returns>
        /// bool - the result of the GreaterThan comparison.
        /// </returns>
        public static bool IsGreaterThan(this float value1, float value2)
        {
            return (value1 > value2) && !value1.IsCloseTo(value2);
        }

        /// <summary>
        /// Returns whether or not the float is "close" to 1.  Same as AreClose(float, 1),
        /// but this is faster.
        /// </summary>
        /// <param name="value"> The float to compare to 1. </param>
        /// <returns>
        /// bool - the result of the AreClose comparison.
        /// </returns>
        public static bool IsOne(this float value)
        {
            return Math.Abs(value - 1.0f) < 10.0f * FloatMin;
        }

        /// <summary>
        /// IsZero - Returns whether or not the float is "close" to 0.  Same as AreClose(float, 0),
        /// but this is faster.
        /// </summary>
        /// <param name="value"> The float to compare to 0. </param>
        /// <returns>
        /// bool - the result of the AreClose comparison.
        /// </returns>
        public static bool IsZero(this float value)
        {
            return Math.Abs(value) < 10.0f * FloatMin;
        }

        /// <summary>
        /// Compares two points for fuzzy equality.  This function
        /// helps compensate for the fact that double values can
        /// acquire error when operated upon
        /// </summary>
        /// <param name='point1'>The first point to compare</param>
        /// <param name='point2'>The second point to compare</param>
        /// <returns>Whether or not the two points are equal</returns>
        public static bool IsCloseTo(this Point point1, Point point2)
        {
            return point1.X.IsCloseTo(point2.X) && point1.Y.IsCloseTo(point2.Y);
        }

        /// <summary>
        /// Compares two Size instances for fuzzy equality.  This function
        /// helps compensate for the fact that double values can
        /// acquire error when operated upon
        /// </summary>
        /// <param name='size1'>The first size to compare</param>
        /// <param name='size2'>The second size to compare</param>
        /// <returns>Whether or not the two Size instances are equal</returns>
        public static bool IsCloseTo(this Size size1, Size size2)
        {
            return size1.Width.IsCloseTo(size2.Width) && size1.Height.IsCloseTo(size2.Height);
        }

        /// <summary>
        /// Compares two rectangles for fuzzy equality.  This function
        /// helps compensate for the fact that double values can
        /// acquire error when operated upon
        /// </summary>
        /// <param name='rect1'>The first rectangle to compare</param>
        /// <param name='rect2'>The second rectangle to compare</param>
        /// <returns>Whether or not the two rectangles are equal</returns>
        public static bool IsCloseTo(this Rect rect1, Rect rect2)
        {
            // If they're both empty, don't bother with the double logic.
            if (rect1.IsEmpty)
            {
                return rect2.IsEmpty;
            }

            // At this point, rect1 isn't empty, so the first thing we can test is rect2.IsEmpty, followed by property-wise compares.
            return (!rect2.IsEmpty)
                   && rect1.X.IsCloseTo(rect2.X) && rect1.Y.IsCloseTo(rect2.Y)
                   && rect1.Height.IsCloseTo(rect2.Height) && rect1.Width.IsCloseTo(rect2.Width);
        }

        /// <summary>
        /// Rounds the given value based on the DPI scale
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="dpiScale">DPI Scale</param>
        /// <returns>The rounded value</returns>
        public static double RoundLayoutValue(double value, double dpiScale)
        {
            double newValue;

            // If DPI == 1, don't use DPI-aware rounding.
            if (!dpiScale.IsCloseTo(1.0))
            {
                newValue = Math.Round(value * dpiScale) / dpiScale;

                // If rounding produces a value unacceptable to layout (NaN, Infinity or MaxValue), use the original value.
                if (double.IsNaN(newValue) ||
                    double.IsInfinity(newValue) ||
                    newValue.IsCloseTo(double.MaxValue))
                {
                    newValue = value;
                }
            }
            else
            {
                newValue = Math.Round(value);
            }

            return newValue;
        }

        /// <summary>
        /// Calculates the linear interpolated value based on the given values.
        /// </summary>
        /// <param name="start">Starting value.</param>
        /// <param name="end">Ending value.</param>
        /// <param name="amount">Weight-age given to the ending value.</param>
        /// <returns>Linear interpolated value.</returns>
        public static float Lerp(this float start, float end, float amount)
        {
            return start + ((end - start) * amount);
        }

        /// <summary>
        /// Verifies if this Thickness contains only valid values. The set of validity checks is passed as parameters.
        /// </summary>
        /// <param name='thick'>Thickness value</param>
        /// <param name='allowNegative'>allows negative values</param>
        /// <param name='allowNaN'>allows double.NaN</param>
        /// <param name='allowPositiveInfinity'>allows double.PositiveInfinity</param>
        /// <param name='allowNegativeInfinity'>allows double.NegativeInfinity</param>
        /// <returns>Whether or not the thickness complies to the range specified</returns>
        public static bool IsValid(this Thickness thick, bool allowNegative, bool allowNaN, bool allowPositiveInfinity, bool allowNegativeInfinity)
        {
            if (!allowNegative)
            {
                if (thick.Left < 0d || thick.Right < 0d || thick.Top < 0d || thick.Bottom < 0d)
                {
                    return false;
                }
            }

            if (!allowNaN)
            {
                if (double.IsNaN(thick.Left) || double.IsNaN(thick.Right)
                                      || double.IsNaN(thick.Top) || double.IsNaN(thick.Bottom))
                {
                    return false;
                }
            }

            if (!allowPositiveInfinity)
            {
                if (double.IsPositiveInfinity(thick.Left) || double.IsPositiveInfinity(thick.Right)
                    || double.IsPositiveInfinity(thick.Top) || double.IsPositiveInfinity(thick.Bottom))
                {
                    return false;
                }
            }

            if (!allowNegativeInfinity)
            {
                if (double.IsNegativeInfinity(thick.Left) || double.IsNegativeInfinity(thick.Right)
                    || double.IsNegativeInfinity(thick.Top) || double.IsNegativeInfinity(thick.Bottom))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Method to add up the left and right size as width, as well as the top and bottom size as height.
        /// </summary>
        /// <param name="thick">Thickness</param>
        /// <returns>Size</returns>
        public static Size CollapseThickness(this Thickness thick)
        {
            return new Size(thick.Left + thick.Right, thick.Top + thick.Bottom);
        }

        /// <summary>
        /// Verifies if the Thickness contains only zero values.
        /// </summary>
        /// <param name="thick">Thickness</param>
        /// <returns>Size</returns>
        public static bool IsZero(this Thickness thick)
        {
            return thick.Left.IsZero()
                    && thick.Top.IsZero()
                    && thick.Right.IsZero()
                    && thick.Bottom.IsZero();
        }

        /// <summary>
        /// Verifies if all the values in Thickness are same.
        /// </summary>
        /// <param name="thick">Thickness</param>
        /// <returns>true if yes, otherwise false</returns>
        public static bool IsUniform(this Thickness thick)
        {
            return thick.Left.IsCloseTo(thick.Top)
                    && thick.Left.IsCloseTo(thick.Right)
                    && thick.Left.IsCloseTo(thick.Bottom);
        }

        /// <summary>
        /// Converts the Thickness object to Vector4. If the Thickness object's component have values NaN, PositiveInfinity or NegativeInfinity, then Vector4.Zero will be returned.
        /// </summary>
        /// <param name="thickness">Thickness object</param>
        /// <returns>Vector4</returns>
        public static Vector4 ToVector4(this Thickness thickness)
        {
            if (thickness.IsValid(true, false, false, false))
            {
                // Sanitize the component by taking only
                return new Vector4(
                    (float)thickness.Left,
                    (float)thickness.Top,
                    (float)thickness.Right,
                    (float)thickness.Bottom);
            }

            return Vector4.Zero;
        }

        /// <summary>
        /// Converts the Thickness object to Vector4. If the Thickness object contains negative components they will be converted to positive values. If the Thickness object's component have values NaN, PositiveInfinity or NegativeInfinity, then Vector4.Zero will be returned.
        /// </summary>
        /// <param name="thickness">Thickness object</param>
        /// <returns>Vector2</returns>
        public static Vector4 ToAbsVector4(this Thickness thickness)
        {
            if (thickness.IsValid(true, false, false, false))
            {
                // Sanitize the component by taking only
                return new Vector4(
                    Math.Abs((float)thickness.Left),
                    Math.Abs((float)thickness.Top),
                    Math.Abs((float)thickness.Right),
                    Math.Abs((float)thickness.Bottom));
            }

            return Vector4.Zero;
        }

        /// <summary>
        /// Gets the top left corner of the thickness structure.
        /// </summary>
        /// <param name="thickness">Thickness object</param>
        /// <returns>Vector2</returns>
        public static Vector2 GetOffset(this Thickness thickness)
        {
            return new Vector2((float)thickness.Left, (float)thickness.Top);
        }

        /// <summary>
        /// Verifies if the CornerRadius contains only zero values.
        /// </summary>
        /// <param name="corner">CornerRadius</param>
        /// <returns>true if yes, otherwise false</returns>
        public static bool IsZero(this CornerRadius corner)
        {
            return corner.TopLeft.IsZero()
                   && corner.TopRight.IsZero()
                   && corner.BottomRight.IsZero()
                   && corner.BottomLeft.IsZero();
        }

        /// <summary>
        /// Verifies if the CornerRadius contains same values.
        /// </summary>
        /// <param name="corner">CornerRadius</param>
        /// <returns>true if yes, otherwise false</returns>
        public static bool IsUniform(this CornerRadius corner)
        {
            var topLeft = corner.TopLeft;
            return topLeft.IsCloseTo(corner.TopRight) &&
                   topLeft.IsCloseTo(corner.BottomLeft) &&
                   topLeft.IsCloseTo(corner.BottomRight);
        }

        /// <summary>
        /// Converts the given corner value to a valid positive value. Returns zero if the corner value is Infinity or NaN or 0.
        /// </summary>
        /// <param name="corner">Corner value</param>
        /// <returns>Valid Corner value</returns>
        public static double ConvertToValidCornerValue(double corner)
        {
            if (double.IsNaN(corner) ||
                double.IsInfinity(corner) ||
                (corner < 0d))
            {
                return 0d;
            }

            return corner;
        }

        /// <summary>
        /// Converts the CornerRadius object to Vector4. If the CornerRadius object's component have values NaN, PositiveInfinity or NegativeInfinity, then Vector4.Zero will be returned.
        /// </summary>
        /// <param name="corner">CornerRadius object</param>
        /// <returns>Vector4</returns>
        public static Vector4 ToVector4(this CornerRadius corner)
        {
            return new Vector4(
                    (float)corner.TopLeft,
                    (float)corner.TopRight,
                    (float)corner.BottomRight,
                    (float)corner.BottomLeft);
        }

        /// <summary>
        /// Deflates rectangle by given thickness.
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <param name="thick">Thickness</param>
        /// <returns>Deflated Rectangle</returns>
        public static Rect Deflate(this Rect rect, Thickness thick)
        {
            return new Rect(
                rect.Left + thick.Left,
                rect.Top + thick.Top,
                Math.Max(0.0, rect.Width - thick.Left - thick.Right),
                Math.Max(0.0, rect.Height - thick.Top - thick.Bottom));
        }

        /// <summary>
        /// Inflates rectangle by given thickness.
        /// </summary>
        /// <param name="rect">Rectangle</param>
        /// <param name="thick">Thickness</param>
        /// <returns>Inflated Rectangle</returns>
        public static Rect Inflate(this Rect rect, Thickness thick)
        {
            return new Rect(
                rect.Left - thick.Left,
                rect.Top - thick.Top,
                Math.Max(0.0, rect.Width + thick.Left + thick.Right),
                Math.Max(0.0, rect.Height + thick.Top + thick.Bottom));
        }

        /// <summary>
        /// Verifies if the given brush is a SolidColorBrush and its color does not include transparency.
        /// </summary>
        /// <param name="brush">Brush</param>
        /// <returns>true if yes, otherwise false</returns>
        public static bool IsOpaqueSolidColorBrush(this Brush brush)
        {
            return (brush as SolidColorBrush)?.Color.A == 0xff;
        }

        /// <summary>
        /// Verifies if the given brush is the same as the otherBrush.
        /// </summary>
        /// <param name="brush">Given <see cref="Brush"/></param>
        /// <param name="otherBrush">The <see cref="Brush"/> to match it with</param>
        /// <returns>true if yes, otherwise false</returns>
        public static bool IsEqualTo(this Brush brush, Brush otherBrush)
        {
            if (brush.GetType() != otherBrush.GetType())
            {
                return false;
            }

            if (ReferenceEquals(brush, otherBrush))
            {
                return true;
            }

            // Are both instances of SolidColorBrush
            if ((brush is SolidColorBrush solidBrushA) && (otherBrush is SolidColorBrush solidBrushB))
            {
                return (solidBrushA.Color == solidBrushB.Color)
                       && solidBrushA.Opacity.IsCloseTo(solidBrushB.Opacity);
            }

            // Are both instances of LinearGradientBrush
            if ((brush is LinearGradientBrush linGradBrushA) && (otherBrush is LinearGradientBrush linGradBrushB))
            {
                var result = (linGradBrushA.ColorInterpolationMode == linGradBrushB.ColorInterpolationMode)
                               && (linGradBrushA.EndPoint == linGradBrushB.EndPoint)
                               && (linGradBrushA.MappingMode == linGradBrushB.MappingMode)
                               && linGradBrushA.Opacity.IsCloseTo(linGradBrushB.Opacity)
                               && (linGradBrushA.StartPoint == linGradBrushB.StartPoint)
                               && (linGradBrushA.SpreadMethod == linGradBrushB.SpreadMethod)
                               && (linGradBrushA.GradientStops.Count == linGradBrushB.GradientStops.Count);
                if (!result)
                {
                    return false;
                }

                for (var i = 0; i < linGradBrushA.GradientStops.Count; i++)
                {
                    result = (linGradBrushA.GradientStops[i].Color == linGradBrushB.GradientStops[i].Color)
                             && linGradBrushA.GradientStops[i].Offset.IsCloseTo(linGradBrushB.GradientStops[i].Offset);

                    if (!result)
                    {
                        break;
                    }
                }

                return result;
            }

            // Are both instances of ImageBrush
            if ((brush is ImageBrush imgBrushA) && (otherBrush is ImageBrush imgBrushB))
            {
                var result = (imgBrushA.AlignmentX == imgBrushB.AlignmentX)
                              && (imgBrushA.AlignmentY == imgBrushB.AlignmentY)
                              && imgBrushA.Opacity.IsCloseTo(imgBrushB.Opacity)
                              && (imgBrushA.Stretch == imgBrushB.Stretch)
                              && (imgBrushA.ImageSource == imgBrushB.ImageSource);

                return result;
            }

            return false;
        }

        /// <summary>
        /// Compares one URI with another URI.
        /// </summary>
        /// <param name="uri">URI to compare with</param>
        /// <param name="otherUri">URI to compare</param>
        /// <returns>true if yes, otherwise false</returns>
        public static bool IsEqualTo(this Uri uri, Uri otherUri)
        {
            return
                Uri.Compare(uri, otherUri, UriComponents.AbsoluteUri, UriFormat.SafeUnescaped, StringComparison.OrdinalIgnoreCase) == 0;
        }

        /// <summary>
        /// Reflects point 'a' over point 'b'.
        /// </summary>
        /// <param name="a">Point to be reflected</param>
        /// <param name="b">Point of reflection</param>
        /// <returns>Reflected point</returns>
        public static Vector2 Reflect(Vector2 a, Vector2 b)
        {
            // Let 'c' be the reflected point. Then point 'b'
            // becomes the middle point between 'a' and 'c'.
            // As per MidPoint formula,
            // b.X = (a.X + c.X) / 2 and
            // b.Y = (a.Y + c.Y) / 2
            // Therefore, c.X = 2 * b.X - a.X
            //            c.y = 2 * b.Y - a.Y
            return new Vector2((2f * b.X) - a.X, (2f * b.Y) - a.Y);
        }

        /// <summary>
        /// Converts a Vector2 structure (x,y) to Vector3 structure (x, y, 0).
        /// </summary>
        /// <param name="v">Input Vector2</param>
        /// <returns>Vector3</returns>
        public static Vector3 ToVector3(this Vector2 v)
        {
            return new Vector3(v, 0);
        }

        /// <summary>
        /// Verifies if the Vector4 contains only zero values.
        /// </summary>
        /// <param name="vector">Vector4</param>
        /// <returns>true if yes, otherwise false</returns>
        public static bool IsZero(this Vector4 vector)
        {
            return vector.X.IsZero()
                   && vector.Y.IsZero()
                   && vector.Z.IsZero()
                   && vector.W.IsZero();
        }

        /// <summary>
        /// Useful in converting the four components of Thickness or Padding to two components by taking a sum of alternate components (X &amp; Z and Y &amp; W).
        /// </summary>
        /// <param name="vector">Vector4</param>
        /// <returns>Vector3</returns>
        public static Vector2 Collapse(this Vector4 vector)
        {
            return new Vector2(vector.X + vector.Z, vector.Y + vector.W);
        }

        /// <summary>
        /// Useful in converting the four components of Thickness or Padding to two components by adding alternate components - (X &amp; Z and Y &amp; W).
        /// </summary>
        /// <param name="vector">Vector4</param>
        /// <returns>Size</returns>
        public static Size ToSize(this Vector4 vector)
        {
            return new Size(vector.X + vector.Z, vector.Y + vector.W);
        }

        /// <summary>
        /// Converts the Vector4 to Thickness - Left(X), Top(Y), Right(Z), Bottom(W).
        /// </summary>
        /// <param name="vector">Vector4</param>
        /// <returns>Thickness</returns>
        public static Thickness ToThickness(this Vector4 vector)
        {
            return new Thickness(vector.X, vector.Y, vector.Z, vector.W);
        }

        /// <summary>
        /// Converts the Vector4 to CornerRadius - TopLeft(X), TopRight(Y), BottomRight(Z), BottomLeft(W).
        /// </summary>
        /// <param name="vector">Vector4</param>
        /// <returns>CornerRadius</returns>
        public static CornerRadius ToCornerRadius(this Vector4 vector)
        {
            return new CornerRadius(vector.X, vector.Y, vector.Z, vector.W);
        }

        /// <summary>
        /// Calculates the linear interpolated Color based on the given Color values.
        /// </summary>
        /// <param name="colorFrom">Source Color.</param>
        /// <param name="colorTo">Target Color.</param>
        /// <param name="amount">Weightage given to the target color.</param>
        /// <returns>Linear Interpolated Color.</returns>
        public static Color Lerp(this Color colorFrom, Color colorTo, float amount)
        {
            // Convert colorFrom components to lerp-able floats
            float sa = colorFrom.A,
                sr = colorFrom.R,
                sg = colorFrom.G,
                sb = colorFrom.B;

            // Convert colorTo components to lerp-able floats
            float ea = colorTo.A,
                er = colorTo.R,
                eg = colorTo.G,
                eb = colorTo.B;

            // lerp the colors to get the difference
            byte a = (byte)Math.Max(0, Math.Min(255, sa.Lerp(ea, amount))),
                r = (byte)Math.Max(0, Math.Min(255, sr.Lerp(er, amount))),
                g = (byte)Math.Max(0, Math.Min(255, sg.Lerp(eg, amount))),
                b = (byte)Math.Max(0, Math.Min(255, sb.Lerp(eb, amount)));

            // return the new color
            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Darkens the color by the given percentage.
        /// </summary>
        /// <param name="color">Source color.</param>
        /// <param name="amount">Percentage to darken. Value should be between 0 and 1.</param>
        /// <returns>Color</returns>
        public static Color DarkerBy(this Color color, float amount)
        {
            return color.Lerp(Colors.Black, amount);
        }

        /// <summary>
        /// Lightens the color by the given percentage.
        /// </summary>
        /// <param name="color">Source color.</param>
        /// <param name="amount">Percentage to lighten. Value should be between 0 and 1.</param>
        /// <returns>Color</returns>
        public static Color LighterBy(this Color color, float amount)
        {
            return color.Lerp(Colors.White, amount);
        }

        /// <summary>
        /// Converts the Point structure P (X,Y) to Vector3 structure
        /// V (P.X, P.Y, 0);
        /// </summary>
        /// <param name="p">Point structure</param>
        /// <returns>Vector3</returns>
        public static Vector3 ToVector3(this Point p)
        {
            return new Vector3((float)p.X, (float)p.Y, 0f);
        }

        /// <summary>
        /// Calculates the best size that can fit in the destination area based on the given stretch and alignment options.
        /// </summary>
        /// <param name="srcWidth">Width of the source.</param>
        /// <param name="srcHeight">Height of the source.</param>
        /// <param name="destWidth">Width of the destination area.</param>
        /// <param name="destHeight">Height of the destination area.</param>
        /// <param name="stretch">Defines how the source should stretch to fit the destination.</param>
        /// <param name="horizontalAlignment">Horizontal Alignment</param>
        /// <param name="verticalAlignment">Vertical Alignment</param>
        /// <returns>The best fitting Rectangle in the destination area.</returns>
        public static Rect GetOptimumSize(double srcWidth, double srcHeight, double destWidth, double destHeight, Stretch stretch, AlignmentX horizontalAlignment, AlignmentY verticalAlignment)
        {
            var ratio = srcWidth / srcHeight;
            var targetWidth = 0d;
            var targetHeight = 0d;

            // Stretch Mode
            switch (stretch)
            {
                case Stretch.None:
                    targetWidth = srcWidth;
                    targetHeight = srcHeight;
                    break;
                case Stretch.Fill:
                    targetWidth = destWidth;
                    targetHeight = destHeight;
                    break;
                case Stretch.Uniform:
                    // If width is greater than height
                    if (ratio > 1.0)
                    {
                        targetHeight = Math.Min(destWidth / ratio, destHeight);
                        targetWidth = targetHeight * ratio;
                    }
                    else
                    {
                        targetWidth = Math.Min(destHeight * ratio, destWidth);
                        targetHeight = targetWidth / ratio;
                    }

                    break;
                case Stretch.UniformToFill:
                    // If width is greater than height
                    if (ratio > 1.0)
                    {
                        targetHeight = Math.Max(destWidth / ratio, destHeight);
                        targetWidth = targetHeight * ratio;
                    }
                    else
                    {
                        targetWidth = Math.Max(destHeight * ratio, destWidth);
                        targetHeight = targetWidth / ratio;
                    }

                    break;
            }

            var left = 0d;
            switch (horizontalAlignment)
            {
                case AlignmentX.Left:
                    left = 0;
                    break;
                case AlignmentX.Center:
                    left = (destWidth - targetWidth) / 2.0;
                    break;
                case AlignmentX.Right:
                    left = destWidth - targetWidth;
                    break;
            }

            var top = 0d;
            switch (verticalAlignment)
            {
                case AlignmentY.Top:
                    top = 0;
                    break;
                case AlignmentY.Center:
                    top = (destHeight - targetHeight) / 2.0;
                    break;
                case AlignmentY.Bottom:
                    top = destHeight - targetHeight;
                    break;
            }

            return new Rect(left, top, targetWidth, targetHeight);
        }
    }
}