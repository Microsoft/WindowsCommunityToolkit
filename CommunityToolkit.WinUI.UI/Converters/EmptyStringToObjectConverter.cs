// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Converters
{
    /// <summary>
    /// This class converts a string value into a an object (if the value is null or empty returns the false value).
    /// Can be used to bind a visibility, a color or an image to the value of a string.
    /// </summary>
    public class EmptyStringToObjectConverter : EmptyObjectToObjectConverter
    {
        /// <summary>
        /// Checks string for emptiness.
        /// </summary>
        /// <param name="value">Value to be checked.</param>
        /// <returns>True if value is null or empty string, false otherwise.</returns>
        protected override bool CheckValueIsEmpty(object value)
        {
            return string.IsNullOrEmpty(value?.ToString());
        }
    }
}