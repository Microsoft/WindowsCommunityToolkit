// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.Mvvm.Input;

namespace SmokeTest
{
    public sealed partial class MainPage
    {
        public RelayCommand TestCommand { get; }

        public MainPage()
        {
            TestCommand = new RelayCommand(ExecuteRelayCommand);

            InitializeComponent();
        }

        private void ExecuteRelayCommand()
        {
            textBlock.Text = "Clicked";
        }
    }
}