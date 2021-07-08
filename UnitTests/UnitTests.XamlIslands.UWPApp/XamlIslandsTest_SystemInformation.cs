// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.ApplicationModel.Activation;

namespace UnitTests.XamlIslands.UWPApp
{
    [STATestClass]
    public partial class XamlIslandsTest_SystemInformation
    {
        [TestMethod]
        public async Task SystemInformationTrackAppUse()
        {
            await App.Dispatcher.EnqueueAsync(() =>
            {
                var e = new FakeArgs
                {
                    PreviousExecutionState = ApplicationExecutionState.NotRunning
                };
                var xamlRoot = App.XamlRoot;
                SystemInformation.Instance.TrackAppUse(e, xamlRoot);
            });
        }

        private class FakeArgs : IActivatedEventArgs
        {
            public ActivationKind Kind { get; set; }

            public ApplicationExecutionState PreviousExecutionState { get; set; }

            public SplashScreen SplashScreen { get; set; }
        }
    }
}