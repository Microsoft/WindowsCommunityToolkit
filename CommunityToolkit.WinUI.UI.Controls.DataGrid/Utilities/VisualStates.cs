// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.UI.Controls.Utilities
{
    /// <summary>
    /// Names and helpers for visual states in the control.
    /// </summary>
    internal static class VisualStates
    {
        // GroupCommon

        /// <summary>
        /// Normal state
        /// </summary>
        public const string StateNormal = "Normal";

        /// <summary>
        /// PointerOver state
        /// </summary>
        public const string StatePointerOver = "PointerOver";

        /// <summary>
        /// Pressed state
        /// </summary>
        public const string StatePressed = "Pressed";

        /// <summary>
        /// Disabled state
        /// </summary>
        public const string StateDisabled = "Disabled";

        /// <summary>
        /// Common state group
        /// </summary>
        public const string GroupCommon = "CommonStates";

        // GroupExpanded

        /// <summary>
        /// Expanded state
        /// </summary>
        public const string StateExpanded = "Expanded";

        /// <summary>
        /// Collapsed state
        /// </summary>
        public const string StateCollapsed = "Collapsed";

        /// <summary>
        /// Empty state
        /// </summary>
        public const string StateEmpty = "Empty";

        // GroupFocus

        /// <summary>
        /// Unfocused state
        /// </summary>
        public const string StateUnfocused = "Unfocused";

        /// <summary>
        /// Focused state
        /// </summary>
        public const string StateFocused = "Focused";

        /// <summary>
        /// Focus state group
        /// </summary>
        public const string GroupFocus = "FocusStates";

        // GroupSelection

        /// <summary>
        /// Selected state
        /// </summary>
        public const string StateSelected = "Selected";

        /// <summary>
        /// Unselected state
        /// </summary>
        public const string StateUnselected = "Unselected";

        /// <summary>
        /// Selection state group
        /// </summary>
        public const string GroupSelection = "SelectionStates";

        // GroupActive

        /// <summary>
        /// Active state
        /// </summary>
        public const string StateActive = "Active";

        /// <summary>
        /// Inactive state
        /// </summary>
        public const string StateInactive = "Inactive";

        /// <summary>
        /// Active state group
        /// </summary>
        public const string GroupActive = "ActiveStates";

        // GroupCurrent

        /// <summary>
        /// Regular state
        /// </summary>
        public const string StateRegular = "Regular";

        /// <summary>
        /// Current state
        /// </summary>
        public const string StateCurrent = "Current";

        /// <summary>
        /// CurrentWithFocus state
        /// </summary>
        public const string StateCurrentWithFocus = "CurrentWithFocus";

        /// <summary>
        /// Current state group
        /// </summary>
        public const string GroupCurrent = "CurrentStates";

        // GroupInteraction

        /// <summary>
        /// Display state
        /// </summary>
        public const string StateDisplay = "Display";

        /// <summary>
        /// Editing state
        /// </summary>
        public const string StateEditing = "Editing";

        /// <summary>
        /// Interaction state group
        /// </summary>
        public const string GroupInteraction = "InteractionStates";

        // GroupSort

        /// <summary>
        /// Unsorted state
        /// </summary>
        public const string StateUnsorted = "Unsorted";

        /// <summary>
        /// Sort Ascending state
        /// </summary>
        public const string StateSortAscending = "SortAscending";

        /// <summary>
        /// Sort Descending state
        /// </summary>
        public const string StateSortDescending = "SortDescending";

        /// <summary>
        /// Sort state group
        /// </summary>
        public const string GroupSort = "SortStates";

        // GroupValidation

        /// <summary>
        /// Invalid state
        /// </summary>
        public const string StateInvalid = "Invalid";

        /// <summary>
        /// RowInvalid state
        /// </summary>
        public const string StateRowInvalid = "RowInvalid";

        /// <summary>
        /// RowValid state
        /// </summary>
        public const string StateRowValid = "RowValid";

        /// <summary>
        /// Valid state
        /// </summary>
        public const string StateValid = "Valid";

#if FEATURE_VALIDATION
        // RuntimeValidationStates
        public const string StateInvalidUnfocused = "InvalidUnfocused";
#endif

        /// <summary>
        /// Validation state group
        /// </summary>
        public const string GroupValidation = "ValidationStates";

        // GroupScrollBarsSeparator

        /// <summary>
        /// SeparatorExpanded state
        /// </summary>
        public const string StateSeparatorExpanded = "SeparatorExpanded";

        /// <summary>
        /// ScrollBarsSeparatorCollapsed state
        /// </summary>
        public const string StateSeparatorCollapsed = "SeparatorCollapsed";

        /// <summary>
        /// SeparatorExpandedWithoutAnimation state
        /// </summary>
        public const string StateSeparatorExpandedWithoutAnimation = "SeparatorExpandedWithoutAnimation";

        /// <summary>
        /// SeparatorCollapsedWithoutAnimation state
        /// </summary>
        public const string StateSeparatorCollapsedWithoutAnimation = "SeparatorCollapsedWithoutAnimation";

        /// <summary>
        /// ScrollBarsSeparator state group
        /// </summary>
        public const string GroupScrollBarsSeparator = "ScrollBarsSeparatorStates";

        // GroupScrollBars

        /// <summary>
        /// TouchIndicator state
        /// </summary>
        public const string StateTouchIndicator = "TouchIndicator";

        /// <summary>
        /// MouseIndicator state
        /// </summary>
        public const string StateMouseIndicator = "MouseIndicator";

        /// <summary>
        /// MouseIndicatorFull state
        /// </summary>
        public const string StateMouseIndicatorFull = "MouseIndicatorFull";

        /// <summary>
        /// NoIndicator state
        /// </summary>
        public const string StateNoIndicator = "NoIndicator";

        /// <summary>
        /// ScrollBars state group
        /// </summary>
        public const string GroupScrollBars = "ScrollBarsStates";

        /// <summary>
        /// Use VisualStateManager to change the visual state of the control.
        /// </summary>
        /// <param name="control">
        /// Control whose visual state is being changed.
        /// </param>
        /// <param name="useTransitions">
        /// true to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        /// <param name="stateNames">
        /// Ordered list of state names and fallback states to transition into.
        /// Only the first state to be found will be used.
        /// </param>
        public static void GoToState(Control control, bool useTransitions, params string[] stateNames)
        {
            Debug.Assert(control != null, "Expected non-null control.");

            if (stateNames == null)
            {
                return;
            }

            foreach (string name in stateNames)
            {
                if (VisualStateManager.GoToState(control, name, useTransitions))
                {
                    break;
                }
            }
        }
    }
}