// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Triggers
{
    /// <summary>
    /// Enables a state if the value is equal to another value
    /// </summary>
    /// <remarks>
    /// <para>
    /// Example: Trigger if a value is null
    /// <code lang="xaml">
    ///     &lt;triggers:EqualsStateTrigger Value="{Binding MyObject}" EqualTo="{x:Null}" />
    /// </code>
    /// </para>
    /// </remarks>
    public class IsEqualStateTrigger : StateTriggerBase
    {
        private void UpdateTrigger() => SetActive(IsEqualStateTrigger.AreValuesEqual(Value, To, true));

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
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(IsEqualStateTrigger), new PropertyMetadata(null, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (IsEqualStateTrigger)d;
            obj.UpdateTrigger();
        }

        /// <summary>
        /// Gets or sets the value to compare equality to.
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
                    DependencyProperty.Register(nameof(To), typeof(object), typeof(IsEqualStateTrigger), new PropertyMetadata(null, OnValuePropertyChanged));

        internal static bool AreValuesEqual(object value1, object value2, bool convertType)
        {
            if (object.Equals(value1, value2))
            {
                return true;
            }

            // If they are the same type but fail with Equals check, don't bother with conversion.
            if (value1 is not null && value2 is not null && convertType
                && value1.GetType() != value2.GetType())
            {
                // Try the conversion in both ways:
                return ConvertTypeEquals(value1, value2) || ConvertTypeEquals(value2, value1);
            }

            return false;
        }

        private static bool ConvertTypeEquals(object value1, object value2)
        {
            // Let's see if we can convert:
            if (value2 is Enum)
            {
                value1 = ConvertToEnum(value2.GetType(), value1);
            }
            else
            {
                value1 = Convert.ChangeType(value1, value2.GetType(), CultureInfo.InvariantCulture);
            }

            return value2.Equals(value1);
        }

        private static object ConvertToEnum(Type enumType, object value)
        {
            // value cannot be the same type of enum now
            return value switch
            {
                string str => Enum.TryParse(enumType, str, out var e) ? e : null,
                int or uint or byte or sbyte or long or ulong or short or ushort
                    => Enum.ToObject(enumType, value),
                _ => null
            };
        }
    }
}
