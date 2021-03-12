// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI;

namespace UnitTests.XamlIslands.UWPApp
{
    [STATestClass]
    public partial class XamlIslandsTest_Eyedropper
    {
        [TestMethod]
        public async Task Eyedropper_DoesntCrash()
        {
            Eyedropper eyedropper = null;
            Color? color = null;
            _ = App.Dispatcher.EnqueueAsync(async () =>
            {
                eyedropper = new Eyedropper
                {
                    XamlRoot = App.XamlRoot
                };
                color = await eyedropper.Open();
            });

            await App.Dispatcher.EnqueueAsync(async () =>
            {
                var xamlRoot = App.XamlRoot;

                var pos = new Point(xamlRoot.Size.Width / 2, xamlRoot.Size.Height / 2);
                uint id = 1;

                await eyedropper.InternalPointerPressedAsync(id, pos, PointerDeviceType.Mouse);

                await Task.Delay(1000);

                for (int i = 0; i < 50; i++)
                {
                    await Task.Delay(100);
                    eyedropper.InternalPointerMoved(id, pos);
                    pos.X += 5;
                    pos.Y += 5;
                }

                await eyedropper.InternalPointerReleasedAsync(id, pos);

                await Task.Delay(1000);

                Assert.AreEqual(Colors.CornflowerBlue, color);
            });
        }
    }
}