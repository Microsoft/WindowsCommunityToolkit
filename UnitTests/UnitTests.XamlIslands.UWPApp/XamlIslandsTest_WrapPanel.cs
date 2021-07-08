// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.XamlIslands.UWPApp
{
    [STATestClass]
    public partial class XamlIslandsTest_WrapPanel
    {
        private ListView _listView;

        [TestInitialize]
        public async Task Init()
        {
            await App.Dispatcher.EnqueueAsync(() =>
            {
                var xamlItemsPanelTemplate = @"<ItemsPanelTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                                                 xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                                                 xmlns:controls='using:CommunityToolkit.WinUI.UI.Controls'>
                      <controls:WrapPanel Padding='0,0,0,0'
                                          VerticalSpacing='5'
                                          HorizontalSpacing='5' />
                    </ItemsPanelTemplate>";

                var xamlDataTemplate = @"<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                                                 xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                        <Image Width='100' Height='100' Margin='0' HorizontalAlignment='Center' Stretch='UniformToFill'>
                            <Image.Source>
                            <BitmapImage DecodePixelHeight='200' UriSource='{Binding}' />
                            </Image.Source>
                        </Image>
                    </DataTemplate>";
                _listView = new ListView
                {
                    ItemsPanel = XamlReader.Load(xamlItemsPanelTemplate) as ItemsPanelTemplate,
                    ItemTemplate = XamlReader.Load(xamlDataTemplate) as DataTemplate
                };
                TestsPage.Instance.SetMainTestContent(_listView);
            });
        }

        [TestMethod]
        public async Task WrapPanel_RendersFine()
        {
            await App.Dispatcher.EnqueueAsync(async () =>
            {
                var item = new Uri("ms-appx:///Assets/StoreLogo.png");
                for (int i = 0; i < 100; i++)
                {
                    _listView.Items.Add(item);
                }

                await Task.Delay(3000);
            });
        }
    }
}