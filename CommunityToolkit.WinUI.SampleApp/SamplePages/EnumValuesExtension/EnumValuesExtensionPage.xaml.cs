// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages
{
    /// <summary>
    /// A page that shows how to use the <see cref="EnumValuesExtension"/> type.
    /// </summary>
    public sealed partial class EnumValuesExtensionPage : IXamlRenderListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValuesExtensionPage"/> class.
        /// </summary>
        public EnumValuesExtensionPage()
        {
            InitializeComponent();
        }

        public void OnXamlRendered(FrameworkElement control)
        {
        }
    }
}