// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Microsoft.Toolkit.Uwp.UI.Behaviors
{
#pragma warning disable SA1402 // File may only contain a single type
    /// <summary>
    /// This behavior sets the focus on the first control of <see cref="Targets"/> which accepts it.
    /// The focus will be set following the <see cref="Targets"/> order. The first control being ready
    /// and accepting the focus will receive it.
    /// The focus can be set to another control with a higher priority if it loads before <see cref="FocusEngagementTimeout"/>.
    /// </summary>
    [ContentProperty(Name = nameof(Targets))]
    public sealed class FocusBehavior : BehaviorBase<UIElement>
    {
        /// <summary>
        /// The DP to store the <see cref="Targets"/> property value.
        /// </summary>
        public static readonly DependencyProperty TargetsProperty = DependencyProperty.Register(
            nameof(Targets),
            typeof(FocusTargetList),
            typeof(FocusBehavior),
            new PropertyMetadata(null, OnTargetsPropertyChanged));

        /// <summary>
        /// The DP to store the <see cref="FocusEngagementTimeout"/> property value.
        /// </summary>
        public static readonly DependencyProperty FocusEngagementTimeoutProperty = DependencyProperty.Register(
            nameof(FocusEngagementTimeout),
            typeof(TimeSpan),
            typeof(FocusBehavior),
            new PropertyMetadata(TimeSpan.FromMilliseconds(100)));

        private DispatcherQueueTimer _timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FocusBehavior"/> class.
        /// </summary>
        public FocusBehavior() => Targets = new FocusTargetList();

        /// <summary>
        /// Gets or sets the ordered list of controls which should receive the focus when the associated object is loaded.
        /// </summary>
        public FocusTargetList Targets
        {
            get => (FocusTargetList)GetValue(TargetsProperty);
            set => SetValue(TargetsProperty, value);
        }

        /// <summary>
        /// Gets or sets the timeout before the <see cref="FocusBehavior"/> stops trying to set the focus to a control with
        /// a higher priority.
        /// </summary>
        public TimeSpan FocusEngagementTimeout
        {
            get => (TimeSpan)GetValue(FocusEngagementTimeoutProperty);
            set => SetValue(FocusEngagementTimeoutProperty, value);
        }

        /// <inheritdoc/>
        protected override void OnAssociatedObjectLoaded() => ApplyFocus();

        /// <inheritdoc/>
        protected override void OnDetaching() => Stop();

        private static void OnTargetsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (FocusBehavior)d;

            if (e.OldValue is FocusTargetList oldTargets)
            {
                behavior.Stop(oldTargets);
            }

            behavior.ApplyFocus();
        }

        private void ApplyFocus()
        {
            if (Targets.Count == 0)
            {
                return;
            }

            var focusedControlIndex = -1;
            for (var i = 0; i < Targets.Count; i++)
            {
                var control = Targets[i].Control;
                if (control.IsLoaded)
                {
                    if (control.Focus(FocusState.Programmatic))
                    {
                        focusedControlIndex = i;
                        break;
                    }
                }
                else
                {
                    control.Loaded += OnControlLoaded;
                }
            }

            if (focusedControlIndex == 0 || Targets.All(t => t.Control?.IsLoaded == true))
            {
                // The first control has received the focus or all the control are loaded and none can take the focus: we stop.
                Stop();
            }
            else if (focusedControlIndex > 0)
            {
                // We have been able to set the focus on one control.
                // We start the timer to detect if we can focus another control with an higher priority.
                // This allows us to handle the case where the controls are not loaded in the order we expect.
                if (_timer is null)
                {
                    _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
                    _timer.Interval = FocusEngagementTimeout;
                    _timer.Tick += OnEngagementTimerTick;
                    _timer.Start();
                }
            }
        }

        private void OnEngagementTimerTick(object sender, object e)
        {
            ApplyFocus();
            Stop();
        }

        private void OnControlLoaded(object sender, RoutedEventArgs e) => ApplyFocus();

        private void Stop(FocusTargetList targets = null)
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer = null;
            }

            foreach (var target in targets ?? Targets)
            {
                if (target.Control is null)
                {
                    continue;
                }

                target.Control.Loaded -= OnControlLoaded;
            }
        }
    }

    /// <summary>
    /// A collection of <see cref="FocusTarget"/>.
    /// </summary>
    public sealed class FocusTargetList : List<FocusTarget>
    {
    }

    /// <summary>
    /// A target for the <see cref="FocusBehavior"/>.
    /// </summary>
    public sealed partial class FocusTarget : DependencyObject
    {
        /// <summary>
        /// The DP to store the <see cref="Control"/> property value.
        /// </summary>
        public static readonly DependencyProperty ControlProperty = DependencyProperty.Register(
            nameof(Control),
            typeof(Control),
            typeof(FocusTarget),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the control that will receive the focus.
        /// </summary>
        public Control Control
        {
            get => (Control)GetValue(ControlProperty);
            set => SetValue(ControlProperty, value);
        }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
