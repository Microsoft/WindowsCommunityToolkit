// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System.Numerics;
using Microsoft.Toolkit.Uwp.UI;
using System;
using Windows.UI.Xaml.Media;

namespace UnitTests.UWP.UI.Animations
{
    [TestClass]
    [TestCategory("Test_AnimationBuilderStart")]
    public class Test_AnimationBuilderStart : VisualUITestBase
    {
        [TestMethod]
        public async Task Start_WithCallback_CompositionOnly()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var button = new Button();
                var grid = new Grid() { Children = { button } };

                await SetTestContentAsync(grid);

                var tcs = new TaskCompletionSource<object>();

                AnimationBuilder.Create()
                    .Scale(
                        to: new Vector3(1.2f, 1, 1),
                        delay: TimeSpan.FromMilliseconds(400))
                    .Opacity(
                        to: 0.7,
                        duration: TimeSpan.FromSeconds(1))
                    .Start(button, () => tcs.SetResult(null));

                await tcs.Task;

                // Note: we're just testing Scale and Opacity here as they're among the Visual properties that
                // are kept in sync on the Visual object after an animation completes, so we can use their
                // values below to check that the animations have run correctly. There is no particular reason
                // why we chose these two animations specifically other than this. For instance, checking
                // Visual.TransformMatrix.Translation or Visual.Offset after an animation targeting those
                // properties doesn't correctly report the final value and remains out of sync ¯\_(ツ)_/¯
                Assert.AreEqual(button.GetVisual().Scale, new Vector3(1.2f, 1, 1));
                Assert.AreEqual(button.GetVisual().Opacity, 0.7f);
            });
        }

        [TestMethod]
        public async Task Start_WithCallback_XamlOnly()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var button = new Button();
                var grid = new Grid() { Children = { button } };

                await SetTestContentAsync(grid);

                var tcs = new TaskCompletionSource<object>();

                AnimationBuilder.Create()
                    .Translation(
                        to: new Vector2(80, 20),
                        layer: FrameworkLayer.Xaml)
                    .Scale(
                        to: new Vector2(1.2f, 1),
                        delay: TimeSpan.FromMilliseconds(400),
                        layer: FrameworkLayer.Xaml)
                    .Opacity(
                        to: 0.7,
                        duration: TimeSpan.FromSeconds(1),
                        layer: FrameworkLayer.Xaml)
                    .Start(button, () => tcs.SetResult(null));

                await tcs.Task;

                CompositeTransform transform = button.RenderTransform as CompositeTransform;

                Assert.IsNotNull(transform);
                Assert.AreEqual(transform.TranslateX, 80);
                Assert.AreEqual(transform.TranslateY, 20);
                Assert.AreEqual(transform.ScaleX, 1.2, 0.0000001);
                Assert.AreEqual(transform.ScaleY, 1, 0.0000001);
                Assert.AreEqual(button.Opacity, 0.7, 0.0000001);
            });
        }

        [TestMethod]
        public async Task Start_WithCallback_CompositionAndXaml()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var button = new Button();
                var grid = new Grid() { Children = { button } };

                await SetTestContentAsync(grid);

                var tcs = new TaskCompletionSource<object>();

                AnimationBuilder.Create()
                    .Scale(
                        to: new Vector3(1.2f, 1, 1),
                        delay: TimeSpan.FromMilliseconds(400))
                    .Opacity(
                        to: 0.7,
                        duration: TimeSpan.FromSeconds(1))
                    .Translation(
                        to: new Vector2(80, 20),
                        layer: FrameworkLayer.Xaml)
                    .Start(button, () => tcs.SetResult(null));

                await tcs.Task;

                CompositeTransform transform = button.RenderTransform as CompositeTransform;

                Assert.AreEqual(button.GetVisual().Scale, new Vector3(1.2f, 1, 1));
                Assert.AreEqual(button.GetVisual().Opacity, 0.7f);
                Assert.IsNotNull(transform);
                Assert.AreEqual(transform.TranslateX, 80);
                Assert.AreEqual(transform.TranslateY, 20);
            });
        }
    }
}