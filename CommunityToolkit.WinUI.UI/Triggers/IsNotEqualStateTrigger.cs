// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Triggers
{
    /// <summary>
    /// Enables a state if the value is not equal to another value
    /// </summary>
    public class IsNotEqualStateTrigger : StateTriggerBase
    {
        private void UpdateTrigger() => SetActive(!IsEqualStateTrigger.AreValuesEqual(Value, To, true));

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
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(IsNotEqualStateTrigger), new PropertyMetadata(null, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (IsNotEqualStateTrigger)d;
            obj.UpdateTrigger();
        }

        /// <summary>
        /// Gets or sets the value to compare inequality to.
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
                    DependencyProperty.Register(nameof(To), typeof(object), typeof(IsNotEqualStateTrigger), new PropertyMetadata(null, OnValuePropertyChanged));
    }
}