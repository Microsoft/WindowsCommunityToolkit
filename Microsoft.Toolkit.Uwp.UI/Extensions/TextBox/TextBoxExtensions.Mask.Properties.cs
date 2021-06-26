// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <inheritdoc cref="TextBoxExtensions"/>
    public static partial class TextBoxExtensions
    {
        /// <summary>
        /// Represents a mask/format for the textbox that the user must follow
        /// </summary>
        public static readonly DependencyProperty MaskProperty = DependencyProperty.RegisterAttached("Mask", typeof(string), typeof(TextBoxExtensions), new PropertyMetadata(null, InitTextBoxMask));

        /// <summary>
        /// Represents the mask place holder which represents the variable character that the user can edit in the textbox
        /// </summary>
        public static readonly DependencyProperty MaskPlaceholderProperty = DependencyProperty.RegisterAttached("MaskPlaceholder", typeof(string), typeof(TextBoxExtensions), new PropertyMetadata(DefaultPlaceHolder, InitTextBoxMask));

        /// <summary>
        /// Represents the custom mask that the user can create to add his own variable characters based on regex expression
        /// </summary>
        public static readonly DependencyProperty CustomMaskProperty = DependencyProperty.RegisterAttached("CustomMask", typeof(string), typeof(TextBoxExtensions), new PropertyMetadata(null, InitTextBoxMask));

        private static readonly DependencyProperty RepresentationDictionaryProperty = DependencyProperty.RegisterAttached("RepresentationDictionary", typeof(Dictionary<char, string>), typeof(TextBoxExtensions), new PropertyMetadata(null));
        private static readonly DependencyProperty OldTextProperty = DependencyProperty.RegisterAttached("OldText", typeof(string), typeof(TextBoxExtensions), new PropertyMetadata(null));
        private static readonly DependencyProperty DefaultDisplayTextProperty = DependencyProperty.RegisterAttached("DefaultDisplayText", typeof(string), typeof(TextBoxExtensions), new PropertyMetadata(null));
        private static readonly DependencyProperty OldSelectionLengthProperty = DependencyProperty.RegisterAttached("OldSelectionLength", typeof(int), typeof(TextBoxExtensions), new PropertyMetadata(0));
        private static readonly DependencyProperty OldSelectionStartProperty = DependencyProperty.RegisterAttached("OldSelectionStart", typeof(int), typeof(TextBoxExtensions), new PropertyMetadata(0));

        private static readonly DependencyProperty EscapedMaskProperty = DependencyProperty.RegisterAttached("EscapedMask", typeof(string), typeof(TextBoxExtensions), new PropertyMetadata(null));
        private static readonly DependencyProperty EscapedCharacterIndicesProperty = DependencyProperty.RegisterAttached("MaskEscapedCharacters", typeof(List<int>), typeof(TextBoxExtensions), new PropertyMetadata(null));

        /// <summary>
        /// Gets mask value
        /// </summary>
        /// <param name="obj">TextBox control</param>
        /// <returns>mask value</returns>
        public static string GetMask(TextBox obj)
        {
            return (string)obj.GetValue(MaskProperty);
        }

        /// <summary>
        /// Sets textbox mask property which represents mask/format for the textbox that the user must follow
        /// </summary>
        /// <param name="obj">TextBox Control</param>
        /// <param name="value">Mask Value</param>
        public static void SetMask(TextBox obj, string value)
        {
            obj.SetValue(MaskProperty, value);
        }

        /// <summary>
        /// Gets placeholder value
        /// </summary>
        /// <param name="obj">TextBox control</param>
        /// <returns>placeholder value</returns>
        public static string GetMaskPlaceholder(TextBox obj)
        {
            return (string)obj.GetValue(MaskPlaceholderProperty);
        }

        /// <summary>
        /// Sets placeholder property which represents the variable character that the user can edit in the textbox
        /// </summary>
        /// <param name="obj">TextBox Control</param>
        /// <param name="value">placeholder Value</param>
        public static void SetMaskPlaceholder(TextBox obj, string value)
        {
            obj.SetValue(MaskPlaceholderProperty, value);
        }

        /// <summary>
        /// Gets CustomMask value
        /// </summary>
        /// <param name="obj">TextBox control</param>
        /// <returns>CustomMask value</returns>
        public static string GetCustomMask(TextBox obj)
        {
            return (string)obj.GetValue(CustomMaskProperty);
        }

        /// <summary>
        /// Sets CustomMask property which represents the custom mask that the user can create to add his own variable characters based on certain regex expression
        /// </summary>
        /// <param name="obj">TextBox Control</param>
        /// <param name="value">CustomMask Value</param>
        public static void SetCustomMask(TextBox obj, string value)
        {
            obj.SetValue(CustomMaskProperty, value);
        }
    }
}