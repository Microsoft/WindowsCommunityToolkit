// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    // Note that this code is NOT compiled for WinRT.
    // WinRT uses a different binding system since it doesn't support implicit type converters.
#if !WINRT
    /// <summary>
    /// A binding value for strings.
    /// </summary>
    public sealed class BindableString
    {
        internal string RawValue { get; private set; }

        /// <summary>
        /// Gets or sets the name that maps to your binding data value.
        /// </summary>
        public string BindingName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableString"/> class.
        /// A new binding for a string value, with the required binding name. Do NOT include surrounding {} brackets.
        /// </summary>
        /// <param name="bindingName">The name that maps to your data binding value.</param>
        public BindableString(string bindingName)
        {
            BindingName = bindingName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableString"/> class.
        /// Private constructor used by the implicit converter to assign the raw value.
        /// </summary>
        private BindableString()
        {
        }

        internal string ToXmlString()
        {
            if (BindingName != null)
            {
                return "{" + BindingName + "}";
            }

            return RawValue;
        }

        /// <summary>
        /// Creates a <see cref="BindableString"/> that has a raw value assigned.
        /// </summary>
        /// <param name="d">The raw value</param>
        public static implicit operator BindableString(string d)
        {
            return new BindableString()
            {
                RawValue = d
            };
        }

        /// <summary>
        /// Returns the raw value of the <see cref="BindableString"/>.
        /// </summary>
        /// <param name="b">The <see cref="BindableString"/> to obtain the raw value from.</param>
        public static implicit operator string(BindableString b)
        {
            return b.RawValue;
        }
    }
#endif
}