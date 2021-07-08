// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Triggers
{
    /// <summary>
    /// Enables a state if the regex expression is true for a given string value
    /// </summary>
    /// <remarks>
    /// <para>
    /// Example: Trigger user entered a valid email
    /// <code lang="xaml">
    ///     &lt;triggers:RegexStateTrigger Value="{x:Bind myTextBox.Text}" Expression="^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$" Options="IgnoreCase" />
    /// </code>
    /// </para>
    /// </remarks>
    public class RegexStateTrigger : StateTriggerBase
    {
        private void UpdateTrigger()
        {
            SetActive(Value != null && !string.IsNullOrEmpty(Expression) && Regex.IsMatch(Value, Expression, Options));
        }

        /// <summary>
        /// Gets or sets the value for regex evaluation.
        /// </summary>
        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Value"/> DependencyProperty
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(RegexStateTrigger), new PropertyMetadata(null, OnValuePropertyChanged));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (RegexStateTrigger)d;
            obj.UpdateTrigger();
        }

        /// <summary>
        /// Gets or sets the regular expression.
        /// </summary>
        public string Expression
        {
            get { return (string)GetValue(ExpressionProperty); }
            set { SetValue(ExpressionProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Expression"/> DependencyProperty
        /// </summary>
        public static readonly DependencyProperty ExpressionProperty =
                    DependencyProperty.Register(nameof(Expression), typeof(string), typeof(RegexStateTrigger), new PropertyMetadata(null, OnValuePropertyChanged));

        /// <summary>
        /// Gets or sets the regular expression options
        /// </summary>
        public RegexOptions Options
        {
            get { return (RegexOptions)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Options"/> DependencyProperty
        /// </summary>
        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register(nameof(Options), typeof(RegexOptions), typeof(RegexStateTrigger), new PropertyMetadata(RegexOptions.None, OnValuePropertyChanged));
    }
}