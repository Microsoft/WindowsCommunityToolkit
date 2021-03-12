// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    public sealed partial class MenuPage : IXamlRenderListener
    {
        #pragma warning disable CS0618 // Type or member is obsolete
        private MenuItem fileMenu;

        public MenuPage()
        {
            InitializeComponent();
            Load();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            fileMenu = control.FindChild("FileMenu") as MenuItem;
        }
        #pragma warning restore CS0618 // Type or member is obsolete

        private void Load()
        {
            SampleController.Current.RegisterNewCommand("Append Item to file menu", (sender, args) =>
            {
                if (fileMenu != null)
                {
                    var flyoutItem = new MenuFlyoutItem
                    {
                        Text = "Click to remove"
                    };

                    flyoutItem.Click += (a, b) =>
                    {
                        fileMenu.Items.Remove(flyoutItem);
                    };

                    fileMenu.Items.Add(flyoutItem);
                }
            });

            SampleController.Current.RegisterNewCommand("Prepend Item to file menu", (sender, args) =>
            {
                if (fileMenu != null)
                {
                    var flyoutItem = new MenuFlyoutItem
                    {
                        Text = "Click to remove"
                    };

                    flyoutItem.Click += (a, b) =>
                    {
                        fileMenu.Items.Remove(flyoutItem);
                    };

                    fileMenu.Items.Insert(0, flyoutItem);
                }
            });
        }
    }
}
