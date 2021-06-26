// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.SampleApp.SamplePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HeaderedItemsControlPage : Page, IXamlRenderListener
    {
        public HeaderedItemsControlPage()
        {
            Items = "The quick brown fox jumped over the lazy river".Split(' ');

            this.InitializeComponent();
        }

        public IEnumerable<string> Items { get; }

        public void OnXamlRendered(FrameworkElement element)
        {
            foreach (var control in element.FindChildren().OfType<HeaderedItemsControl>())
            {
                control.DataContext = this;
            }
        }
    }
}