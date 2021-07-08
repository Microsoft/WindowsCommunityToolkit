// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;

namespace CommunityToolkit.WinUI.UI.Triggers
{
    /// <summary>
    /// Trigger for switching UI based on whether the user favors their left or right hand.
    /// </summary>
    public class UserHandPreferenceStateTrigger : StateTriggerBase
    {
        private static HandPreference handPreference;

        static UserHandPreferenceStateTrigger()
        {
            handPreference = new Windows.UI.ViewManagement.UISettings().HandPreference;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserHandPreferenceStateTrigger"/> class.
        /// </summary>
        public UserHandPreferenceStateTrigger()
        {
            SetActive(handPreference == HandPreference.RightHanded);
        }

        /// <summary>
        /// Gets or sets the hand preference to trigger on.
        /// </summary>
        /// <value>A value from the <see cref="Windows.UI.ViewManagement.HandPreference"/> enum.</value>
        public HandPreference HandPreference
        {
            get { return (HandPreference)GetValue(HandPreferenceProperty); }
            set { SetValue(HandPreferenceProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="HandPreference"/> DependencyProperty
        /// </summary>
        public static readonly DependencyProperty HandPreferenceProperty =
            DependencyProperty.Register(nameof(HandPreference), typeof(HandPreference), typeof(UserHandPreferenceStateTrigger), new PropertyMetadata(HandPreference.RightHanded, OnHandPreferencePropertyChanged));

        private static void OnHandPreferencePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = (UserHandPreferenceStateTrigger)d;
            var val = (HandPreference)e.NewValue;
            obj.SetActive(handPreference == val);
        }
    }
}