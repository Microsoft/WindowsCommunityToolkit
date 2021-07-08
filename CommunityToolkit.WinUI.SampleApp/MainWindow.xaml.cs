// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.SampleApp
{
    public sealed partial class MainWindow
    {
        public MainWindow(string launchParameters)
        {
            InitializeComponent();

            Title = "Windows Community Toolkit Sample App";

            rootFrame.Navigate(typeof(Shell), launchParameters);
        }
    }
}
