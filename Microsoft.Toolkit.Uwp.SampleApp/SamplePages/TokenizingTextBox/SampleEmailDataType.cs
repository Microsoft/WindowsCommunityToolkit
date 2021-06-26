// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// Sample of strongly-typed email address simulated data for <see cref="Microsoft.Toolkit.Uwp.UI.Controls.TokenizingTextBox"/>.
    /// </summary>
    public class SampleEmailDataType
    {
        /// <summary>
        /// Gets the initials to Display
        /// </summary>
        public string Initials => string.Empty + FirstName[0] + FamilyName[0];

        /// <summary>
        /// Gets or sets the first name .
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the family name .
        /// </summary>
        public string FamilyName { get; set; }

        /// <summary>
        /// Gets the display text.
        /// </summary>
        public string DisplayName => $"{FirstName} {FamilyName}";

        /// <summary>
        /// Gets the formatted email address
        /// </summary>
        public string EmailAddress => $"{DisplayName} <{FirstName}.{FamilyName}@contoso.com>";

        public override string ToString()
        {
            return EmailAddress;
        }
    }
}