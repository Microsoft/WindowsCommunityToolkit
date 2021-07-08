// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel;
using Windows.UI.Core;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Panel that allows for a List/Details pattern.
    /// </summary>
    [TemplatePart(Name = PartDetailsPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = PartDetailsPanel, Type = typeof(FrameworkElement))]
    [TemplateVisualState(Name = NoSelectionNarrowState, GroupName = SelectionStates)]
    [TemplateVisualState(Name = NoSelectionWideState, GroupName = SelectionStates)]
    [TemplateVisualState(Name = HasSelectionWideState, GroupName = SelectionStates)]
    [TemplateVisualState(Name = HasSelectionNarrowState, GroupName = SelectionStates)]
    [TemplateVisualState(Name = NarrowState, GroupName = WidthStates)]
    [TemplateVisualState(Name = WideState, GroupName = WidthStates)]
    public partial class ListDetailsView : ItemsControl
    {
        private const string PartDetailsPresenter = "DetailsPresenter";
        private const string PartDetailsPanel = "DetailsPanel";
        private const string PartBackButton = "ListDetailsBackButton";
        private const string PartHeaderContentPresenter = "HeaderContentPresenter";
        private const string NarrowState = "NarrowState";
        private const string WideState = "WideState";
        private const string WidthStates = "WidthStates";
        private const string SelectionStates = "SelectionStates";
        private const string HasSelectionNarrowState = "HasSelectionNarrow";
        private const string HasSelectionWideState = "HasSelectionWide";
        private const string NoSelectionNarrowState = "NoSelectionNarrow";
        private const string NoSelectionWideState = "NoSelectionWide";

        private AppViewBackButtonVisibility? _previousSystemBackButtonVisibility;
        private bool _previousNavigationViewBackEnabled;

        // Int used because the underlying type is an enum, but we don't have access to the enum
        private int _previousNavigationViewBackVisibilty;
        private ContentPresenter _detailsPresenter;
        private VisualStateGroup _selectionStateGroup;
        private Button _inlineBackButton;
        private object _navigationView;
        private Frame _frame;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListDetailsView"/> class.
        /// </summary>
        public ListDetailsView()
        {
            DefaultStyleKey = typeof(ListDetailsView);

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call
        /// ApplyTemplate. In simplest terms, this means the method is called just before a UI element displays
        /// in your app. Override this method to influence the default post-template logic of a class.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_inlineBackButton != null)
            {
                _inlineBackButton.Click -= OnInlineBackButtonClicked;
            }

            _inlineBackButton = (Button)GetTemplateChild(PartBackButton);
            if (_inlineBackButton != null)
            {
                _inlineBackButton.Click += OnInlineBackButtonClicked;
            }

            _detailsPresenter = (ContentPresenter)GetTemplateChild(PartDetailsPresenter);
            SetDetailsContent();

            SetListHeaderVisibility();
            OnDetailsCommandBarChanged();
            OnListCommandBarChanged();

            SizeChanged -= ListDetailsView_SizeChanged;
            SizeChanged += ListDetailsView_SizeChanged;

            UpdateView(true);
        }

        /// <summary>
        /// Fired when the SelectedIndex changes.
        /// </summary>
        /// <param name="d">The sender</param>
        /// <param name="e">The event args</param>
        /// <remarks>
        /// Sets up animations for the DetailsPresenter for animating in/out.
        /// </remarks>
        private static void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ListDetailsView)d;

            var newValue = (int)e.NewValue < 0 ? null : view.Items[(int)e.NewValue];
            var oldValue = e.OldValue == null ? null : view.Items.ElementAtOrDefault((int)e.OldValue);

            // check if selection actually changed
            if (view.SelectedItem != newValue)
            {
                // sync SelectedItem
                view.SetValue(SelectedItemProperty, newValue);
                view.UpdateSelection(oldValue, newValue);
            }
        }

        /// <summary>
        /// Fired when the SelectedItem changes.
        /// </summary>
        /// <param name="d">The sender</param>
        /// <param name="e">The event args</param>
        /// <remarks>
        /// Sets up animations for the DetailsPresenter for animating in/out.
        /// </remarks>
        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ListDetailsView)d;
            var index = e.NewValue == null ? -1 : view.Items.IndexOf(e.NewValue);

            // check if selection actually changed
            if (view.SelectedIndex != index)
            {
                // sync SelectedIndex
                view.SetValue(SelectedIndexProperty, index);
                view.UpdateSelection(e.OldValue, e.NewValue);
            }
        }

        /// <summary>
        /// Fired when the <see cref="ListHeader"/> is changed.
        /// </summary>
        /// <param name="d">The sender</param>
        /// <param name="e">The event args</param>
        private static void OnListHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ListDetailsView)d;
            view.SetListHeaderVisibility();
        }

        /// <summary>
        /// Fired when the DetailsCommandBar changes.
        /// </summary>
        /// <param name="d">The sender</param>
        /// <param name="e">The event args</param>
        private static void OnDetailsCommandBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ListDetailsView)d;
            view.OnDetailsCommandBarChanged();
        }

        /// <summary>
        /// Fired when CompactModeThresholdWIdthChanged
        /// </summary>
        /// <param name="d">The sender</param>
        /// <param name="e">The event args</param>
        private static void OnCompactModeThresholdWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ListDetailsView)d).HandleStateChanges();
        }

        private static void OnBackButtonBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ListDetailsView)d;
            view.SetBackButtonVisibility();
        }

        /// <summary>
        /// Fired when the <see cref="ListCommandBar"/> changes.
        /// </summary>
        /// <param name="d">The sender</param>
        /// <param name="e">The event args</param>
        private static void OnListCommandBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = (ListDetailsView)d;
            view.OnListCommandBarChanged();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled == false)
            {
                if (Window.Current != null)
                {
                    SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
                }

                if (_frame != null)
                {
                    _frame.Navigating -= OnFrameNavigating;
                }

                _navigationView = this.FindAscendants().FirstOrDefault(p => p.GetType().FullName == "Microsoft.UI.Xaml.Controls.NavigationView");
                _frame = this.FindAscendant<Frame>();
                if (_frame != null)
                {
                    _frame.Navigating += OnFrameNavigating;
                }

                _selectionStateGroup = (VisualStateGroup)GetTemplateChild(SelectionStates);
                if (_selectionStateGroup != null)
                {
                    _selectionStateGroup.CurrentStateChanged += OnSelectionStateChanged;
                }

                UpdateView(true);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (DesignMode.DesignModeEnabled == false)
            {
                if (Window.Current != null)
                {
                    SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
                }

                if (_frame != null)
                {
                    _frame.Navigating -= OnFrameNavigating;
                }

                _selectionStateGroup = (VisualStateGroup)GetTemplateChild(SelectionStates);
                if (_selectionStateGroup != null)
                {
                    _selectionStateGroup.CurrentStateChanged -= OnSelectionStateChanged;
                    _selectionStateGroup = null;
                }
            }
        }

        private void ListDetailsView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // if size is changing
            if ((e.PreviousSize.Width < CompactModeThresholdWidth && e.NewSize.Width >= CompactModeThresholdWidth) ||
                (e.PreviousSize.Width >= CompactModeThresholdWidth && e.NewSize.Width < CompactModeThresholdWidth))
            {
                HandleStateChanges();
            }
        }

        private void OnInlineBackButtonClicked(object sender, RoutedEventArgs e)
        {
            SelectedItem = null;
        }

        /// <summary>
        /// Raises SelectionChanged event and updates view.
        /// </summary>
        /// <param name="oldSelection">Old selection.</param>
        /// <param name="newSelection">New selection.</param>
        private void UpdateSelection(object oldSelection, object newSelection)
        {
            OnSelectionChanged(new SelectionChangedEventArgs(new List<object> { oldSelection }, new List<object> { newSelection }));

            UpdateView(true);

            // If there is no selection, do not remove the DetailsPresenter content but let it animate out.
            if (SelectedItem != null)
            {
                SetDetailsContent();
            }
        }

        private void HandleStateChanges()
        {
            UpdateView(true);
            SetListSelectionWithKeyboardFocusOnVisualStateChanged(ViewState);
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
                SelectedItem = null;
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
                    SelectedItem = null;
                }

                args.Handled = true;
            }
        }

        private void SetListHeaderVisibility()
        {
            if (GetTemplateChild(PartHeaderContentPresenter) is FrameworkElement headerPresenter)
            {
                headerPresenter.Visibility = ListHeader != null
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        private void UpdateView(bool animate)
        {
            UpdateViewState();
            SetVisualState(animate);
        }

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
                    if (Window.Current != null)
                    {
                        // Continue to support the system back button if it is being used
                        var navigationManager = SystemNavigationManager.GetForCurrentView();
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
                }
                else if (BackButtonBehavior != BackButtonBehavior.Manual)
                {
                    if (Window.Current != null)
                    {
                        var navigationManager = SystemNavigationManager.GetForCurrentView();
                        _previousSystemBackButtonVisibility = navigationManager.AppViewBackButtonVisibility;

                        navigationManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    }
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
                    if (_previousSystemBackButtonVisibility.HasValue == false)
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
                    if (Window.Current != null)
                    {
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = _previousSystemBackButtonVisibility.Value;
                        _previousSystemBackButtonVisibility = null;
                    }
                }
            }
        }

        private void UpdateViewState()
        {
            var previousState = ViewState;

            if (ActualWidth < CompactModeThresholdWidth)
            {
                ViewState = SelectedItem == null ? ListDetailsViewState.List : ListDetailsViewState.Details;
            }
            else
            {
                ViewState = ListDetailsViewState.Both;
            }

            if (previousState != ViewState)
            {
                ViewStateChanged?.Invoke(this, ViewState);
                SetBackButtonVisibility(previousState);
            }
        }

        private void SetVisualState(bool animate)
        {
            string state;
            string noSelectionState;
            string hasSelectionState;
            if (ActualWidth < CompactModeThresholdWidth)
            {
                state = NarrowState;
                noSelectionState = NoSelectionNarrowState;
                hasSelectionState = HasSelectionNarrowState;
            }
            else
            {
                state = WideState;
                noSelectionState = NoSelectionWideState;
                hasSelectionState = HasSelectionWideState;
            }

            VisualStateManager.GoToState(this, state, animate);
            VisualStateManager.GoToState(this, SelectedItem == null ? noSelectionState : hasSelectionState, animate);
        }

        private void SetNavigationViewBackButtonState(int visible, bool enabled)
        {
            if (_navigationView == null)
            {
                return;
            }

            var navType = _navigationView.GetType();
            var visibleProperty = navType.GetProperty("IsBackButtonVisible");
            if (visibleProperty != null)
            {
                _previousNavigationViewBackVisibilty = (int)visibleProperty.GetValue(_navigationView);
                visibleProperty.SetValue(_navigationView, visible);
            }

            var enabledProperty = navType.GetProperty("IsBackEnabled");
            if (enabledProperty != null)
            {
                _previousNavigationViewBackEnabled = (bool)enabledProperty.GetValue(_navigationView);
                enabledProperty.SetValue(_navigationView, enabled);
            }
        }

        private void SetDetailsContent()
        {
            if (_detailsPresenter != null)
            {
                _detailsPresenter.Content = MapDetails == null
                    ? SelectedItem
                    : SelectedItem != null ? MapDetails(SelectedItem) : null;
            }
        }

        private void OnListCommandBarChanged()
        {
            OnCommandBarChanged("ListCommandBarPanel", ListCommandBar);
        }

        private void OnDetailsCommandBarChanged()
        {
            OnCommandBarChanged("DetailsCommandBarPanel", DetailsCommandBar);
        }

        private void OnCommandBarChanged(string panelName, CommandBar commandbar)
        {
            var panel = GetTemplateChild(panelName) as Panel;
            if (panel == null)
            {
                return;
            }

            panel.Children.Clear();
            if (commandbar != null)
            {
                panel.Children.Add(commandbar);
            }
        }

        /// <summary>
        /// Sets whether the selected item should change when focused with the keyboard based on the view state
        /// </summary>
        /// <param name="viewState">the view state</param>
        private void SetListSelectionWithKeyboardFocusOnVisualStateChanged(ListDetailsViewState viewState)
        {
            if (viewState == ListDetailsViewState.Both)
            {
                SetListSelectionWithKeyboardFocus(true);
            }
            else
            {
                SetListSelectionWithKeyboardFocus(false);
            }
        }

        /// <summary>
        /// Sets whether the selected item should change when focused with the keyboard
        /// </summary>
        private void SetListSelectionWithKeyboardFocus(bool singleSelectionFollowsFocus)
        {
            if (GetTemplateChild("List") is Microsoft.UI.Xaml.Controls.ListViewBase list)
            {
                list.SingleSelectionFollowsFocus = singleSelectionFollowsFocus;
            }
        }

        /// <summary>
        /// Fires when the selection state of the control changes
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="e">the event args</param>
        /// <remarks>
        /// Sets focus to the item list when the viewState is not Details.
        /// Sets whether the selected item should change when focused with the keyboard.
        /// </remarks>
        private void OnSelectionStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            SetFocus(ViewState);
            SetListSelectionWithKeyboardFocusOnVisualStateChanged(ViewState);
        }

        /// <summary>
        /// Sets focus to the relevant control based on the viewState.
        /// </summary>
        /// <param name="viewState">the view state</param>
        private void SetFocus(ListDetailsViewState viewState)
        {
            if (viewState != ListDetailsViewState.Details)
            {
                FocusItemList();
            }
            else
            {
                FocusFirstFocusableElementInDetails();
            }
        }

        /// <summary>
        /// Sets focus to the first focusable element in the details template
        /// </summary>
        private void FocusFirstFocusableElementInDetails()
        {
            if (GetTemplateChild(PartDetailsPanel) is DependencyObject details)
            {
                var focusableElement = FocusManager.FindFirstFocusableElement(details);
                (focusableElement as Control)?.Focus(FocusState.Programmatic);
            }
        }

        /// <summary>
        /// Sets focus to the item list
        /// </summary>
        private void FocusItemList()
        {
            if (GetTemplateChild("List") is Control list)
            {
                list.Focus(FocusState.Programmatic);
            }
        }
    }
}