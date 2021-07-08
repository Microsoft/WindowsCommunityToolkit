// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace CommunityToolkit.WinUI.Notifications
{
    /// <summary>
    /// New in Creators Update: Additional options relating to activation.
    /// </summary>
    public sealed class ToastActivationOptions
    {
        /// <summary>
        /// Gets or sets the target PFN if you are using <see cref="ToastActivationType.Protocol"/>. You can optionally specify, so that regardless of whether multiple apps are registered to handle the same protocol uri, your desired app will always be launched.
        /// </summary>
        public string ProtocolActivationTargetApplicationPfn { get; set; }

        /// <summary>
        /// Gets or sets the behavior that the toast should use when the user invokes this action.
        /// Note that this option only works on <see cref="ToastButton"/> and <see cref="ToastContextMenuItem"/>.
        /// Desktop-only, supported in builds 16251 or higher. New in Fall Creators Update
        /// </summary>
        public ToastAfterActivationBehavior AfterActivationBehavior { get; set; } = ToastAfterActivationBehavior.Default;

        internal void PopulateElement(IElement_ToastActivatable el)
        {
            // If protocol PFN is specified but protocol activation isn't used, throw exception
            if (ProtocolActivationTargetApplicationPfn != null && el.ActivationType != Element_ToastActivationType.Protocol)
            {
                throw new InvalidOperationException($"You cannot specify {nameof(ProtocolActivationTargetApplicationPfn)} without using ActivationType of Protocol.");
            }

            el.ProtocolActivationTargetApplicationPfn = ProtocolActivationTargetApplicationPfn;
            el.AfterActivationBehavior = AfterActivationBehavior;
        }
    }
}