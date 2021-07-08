// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Allows raise an event when the value of a dependency property changes when a view model is otherwise not necessary.
    /// </summary>
    /// <typeparam name="TPropertyType">Type of the DependencyProperty</typeparam>
    internal partial class PropertyChangeEventSource<TPropertyType> : FrameworkElement
    {
        private readonly DependencyObject _source;

        /// <summary>
        /// Occurs when the value changes.
        /// </summary>
        public event EventHandler<TPropertyType> ValueChanged;

        /// <summary>
        /// Value Dependency Property
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",
                typeof(TPropertyType),
                typeof(PropertyChangeEventSource<TPropertyType>),
                new PropertyMetadata(default(TPropertyType), OnValueChanged));

        /// <summary>
        /// Gets or sets the Value property. This dependency property
        /// indicates the value.
        /// </summary>
        public TPropertyType Value
        {
            get { return (TPropertyType)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Value property.
        /// </summary>
        /// <param name="d">
        /// The <see cref="DependencyObject"/> on which the property has changed value.
        /// </param>
        /// <param name="e">
        /// Event data that is issued by any event that
        /// tracks changes to the effective value of this property.
        /// </param>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = (PropertyChangeEventSource<TPropertyType>)d;
            TPropertyType oldValue = (TPropertyType)e.OldValue;
            TPropertyType newValue = target.Value;
            target.OnValueChanged(oldValue, newValue);
        }

        /// <summary>
        /// Provides derived classes an opportunity to handle changes to the Value property.
        /// </summary>
        /// <param name="oldValue">The old Value value</param>
        /// <param name="newValue">The new Value value</param>
        private void OnValueChanged(TPropertyType oldValue, TPropertyType newValue)
        {
            ValueChanged?.Invoke(_source, newValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangeEventSource{TPropertyType}"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="bindingMode">The binding mode.</param>
        public PropertyChangeEventSource(
            DependencyObject source,
            string propertyName,
            BindingMode bindingMode = BindingMode.TwoWay)
        {
            _source = source;

            // Bind to the property to be able to get its changes relayed as events through the ValueChanged event.
            var binding =
                new Binding
                {
                    Source = source,
                    Path = new PropertyPath(propertyName),
                    Mode = bindingMode
                };

            SetBinding(ValueProperty, binding);
        }
    }
}