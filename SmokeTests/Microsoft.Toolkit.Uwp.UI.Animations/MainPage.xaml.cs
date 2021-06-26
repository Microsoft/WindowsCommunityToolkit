// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml.Controls;

namespace SmokeTest
{
    public sealed partial class MainPage
    {
        private Random _random = new Random();

        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Canvas.SetTop(Element, _random.NextDouble() * this.ActualHeight);
            Canvas.SetLeft(Element, _random.NextDouble() * this.ActualWidth);
        }
    }
}