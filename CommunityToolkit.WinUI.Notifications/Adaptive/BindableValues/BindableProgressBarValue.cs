// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    // Note that this code is NOT compiled for WinRT.
    // WinRT uses a different binding system since it doesn't support implicit type converters.
#if !WINRT
    /// <summary>
    /// A binding value for doubles.
    /// </summary>
    public sealed class BindableProgressBarValue
    {
        /// <summary>
        /// Gets raw value used for the implicit converter case, where dev provided a raw double. We store the raw value,
        /// so that later on when generating the XML, we can provide this value rather than binding syntax.
        /// </summary>
        internal AdaptiveProgressBarValue RawValue { get; private set; }

        internal bool RawIsIndeterminate { get; private set; }

        /// <summary>
        /// Gets or sets the name that maps to your binding data value.
        /// </summary>
        public string BindingName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableProgressBarValue"/> class.
        /// A new binding for a double value, with the required binding value name. Do NOT include surrounding {} brackets.
        /// </summary>
        /// <param name="bindingName">The name that maps to your binding data value.</param>
        public BindableProgressBarValue(string bindingName)
        {
            BindingName = bindingName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableProgressBarValue"/> class.
        /// Private constructor used by the implicit converter to assign the raw value.
        /// </summary>
        private BindableProgressBarValue()
        {
        }

        internal string ToXmlString()
        {
            if (BindingName != null)
            {
                return "{" + BindingName + "}";
            }

            if (RawValue != null)
            {
                return RawValue.ToXmlString();
            }

            return null;
        }

        /// <summary>
        /// Creates a <see cref="BindableProgressBarValue"/> that has a raw value assigned.
        /// </summary>
        /// <param name="v">The raw value</param>
        public static implicit operator BindableProgressBarValue(AdaptiveProgressBarValue v)
        {
            return new BindableProgressBarValue()
            {
                RawValue = v
            };
        }

        /// <summary>
        /// Returns the raw value of the <see cref="BindableProgressBarValue"/>.
        /// </summary>
        /// <param name="b">The <see cref="BindableProgressBarValue"/> to obtain the raw value from.</param>
        public static implicit operator AdaptiveProgressBarValue(BindableProgressBarValue b)
        {
            return b.RawValue;
        }

        /// <summary>
        /// Creates an <see cref="BindableProgressBarValue"/> that has the raw double value.
        /// </summary>
        /// <param name="d">The raw value</param>
        public static implicit operator BindableProgressBarValue(double d)
        {
            return AdaptiveProgressBarValue.FromValue(d);
        }
    }
#endif
}