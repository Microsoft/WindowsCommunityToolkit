// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;

namespace CommunityToolkit.WinUI.UI.Triggers
{
    /// <summary>
    /// Trigger for switching when the User interaction mode changes (tablet mode)
    /// </summary>
    public sealed class UserInteractionModeStateTrigger : StateTriggerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserInteractionModeStateTrigger"/> class.
        /// </summary>
        public UserInteractionModeStateTrigger()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                var weakEvent =
                    new WeakEventListener<UserInteractionModeStateTrigger, object, WindowSizeChangedEventArgs>(this)
                    {
                        OnEventAction = (instance, source, eventArgs) => UserInteractionModeTrigger_SizeChanged(source, eventArgs),
                        OnDetachAction = (weakEventListener) =>
                        {
                            if (Window.Current != null)
                            {
                                Window.Current.SizeChanged -= weakEventListener.OnEvent;
                            }
                        }
                    };

                if (Window.Current != null)
                {
                    Window.Current.SizeChanged += weakEvent.OnEvent;
                }

                UpdateTrigger(InteractionMode);
            }
        }

        /// <summary>
        /// Gets or sets the InteractionMode to trigger on.
        /// </summary>
        public UserInteractionMode InteractionMode
        {
            get { return (UserInteractionMode)GetValue(InteractionModeProperty); }
            set { SetValue(InteractionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="InteractionMode"/> parameter.
        /// </summary>
        public static readonly DependencyProperty InteractionModeProperty =
            DependencyProperty.Register(nameof(InteractionMode), typeof(UserInteractionMode), typeof(UserInteractionModeStateTrigger), new PropertyMetadata(UserInteractionMode.Mouse, OnInteractionModeChanged));

        private static void OnInteractionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (UserInteractionModeStateTrigger)d;
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                var orientation = (UserInteractionMode)e.NewValue;
                obj.UpdateTrigger(orientation);
            }
        }

        private void UpdateTrigger(UserInteractionMode interactionMode)
        {
            SetActive(interactionMode == UIViewSettings.GetForCurrentView().UserInteractionMode);
        }

        private void UserInteractionModeTrigger_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            UpdateTrigger(InteractionMode);
        }
    }
}