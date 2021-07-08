// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace CommunityToolkit.WinUI.Notifications
{
    [NotificationXmlElement("actions")]
    internal sealed class Element_ToastActions
    {
        internal const ToastSystemCommand DEFAULT_SYSTEM_COMMAND = ToastSystemCommand.None;

        [NotificationXmlAttribute("hint-systemCommands", DEFAULT_SYSTEM_COMMAND)]
        public ToastSystemCommand SystemCommands { get; set; } = ToastSystemCommand.None;

        public IList<IElement_ToastActionsChild> Children { get; private set; } = new List<IElement_ToastActionsChild>();
    }

    internal interface IElement_ToastActionsChild
    {
    }

    internal enum ToastSystemCommand
    {
        None,
        SnoozeAndDismiss
    }
}