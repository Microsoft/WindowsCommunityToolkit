// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Triggers
{
    /// <summary>
    /// A conditional state trigger that functions
    /// based on the target control's width.
    /// </summary>
    public class ControlWidthTrigger : StateTriggerBase
    {
        /// <summary>
        /// Gets or sets a value indicating
        /// whether this trigger will be active or not.
        /// </summary>
        public bool CanTrigger
        {
            get => (bool)GetValue(CanTriggerProperty);
            set => SetValue(CanTriggerProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CanTrigger"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty CanTriggerProperty = DependencyProperty.Register(
            nameof(CanTrigger),
            typeof(bool),
            typeof(ControlWidthTrigger),
            new PropertyMetadata(true, OnCanTriggerProperty));

        private static void OnCanTriggerProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ControlWidthTrigger)d).UpdateTrigger();
        }

        /// <summary>
        /// Gets or sets the max size at which to trigger.
        /// </summary>
        public double MaxWidth
        {
            get => (double)GetValue(MaxWidthProperty);
            set => SetValue(MaxWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MaxWidth"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty MaxWidthProperty = DependencyProperty.Register(
            nameof(MaxWidth),
            typeof(double),
            typeof(ControlWidthTrigger),
            new PropertyMetadata(double.PositiveInfinity, OnMaxWidthPropertyChanged));

        private static void OnMaxWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ControlWidthTrigger)d).UpdateTrigger();
        }

        /// <summary>
        /// Gets or sets the min size at which to trigger.
        /// </summary>
        public double MinWidth
        {
            get => (double)GetValue(MinWidthProperty);
            set => SetValue(MinWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MinWidth"/> DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty MinWidthProperty = DependencyProperty.Register(
            nameof(MinWidth),
            typeof(double),
            typeof(ControlWidthTrigger),
            new PropertyMetadata(0.0, OnMinWidthPropertyChanged));

        private static void OnMinWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ControlWidthTrigger)d).UpdateTrigger();
        }

        /// <summary>
        /// Gets or sets the element whose width will observed
        /// for the trigger.
        /// </summary>
        public FrameworkElement TargetElement
        {
            get => (FrameworkElement)GetValue(TargetElementProperty);
            set => SetValue(TargetElementProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TargetElement"/> DependencyProperty.
        /// </summary>
        /// <remarks>
        /// Using a DependencyProperty as the backing store for TargetElement. This enables animation, styling, binding, etc.
        /// </remarks>
        public static readonly DependencyProperty TargetElementProperty = DependencyProperty.Register(
            nameof(TargetElement),
            typeof(FrameworkElement),
            typeof(ControlWidthTrigger),
            new PropertyMetadata(null, OnTargetElementPropertyChanged));

        private static void OnTargetElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ControlWidthTrigger)d).UpdateTargetElement((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
        }

        // Handle event to get current values
        private void OnTargetElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateTrigger();
        }

        private void UpdateTargetElement(FrameworkElement oldValue, FrameworkElement newValue)
        {
            if (oldValue != null)
            {
                oldValue.SizeChanged -= OnTargetElementSizeChanged;
            }

            if (newValue != null)
            {
                newValue.SizeChanged += OnTargetElementSizeChanged;
            }

            UpdateTrigger();
        }

        // Logic to evaluate and apply trigger value
        private void UpdateTrigger()
        {
            if (TargetElement == null || !CanTrigger)
            {
                SetActive(false);
                return;
            }

            SetActive(MinWidth <= TargetElement.ActualWidth && TargetElement.ActualWidth < MaxWidth);
        }
    }
}
