// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UITests.App.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TextBoxMaskTestPage : Page
    {
        private const string INITIAL_VALUE = "12:50:59";
        private const string NEW_VALUE = "00:00:00";

        public string InitialValue => INITIAL_VALUE;

        public string NewValue => NEW_VALUE;

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(string), typeof(TextBoxMaskTestPage), new PropertyMetadata(INITIAL_VALUE));

        public TextBoxMaskTestPage()
        {
            this.InitializeComponent();
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            Value = NEW_VALUE;
            Log.Comment("Value Changed to {0}", Value);
        }

        private void SetEmptyButton_Click(object sender, RoutedEventArgs e)
        {
            Value = string.Empty;
            Log.Comment("Value Changed to {0}", Value);
        }
    }
}