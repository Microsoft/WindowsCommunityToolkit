// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Dispatching;
using CommunityToolkit.Win32.UI.XamlHost;
using Microsoft.UI.Xaml;

namespace UnitTests.XamlIslands.UWPApp
{
    public sealed partial class App : XamlApplication
    {
        internal static DispatcherQueue Dispatcher { get; set; }

        internal static XamlRoot XamlRoot { get; set; }

        public App()
        {
            Initialize();
            InitializeComponent();
        }
    }
}