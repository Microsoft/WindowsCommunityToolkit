// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// DockPanel sample page
    /// </summary>
    public sealed partial class DockPanelPage : Page, IXamlRenderListener
    {
        private static readonly Random Rand = new Random();
        private DockPanel _sampleDockPanel;

        public DockPanelPage()
        {
            InitializeComponent();
            Load();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            _sampleDockPanel = control.FindChild("SampleDockPanel") as DockPanel;
        }

        private void Load()
        {
            SampleController.Current.RegisterNewCommand("Add Top Child", AddTopDock);
            SampleController.Current.RegisterNewCommand("Add Left Child", AddLeftDock);
            SampleController.Current.RegisterNewCommand("Add Bottom Child", AddBottomDock);
            SampleController.Current.RegisterNewCommand("Add Right Child", AddRightDock);
            SampleController.Current.RegisterNewCommand("Add Stretch Child", AddStretchDock);
            SampleController.Current.RegisterNewCommand("Clear All", ClearAllDock);
        }

        private void ClearAllDock(object sender, RoutedEventArgs e)
        {
            _sampleDockPanel.Children.Clear();
            _sampleDockPanel.LastChildFill = false;
        }

        private void AddStretchDock(object sender, RoutedEventArgs e)
        {
            AddChild(Dock.Bottom, false, false);
            _sampleDockPanel.LastChildFill = true;
        }

        private void AddBottomDock(object sender, RoutedEventArgs e)
        {
            AddChild(Dock.Bottom, false, true);
        }

        private void AddTopDock(object sender, RoutedEventArgs e)
        {
            AddChild(Dock.Top, false, true);
        }

        private void AddLeftDock(object sender, RoutedEventArgs e)
        {
            AddChild(Dock.Left, true, false);
        }

        private void AddRightDock(object sender, RoutedEventArgs e)
        {
            AddChild(Dock.Right, true, false);
        }

        private void AddChild(Dock dock, bool setWidth = false, bool setHeight = false)
        {
            if (_sampleDockPanel.LastChildFill)
            {
                return;
            }

            const int maxColor = 255;
            var childStackPanel = new StackPanel
            {
                Background = new SolidColorBrush(Color.FromArgb(
                                    (byte)Rand.Next(0, maxColor),
                                    (byte)Rand.Next(0, maxColor),
                                    (byte)Rand.Next(0, maxColor),
                                    1))
            };

            if (setHeight)
            {
                childStackPanel.Height = Rand.Next(50, 80);
            }

            if (setWidth)
            {
                childStackPanel.Width = Rand.Next(50, 80);
            }

            childStackPanel.SetValue(DockPanel.DockProperty, dock);
            childStackPanel.PointerPressed += ChildStackPanel_PointerPressed;
            _sampleDockPanel.Children.Add(childStackPanel);
        }

        private void ChildStackPanel_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            _sampleDockPanel.Children.Remove((StackPanel)sender);
        }
    }
}