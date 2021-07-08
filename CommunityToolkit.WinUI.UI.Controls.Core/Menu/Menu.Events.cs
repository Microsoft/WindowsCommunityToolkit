// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Foundation;
using Windows.System;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Menu Control defines a menu of choices for users to invoke.
    /// </summary>
    public partial class Menu
    {
        private bool _onlyAltCharacterPressed = true;
        private Control _lastFocusElement;
        private bool _isLostFocus = true;
        private Control _lastFocusElementBeforeMenu;
        private Rect _bounds;

        private bool AllowTooltip => (bool)GetValue(AllowTooltipProperty);

        private void Menu_Loaded(object sender, RoutedEventArgs e)
        {
            _wrapPanel = ItemsPanelRoot as WrapPanel;
            if (_wrapPanel != null)
            {
                _wrapPanel.Orientation = Orientation;
            }

            UpdateMenuItemsFlyoutPlacement();

            LostFocus -= Menu_LostFocus;
            LostFocus += Menu_LostFocus;
            if (XamlRoot.Content != null)
            {
                XamlRoot.Content.ProcessKeyboardAccelerators -= Content_ProcessKeyboardAccelerators;
                XamlRoot.Content.ProcessKeyboardAccelerators += Content_ProcessKeyboardAccelerators;
                XamlRoot.Content.PointerMoved -= Content_PointerMoved;
                XamlRoot.Content.PointerMoved += Content_PointerMoved;
                XamlRoot.Content.KeyDown -= Content_KeyDown;
                XamlRoot.Content.KeyDown += Content_KeyDown;
            }
        }

        private void Menu_Unloaded(object sender, RoutedEventArgs e)
        {
            if (XamlRoot.Content != null)
            {
                XamlRoot.Content.ProcessKeyboardAccelerators -= Content_ProcessKeyboardAccelerators;
                XamlRoot.Content.PointerMoved -= Content_PointerMoved;
                XamlRoot.Content.KeyDown -= Content_KeyDown;
            }

            // Clear Cache
            foreach (MenuItem menuItem in Items)
            {
                var menuFlyoutItems = menuItem.GetMenuFlyoutItems();
                foreach (var flyoutItem in menuFlyoutItems)
                {
                    RemoveElementFromCache(flyoutItem);
                }

                RemoveElementFromCache(menuItem);
            }
        }

        private void Content_PointerMoved(object sender, PointerRoutedEventArgs args)
        {
            var currentPoint = args.GetCurrentPoint(sender as UIElement);

            // if contained with the whole Menu control
            if (IsOpened && _bounds.Contains(currentPoint.Position))
            {
                // if hover over current opened item
                if (SelectedMenuItem.ContainsPoint(currentPoint.Position))
                {
                    return;
                }

                foreach (MenuItem menuItem in Items)
                {
                    if (menuItem.ContainsPoint(currentPoint.Position))
                    {
                        SelectedMenuItem.HideMenu();
                        menuItem.Focus(FocusState.Keyboard);
                        menuItem.ShowMenu();
                    }
                }
            }
        }

        private void Content_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (IsInTransitionState)
            {
                return;
            }

            if (NavigateUsingKeyboard(e.Key, this, Orientation))
            {
                return;
            }

            string gestureKey = MapInputToGestureKey(e.Key);

            if (gestureKey == null)
            {
                return;
            }

            if (MenuItemInputGestureCache.ContainsKey(gestureKey))
            {
                var cachedMenuItem = MenuItemInputGestureCache[gestureKey];
                if (cachedMenuItem is MenuFlyoutItem)
                {
                    var menuFlyoutItem = (MenuFlyoutItem)cachedMenuItem;
                    menuFlyoutItem.Command?.Execute(menuFlyoutItem.CommandParameter);
                }
            }

            if (e.Key == VirtualKey.Menu)
            {
                if (!IsOpened)
                {
                    if (_isLostFocus)
                    {
                        if (_onlyAltCharacterPressed && e.KeyStatus.IsKeyReleased)
                        {
                            ((MenuItem)Items[0]).Focus(FocusState.Programmatic);

                            if (!(_lastFocusElement is MenuItem))
                            {
                                _lastFocusElementBeforeMenu = _lastFocusElement;
                            }
                        }

                        if (AllowTooltip)
                        {
                            ShowMenuItemsToolTips();
                        }
                        else
                        {
                            UnderlineMenuItems();
                        }

                        if (e.KeyStatus.IsKeyReleased)
                        {
                            _isLostFocus = false;
                        }
                    }
                    else if (e.KeyStatus.IsKeyReleased)
                    {
                        HideToolTip();
                        _lastFocusElementBeforeMenu?.Focus(FocusState.Keyboard);
                    }
                }
            }
            else if (e.KeyStatus.IsKeyReleased)
            {
                _onlyAltCharacterPressed = true;
                _isLostFocus = true;
                HideToolTip();
            }
        }

        private void Menu_LostFocus(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem;
            if (XamlRoot != null)
            {
                menuItem = FocusManager.GetFocusedElement(XamlRoot) as MenuItem;
            }
            else
            {
                menuItem = FocusManager.GetFocusedElement() as MenuItem;
            }

            if (AllowTooltip)
            {
                HideMenuItemsTooltips();
            }

            if (menuItem != null || IsOpened)
            {
                return;
            }

            _isLostFocus = true;
            if (!AllowTooltip)
            {
                RemoveUnderlineMenuItems();
            }
        }

        private void Content_ProcessKeyboardAccelerators(UIElement sender, ProcessKeyboardAcceleratorEventArgs args)
        {
            if (Items.Count == 0)
            {
                return;
            }

            if (XamlRoot != null)
            {
                _lastFocusElement = FocusManager.GetFocusedElement(XamlRoot) as Control;
            }
            else
            {
                _lastFocusElement = FocusManager.GetFocusedElement() as Control;
            }

            if (args.Modifiers == VirtualKeyModifiers.Menu)
            {
                _onlyAltCharacterPressed = false;
            }

            if (args.Modifiers == VirtualKeyModifiers.Menu || !_isLostFocus)
            {
                var gestureKey = MapInputToGestureKey(args.Key, !_isLostFocus);
                if (gestureKey == null)
                {
                    return;
                }

                if (MenuItemInputGestureCache.ContainsKey(gestureKey))
                {
                    var cachedMenuItem = MenuItemInputGestureCache[gestureKey];
                    if (cachedMenuItem is MenuItem)
                    {
                        var menuItem = (MenuItem)cachedMenuItem;
                        menuItem.ShowMenu();
                        menuItem.Focus(FocusState.Keyboard);
                    }
                }
            }
        }

        private void HideToolTip()
        {
            if (AllowTooltip)
            {
                HideMenuItemsTooltips();
            }
            else
            {
                RemoveUnderlineMenuItems();
            }
        }
    }
}