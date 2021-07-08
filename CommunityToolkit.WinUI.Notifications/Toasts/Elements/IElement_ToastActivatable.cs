// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    internal interface IElement_ToastActivatable
    {
        Element_ToastActivationType ActivationType { get; set; }

        string ProtocolActivationTargetApplicationPfn { get; set; }

        ToastAfterActivationBehavior AfterActivationBehavior { get; set; }
    }
}