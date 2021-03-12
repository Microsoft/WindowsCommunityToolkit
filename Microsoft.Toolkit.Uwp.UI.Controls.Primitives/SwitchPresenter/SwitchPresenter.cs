// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// The <see cref="SwitchPresenter"/> is a <see cref="ContentPresenter"/> which can allow a developer to mimic a <c>switch</c> statement within XAML.
    /// When provided a set of <see cref="Case"/>s and a <see cref="Value"/>, it will pick the matching <see cref="Case"/> with the corresponding <see cref="Case.Value"/>.
    /// </summary>
    [ContentProperty(Name = nameof(SwitchCases))]
    public sealed partial class SwitchPresenter : ContentPresenter
    {
        /// <summary>
        /// Gets the current <see cref="Case"/> which is being displayed.
        /// </summary>
        public Case CurrentCase
        {
            get { return (Case)GetValue(CurrentCaseProperty); }
            private set { SetValue(CurrentCaseProperty, value); }
        }

        /// <summary>
        /// Indicates the <see cref="CurrentCase"/> property.
        /// </summary>
        public static readonly DependencyProperty CurrentCaseProperty =
            DependencyProperty.Register(nameof(CurrentCase), typeof(Case), typeof(SwitchPresenter), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets a value representing the collection of cases to evaluate.
        /// </summary>
        public CaseCollection SwitchCases
        {
            get { return (CaseCollection)GetValue(SwitchCasesProperty); }
            set { SetValue(SwitchCasesProperty, value); }
        }

        /// <summary>
        /// Indicates the <see cref="SwitchCases"/> property.
        /// </summary>
        public static readonly DependencyProperty SwitchCasesProperty =
            DependencyProperty.Register(nameof(SwitchCases), typeof(CaseCollection), typeof(SwitchPresenter), new PropertyMetadata(null, OnSwitchCasesPropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating the value to compare all cases against. When this value is bound to and changes, the presenter will automatically evaluate cases and select the new appropriate content from the switch.
        /// </summary>
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Indicates the <see cref="Value"/> property.
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(object), typeof(SwitchPresenter), new PropertyMetadata(null, OnValuePropertyChanged));

        /// <summary>
        /// Gets or sets a value indicating which type to first cast and compare provided values against.
        /// </summary>
        public Type TargetType
        {
            get { return (Type)GetValue(TargetTypeProperty); }
            set { SetValue(TargetTypeProperty, value); }
        }

        /// <summary>
        /// Indicates the <see cref="TargetType"/> property.
        /// </summary>
        public static readonly DependencyProperty TargetTypeProperty =
            DependencyProperty.Register(nameof(TargetType), typeof(Type), typeof(SwitchPresenter), new PropertyMetadata(null));

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // When our Switch's expression changes, re-evaluate.
            if (d is SwitchPresenter xswitch)
            {
                xswitch.EvaluateCases();
            }
        }

        private static void OnSwitchCasesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // If our collection somehow changes, we should re-evaluate.
            if (d is SwitchPresenter xswitch)
            {
                xswitch.EvaluateCases();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwitchPresenter"/> class.
        /// </summary>
        public SwitchPresenter()
        {
            this.SwitchCases = new CaseCollection();

            Loaded += this.SwitchPresenter_Loaded;
        }

        private void SwitchPresenter_Loaded(object sender, RoutedEventArgs e)
        {
            // In case we're in a template, we may have loaded cases later.
            EvaluateCases();
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            EvaluateCases();
        }

        private void EvaluateCases()
        {
            if (SwitchCases == null ||
                SwitchCases.Count == 0)
            {
                // If we have no cases, then we can't match anything.
                if (CurrentCase != null)
                {
                    // Only bother clearing our actual content if we had something before.
                    Content = null;
                    CurrentCase = null;
                }

                return;
            }
            else if (CurrentCase?.Value != null &&
                CurrentCase.Value.Equals(Value))
            {
                // If the current case we're on already matches our current value,
                // then we don't have any work to do.
                return;
            }

            Case xdefault = null;
            Case newcase = null;

            foreach (Case xcase in SwitchCases)
            {
                if (xcase.IsDefault)
                {
                    // If there are multiple default cases provided, this will override and just grab the last one, the developer will have to fix this in their XAML. We call this out in the case comments.
                    xdefault = xcase;
                    continue;
                }

                if (CompareValues(Value, xcase.Value))
                {
                    newcase = xcase;
                    break;
                }
            }

            if (newcase == null && xdefault != null)
            {
                // Inject default if we found one without matching anything
                newcase = xdefault;
            }

            // Only bother changing things around if we actually have a new case.
            if (newcase != CurrentCase)
            {
                // If we don't have any cases or default, setting these to null is what we want to be blank again.
                Content = newcase?.Content;
                CurrentCase = newcase;
            }
        }

        /// <summary>
        /// Compares two values using the TargetType.
        /// </summary>
        /// <param name="compare">Our main value in our SwitchPresenter.</param>
        /// <param name="value">The value from the case to compare to.</param>
        /// <returns>true if the two values are equal</returns>
        private bool CompareValues(object compare, object value)
        {
            if (compare == null || value == null)
            {
                return compare == value;
            }

            if (TargetType == null ||
                (TargetType == compare.GetType() &&
                 TargetType == value.GetType()))
            {
                // Default direct object comparison or we're all the proper type
                return compare.Equals(value);
            }
            else if (compare.GetType() == TargetType)
            {
                // If we have a TargetType and the first value is ther right type
                // Then our 2nd value isn't, so convert to string and coerce.
                var valueBase2 = ConvertValue(TargetType, value);

                return compare.Equals(valueBase2);
            }

            // Neither of our two values matches the type so
            // we'll convert both to a String and try and coerce it to the proper type.
            var compareBase = ConvertValue(TargetType, compare);

            var valueBase = ConvertValue(TargetType, value);

            return compareBase.Equals(valueBase);
        }

        /// <summary>
        /// Helper method to convert a value from a source type to a target type.
        /// </summary>
        /// <param name="targetType">The target type</param>
        /// <param name="value">The value to convert</param>
        /// <returns>The converted value</returns>
        internal static object ConvertValue(Type targetType, object value)
        {
            if (targetType.IsInstanceOfType(value))
            {
                return value;
            }
            else if (targetType.IsEnum && value is string str)
            {
                if (Enum.TryParse(targetType, str, out object result))
                {
                    return result;
                }

                static object ThrowExceptionForKeyNotFound()
                {
                    throw new InvalidOperationException("The requested enum value was not present in the provided type.");
                }

                return ThrowExceptionForKeyNotFound();
            }
            else
            {
                return XamlBindingHelper.ConvertValue(targetType, value);
            }
        }
    }
}
