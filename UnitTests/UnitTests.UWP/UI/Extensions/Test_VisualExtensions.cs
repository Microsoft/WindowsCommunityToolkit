// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI;
using System.Numerics;
using Windows.UI.Composition;
using Microsoft.Toolkit.Uwp.UI.Animations;

namespace UnitTests.UWP.UI
{
    [TestClass]
    [TestCategory("Test_VisualExtensions")]
    public class Test_VisualExtensions : VisualUITestBase
    {
        [TestMethod]
        public async Task GetDefaultTranslation()
        {
            await App.DispatcherQueue.EnqueueAsync(() =>
            {
                var button = new Button();

                string text = VisualExtensions.GetTranslation(button);

                Assert.AreEqual(text, "<0, 0, 0>");
            });
        }

        [TestMethod]
        public async Task SetAndGetTranslation()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var button = new Button();
                var grid = new Grid() { Children = { button } };

                VisualExtensions.SetTranslation(button, "80, 20, 0");

                await SetTestContentAsync(grid);

                var success = button.GetVisual().Properties.TryGetVector3("Translation", out Vector3 translation);

                Assert.AreEqual(success, CompositionGetValueStatus.Succeeded);
                Assert.AreEqual(translation, new Vector3(80, 20, 0));

                string text = VisualExtensions.GetTranslation(button);

                Assert.AreEqual(text, new Vector3(80, 20, 0).ToString());
            });
        }

        [TestMethod]
        public async Task SetAndAnimateTranslation()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var button = new Button();
                var grid = new Grid() { Children = { button } };

                VisualExtensions.SetTranslation(button, "80, 20, 0");

                await SetTestContentAsync(grid);

                await AnimationBuilder.Create()
                    .Translation(to: new Vector3(11, 22, 0))
                    .StartAsync(button);

                var success = button.GetVisual().Properties.TryGetVector3("Translation", out Vector3 translation);

                Assert.AreEqual(success, CompositionGetValueStatus.Succeeded);
                Assert.AreEqual(translation, new Vector3(11, 22, 0));

                string text = VisualExtensions.GetTranslation(button);

                Assert.AreEqual(text, new Vector3(11, 22, 0).ToString());
            });
        }
    }
}