// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarButtons;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.System;
using Windows.UI.Core;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Toolbar for Editing Text attached to a RichEditBox
    /// </summary>
    public partial class TextToolbar
    {
        /// <summary>
        /// Gets the Button Instance from the DefaultButton
        /// </summary>
        /// <param name="button">Default Button</param>
        /// <returns>Default Toolbar Button</returns>
        public ToolbarButton GetDefaultButton(ButtonType button)
        {
            if (GetTemplateChild(RootControl) is CommandBar root)
            {
                var element = root.PrimaryCommands.OfType<ToolbarButton>().FirstOrDefault(item => ((FrameworkElement)item).Name == button.ToString());
                return element;
            }

            return null;
        }

        /// <summary>
        /// Attaches all of the Default Buttons, Removing any that are to be removed, and inserting Custom buttons.
        /// </summary>
        private void BuildBar()
        {
            if (GetTemplateChild(RootControl) is CommandBar root)
            {
                root.PrimaryCommands.Clear();

                AttachButtonMap(DefaultButtons, root);

                foreach (var button in ButtonModifications)
                {
                    var element = GetDefaultButton(button.Type);
                    button.Button = element;
                }

                if (CustomButtons != null)
                {
                    AttachButtonMap(CustomButtons, root);
                }
            }
        }

        /// <summary>
        /// Adds all of the ToolbarButtons to the Root Control
        /// </summary>
        /// <param name="map">Collection of Buttons to add</param>
        /// <param name="root">Root Control</param>
        private void AttachButtonMap(ButtonMap map, CommandBar root)
        {
            if (root == null || map == null)
            {
                return;
            }

            foreach (var item in map)
            {
                AddToolbarItem(item, root);
            }
        }

        /// <summary>
        /// Adds an element to the Toolbar
        /// </summary>
        /// <param name="item">Item to add to the Toolbar</param>
        /// <param name="root">Root Control</param>
        private void AddToolbarItem(IToolbarItem item, CommandBar root)
        {
            if (item == null)
            {
                return;
            }

            if (!root.PrimaryCommands.Contains(item))
            {
                if (item is ToolbarButton button)
                {
                    button.Model = this;
                }

                if (item.Position != -1 && item.Position <= root.PrimaryCommands.Count)
                {
                    root.PrimaryCommands.Insert(item.Position, item);
                }
                else
                {
                    item.Position = root.PrimaryCommands.Count;
                    root.PrimaryCommands.Add(item);
                }
            }
        }

        /// <summary>
        /// Moves a Toolbar element to a new location on the Toolbar
        /// </summary>
        /// <param name="item">Item to Move</param>
        /// <param name="root">Root Control</param>
        private void MoveToolbarItem(IToolbarItem item, CommandBar root)
        {
            if (root.PrimaryCommands.Contains(item))
            {
                root.PrimaryCommands.Remove(item);
                root.PrimaryCommands.Insert(item.Position, item);
            }
        }

        /// <summary>
        /// Removes an Element from the Toolbar
        /// </summary>
        /// <param name="item">Item to Remove</param>
        public void RemoveToolbarItem(IToolbarItem item)
        {
            var root = GetTemplateChild(RootControl) as CommandBar;
            if (root.PrimaryCommands.Contains(item))
            {
                root.PrimaryCommands.Remove(item);
            }
        }

        /// <summary>
        /// Returns the best Match for some keys that don't have names.
        /// </summary>
        /// <returns>Best Match</returns>
        private static VirtualKey FindBestAlternativeKey(VirtualKey original)
        {
            switch (original)
            {
                case (VirtualKey)189:
                    return VirtualKey.Subtract;

                default:
                    return original;
            }
        }

        /// <summary>
        /// Checks if the key is pressed down.
        /// </summary>
        /// <param name="state">Key State</param>
        /// <returns>Is Key pressed down?</returns>
        private static bool IsKeyActive(CoreVirtualKeyStates state)
        {
            var downAndLocked = CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked;
            return state == CoreVirtualKeyStates.Down || state == downAndLocked;
        }
    }
}