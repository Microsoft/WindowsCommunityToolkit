// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace Microsoft.Toolkit.Uwp.Notifications
{
    internal static class ArgumentValidator
    {
        public static void SetProperty<T>(ref T property, T value, string propertyName, ArgumentValidatorOptions options)
        {
            if (options.HasFlag(ArgumentValidatorOptions.NotNull))
            {
                if (value == null)
                {
                    throw new ArgumentNullException(propertyName);
                }
            }

            property = value;
        }
    }

    [Flags]
    internal enum ArgumentValidatorOptions
    {
        NotNull
    }
}