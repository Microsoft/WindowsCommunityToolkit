// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml.Markup;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Custom <see cref="MarkupExtension"/> which can provide nullable bool values.
    /// See https://wpdev.uservoice.com/forums/110705-universal-windows-platform/suggestions/17767198-nullable-dependency-properties.
    /// </summary>
    [MarkupExtensionReturnType(ReturnType = typeof(bool?))]
    public class NullableBoolExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets a value indicating whether the value of the Boolean is true.  Ignored if <see cref="IsNull"/> is true.
        /// </summary>
        public bool Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value should be null.  Overrides the <see cref="Value"/> property.
        /// </summary>
        public bool IsNull { get; set; }

        /// <inheritdoc/>
        protected override object ProvideValue()
        {
            if (IsNull)
            {
                return null;
            }

            return Value;
        }
    }
}
