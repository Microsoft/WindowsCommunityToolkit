// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImplicitAnimationsPage : IXamlRenderListener
    {
        private Random _random = new Random();
        private UIElement _element;

        public ImplicitAnimationsPage()
        {
            this.InitializeComponent();
            Load();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            _element = control.FindChild("Element");
        }

        private void Load()
        {
            SampleController.Current.RegisterNewCommand("Toggle Visibility", (sender, args) =>
            {
                if (_element != null)
                {
                    _element.Visibility = _element.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
                }
            });

            SampleController.Current.RegisterNewCommand("Move Element", (sender, args) =>
            {
                if (_element != null)
                {
                    Canvas.SetTop(_element, _random.NextDouble() * this.ActualHeight);
                    Canvas.SetLeft(_element, _random.NextDouble() * this.ActualWidth);
                }
            });

            SampleController.Current.RegisterNewCommand("Scale Element", (sender, args) =>
            {
                if (_element != null)
                {
                    var visual = ElementCompositionPreview.GetElementVisual(_element);
                    visual.Scale = new Vector3(
                        (float)_random.NextDouble() * 2,
                        (float)_random.NextDouble() * 2,
                        1);
                }
            });
        }
    }
}