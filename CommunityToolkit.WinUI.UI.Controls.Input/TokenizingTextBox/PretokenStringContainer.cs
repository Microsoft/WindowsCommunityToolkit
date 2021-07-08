// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// <see cref="TokenizingTextBox"/> support class
    /// </summary>
    internal partial class PretokenStringContainer : DependencyObject, ITokenStringContainer
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(PretokenStringContainer), new PropertyMetadata(string.Empty));

        public bool IsLast { get; private set; }

        public PretokenStringContainer(bool isLast = false)
        {
            IsLast = isLast;
        }

        public PretokenStringContainer(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Override and provide the content of the container on ToString() so the calling app can access the token string
        /// </summary>
        /// <returns>The content of the string token</returns>
        public override string ToString()
        {
            return Text;
        }
    }
}