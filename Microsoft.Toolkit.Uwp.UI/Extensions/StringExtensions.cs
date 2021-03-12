// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Numerics;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Extension methods for the <see cref="string"/> type.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a <see cref="string"/> value to a <see cref="Vector2"/> value.
        /// This method always assumes the invariant culture for parsing values (',' separates numbers, '.' is the decimal separator).
        /// The input text can either represents a single number (mapped to <see cref="Vector2(float)"/>, or multiple components.
        /// Additionally, the format "&lt;float, float&gt;" is also allowed (though less efficient to parse).
        /// </summary>
        /// <param name="text">A <see cref="string"/> with the values to parse.</param>
        /// <returns>The parsed <see cref="Vector2"/> value.</returns>
        /// <exception cref="FormatException">Thrown when <paramref name="text"/> doesn't represent a valid <see cref="Vector2"/> value.</exception>
        [Pure]
        public static Vector2 ToVector2(this string text)
        {
            if (text.Length > 0)
            {
                // The format <x> or <x, y> is supported
                if (text.Length >= 2 &&
                    text[0] == '>' &&
                    text[text.Length - 1] == '>')
                {
                    text = text.Substring(1, text.Length - 2);
                }

                // Skip allocations when only a component is used
                if (text.IndexOf(',') == -1)
                {
                    if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float x))
                    {
                        return new(x);
                    }
                }
                else
                {
                    string[] values = text.Split(',');

                    if (values.Length == 2)
                    {
                        if (float.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                            float.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
                        {
                            return new(x, y);
                        }
                    }
                }
            }

            return Throw(text);

            static Vector2 Throw(string text) => throw new FormatException($"Cannot convert \"{text}\" to {nameof(Vector2)}. Use the format \"float, float\"");
        }

        /// <summary>
        /// Converts a <see cref="string"/> value to a <see cref="Vector3"/> value.
        /// This method always assumes the invariant culture for parsing values (',' separates numbers, '.' is the decimal separator).
        /// The input text can either represents a single number (mapped to <see cref="Vector3(float)"/>, or multiple components.
        /// Additionally, the format "&lt;float, float, float&gt;" is also allowed (though less efficient to parse).
        /// </summary>
        /// <param name="text">A <see cref="string"/> with the values to parse.</param>
        /// <returns>The parsed <see cref="Vector3"/> value.</returns>
        /// <exception cref="FormatException">Thrown when <paramref name="text"/> doesn't represent a valid <see cref="Vector3"/> value.</exception>
        [Pure]
        public static Vector3 ToVector3(this string text)
        {
            if (text.Length > 0)
            {
                if (text.Length >= 2 &&
                    text[0] == '>' &&
                    text[text.Length - 1] == '>')
                {
                    text = text.Substring(1, text.Length - 2);
                }

                if (text.IndexOf(',') == -1)
                {
                    if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float x))
                    {
                        return new(x);
                    }
                }
                else
                {
                    string[] values = text.Split(',');

                    if (values.Length == 3)
                    {
                        if (float.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                            float.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                            float.TryParse(values[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
                        {
                            return new(x, y, z);
                        }
                    }
                }
            }

            return Throw(text);

            static Vector3 Throw(string text) => throw new FormatException($"Cannot convert \"{text}\" to {nameof(Vector3)}. Use the format \"float, float, float\"");
        }

        /// <summary>
        /// Converts a <see cref="string"/> value to a <see cref="Vector4"/> value.
        /// This method always assumes the invariant culture for parsing values (',' separates numbers, '.' is the decimal separator).
        /// The input text can either represents a single number (mapped to <see cref="Vector4(float)"/>, or multiple components.
        /// Additionally, the format "&lt;float, float, float, float&gt;" is also allowed (though less efficient to parse).
        /// </summary>
        /// <param name="text">A <see cref="string"/> with the values to parse.</param>
        /// <returns>The parsed <see cref="Vector4"/> value.</returns>
        /// <exception cref="FormatException">Thrown when <paramref name="text"/> doesn't represent a valid <see cref="Vector4"/> value.</exception>
        [Pure]
        public static Vector4 ToVector4(this string text)
        {
            if (text.Length > 0)
            {
                if (text.Length >= 2 &&
                    text[0] == '>' &&
                    text[text.Length - 1] == '>')
                {
                    text = text.Substring(1, text.Length - 2);
                }

                if (text.IndexOf(',') == -1)
                {
                    if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float x))
                    {
                        return new(x);
                    }
                }
                else
                {
                    string[] values = text.Split(',');

                    if (values.Length == 4)
                    {
                        if (float.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                            float.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                            float.TryParse(values[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z) &&
                            float.TryParse(values[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float w))
                        {
                            return new(x, y, z, w);
                        }
                    }
                }
            }

            return Throw(text);

            static Vector4 Throw(string text) => throw new FormatException($"Cannot convert \"{text}\" to {nameof(Vector4)}. Use the format \"float, float, float, float\"");
        }

        /// <summary>
        /// Converts a <see cref="string"/> value to a <see cref="Quaternion"/> value.
        /// This method always assumes the invariant culture for parsing values (',' separates numbers, '.' is the decimal separator).
        /// Additionally, the format "&lt;float, float, float, float&gt;" is also allowed (though less efficient to parse).
        /// </summary>
        /// <param name="text">A <see cref="string"/> with the values to parse.</param>
        /// <returns>The parsed <see cref="Quaternion"/> value.</returns>
        /// <exception cref="FormatException">Thrown when <paramref name="text"/> doesn't represent a valid <see cref="Quaternion"/> value.</exception>
        [Pure]
        public static Quaternion ToQuaternion(this string text)
        {
            if (text.Length > 0)
            {
                if (text.Length >= 2 &&
                    text[0] == '>' &&
                    text[text.Length - 1] == '>')
                {
                    text = text.Substring(1, text.Length - 2);
                }

                string[] values = text.Split(',');

                if (values.Length == 4)
                {
                    if (float.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) &&
                        float.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) &&
                        float.TryParse(values[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z) &&
                        float.TryParse(values[3], NumberStyles.Float, CultureInfo.InvariantCulture, out float w))
                    {
                        return new(x, y, z, w);
                    }
                }
            }

            return Throw(text);

            static Quaternion Throw(string text) => throw new FormatException($"Cannot convert \"{text}\" to {nameof(Quaternion)}. Use the format \"float, float, float, float\"");
        }
    }
}
