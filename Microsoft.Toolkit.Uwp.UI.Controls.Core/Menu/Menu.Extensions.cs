// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// Menu Control defines a menu of choices for users to invoke.
    /// </summary>
    public partial class Menu
    {
        private const string InputGestureTextName = "InputGestureText";
        private const string AllowTooltipName = "AllowTooltip";

        /// <summary>
        /// Sets the text describing an input gesture that will call the command tied to the specified item.
        /// </summary>
        public static readonly DependencyProperty InputGestureTextProperty = DependencyProperty.RegisterAttached(InputGestureTextName, typeof(string), typeof(FrameworkElement), new PropertyMetadata(null, InputGestureTextChanged));

        private static void InputGestureTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = sender as FrameworkElement;

            var inputGestureValue = element?.GetValue(InputGestureTextProperty).ToString();
            if (string.IsNullOrEmpty(inputGestureValue))
            {
                return;
            }

            inputGestureValue = inputGestureValue.ToUpper();
            if (MenuItemInputGestureCache.ContainsKey(inputGestureValue))
            {
                MenuItemInputGestureCache[inputGestureValue] = element;
                return;
            }

            MenuItemInputGestureCache.Add(inputGestureValue.ToUpper(), element);
        }

        /// <summary>
        /// Gets InputGestureText attached property
        /// </summary>
        /// <param name="obj">Target MenuFlyoutItem</param>
        /// <returns>Input gesture text</returns>
        public static string GetInputGestureText(FrameworkElement obj)
        {
            return (string)obj.GetValue(InputGestureTextProperty);
        }

        /// <summary>
        /// Sets InputGestureText attached property
        /// </summary>
        /// <param name="obj">Target MenuFlyoutItem</param>
        /// <param name="value">Input gesture text</param>
        public static void SetInputGestureText(FrameworkElement obj, string value)
        {
            obj.SetValue(InputGestureTextProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to allow tooltip on alt or not
        /// </summary>
        public static readonly DependencyProperty AllowTooltipProperty = DependencyProperty.RegisterAttached(AllowTooltipName, typeof(bool), typeof(Menu), new PropertyMetadata(false));

        /// <summary>
        /// Gets AllowTooltip attached property
        /// </summary>
        /// <param name="obj">Target Menu</param>
        /// <returns>AllowTooltip</returns>
        public static bool GetAllowTooltip(Menu obj)
        {
            return (bool)obj.GetValue(AllowTooltipProperty);
        }

        /// <summary>
        /// Sets AllowTooltip attached property
        /// </summary>
        /// <param name="obj">Target Menu</param>
        /// <param name="value">AllowTooltip</param>
        public static void SetAllowTooltip(Menu obj, bool value)
        {
            obj.SetValue(AllowTooltipProperty, value);
        }
    }
}