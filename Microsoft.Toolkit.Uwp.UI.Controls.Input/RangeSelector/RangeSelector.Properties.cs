// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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

            if (newValue > rangeSelector.Maximum)
            {
                rangeSelector.Maximum = newValue;
                rangeSelector.RangeStart = newValue;
                rangeSelector.RangeEnd = newValue;
            }
            else if (newValue > rangeSelector.RangeStart)
            {
                rangeSelector.RangeStart = newValue;

                if (newValue > rangeSelector.RangeEnd)
                {
                    rangeSelector.RangeEnd = newValue;
                }
            }
            else
            {
                rangeSelector.RangeStart = newValue + SteppedDistanceFromBound(rangeSelector.StepFrequency, rangeSelector.RangeStart - newValue);
            }

            rangeSelector.Minimum = newValue;

            rangeSelector.SyncThumbs();
        }

        private static void MaximumChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rangeSelector = d as RangeSelector;

            if (rangeSelector == null || !rangeSelector._valuesAssigned)
            {
                return;
            }

            var newValue = (double)e.NewValue;

            if (newValue < rangeSelector.Minimum)
            {
                rangeSelector.Minimum = newValue;
                rangeSelector.RangeStart = newValue;
                rangeSelector.RangeEnd = newValue;
            }
            else if (newValue < rangeSelector.RangeEnd)
            {
                rangeSelector.RangeEnd = newValue;

                if (newValue < rangeSelector.RangeStart)
                {
                    rangeSelector.RangeStart = newValue;
                }
            }
            else
            {
                rangeSelector.RangeEnd = newValue - SteppedDistanceFromBound(rangeSelector.StepFrequency, newValue - rangeSelector.RangeEnd);
            }

            rangeSelector.Maximum = newValue;

            rangeSelector.SyncThumbs();
        }

        private static double SteppedDistanceFromBound(double stepSize, double distanceFromBound)
        {
            var smallerStep = Math.Floor(distanceFromBound / stepSize) * stepSize;
            var biggerStep = Math.Ceiling(distanceFromBound / stepSize) * stepSize;
            var distanceToSmallerStep = distanceFromBound - smallerStep;
            var distanceToBiggerStep = biggerStep - distanceFromBound;

            return distanceToSmallerStep <= distanceToBiggerStep ? smallerStep : biggerStep;
        }

        private static void RangeMinChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rangeSelector = d as RangeSelector;

            if (rangeSelector == null || !rangeSelector._valuesAssigned)
            {
                return;
            }

            var newValue = (double)e.NewValue;

            if (newValue < rangeSelector.Minimum)
            {
                rangeSelector.RangeStart = rangeSelector.Minimum;
            }
            else
            {
                var steppedNewValue = rangeSelector.Minimum + SteppedDistanceFromBound(rangeSelector.StepFrequency, newValue - rangeSelector.Minimum);

                if (steppedNewValue > rangeSelector.RangeEnd)
                {
                    var steppedMax = rangeSelector.Minimum + SteppedDistanceFromBound(rangeSelector.StepFrequency, rangeSelector.RangeEnd - rangeSelector.Minimum);
                    rangeSelector.RangeStart = steppedMax > rangeSelector.RangeEnd ? steppedMax - rangeSelector.StepFrequency : steppedMax;
                }
                else
                {
                    rangeSelector.RangeStart = steppedNewValue;
                }
            }

            rangeSelector.SyncActiveRectangle();

            rangeSelector.SyncThumbs();
        }

        private static void RangeMaxChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var rangeSelector = d as RangeSelector;

            if (rangeSelector == null || !rangeSelector._valuesAssigned)
            {
                return;
            }

            var newValue = (double)e.NewValue;

            if (newValue > rangeSelector.Maximum)
            {
                rangeSelector.RangeEnd = rangeSelector.Maximum;
            }
            else
            {
                var steppedNewValue = rangeSelector.Maximum - SteppedDistanceFromBound(rangeSelector.StepFrequency, rangeSelector.Maximum - newValue);

                if (steppedNewValue < rangeSelector.RangeStart)
                {
                    var steppedMin = rangeSelector.Maximum - SteppedDistanceFromBound(rangeSelector.StepFrequency, rangeSelector.Maximum - rangeSelector.RangeStart);
                    rangeSelector.RangeEnd = steppedMin < rangeSelector.RangeStart ? steppedMin + rangeSelector.StepFrequency : steppedMin;
                }
                else
                {
                    rangeSelector.RangeEnd = steppedNewValue;
                }
            }

            rangeSelector.SyncActiveRectangle();

            rangeSelector.SyncThumbs();
        }
    }
}