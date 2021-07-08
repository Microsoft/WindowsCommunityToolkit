// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Controls.TextToolbarButtons;

namespace CommunityToolkit.WinUI.UI.Controls.TextToolbarFormats
{
    /// <summary>
    /// The Actions taken when a button is pressed. Required for Common Button actions (Unless you override both Activation and ShiftActivation)
    /// </summary>
    public abstract class ButtonActions
    {
        /// <summary>
        /// Applies Bold
        /// </summary>
        public abstract void FormatBold(ToolbarButton button);

        /// <summary>
        /// Applies Italics
        /// </summary>
        public abstract void FormatItalics(ToolbarButton button);

        /// <summary>
        /// Applies Strikethrough
        /// </summary>
        public abstract void FormatStrikethrough(ToolbarButton button);

        /// <summary>
        /// Applies Link
        /// </summary>
        public abstract void FormatLink(ToolbarButton button, string label, string formattedLabel, string link);

        /// <summary>
        /// Applies List
        /// </summary>
        public abstract void FormatList(ToolbarButton button);

        /// <summary>
        /// Applies Ordered List
        /// </summary>
        public abstract void FormatOrderedList(ToolbarButton button);
    }
}