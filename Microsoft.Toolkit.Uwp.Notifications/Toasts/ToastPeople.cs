// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.Notifications
{
    /// <summary>
    /// Specify what person this toast is related to. For more info, see the My People documentation. New in Fall Creators Update.
    /// </summary>
    public sealed class ToastPeople
    {
        /// <summary>
        /// Gets or sets a remote identifier that corresponds with the RemoteId you set on a Contact you created
        /// with the ContactStore APIs. For more info, see the My People documentation.
        /// </summary>
        public string RemoteId { get; set; }

        /// <summary>
        /// Gets or sets an email address that corresponds with a contact in the local Windows ContactStore. Note
        /// that if <see cref="ToastPeople.RemoteId"/> is specified, this property will be ignored. For more info,
        /// see the My People documentation.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets a phone number that corresponds with a contact in the local Windows ContactStore. Note
        /// that if <see cref="ToastPeople.EmailAddress"/> is specified, this property will be ignored.
        /// For more info, see the My People documentation.
        /// </summary>
        public string PhoneNumber { get; set; }

        internal void PopulateToastElement(Element_Toast toast)
        {
            string hintPeople;

            if (RemoteId != null)
            {
                hintPeople = "remoteid:" + RemoteId;
            }
            else if (EmailAddress != null)
            {
                hintPeople = "mailto:" + EmailAddress;
            }
            else if (PhoneNumber != null)
            {
                hintPeople = "tel:" + PhoneNumber;
            }
            else
            {
                return;
            }

            toast.HintPeople = hintPeople;
        }
    }
}