// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Toolkit.Uwp.UI.Behaviors;
using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml.Media.Imaging;

namespace SmokeTest
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            Loaded += this.MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var behaviors = Interaction.GetBehaviors(EffectElementHost);
            var viewportBehavior = behaviors.OfType<ViewportBehavior>().FirstOrDefault();
            if (viewportBehavior != null)
            {
                viewportBehavior.EnteredViewport += EffectElementHost_EnteredViewport;
                viewportBehavior.EnteringViewport += EffectElementHost_EnteringViewport;
                viewportBehavior.ExitedViewport += EffectElementHost_ExitedViewport;
                viewportBehavior.ExitingViewport += EffectElementHost_ExitingViewport;
            }
        }

        private void EffectElementHost_EnteredViewport(object sender, EventArgs e)
        {
            Debug.WriteLine("Entered viewport");
        }

        private void EffectElementHost_EnteringViewport(object sender, EventArgs e)
        {
            Debug.WriteLine("Entering viewport");

            EffectElement.Source = new BitmapImage(new Uri("ms-appx:///Assets/ToolkitLogo.png"));
        }

        private void EffectElementHost_ExitedViewport(object sender, EventArgs e)
        {
            Debug.WriteLine("Exited viewport");

            EffectElement.Source = null;
        }

        private void EffectElementHost_ExitingViewport(object sender, EventArgs e)
        {
            Debug.WriteLine("Exiting viewport");
        }
    }
}