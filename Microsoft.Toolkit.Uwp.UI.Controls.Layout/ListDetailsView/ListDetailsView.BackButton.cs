// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Windows.ApplicationModel;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Panel that allows for a List/Details pattern.
    /// </summary>
    /// <seealso cref="ItemsControl" />
    public partial class ListDetailsView
    {
        private AppViewBackButtonVisibility? _previousSystemBackButtonVisibility;
        private bool _previousNavigationViewBackEnabled;

        // Int used because the underlying type is an enum, but we don't have access to the enum
        private int _previousNavigationViewBackVisibilty;
        private Button _inlineBackButton;
        private object _navigationView;
        private Frame _frame;

        /// <summary>
        /// Sets the back button visibility based on the current visual state and selected item
        /// </summary>
        private void SetBackButtonVisibility(ListDetailsViewState? previousState = null)
        {
            const int backButtonVisible = 1;

            if (DesignMode.DesignModeEnabled)
            {
                return;
            }

            if (ViewState == ListDetailsViewState.Details)
            {
                if ((BackButtonBehavior == BackButtonBehavior.Inline) && (_inlineBackButton != null))
                {
                    _inlineBackButton.Visibility = Visibility.Visible;
                }
                else if (BackButtonBehavior == BackButtonBehavior.Automatic)
                {
                    // Continue to support the system back button if it is being used
                    SystemNavigationManager navigationManager = SystemNavigationManager.GetForCurrentView();
                    if (navigationManager.AppViewBackButtonVisibility == AppViewBackButtonVisibility.Visible)
                    {
                        // Setting this indicates that the system back button is being used
                        _previousSystemBackButtonVisibility = navigationManager.AppViewBackButtonVisibility;
                    }
                    else if ((_inlineBackButton != null) && ((_navigationView == null) || (_frame == null)))
                    {
                        // We can only use the new NavigationView if we also have a Frame
                        // If there is no frame we have to use the inline button
                        _inlineBackButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        SetNavigationViewBackButtonState(backButtonVisible, true);
                    }
                }
                else if (BackButtonBehavior != BackButtonBehavior.Manual)
                {
                    SystemNavigationManager navigationManager = SystemNavigationManager.GetForCurrentView();
                    _previousSystemBackButtonVisibility = navigationManager.AppViewBackButtonVisibility;

                    navigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                }
            }
            else if (previousState == ListDetailsViewState.Details)
            {
                if ((BackButtonBehavior == BackButtonBehavior.Inline) && (_inlineBackButton != null))
                {
                    _inlineBackButton.Visibility = Visibility.Collapsed;
                }
                else if (BackButtonBehavior == BackButtonBehavior.Automatic)
                {
                    if (!_previousSystemBackButtonVisibility.HasValue)
                    {
                        if ((_inlineBackButton != null) && ((_navigationView == null) || (_frame == null)))
                        {
                            _inlineBackButton.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            SetNavigationViewBackButtonState(_previousNavigationViewBackVisibilty, _previousNavigationViewBackEnabled);
                        }
                    }
                }

                if (_previousSystemBackButtonVisibility.HasValue)
                {
                    // Make sure we show the back button if the stack can navigate back
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = _previousSystemBackButtonVisibility.Value;
                    _previousSystemBackButtonVisibility = null;
                }
            }
        }

        private void SetNavigationViewBackButtonState(int visible, bool enabled)
        {
            if (_navigationView == null)
            {
                return;
            }

            System.Type navType = _navigationView.GetType();
            PropertyInfo visibleProperty = navType.GetProperty("IsBackButtonVisible");
            if (visibleProperty != null)
            {
                _previousNavigationViewBackVisibilty = (int)visibleProperty.GetValue(_navigationView);
                visibleProperty.SetValue(_navigationView, visible);
            }

            PropertyInfo enabledProperty = navType.GetProperty("IsBackEnabled");
            if (enabledProperty != null)
            {
                _previousNavigationViewBackEnabled = (bool)enabledProperty.GetValue(_navigationView);
                enabledProperty.SetValue(_navigationView, enabled);
            }
        }

        /// <summary>
        /// Closes the details pane if we are in narrow state
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The event args</param>
        private void OnFrameNavigating(object sender, NavigatingCancelEventArgs args)
        {
            if ((args.NavigationMode == NavigationMode.Back) && (ViewState == ListDetailsViewState.Details))
            {
                ClearSelectedItem();
                args.Cancel = true;
            }
        }

        /// <summary>
        /// Closes the details pane if we are in narrow state
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="args">The event args</param>
        private void OnBackRequested(object sender, BackRequestedEventArgs args)
        {
            if (ViewState == ListDetailsViewState.Details)
            {
                // let the OnFrameNavigating method handle it if
                if (_frame == null || !_frame.CanGoBack)
                {
                    ClearSelectedItem();
                }

                args.Handled = true;
            }
        }

        private void OnInlineBackButtonClicked(object sender, RoutedEventArgs e)
        {
            ClearSelectedItem();
        }
    }
}
