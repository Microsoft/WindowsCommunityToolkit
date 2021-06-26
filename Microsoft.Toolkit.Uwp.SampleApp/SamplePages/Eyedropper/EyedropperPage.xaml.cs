// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
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