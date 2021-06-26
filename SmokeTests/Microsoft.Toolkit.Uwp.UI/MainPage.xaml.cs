// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml.Controls;

namespace SmokeTest
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            Loaded += this.MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var border = this.FindDescendant<Border>();
        }
    }
}