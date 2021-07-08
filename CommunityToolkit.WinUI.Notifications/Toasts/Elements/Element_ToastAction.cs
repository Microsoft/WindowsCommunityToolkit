// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Notifications
{
    [NotificationXmlElement("action")]
    internal sealed class Element_ToastAction : IElement_ToastActionsChild, IElement_ToastActivatable
    {
        internal const Element_ToastActivationType DEFAULT_ACTIVATION_TYPE = Element_ToastActivationType.Foreground;
        internal const ToastAfterActivationBehavior DEFAULT_AFTER_ACTIVATION_BEHAVIOR = ToastAfterActivationBehavior.Default;
        internal const Element_ToastActionPlacement DEFAULT_PLACEMENT = Element_ToastActionPlacement.Inline;

        /// <summary>
        /// Gets or sets the text to be displayed on the button.
        /// </summary>
        [NotificationXmlAttribute("content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the arguments attribute describing the app-defined data that the app can later retrieve once it is activated from user taking this action.
        /// </summary>
        [NotificationXmlAttribute("arguments")]
        public string Arguments { get; set; }

        [NotificationXmlAttribute("activationType", DEFAULT_ACTIVATION_TYPE)]
        public Element_ToastActivationType ActivationType { get; set; } = DEFAULT_ACTIVATION_TYPE;

        [NotificationXmlAttribute("protocolActivationTargetApplicationPfn")]
        public string ProtocolActivationTargetApplicationPfn { get; set; }

        [NotificationXmlAttribute("afterActivationBehavior", DEFAULT_AFTER_ACTIVATION_BEHAVIOR)]
        public ToastAfterActivationBehavior AfterActivationBehavior { get; set; } = DEFAULT_AFTER_ACTIVATION_BEHAVIOR;

        /// <summary>
        /// Gets or sets optional value to provide an image icon for this action to display inside the button alone with the text content.
        /// </summary>
        [NotificationXmlAttribute("imageUri")]
        public string ImageUri { get; set; }

        /// <summary>
        /// Gets or sets value used for the quick reply scenario.
        /// </summary>
        [NotificationXmlAttribute("hint-inputId")]
        public string InputId { get; set; }

        [NotificationXmlAttribute("placement", DEFAULT_PLACEMENT)]
        public Element_ToastActionPlacement Placement { get; set; } = DEFAULT_PLACEMENT;

        [NotificationXmlAttribute("hint-actionId")]
        public string HintActionId { get; set; }
    }

    internal enum Element_ToastActionPlacement
    {
        Inline,

        [EnumString("contextMenu")]
        ContextMenu
    }

    internal enum Element_ToastActivationType
    {
        /// <summary>
        /// Default value. Your foreground app is launched.
        /// </summary>
        Foreground,

        /// <summary>
        /// Your corresponding background task (assuming you set everything up) is triggered, and you can execute code in the background (like sending the user's quick reply message) without interrupting the user.
        /// </summary>
        [EnumString("background")]
        Background,

        /// <summary>
        /// Launch a different app using protocol activation.
        /// </summary>
        [EnumString("protocol")]
        Protocol,

        /// <summary>
        /// System handles the activation.
        /// </summary>
        [EnumString("system")]
        System
    }
}