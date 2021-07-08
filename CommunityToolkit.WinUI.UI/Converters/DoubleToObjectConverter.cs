// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace CommunityToolkit.WinUI.UI.Converters
{
    /// <summary>
    /// This class converts a double value into an other object.
    /// Can be used to convert doubles to visibility, a couple of colors, couple of images, etc.
    /// If GreaterThan and LessThan are both set, the logic looks for a value between the two values.
    /// Otherwise the logic looks for the value to be GreaterThan or LessThan the specified value.
    /// The ConverterParameter can be used to invert the logic.
    /// </summary>
    public partial class DoubleToObjectConverter : DependencyObject, IValueConverter
    {
        /// <summary>
        /// Identifies the <see cref="TrueValue"/> property.
        /// </summary>
        public static readonly DependencyProperty TrueValueProperty =
            DependencyProperty.Register(nameof(TrueValue), typeof(object), typeof(DoubleToObjectConverter), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="FalseValue"/> property.
        /// </summary>
        public static readonly DependencyProperty FalseValueProperty =
            DependencyProperty.Register(nameof(FalseValue), typeof(object), typeof(DoubleToObjectConverter), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="NullValue"/> property.
        /// </summary>
        public static readonly DependencyProperty NullValueProperty =
            DependencyProperty.Register(nameof(NullValue), typeof(object), typeof(DoubleToObjectConverter), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="GreaterThan"/> property.
        /// </summary>
        public static readonly DependencyProperty GreaterThanProperty =
            DependencyProperty.Register(nameof(GreaterThan), typeof(double), typeof(DoubleToObjectConverter), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Identifies the <see cref="LessThan"/> property.
        /// </summary>
        public static readonly DependencyProperty LessThanProperty =
            DependencyProperty.Register(nameof(LessThan), typeof(double), typeof(DoubleToObjectConverter), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets or sets the value to be returned when the expression is true
        /// </summary>
        public object TrueValue
        {
            get { return GetValue(TrueValueProperty); }
            set { SetValue(TrueValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value to be returned when the expression is false
        /// </summary>
        public object FalseValue
        {
            get { return GetValue(FalseValueProperty); }
            set { SetValue(FalseValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value to be returned when the value passed is null
        /// </summary>
        public object NullValue
        {
            get { return GetValue(NullValueProperty); }
            set { SetValue(NullValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value to check if the value is GreaterThan this value.
        /// </summary>
        public double GreaterThan
        {
            get { return (double)GetValue(GreaterThanProperty); }
            set { SetValue(GreaterThanProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value to check if the value is LessThan this value.
        /// </summary>
        public double LessThan
        {
            get { return (double)GetValue(LessThanProperty); }
            set { SetValue(LessThanProperty, value); }
        }

        /// <summary>
        /// Convert a boolean value to an other object.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">An optional parameter to be used to invert the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return NullValue;
            }

            double vd = 0.0; // DEFAULT?
            if (value is double dbl)
            {
                vd = dbl;
            }
            else if (double.TryParse(value.ToString(), out double result))
            {
                vd = result;
            }

            var boolValue = false;

            if (!double.IsNaN(GreaterThan) && !double.IsNaN(LessThan) &&
                vd > GreaterThan && vd < LessThan)
            {
                boolValue = true;
            }
            else if (!double.IsNaN(GreaterThan) && vd > GreaterThan)
            {
                boolValue = true;
            }
            else if (!double.IsNaN(LessThan) && vd < LessThan)
            {
                boolValue = true;
            }

            // Negate if needed
            if (ConverterTools.TryParseBool(parameter))
            {
                boolValue = !boolValue;
            }

            return ConverterTools.Convert(boolValue ? TrueValue : FalseValue, targetType);
        }

        /// <summary>
        /// Convert back the value to a boolean
        /// </summary>
        /// <remarks>If the <paramref name="value"/> parameter is a reference type, <see cref="TrueValue"/> must match its reference to return true.</remarks>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The type of the target property, as a type reference (System.Type for Microsoft .NET, a TypeName helper struct for Visual C++ component extensions (C++/CX)).</param>
        /// <param name="parameter">An optional parameter to be used to invert the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The value to be passed to the source object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}