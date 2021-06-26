// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace UnitTests.UWP.UI.Controls
{
    [TestClass]
    public class Test_ImageEx : VisualUITestBase
    {
        private const string ImageString = "data:image/png;base64,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";

        [TestMethod]
        public async Task SetSourceToOpenedBitmapImage()
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var bitmapImage = new BitmapImage();

                using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(@"UnitTests.UWP.Assets.StoreLogo.embeded.png");
                using var memStream = new MemoryStream();
                await stream.CopyToAsync(memStream);
                memStream.Seek(0, SeekOrigin.Begin);
                await bitmapImage.SetSourceAsync(memStream.AsRandomAccessStream());

                var imageLoader = new ImageEx();

                await SetTestContentAsync(imageLoader);

                Assert.AreEqual("Unloaded", GetCurrentState(imageLoader));

                var imageOpendedCallCount = 0;

                imageLoader.ImageExOpened += (s, e) =>
                {
                    imageOpendedCallCount++;
                    Assert.AreEqual("Loaded", GetCurrentState(imageLoader));
                };

                imageLoader.Source = bitmapImage;

                Assert.AreEqual(1, imageOpendedCallCount, "{0} should only be called once", nameof(ImageEx.ImageExOpened));
            });
        }

        [TestMethod]
        [DataRow(ImageString)]
        [DataRow(@"ms-appx:///Assets/StoreLogo.png")]
        public async Task SetSourceToUri(string uri)
        {
            await App.DispatcherQueue.EnqueueAsync(async () =>
            {
                var imageLoader = new ImageEx();

                await SetTestContentAsync(imageLoader);

                Assert.AreEqual("Unloaded", GetCurrentState(imageLoader));

                var imageOpendedCallCount = 0;
                imageLoader.ImageExOpened += (s, e) =>
                {
                    imageOpendedCallCount++;
                    Assert.AreEqual("Loaded", GetCurrentState(imageLoader));
                };

                imageLoader.Source = new Uri(uri);

                // TODO (2021.05.11): Test in a more deterministic way.
                // Setting source causes some async code to trigger and
                // we have no way to await or handle its complementation regardless of the result.
                await Task.Delay(1000);
                Assert.AreEqual(1, imageOpendedCallCount, "{0} should only be called once", nameof(ImageEx.ImageExOpened));
            });
        }

        private static string GetCurrentState(ImageEx image)
            => VisualStateManager.GetVisualStateGroups(image.FindDescendant<Grid>()).First(g => g.Name == "CommonStates").CurrentState.Name;
    }
}