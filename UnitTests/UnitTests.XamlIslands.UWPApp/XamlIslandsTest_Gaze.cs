// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.Input.GazeInteraction;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace UnitTests.XamlIslands.UWPApp
{
    [STATestClass]
    public partial class XamlIslandsTest_Gaze
    {
        private Button _button;
        private Grid _grid;

        [TestInitialize]
        public async Task Init()
        {
            await App.Dispatcher.EnqueueAsync(() =>
            {
                var xamlItemsPanelTemplate = @"<Grid xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' 
                                                 xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
                                                 xmlns:g='using:Microsoft.Toolkit.Uwp.Input.GazeInteraction'>
                        <Button HorizontalAlignment='Center' BorderBrush='#7FFFFFFF'
                                    g:GazeInput.ThresholdDuration='50'
                                    g:GazeInput.FixationDuration='350'
                                    g:GazeInput.DwellDuration='400'
                                    g:GazeInput.RepeatDelayDuration='400'
                                    g:GazeInput.DwellRepeatDuration='400'
                                    g:GazeInput.MaxDwellRepeatCount='0'
                                    Content='Gaze click here'
                                    Width='100'
                                    Height='100'/>
                    </Grid>";
                _grid = XamlReader.Load(xamlItemsPanelTemplate) as Grid;

                GazeInput.SetInteraction(_grid, Interaction.Enabled);
                GazeInput.SetIsCursorVisible(_grid, true);
                GazeInput.SetCursorRadius(_grid, 20);
                GazeInput.SetIsSwitchEnabled(_grid, false);

                _button = (Button)_grid.Children.First();
                _button.Click += (sender, e) =>
                {
                    _button.Content = "Clicked";
                };

                var gazeButtonControl = GazeInput.GetGazeElement(_button);

                if (gazeButtonControl == null)
                {
                    gazeButtonControl = new GazeElement();
                    GazeInput.SetGazeElement(_button, gazeButtonControl);
                }

                TestsPage.Instance.SetMainTestContent(_grid);
            });
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            await App.Dispatcher.EnqueueAsync(() =>
            {
                GazeInput.SetInteraction(_grid, Interaction.Disabled);
            });
        }

        // Ignoring since Gaze isn't working on Xaml Islands
        [TestMethod]
        [Ignore]
        public async Task Gaze_DoesNotCrashOnIslands()
        {
            await App.Dispatcher.EnqueueAsync(async () =>
            {
                await Task.Delay(10000);

                Assert.AreEqual("Clicked", _button.Content as string);
            });
        }
    }
}