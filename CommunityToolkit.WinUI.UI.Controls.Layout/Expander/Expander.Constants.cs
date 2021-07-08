// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// The <see cref="Expander"/> control allows user to show/hide content based on a boolean state
    /// </summary>
    public partial class Expander
    {
        /// <summary>
        /// Key of the VisualStateGroup that trigger display mode (visible/collapsed) and direction content
        /// </summary>
        private const string DisplayModeAndDirectionStatesGroupStateContent = "DisplayModeAndDirectionStates";

        /// <summary>
        /// Key of the VisualState when expander is visible and expander direction is set to Left
        /// </summary>
        private const string StateContentVisibleLeft = "VisibleLeft";

        /// <summary>
        /// Key of the VisualState when expander is visible and expander direction is set to Down
        /// </summary>
        private const string StateContentVisibleDown = "VisibleDown";

        /// <summary>
        /// Key of the VisualState when expander is visible and expander direction is set to Right
        /// </summary>
        private const string StateContentVisibleRight = "VisibleRight";

        /// <summary>
        /// Key of the VisualState when expander is visible and expander direction is set to Up
        /// </summary>
        private const string StateContentVisibleUp = "VisibleUp";

        /// <summary>
        /// Key of the VisualState when expander is collapsed and expander direction is set to Left
        /// </summary>
        private const string StateContentCollapsedLeft = "CollapsedLeft";

        /// <summary>
        /// Key of the VisualState when expander is collapsed and expander direction is set to Down
        /// </summary>
        private const string StateContentCollapsedDown = "CollapsedDown";

        /// <summary>
        /// Key of the VisualState when expander is collapsed and expander direction is set to Right
        /// </summary>
        private const string StateContentCollapsedRight = "CollapsedRight";

        /// <summary>
        /// Key of the VisualState when expander is collapsed and expander direction is set to Up
        /// </summary>
        private const string StateContentCollapsedUp = "CollapsedUp";

        /// <summary>
        /// Key of the UI Element that toggle IsExpanded property
        /// </summary>
        private const string ExpanderToggleButtonPart = "PART_ExpanderToggleButton";

        /// <summary>
        /// Key of the UI Element that contains the content of the control that is expanded/collapsed
        /// </summary>
        private const string MainContentPart = "PART_MainContent";

        /// <summary>
        /// Key of the VisualStateGroup that set expander direction of the control
        /// </summary>
        private const string ExpandDirectionGroupStateContent = "ExpandDirectionStates";

        /// <summary>
        /// Key of the VisualState when expander direction is set to Left
        /// </summary>
        private const string StateContentLeftDirection = "LeftDirection";

        /// <summary>
        /// Key of the VisualState when expander direction is set to Down
        /// </summary>
        private const string StateContentDownDirection = "DownDirection";

        /// <summary>
        /// Key of the VisualState when expander direction is set to Right
        /// </summary>
        private const string StateContentRightDirection = "RightDirection";

        /// <summary>
        /// Key of the VisualState when expander direction is set to Up
        /// </summary>
        private const string StateContentUpDirection = "UpDirection";

        /// <summary>
        /// Key of the UI Element that contains the content of the entire control
        /// </summary>
        private const string RootGridPart = "PART_RootGrid";

        /// <summary>
        /// Key of the UI Element that contains the content of the LayoutTransformer (of the expander button)
        /// </summary>
        private const string LayoutTransformerPart = "PART_LayoutTransformer";
    }
}