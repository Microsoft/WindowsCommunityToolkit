// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Provides attached dependency properties for the <see cref="FrameworkElement"/> type.
    /// </summary>
    public static partial class FrameworkElementExtensions
    {
        /// <summary>
        /// Attached <see cref="DependencyProperty"/> for enabling actual size binding on any  <see cref="Windows.UI.Xaml.FrameworkElement"/>.
        /// </summary>
        public static readonly DependencyProperty EnableActualSizeBindingProperty = DependencyProperty.RegisterAttached("EnableActualSizeBinding", typeof(bool), typeof(FrameworkElementExtensions), new PropertyMetadata(false, OnEnableActualSizeBindingPropertyChanged));

        /// <summary>
        /// Attached <see cref="DependencyProperty"/> for binding a <see cref="double"/> for the <see cref="FrameworkElement.ActualHeight"/>
        /// </summary>
        public static readonly DependencyProperty ActualHeightProperty = DependencyProperty.RegisterAttached("ActualHeight", typeof(double), typeof(FrameworkElementExtensions), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Attached <see cref="DependencyProperty"/> for binding a <see cref="double"/> for the <see cref="FrameworkElement.ActualWidth"/>
        /// </summary>
        public static readonly DependencyProperty ActualWidthProperty = DependencyProperty.RegisterAttached("ActualWidth", typeof(double), typeof(FrameworkElementExtensions), new PropertyMetadata(double.NaN));

        /// <summary>
        /// Gets the <see cref="bool"/> that enables/disables actual size binding update.
        /// </summary>
        /// <param name="obj">The <see cref="FrameworkElement"/> to get the associated <see cref="bool"/> from</param>
        /// <returns>The <see cref="bool"/> associated with the <see cref="FrameworkElement"/></returns>
        public static bool GetEnableActualSizeBinding(FrameworkElement obj)
        {
            return (bool)obj.GetValue(EnableActualSizeBindingProperty);
        }

        /// <summary>
        /// Sets the <see cref="bool"/> that enables/disables actual size binding update.
        /// </summary>
        /// <param name="obj">The <see cref="FrameworkElement"/> to associate the <see cref="bool"/> with</param>
        /// <param name="value">The <see cref="bool"/> for binding to the <see cref="FrameworkElement"/></param>
        public static void SetEnableActualSizeBinding(FrameworkElement obj, bool value)
        {
            obj.SetValue(EnableActualSizeBindingProperty, value);
        }

        /// <summary>
        /// Gets the <see cref="double"/> for the <see cref="FrameworkElement.ActualHeight"/>
        /// </summary>
        /// <param name="obj">The <see cref="FrameworkElement"/> to get the associated <see cref="double"/> from</param>
        /// <returns>The <see cref="double"/> associated with the <see cref="FrameworkElement"/></returns>
        public static double GetActualHeight(FrameworkElement obj)
        {
            return (double)obj.GetValue(ActualHeightProperty);
        }

        /// <summary>
        /// Sets the <see cref="double"/> for the <see cref="FrameworkElement.ActualHeight"/>
        /// </summary>
        /// <param name="obj">The <see cref="FrameworkElement"/> to associate the <see cref="double"/> with</param>
        /// <param name="value">The <see cref="double"/> for binding to the <see cref="FrameworkElement"/></param>
        public static void SetActualHeight(FrameworkElement obj, double value)
        {
            obj.SetValue(ActualHeightProperty, value);
        }

        /// <summary>
        /// Gets the <see cref="double"/> for the <see cref="FrameworkElement.ActualWidth"/>
        /// </summary>
        /// <param name="obj">The <see cref="FrameworkElement"/> to get the associated <see cref="double"/> from</param>
        /// <returns>The <see cref="double"/> associated with the <see cref="FrameworkElement"/></returns>
        public static double GetActualWidth(FrameworkElement obj)
        {
            return (double)obj.GetValue(ActualWidthProperty);
        }

        /// <summary>
        /// Sets the <see cref="double"/> for the <see cref="FrameworkElement.ActualWidth"/>
        /// </summary>
        /// <param name="obj">The <see cref="FrameworkElement"/> to associate the <see cref="double"/> with</param>
        /// <param name="value">The <see cref="double"/> for binding to the <see cref="FrameworkElement"/></param>
        public static void SetActualWidth(FrameworkElement obj, double value)
        {
            obj.SetValue(ActualWidthProperty, value);
        }

        private static void OnEnableActualSizeBindingPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var baseElement = sender as FrameworkElement;
            if (baseElement == null)
            {
                return;
            }

            if ((bool)args.NewValue)
            {
                // Size may have changed while this was disabled, so we force an updated once user enables it
                UpdateActualSizeProperties(baseElement, null);

                // Subscribe to event
                baseElement.SizeChanged += UpdateActualSizeProperties;
            }
            else
            {
                // Unsubscribe from event
                baseElement.SizeChanged -= UpdateActualSizeProperties;
            }
        }

        private static void UpdateActualSizeProperties(object sender, RoutedEventArgs routedEventArgs)
        {
            var baseElement = sender as FrameworkElement;
            if (baseElement == null)
            {
                return;
            }

            // Update only if needed
            var currentHeight = GetActualHeight(baseElement);
            if (currentHeight != baseElement.ActualHeight)
            {
                SetActualHeight(baseElement, baseElement.ActualHeight);
            }

            // Update only if needed
            var currentWidth = GetActualWidth(baseElement);
            if (currentWidth != baseElement.ActualWidth)
            {
                SetActualWidth(baseElement, baseElement.ActualWidth);
            }
        }
    }
}
