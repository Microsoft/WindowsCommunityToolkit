// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI
{
    /// <inheritdoc cref="TextBoxExtensions"/>
    public static partial class TextBoxExtensions
    {
        /// <summary>
        /// Regex validation mode.
        /// </summary>
        public enum ValidationMode
        {
            /// <summary>
            /// Update <see cref="IsValidProperty"/> with validation result at text changed.
            /// </summary>
            Normal,

            /// <summary>
            /// Update <see cref="IsValidProperty"/> with validation result and in case the textbox is not valid clear its value when the TextBox lose focus
            /// </summary>
            Forced,

            /// <summary>
            /// Update <see cref="IsValidProperty"/> with validation result at text changed and clear the newest character at input which is not valid
            /// </summary>
            Dynamic
        }

        /// <summary>
        /// Specify the type of validation required
        /// </summary>
        public enum ValidationType
        {
            /// <summary>
            /// The default validation that required property Regex to be setted
            /// </summary>
            Custom,

            /// <summary>
            /// Email validation
            /// </summary>
            Email,

            /// <summary>
            /// Number validation
            /// </summary>
            Number,

            /// <summary>
            /// Decimal validation
            /// </summary>
            Decimal,

            /// <summary>
            /// Text only validation
            /// </summary>
            Characters,

            /// <summary>
            /// Phone number validation
            /// </summary>
            PhoneNumber
        }
    }
}