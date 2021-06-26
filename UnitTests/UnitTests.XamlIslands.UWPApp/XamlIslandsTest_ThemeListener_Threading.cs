// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Xaml;

namespace UnitTests.XamlIslands.UWPApp
{
    [STATestClass]
    public partial class XamlIslandsTest_ThemeListener_Threading
    {
        private TaskCompletionSource<object> _taskCompletionSource;
        private ThemeListener _themeListener = null;

        [TestInitialize]
        public Task Init()
        {
            return App.Dispatcher.EnqueueAsync(() =>
            {
                _taskCompletionSource = new TaskCompletionSource<object>();

                _themeListener = new ThemeListener
                {
                    CurrentTheme = Application.Current.RequestedTheme
                };
                _themeListener.ThemeChanged += (s) =>
                {
                    _taskCompletionSource.TrySetResult(null);
                };

                _themeListener.CurrentTheme = Application.Current.RequestedTheme == ApplicationTheme.Light ? ApplicationTheme.Dark : ApplicationTheme.Light;
            });
        }

        [TestMethod]
        public async Task ThemeListenerDispatcherTestAsync()
        {
            await _themeListener.OnColorValuesChanged();

            await _taskCompletionSource.Task;
        }

        [TestMethod]
        public async Task ThemeListenerDispatcherTestFromOtherThreadAsync()
        {
            await Task.Run(async () =>
            {
                await _themeListener.OnColorValuesChanged();
            });
            await _taskCompletionSource.Task;
        }
    }
}