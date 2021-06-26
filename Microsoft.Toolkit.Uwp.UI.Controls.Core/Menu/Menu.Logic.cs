// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Text;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Menu Control defines a menu of choices for users to invoke.
    /// </summary>
    public partial class Menu
    {
        private const string CtrlValue = "CTRL";
        private const string ShiftValue = "SHIFT";
        private const string AltValue = "ALT";

        /// <summary>
        /// Gets or sets the current flyout placement, internal because the child menu item needs it to respect the menu direction of sub-menus
        /// </summary>
        internal FlyoutPlacementMode? CurrentFlyoutPlacement { get; set; }

        private static bool NavigateUsingKeyboard(KeyEventArgs args, Menu menu, Orientation orientation)
        {
            object element;
            if (ControlHelpers.IsXamlRootAvailable && menu.XamlRoot != null)
            {
                element = FocusManager.GetFocusedElement(menu.XamlRoot);
            }
            else
            {
                element = FocusManager.GetFocusedElement();
            }

            if (element is MenuFlyoutPresenter &&
                ((args.VirtualKey == VirtualKey.Down) ||
                 (args.VirtualKey == VirtualKey.Up) ||
                 (args.VirtualKey == VirtualKey.Left) ||
                 (args.VirtualKey == VirtualKey.Right)))
            {
                // Hack to delay and let next element get focus
                FocusManager.FindNextFocusableElement(FocusNavigationDirection.Right);
                return true;
            }

            if (!menu.IsOpened && element is MenuItem)
            {
                menu.UpdateMenuItemsFlyoutPlacement();

                if (args.VirtualKey == VirtualKey.Enter ||
                    ((args.VirtualKey == VirtualKey.Down) && menu.CurrentFlyoutPlacement == FlyoutPlacementMode.Bottom) ||
                    ((args.VirtualKey == VirtualKey.Up) && menu.CurrentFlyoutPlacement == FlyoutPlacementMode.Top) ||
                    ((args.VirtualKey == VirtualKey.Left) && menu.CurrentFlyoutPlacement == FlyoutPlacementMode.Left) ||
                    ((args.VirtualKey == VirtualKey.Right) && menu.CurrentFlyoutPlacement == FlyoutPlacementMode.Right))
                {
                    menu.SelectedMenuItem.ShowMenu();
                    return true;
                }

                if ((args.VirtualKey == VirtualKey.Left && orientation == Orientation.Horizontal) ||
                    (args.VirtualKey == VirtualKey.Up && orientation == Orientation.Vertical))
                {
                    GetNextMenuItem(menu, -1);
                    return true;
                }

                if ((args.VirtualKey == VirtualKey.Right && orientation == Orientation.Horizontal) ||
                    (args.VirtualKey == VirtualKey.Down && orientation == Orientation.Vertical))
                {
                    GetNextMenuItem(menu, +1);
                    return true;
                }
            }

            if ((menu.CurrentFlyoutPlacement == FlyoutPlacementMode.Left &&
                 args.VirtualKey == VirtualKey.Right) ||
                 (args.VirtualKey == VirtualKey.Left &&
                 menu.CurrentFlyoutPlacement != FlyoutPlacementMode.Left))
            {
                if (element is MenuFlyoutItem)
                {
                    menu.IsInTransitionState = true;
                    menu.SelectedMenuItem.HideMenu();
                    GetNextMenuItem(menu, -1).ShowMenu();
                    return true;
                }

                if (element is MenuFlyoutSubItem menuFlyoutSubItem)
                {
                    if (menuFlyoutSubItem.Parent is MenuItem && element == menu._lastFocusElement)
                    {
                        menu.IsInTransitionState = true;
                        menu.SelectedMenuItem.HideMenu();
                        GetNextMenuItem(menu, -1).ShowMenu();
                        return true;
                    }
                }
            }

            if ((args.VirtualKey == VirtualKey.Right &&
                menu.CurrentFlyoutPlacement != FlyoutPlacementMode.Left) ||
                (args.VirtualKey == VirtualKey.Left &&
                menu.CurrentFlyoutPlacement == FlyoutPlacementMode.Left))
            {
                if (element is MenuFlyoutItem)
                {
                    menu.IsInTransitionState = true;
                    menu.SelectedMenuItem.HideMenu();
                    GetNextMenuItem(menu, +1).ShowMenu();
                    return true;
                }
            }

            return false;
        }

        private static MenuItem GetNextMenuItem(Menu menu, int moveCount)
        {
            var currentMenuItemIndex = menu.Items.IndexOf(menu.SelectedMenuItem);
            var nextIndex = (currentMenuItemIndex + moveCount + menu.Items.Count) % menu.Items.Count;
            var nextItem = menu.Items.ElementAt(nextIndex) as MenuItem;
            nextItem?.Focus(FocusState.Keyboard);
            return nextItem;
        }

        private static string MapInputToGestureKey(VirtualKey key, bool menuHasFocus = false)
        {
            var isCtrlDown = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);
            var isShiftDown = Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down);
            var isAltDown = Window.Current.CoreWindow.GetKeyState(VirtualKey.Menu).HasFlag(CoreVirtualKeyStates.Down) || menuHasFocus;

            if (!isCtrlDown && !isShiftDown && !isAltDown)
            {
                return null;
            }

            StringBuilder gestureKeyBuilder = new StringBuilder();

            if (isCtrlDown)
            {
                gestureKeyBuilder.Append(CtrlValue);
                gestureKeyBuilder.Append("+");
            }

            if (isShiftDown)
            {
                gestureKeyBuilder.Append(ShiftValue);
                gestureKeyBuilder.Append("+");
            }

            if (isAltDown)
            {
                gestureKeyBuilder.Append(AltValue);
                gestureKeyBuilder.Append("+");
            }

            if (key == VirtualKey.None)
            {
                gestureKeyBuilder.Remove(gestureKeyBuilder.Length - 1, 1);
            }
            else
            {
                gestureKeyBuilder.Append(key);
            }

            return gestureKeyBuilder.ToString();
        }

        internal bool UpdateMenuItemsFlyoutPlacement()
        {
            var placementMode = GetMenuFlyoutPlacementMode();

            if (placementMode == CurrentFlyoutPlacement)
            {
                return false;
            }

            CurrentFlyoutPlacement = placementMode;

            foreach (MenuItem menuItem in Items)
            {
                if (menuItem.MenuFlyout != null)
                {
                    menuItem.MenuFlyout.Placement = CurrentFlyoutPlacement.Value;
                }
            }

            return true;
        }

        internal FlyoutPlacementMode GetMenuFlyoutPlacementMode()
        {
            UIElement content;
            double height;
            double width;
            if (ControlHelpers.IsXamlRootAvailable && XamlRoot != null)
            {
                content = XamlRoot.Content;
                height = XamlRoot.Size.Height;
                width = XamlRoot.Size.Width;
            }
            else
            {
                content = Window.Current.Content;
                height = Window.Current.Bounds.Height;
                width = Window.Current.Bounds.Width;
            }

            var ttv = TransformToVisual(content);
            var menuCoords = ttv.TransformPoint(new Point(0, 0));

            if (Orientation == Orientation.Horizontal)
            {
                var menuCenter = menuCoords.Y + (ActualHeight / 2);

                if (menuCenter <= height / 2)
                {
                    return FlyoutPlacementMode.Bottom;
                }
                else
                {
                    return FlyoutPlacementMode.Top;
                }
            }
            else
            {
                var menuCenter = menuCoords.X + (ActualWidth / 2);

                if (menuCenter <= width / 2)
                {
                    return FlyoutPlacementMode.Right;
                }
                else
                {
                    return FlyoutPlacementMode.Left;
                }
            }
        }

        private static void OrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = (Menu)d;
            if (menu._wrapPanel != null)
            {
                menu._wrapPanel.Orientation = menu.Orientation;
            }

            menu.UpdateMenuItemsFlyoutPlacement();
        }

        private static void RemoveElementFromCache(FrameworkElement descendant)
        {
            var value = descendant.GetValue(InputGestureTextProperty);
            if (value == null)
            {
                return;
            }

            var inputGestureText = value.ToString().ToUpper();
            if (!MenuItemInputGestureCache.ContainsKey(inputGestureText))
            {
                return;
            }

            var cachedMenuItem = MenuItemInputGestureCache[inputGestureText];
            if (cachedMenuItem == descendant)
            {
                MenuItemInputGestureCache.Remove(inputGestureText);
            }
        }

        private void ShowMenuItemsToolTips()
        {
            foreach (MenuItem item in Items)
            {
                item.ShowTooltip();
            }
        }

        private void UnderlineMenuItems()
        {
            foreach (MenuItem item in Items)
            {
                item.Underline();
            }
        }

        private void RemoveUnderlineMenuItems()
        {
            foreach (MenuItem item in Items)
            {
                item.RemoveUnderline();
            }
        }

        private void HideMenuItemsTooltips()
        {
            foreach (MenuItem item in Items)
            {
                item.HideTooltip();
            }
        }

        internal void CalculateBounds()
        {
            var ttv = TransformToVisual(ControlHelpers.IsXamlRootAvailable && XamlRoot != null ? XamlRoot.Content : Window.Current.Content);
            Point screenCoords = ttv.TransformPoint(new Point(0, 0));
            _bounds.X = screenCoords.X;
            _bounds.Y = screenCoords.Y;
            _bounds.Width = ActualWidth;
            _bounds.Height = ActualHeight;

            foreach (MenuItem item in Items)
            {
                item.CalculateBounds();
            }
        }
    }
}