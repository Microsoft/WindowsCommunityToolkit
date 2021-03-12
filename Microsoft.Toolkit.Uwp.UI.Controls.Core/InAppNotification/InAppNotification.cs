// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// In App Notification defines a control to show local notification in the app.
    /// </summary>
    [TemplateVisualState(Name = StateContentVisible, GroupName = GroupContent)]
    [TemplateVisualState(Name = StateContentCollapsed, GroupName = GroupContent)]
    [TemplatePart(Name = DismissButtonPart, Type = typeof(Button))]
    [TemplatePart(Name = ContentPresenterPart, Type = typeof(ContentPresenter))]
    public partial class InAppNotification : ContentControl
    {
        private ContentPresenter _contentProvider;
        private DispatcherQueueTimer _dismissTimer;
        private Button _dismissButton;
        private DispatcherQueue _dispatcherQueue;
        private InAppNotificationDismissKind _lastDismissKind;
        private List<NotificationOptions> _stackedNotificationOptions;
        private VisualStateGroup _visualStateGroup;

        /// <summary>
        /// Initializes a new instance of the <see cref="InAppNotification"/> class.
        /// </summary>
        public InAppNotification()
        {
            DefaultStyleKey = typeof(InAppNotification);

            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            _dismissTimer = _dispatcherQueue.CreateTimer();
            _dismissTimer.Tick += DismissTimer_Tick;

            _stackedNotificationOptions = new List<NotificationOptions>();
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_dismissButton != null)
            {
                _dismissButton.Click -= DismissButton_Click;
            }

            if (_visualStateGroup != null)
            {
                _visualStateGroup.CurrentStateChanging -= OnCurrentStateChanging;
                _visualStateGroup.CurrentStateChanged -= OnCurrentStateChanged;
            }

            _dismissButton = (Button)GetTemplateChild(DismissButtonPart);
            _visualStateGroup = (VisualStateGroup)GetTemplateChild(GroupContent);
            _contentProvider = (ContentPresenter)GetTemplateChild(ContentPresenterPart);

            if (_dismissButton != null)
            {
                _dismissButton.Visibility = ShowDismissButton ? Visibility.Visible : Visibility.Collapsed;
                _dismissButton.Click += DismissButton_Click;
                AutomationProperties.SetName(_dismissButton, "WCT_InAppNotification_DismissButton_AutomationName".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources"));
            }

            if (_visualStateGroup != null)
            {
                _visualStateGroup.CurrentStateChanging += OnCurrentStateChanging;
                _visualStateGroup.CurrentStateChanged += OnCurrentStateChanged;
            }

            var firstNotification = _stackedNotificationOptions.FirstOrDefault();
            if (firstNotification != null)
            {
                UpdateContent(firstNotification);
                VisualStateManager.GoToState(this, StateContentVisible, true);
            }

            AutomationProperties.SetLabeledBy(this, this.FindDescendant<ContentPresenter>());
        }

        /// <summary>
        /// Show notification using the current content.
        /// </summary>
        /// <param name="duration">Displayed duration of the notification in ms (less or equal 0 means infinite duration)</param>
        public void Show(int duration = 0)
        {
            // We keep our current content
            var notificationOptions = new NotificationOptions
            {
                Duration = duration,
                Content = Content
            };

            Show(notificationOptions);
        }

        /// <summary>
        /// Show notification using text as the content of the notification
        /// </summary>
        /// <param name="text">Text used as the content of the notification</param>
        /// <param name="duration">Displayed duration of the notification in ms (less or equal 0 means infinite duration)</param>
        public void Show(string text, int duration = 0)
        {
            var notificationOptions = new NotificationOptions
            {
                Duration = duration,
                Content = text
            };
            Show(notificationOptions);
        }

        /// <summary>
        /// Show notification using UIElement as the content of the notification
        /// </summary>
        /// <param name="element">UIElement used as the content of the notification</param>
        /// <param name="duration">Displayed duration of the notification in ms (less or equal 0 means infinite duration)</param>
        public void Show(UIElement element, int duration = 0)
        {
            var notificationOptions = new NotificationOptions
            {
                Duration = duration,
                Content = element
            };
            Show(notificationOptions);
        }

        /// <summary>
        /// Show notification using <paramref name="dataTemplate"/> as the content of the notification
        /// </summary>
        /// <param name="dataTemplate">DataTemplate used as the content of the notification</param>
        /// <param name="duration">Displayed duration of the notification in ms (less or equal 0 means infinite duration)</param>
        public void Show(DataTemplate dataTemplate, int duration = 0)
        {
            var notificationOptions = new NotificationOptions
            {
                Duration = duration,
                Content = dataTemplate
            };
            Show(notificationOptions);
        }

        /// <summary>
        /// Show notification using <paramref name="content"/> as the content of the notification.
        /// The <paramref name="content"/> will be displayed with the current <see cref="ContentControl.ContentTemplate"/>.
        /// </summary>
        /// <param name="content">The content of the notification</param>
        /// <param name="duration">Displayed duration of the notification in ms (less or equal 0 means infinite duration)</param>
        public void Show(object content, int duration = 0)
        {
            var notificationOptions = new NotificationOptions
            {
                Duration = duration,
                Content = content
            };
            Show(notificationOptions);
        }

        /// <summary>
        /// Dismiss the notification
        /// </summary>
        public void Dismiss(bool dismissAll = false)
        {
            Dismiss(InAppNotificationDismissKind.Programmatic, dismissAll);
        }

        /// <summary>
        /// Dismiss the notification
        /// </summary>
        /// <param name="dismissKind">Kind of action that triggered dismiss event</param>
        /// <param name="dismissAll">Indicates if one or all notifications should be dismissed.</param>
        private void Dismiss(InAppNotificationDismissKind dismissKind, bool dismissAll = false)
        {
            if (_stackedNotificationOptions.Count == 0)
            {
                // There is nothing to dismiss.
                return;
            }

            _dismissTimer.Stop();

            // Dismiss all if requested
            if (dismissAll)
            {
                _stackedNotificationOptions.Clear();
            }
            else
            {
                _stackedNotificationOptions.RemoveAt(0);
            }

            // Continue to display notification if on remaining stacked notification
            if (_stackedNotificationOptions.Any())
            {
                var notificationOptions = _stackedNotificationOptions[0];

                UpdateContent(notificationOptions);

                if (notificationOptions.Duration > 0)
                {
                    _dismissTimer.Interval = TimeSpan.FromMilliseconds(notificationOptions.Duration);
                    _dismissTimer.Start();
                }

                return;
            }

            var closingEventArgs = new InAppNotificationClosingEventArgs(dismissKind);
            Closing?.Invoke(this, closingEventArgs);

            if (closingEventArgs.Cancel)
            {
                return;
            }

            var result = VisualStateManager.GoToState(this, StateContentCollapsed, true);
            if (!result)
            {
                // The state transition cannot be executed.
                // It means that the control's template hasn't been applied or that it doesn't contain the state.
                Visibility = Visibility.Collapsed;
            }

            _lastDismissKind = dismissKind;
        }

        /// <summary>
        /// Update the Content of the notification
        /// </summary>
        /// <param name="notificationOptions">Information about the notification to display</param>
        private void UpdateContent(NotificationOptions notificationOptions)
        {
            if (_contentProvider is null)
            {
                // The control template has not been applied yet.
                return;
            }

            switch (notificationOptions.Content)
            {
                case string text:
                    _contentProvider.ContentTemplate = null;
                    _contentProvider.Content = text;
                    break;
                case UIElement element:
                    _contentProvider.ContentTemplate = null;
                    _contentProvider.Content = element;
                    break;
                case DataTemplate dataTemplate:
                    // Without this check, the dataTemplate will fail to render.
                    // Why? Setting the ContentTemplate causes the control to re-evaluate it's Content value.
                    // When we set the ContentTemplate to the same instance of itself, we aren't actually changing the value.
                    // This means that the Content value won't be re-evaluated and stay null, causing the render to fail.
                    if (_contentProvider.ContentTemplate != dataTemplate)
                    {
                        _contentProvider.ContentTemplate = dataTemplate;
                        _contentProvider.Content = null;
                    }

                    break;
                case object content:
                    _contentProvider.ContentTemplate = ContentTemplate;
                    _contentProvider.Content = content;
                    break;
            }

            RaiseAutomationNotification();
        }

        /// <summary>
        /// Handle the display of the notification based on the current StackMode
        /// </summary>
        /// <param name="notificationOptions">Information about the notification to display</param>
        private void Show(NotificationOptions notificationOptions)
        {
            var eventArgs = new InAppNotificationOpeningEventArgs();
            Opening?.Invoke(this, eventArgs);

            if (eventArgs.Cancel)
            {
                return;
            }

            var shouldDisplayImmediately = true;
            switch (StackMode)
            {
                case StackMode.Replace:
                    _stackedNotificationOptions.Clear();
                    _stackedNotificationOptions.Add(notificationOptions);
                    break;
                case StackMode.StackInFront:
                    _stackedNotificationOptions.Insert(0, notificationOptions);
                    break;
                case StackMode.QueueBehind:
                    _stackedNotificationOptions.Add(notificationOptions);
                    shouldDisplayImmediately = _stackedNotificationOptions.Count == 1;
                    break;
                default:
                    break;
            }

            if (shouldDisplayImmediately)
            {
                Visibility = Visibility.Visible;
                VisualStateManager.GoToState(this, StateContentVisible, true);

                UpdateContent(notificationOptions);

                if (notificationOptions.Duration > 0)
                {
                    _dismissTimer.Interval = TimeSpan.FromMilliseconds(notificationOptions.Duration);
                    _dismissTimer.Start();
                }
                else
                {
                    _dismissTimer.Stop();
                }
            }
        }
    }
}
