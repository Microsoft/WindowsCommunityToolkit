// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace UnitTests.XamlIslands.UWPApp
{
    [STATestClass]
    public partial class XamlIslandsTest_DropShadowPanel
    {
        private DropShadowPanel _dropShadowPanel;

        [TestInitialize]
        public async Task Init()
        {
            await App.Dispatcher.EnqueueAsync(() =>
            {
                _dropShadowPanel = new DropShadowPanel
                {
                    BlurRadius = 8,
                    ShadowOpacity = 1,
                    OffsetX = 2,
                    OffsetY = 2,
                    Color = Colors.Black,
                    VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                    HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                    IsMasked = true
                };
                TestsPage.Instance.SetMainTestContent(_dropShadowPanel);
            });
        }

        [TestMethod]
        public async Task DropShadowPanel_RendersFine()
        {
            await App.Dispatcher.EnqueueAsync(async () =>
            {
                var textBlock = new TextBlock
                {
                    TextWrapping = Windows.UI.Xaml.TextWrapping.Wrap,
                    Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. In eget sem luctus, gravida diam cursus, rutrum ipsum." +
"Pellentesque semper magna nec sapien ornare tincidunt. Sed pellentesque, turpis quis laoreet pellentesque, urna sapien efficitur nulla," +
"at interdum dolor sapien ut odio. Sed ullamcorper sapien velit, id finibus risus gravida vitae. Morbi ac ultrices lectus. Aenean felis" +
"justo, aliquet a risus ut, condimentum malesuada metus. Duis vehicula pharetra dolor vel finibus. Nunc auctor tortor nunc, in varius velit" +
"lobortis vel. Duis viverra, ante id mollis mattis, sem mauris ullamcorper dolor, sed rhoncus est erat eget ligula. Aliquam rutrum velit et" +
"felis sollicitudin, eget dapibus dui accumsan."
                };

                _dropShadowPanel.Content = textBlock;
                await Task.Delay(3000);
            });
        }
    }
}