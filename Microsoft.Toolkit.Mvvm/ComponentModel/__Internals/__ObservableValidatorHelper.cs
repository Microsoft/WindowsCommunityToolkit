// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable SA1300

using System;
using System.ComponentModel;

namespace Microsoft.Toolkit.Mvvm.ComponentModel.__Internals
{
    /// <summary>
    /// An internal helper to support the source generator APIs related to <see cref="ObservableValidator"/>.
    /// This type is not intended to be used directly by user code.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This type is not intended to be used directly by user code")]
    public static class __ObservableValidatorHelper
    {
        /// <summary>
        /// Invokes <see cref="ObservableValidator.ValidateProperty(object?, string?)"/> externally on a target instance.
        /// </summary>
        /// <param name="instance">The target <see cref="ObservableValidator"/> instance.</param>
        /// <param name="value">The value to test for the specified property.</param>
        /// <param name="propertyName">The name of the property to validate.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This method is not intended to be called directly by user code")]
        public static void ValidateProperty(ObservableValidator instance, object? value, string propertyName)
        {
            instance.ValidateProperty(value, propertyName);
        }
    }
}