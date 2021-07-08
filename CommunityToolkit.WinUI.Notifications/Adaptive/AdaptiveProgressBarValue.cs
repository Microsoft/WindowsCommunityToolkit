// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// A class that represents the progress bar's value.
    /// </summary>
    public sealed class AdaptiveProgressBarValue
    {
        /// <summary>
        /// Gets or sets the property name to bind to.
        /// </summary>
        public string BindingName { get; set; }

        /// <summary>
        /// Gets or sets the value (0-1) representing the percent complete.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the progress bar is indeterminate.
        /// </summary>
        public bool IsIndeterminate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveProgressBarValue"/> class.
        /// </summary>
        private AdaptiveProgressBarValue()
        {
        }

        internal string ToXmlString()
        {
            if (IsIndeterminate)
            {
                return "indeterminate";
            }

            if (BindingName != null)
            {
                return "{" + BindingName + "}";
            }

            return Value.ToString();
        }

        /// <summary>
        /// Gets an indeterminate progress bar value.
        /// </summary>
        public static AdaptiveProgressBarValue Indeterminate
        {
            get
            {
                return new AdaptiveProgressBarValue()
                {
                    IsIndeterminate = true
                };
            }
        }

        /// <summary>
        /// Returns a progress bar value using the specified value (0-1) representing the percent complete.
        /// </summary>
        /// <param name="d">The value, 0-1, inclusive.</param>
        /// <returns>A progress bar value.</returns>
        public static AdaptiveProgressBarValue FromValue(double d)
        {
            if (d < 0 || d > 1)
            {
                throw new ArgumentOutOfRangeException("d", "Value must be between 0 and 1, inclusive.");
            }

            return new AdaptiveProgressBarValue()
            {
                Value = d
            };
        }

        /// <summary>
        /// Returns a progress bar value using the specified binding name.
        /// </summary>
        /// <param name="bindingName">The property to bind to.</param>
        /// <returns>A progress bar value.</returns>
        public static AdaptiveProgressBarValue FromBinding(string bindingName)
        {
            return new AdaptiveProgressBarValue()
            {
                BindingName = bindingName
            };
        }
    }
}