// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// An page that shows how to use the Blade Control
    /// </summary>
    public sealed partial class BladePage : IXamlRenderListener
    {
        private BladeView bladeView;
        private Button addBlade;
        private ResourceDictionary resources;

        public BladePage()
        {
            InitializeComponent();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
            bladeView = control.FindChild("BladeView") as BladeView;
            addBlade = control.FindChild("AddBlade") as Button;

            if (addBlade != null)
            {
                addBlade.Click += OnAddBladeButtonClicked;
            }

            resources = control.Resources;
        }

        private void OnAddBladeButtonClicked(object sender, RoutedEventArgs e)
        {
            if (resources?.ContainsKey("BladeStyle") == true)
            {
                BladeItem bladeItem = new BladeItem()
                {
                    Style = resources["BladeStyle"] as Style
                };

                bladeView?.Items?.Add(bladeItem);
            }
        }
    }
}