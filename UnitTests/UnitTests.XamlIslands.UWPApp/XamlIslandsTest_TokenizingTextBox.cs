// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace UnitTests.XamlIslands.UWPApp
{
    [STATestClass]
    public partial class XamlIslandsTest_TokenizingTextBox
    {
        private TokenizingTextBox _tokenizingTextBox;
        private AdvancedCollectionView _acv;

        public class SampleDataType
        {
            public Symbol Icon { get; set; }

            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        private readonly List<SampleDataType> _samples = new List<SampleDataType>()
        {
            new SampleDataType() { Text = "Account", Icon = Symbol.Account },
            new SampleDataType() { Text = "Add Friend", Icon = Symbol.AddFriend },
            new SampleDataType() { Text = "Attach", Icon = Symbol.Attach },
            new SampleDataType() { Text = "Attach Camera", Icon = Symbol.AttachCamera },
            new SampleDataType() { Text = "Audio", Icon = Symbol.Audio },
            new SampleDataType() { Text = "Block Contact", Icon = Symbol.BlockContact },
            new SampleDataType() { Text = "Calculator", Icon = Symbol.Calculator },
            new SampleDataType() { Text = "Calendar", Icon = Symbol.Calendar },
            new SampleDataType() { Text = "Camera", Icon = Symbol.Camera },
            new SampleDataType() { Text = "Contact", Icon = Symbol.Contact },
            new SampleDataType() { Text = "Favorite", Icon = Symbol.Favorite },
            new SampleDataType() { Text = "Link", Icon = Symbol.Link },
            new SampleDataType() { Text = "Mail", Icon = Symbol.Mail },
            new SampleDataType() { Text = "Map", Icon = Symbol.Map },
            new SampleDataType() { Text = "Phone", Icon = Symbol.Phone },
            new SampleDataType() { Text = "Pin", Icon = Symbol.Pin },
            new SampleDataType() { Text = "Rotate", Icon = Symbol.Rotate },
            new SampleDataType() { Text = "Rotate Camera", Icon = Symbol.RotateCamera },
            new SampleDataType() { Text = "Send", Icon = Symbol.Send },
            new SampleDataType() { Text = "Tags", Icon = Symbol.Tag },
            new SampleDataType() { Text = "UnFavorite", Icon = Symbol.UnFavorite },
            new SampleDataType() { Text = "UnPin", Icon = Symbol.UnPin },
            new SampleDataType() { Text = "Zoom", Icon = Symbol.Zoom },
            new SampleDataType() { Text = "ZoomIn", Icon = Symbol.ZoomIn },
            new SampleDataType() { Text = "ZoomOut", Icon = Symbol.ZoomOut },
        };

        [TestInitialize]
        public async Task Init()
        {
            await App.Dispatcher.EnqueueAsync(() =>
            {
                _acv = new AdvancedCollectionView(_samples, false);

                _acv.SortDescriptions.Add(new SortDescription(nameof(SampleDataType.Text), SortDirection.Ascending));

                var xamlTokenizingTextBox = @"<controls:TokenizingTextBox xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                                                 xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                                                 xmlns:controls='using:Microsoft.Toolkit.Uwp.UI.Controls'
                          PlaceholderText='Add Actions'
                          MaxHeight='104'
                          HorizontalAlignment='Center'
                          VerticalAlignment='Center'
                          TextMemberPath='Text'
                          TokenDelimiter = ','>
                        <controls:TokenizingTextBox.SuggestedItemTemplate>
                          <DataTemplate>
                            <StackPanel Orientation='Horizontal'>
                              <SymbolIcon Symbol='{Binding Icon}'/>
                              <TextBlock Text='{Binding Text}' Padding='4,0,0,0'/>
                            </StackPanel>
                          </DataTemplate>
                        </controls:TokenizingTextBox.SuggestedItemTemplate>
                        <controls:TokenizingTextBox.TokenItemTemplate>
                          <DataTemplate>
                            <StackPanel Orientation='Horizontal'>
                              <SymbolIcon Symbol='{Binding Icon}'/>
                              <TextBlock Text='{Binding Text}' Padding='4,0,0,0'/>
                            </StackPanel>
                          </DataTemplate>
                        </controls:TokenizingTextBox.TokenItemTemplate>
                      </controls:TokenizingTextBox>";

                _tokenizingTextBox = XamlReader.Load(xamlTokenizingTextBox) as TokenizingTextBox;
                _tokenizingTextBox.SuggestedItemsSource = _acv;

                TestsPage.Instance.SetMainTestContent(_tokenizingTextBox);

                _tokenizingTextBox.AddTokenItem(_samples[0], true);
                _tokenizingTextBox.AddTokenItem(_samples[1], true);
                _tokenizingTextBox.AddTokenItem(_samples[2], true);
            });
        }

        [TestMethod]
        public async Task TokenizingTextBox_GetFocusedElement_RemoveAllSelectedTokens()
        {
            await App.Dispatcher.EnqueueAsync(async () =>
            {
                await Task.Delay(500);

                _tokenizingTextBox.SelectedIndex = 1;

                await Task.Delay(500);

                await _tokenizingTextBox.TokenizingTextBox_PreviewKeyDown(Windows.System.VirtualKey.Left);

                await Task.Delay(500);

                Assert.AreEqual(4, _tokenizingTextBox.Items.Count);

                await _tokenizingTextBox.RemoveAllSelectedTokens();

                await Task.Delay(500);

                Assert.AreEqual(3, _tokenizingTextBox.Items.Count);
            });
        }

        [TestMethod]
        public async Task TokenizingTextBox_PopupShowsInCorrectXamlRoot()
        {
            await App.Dispatcher.EnqueueAsync(async () =>
            {
                await Task.Delay(500);

                _tokenizingTextBox.SelectedIndex = 1;

                await Task.Delay(500);

                await _tokenizingTextBox.TokenizingTextBox_PreviewKeyDown(Windows.System.VirtualKey.Left);

                var tokenizingTextBoxItem = _tokenizingTextBox.ContainerFromItem(_tokenizingTextBox.SelectedItem) as TokenizingTextBoxItem;
                tokenizingTextBoxItem.ContextFlyout.ShowAt(tokenizingTextBoxItem);

                await Task.Delay(1000);
            });
        }
    }
}