// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Triggers
{
    /// <summary>
    /// Enables a state if the value is equal to, greater than, or less than another value
    /// </summary>
    /// <remarks>
    /// <para>
    /// Example: Trigger if a value is greater than 0
    /// <code lang="xaml">
    ///     &lt;triggers:CompareStateTrigger Value="{Binding MyValue}" CompareTo="0" Comparison="GreaterThan" />
    /// </code>
    /// </para>
    /// </remarks>
    public class CompareStateTrigger : StateTriggerBase
    {
        private void UpdateTrigger()
        {
            var evaluation = CompareValues();
            SetActive(evaluation == Comparison ||
                (Comparison == Comparison.LessThanOrEqual && (evaluation == Comparison.LessThan || evaluation == Comparison.Equal)) ||
                (Comparison == Comparison.GreaterThanOrEqual && (evaluation == Comparison.GreaterThan || evaluation == Comparison.Equal)));
        }

        /// <summary>
        /// Gets or sets the value for comparison.
        /// </summary>
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Value"/> DependencyProperty
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(CompareStateTrigger), new PropertyMetadata(null, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (CompareStateTrigger)d;
            obj.UpdateTrigger();
        }

        /// <summary>
        /// Gets or sets the value to compare to.
        /// </summary>
        public object To
        {
            get { return (object)GetValue(ToProperty); }
            set { SetValue(ToProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="To"/> DependencyProperty
        /// </summary>
        public static readonly DependencyProperty ToProperty =
                    DependencyProperty.Register(nameof(To), typeof(object), typeof(CompareStateTrigger), new PropertyMetadata(null, OnValuePropertyChanged));

        /// <summary>
        /// Gets or sets the comparison type
        /// </summary>
        public Comparison Comparison
        {
            get { return (Comparison)GetValue(ComparisonProperty); }
            set { SetValue(ComparisonProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Comparison"/> DependencyProperty
        /// </summary>
        public static readonly DependencyProperty ComparisonProperty =
            DependencyProperty.Register(nameof(Comparison), typeof(Comparison), typeof(CompareStateTrigger), new PropertyMetadata(Comparison.Equal, OnValuePropertyChanged));

        internal Comparison CompareValues()
        {
            var v1 = Value;
            var v2 = To;
            if ((Comparison == Comparison.Equal || Comparison == Comparison.LessThanOrEqual || Comparison == Comparison.GreaterThanOrEqual) && v1 == v2)
            {
                return Comparison.Equal;
            }

            if (v1 != null && v2 != null)
            {
                // Let's see if we can convert - for perf reasons though, try and use the right type in and out
                if (v1.GetType() != v2.GetType())
                {
                    if (v1 is Enum)
                    {
                        v2 = Enum.Parse(v1.GetType(), v2.ToString());
                    }
                    else if (v2 is Enum)
                    {
                        v1 = Enum.Parse(v2.GetType(), v1.ToString());
                    }
                    else if (v1 is IComparable)
                    {
                        v2 = Convert.ChangeType(v2, v1.GetType(), CultureInfo.InvariantCulture);
                    }
                    else if (v2 is IComparable)
                    {
                        v1 = Convert.ChangeType(v1, v2.GetType(), CultureInfo.InvariantCulture);
                    }
                }

                if (v1.GetType() == v2.GetType())
                {
                    if (v1 is IComparable)
                    {
                        var result = ((IComparable)v1).CompareTo(v2);
                        if (result < 0)
                        {
                            return Comparison.LessThan;
                        }
                        else if (result == 0)
                        {
                            return Comparison.Equal;
                        }
                        else
                        {
                            return Comparison.GreaterThan;
                        }
                    }
                }
            }

            return (Comparison)(-1);
        }
    }

    /// <summary>
    /// Comparison types
    /// </summary>
    public enum Comparison : int
    {
        /// <summary>
        /// Less than
        /// </summary>
        LessThan,

        /// <summary>
        /// Less than or equal
        /// </summary>
        LessThanOrEqual,

        /// <summary>
        /// Equals
        /// </summary>
        Equal,

        /// <summary>
        /// Greater than or equal
        /// </summary>
        GreaterThanOrEqual,

        /// <summary>
        /// Greater than
        /// </summary>
        GreaterThan
    }
}