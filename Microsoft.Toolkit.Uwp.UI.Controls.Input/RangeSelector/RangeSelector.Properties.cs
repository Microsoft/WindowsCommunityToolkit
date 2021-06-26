// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// RangeSelector is a "double slider" control for range values.
    /// </summary>
    public partial class RangeSelector : Control
    {
        /// <summary>
        /// Identifies the <see cref="Minimum"/> property.
        /// </summary>
        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                nameof(Minimum),
                typeof(double),
                typeof(RangeSelector),
                new PropertyMetadata(DefaultMinimum, MinimumChangedCallback));

        /// <summary>
        /// Identifies the <see cref="Maximum"/> property.
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                nameof(Maximum),
                typeof(double),
                typeof(RangeSelector),
                new PropertyMetadata(DefaultMaximum, MaximumChangedCallback));

        /// <summary>
        /// Identifies the <see cref="RangeStart"/> property.
        /// </summary>
        public static readonly DependencyProperty RangeStartProperty =
            DependencyProperty.Register(
                nameof(RangeStart),
                typeof(double),
                typeof(RangeSelector),
                new PropertyMetadata(DefaultMinimum, RangeMinChangedCallback));

        /// <summary>
        /// Identifies the <see cref="RangeEnd"/> property.
        /// </summary>
        public static readonly DependencyProperty RangeEndProperty =
            DependencyProperty.Register(
                nameof(RangeEnd),
                typeof(double),
                typeof(RangeSelector),
                new PropertyMetadata(DefaultMaximum, RangeMaxChangedCallback));

        /// <summary>
        /// Identifies the <see cref="StepFrequency"/> property.
        /// </summary>
        public static readonly DependencyProperty StepFrequencyProperty =
            DependencyProperty.Register(
                nameof(StepFrequency),
                typeof(double),
                typeof(RangeSelector),
                new PropertyMetadata(DefaultStepFrequency));

        /// <summary>
        /// Gets or sets the absolute minimum value of the range.
        /// </summary>
        /// <value>
        /// The minimum.
        /// </value>
        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        /// <summary>
        /// Gets or sets the absolute maximum value of the range.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        /// <summary>
        /// Gets or sets the current selected lower limit value of the range, modifiable by the user.
        /// </summary>
        /// <value>
        /// The current lower limit.
        /// </value>
        public double RangeStart
        {
            get => (double)GetValue(RangeStartProperty);
            set => SetValue(RangeStartProperty, value);
        }

        /// <summary>
        /// Gets or sets the current selected upper limit value of the range, modifiable by the user.
        /// </summary>
        /// <value>
        /// The current upper limit.
        /// </value>
        public double RangeEnd
        {
            get => (double)GetValue(RangeEndProperty);
            set => SetValue(RangeEndProperty, value);
        }

        /// <summary>
        /// Gets or sets the value part of a value range that steps should be created for.
        /// </summary>
        /// <value>
        /// The value part of a value range that steps should be created for.
        /// </value>
        public double StepFrequency
        {
            get => (double)GetValue(StepFrequencyProperty);
            set => SetValue(StepFrequencyProperty, value);
        }

        private static void MinimumChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rangeSelector = d as RangeSelector;

            if (rangeSelector == null || !rangeSelector._valuesAssigned)
            {
                return;
            }

            var newValue = (double)e.NewValue;
            var oldValue = (double)e.OldValue;

            if (rangeSelector.Maximum < newValue)
            {
                rangeSelector.Maximum = newValue + Epsilon;
            }

            if (rangeSelector.RangeStart < newValue)
            {
                rangeSelector.RangeStart = newValue;
            }

            if (rangeSelector.RangeEnd < newValue)
            {
                rangeSelector.RangeEnd = newValue;
            }

            if (newValue != oldValue)
            {
                rangeSelector.SyncThumbs();
            }
        }

        private static void MaximumChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rangeSelector = d as RangeSelector;

            if (rangeSelector == null || !rangeSelector._valuesAssigned)
            {
                return;
            }

            var newValue = (double)e.NewValue;
            var oldValue = (double)e.OldValue;

            if (rangeSelector.Minimum > newValue)
            {
                rangeSelector.Minimum = newValue - Epsilon;
            }

            if (rangeSelector.RangeEnd > newValue)
            {
                rangeSelector.RangeEnd = newValue;
            }

            if (rangeSelector.RangeStart > newValue)
            {
                rangeSelector.RangeStart = newValue;
            }

            if (newValue != oldValue)
            {
                rangeSelector.SyncThumbs();
            }
        }

        private static void RangeMinChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rangeSelector = d as RangeSelector;

            if (rangeSelector == null)
            {
                return;
            }

            rangeSelector._minSet = true;

            if (!rangeSelector._valuesAssigned)
            {
                return;
            }

            var newValue = (double)e.NewValue;
            rangeSelector.RangeMinToStepFrequency();

            if (rangeSelector._valuesAssigned)
            {
                if (newValue < rangeSelector.Minimum)
                {
                    rangeSelector.RangeStart = rangeSelector.Minimum;
                }
                else if (newValue > rangeSelector.Maximum)
                {
                    rangeSelector.RangeStart = rangeSelector.Maximum;
                }

                rangeSelector.SyncActiveRectangle();

                // If the new value is greater than the old max, move the max also
                if (newValue > rangeSelector.RangeEnd)
                {
                    rangeSelector.RangeEnd = newValue;
                }
            }

            rangeSelector.SyncThumbs();
        }

        private static void RangeMaxChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rangeSelector = d as RangeSelector;

            if (rangeSelector == null)
            {
                return;
            }

            rangeSelector._maxSet = true;

            if (!rangeSelector._valuesAssigned)
            {
                return;
            }

            var newValue = (double)e.NewValue;
            rangeSelector.RangeMaxToStepFrequency();

            if (rangeSelector._valuesAssigned)
            {
                if (newValue < rangeSelector.Minimum)
                {
                    rangeSelector.RangeEnd = rangeSelector.Minimum;
                }
                else if (newValue > rangeSelector.Maximum)
                {
                    rangeSelector.RangeEnd = rangeSelector.Maximum;
                }

                rangeSelector.SyncActiveRectangle();

                // If the new max is less than the old minimum then move the minimum
                if (newValue < rangeSelector.RangeStart)
                {
                    rangeSelector.RangeStart = newValue;
                }
            }

            rangeSelector.SyncThumbs();
        }
    }
}