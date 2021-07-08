// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace UnitTests.XamlIslands.UWPApp
{
    [STATestClass]
    public partial class XamlIslandsTest_TextToolbar
    {
        private TextToolbar _textToolbar;

        [TestInitialize]
        public async Task Init()
        {
            await App.Dispatcher.EnqueueAsync(() =>
            {
                var richEditBox = new RichEditBox
                {
                    PlaceholderText = "Enter Text Here",
                    TextWrapping = TextWrapping.Wrap,
                    VerticalContentAlignment = VerticalAlignment.Stretch,
                    MinHeight = 300,
                    BorderThickness = new Thickness(1),
                    SelectionFlyout = null
                };

                _textToolbar = new TextToolbar
                {
                    Editor = richEditBox,
                    IsEnabled = true
                };

                var grid = new Grid
                {
                    Children =
                    {
                        _textToolbar,
                        richEditBox
                    },
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Height = 200,
                    Width = 300
                };
                grid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });
                grid.RowDefinitions.Add(new RowDefinition());
                TestsPage.Instance.SetMainTestContent(grid);
                Grid.SetRow(richEditBox, 1);
            });
        }

        [TestMethod]
        public async Task TextToobar_PopupShowsInCorrectXamlRoot()
        {
            await App.Dispatcher.EnqueueAsync(async () =>
            {
                await Task.Delay(500);

                var args = new ShortcutKeyRequestArgs(Windows.System.VirtualKey.K, false, null);

                _textToolbar.GetDefaultButton(CommunityToolkit.WinUI.UI.Controls.TextToolbarButtons.ButtonType.Link).ShortcutRequested(ref args);

                await Task.Delay(10000);
            });
        }
    }
}