// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Microsoft.Toolkit.Uwp.UI.Converters
{
    /// <summary>
    /// This class returns an object or another, depending on whether the type of the provided value matches another provided Type.
    /// </summary>
    public partial class TypeToObjectConverter : DependencyObject, IValueConverter
    {
        /// <summary>
        /// Identifies the <see cref="TrueValue"/> property.
        /// </summary>
        public static readonly DependencyProperty TrueValueProperty =
            DependencyProperty.Register(nameof(TrueValue), typeof(object), typeof(TypeToObjectConverter), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="FalseValue"/> property.
        /// </summary>
        public static readonly DependencyProperty FalseValueProperty =
            DependencyProperty.Register(nameof(FalseValue), typeof(object), typeof(TypeToObjectConverter), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Type"/> property.
        /// </summary>
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(nameof(Type), typeof(Type), typeof(TypeToObjectConverter), new PropertyMetadata(typeof(object)));

        /// <summary>
        /// Gets or sets the value to be returned when the type of the provided value matches <see cref="Type"/>.
        /// </summary>
        public object TrueValue
        {
            get { return GetValue(TrueValueProperty); }
            set { SetValue(TrueValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the value to be returned when the type of the provided value does not match <see cref="Type"/>.
        /// </summary>
        public object FalseValue
        {
            get { return GetValue(FalseValueProperty); }
            set { SetValue(FalseValueProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Type used to compare the type of the provided value.
        /// </summary>
        public Type Type
        {
            get { return (Type)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        /// <summary>
        /// Convert the <paramref name="value"/>'s Type to an other object.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">An optional parameter to be used to invert the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var typeMatches = value != null && Type.Equals(value.GetType());

            // Negate if needed
            if (ConverterTools.TryParseBool(parameter))
            {
                typeMatches = !typeMatches;
            }

            return ConverterTools.Convert(typeMatches ? TrueValue : FalseValue, targetType);
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property, as a type reference.</param>
        /// <param name="parameter">Optional parameter. Not used.</param>
        /// <param name="language">The language of the conversion. Not used.</param>
        /// <returns>The value to be passed to the target dependency property.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}