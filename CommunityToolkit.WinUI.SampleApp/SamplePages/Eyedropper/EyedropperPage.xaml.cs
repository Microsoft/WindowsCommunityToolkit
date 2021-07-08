// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    /// <summary>
    /// A page that shows how to use the Eyedropper control.
    /// </summary>
    public sealed partial class EyedropperPage : Page, IXamlRenderListener
    {
        public EyedropperPage()
        {
            this.InitializeComponent();
            Load();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
        }

        private void Load()
        {
            SampleController.Current.RegisterNewCommand("Global Eyedropper", async (sender, args) =>
            {
                var eyedropper = new Eyedropper();
                var color = await eyedropper.Open();
                InAppNotification.Show($"You get {color}.", 3000);
            });
        }
    }
}