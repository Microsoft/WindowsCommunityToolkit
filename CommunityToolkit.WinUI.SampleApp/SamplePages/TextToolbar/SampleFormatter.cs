// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Controls;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarButtons;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarButtons.Common;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarFormats;

namespace CommunityToolkit.WinUI.SampleApp.SamplePages.TextToolbarSamples
{
    public class SampleFormatter : Formatter
    {
        public override void SetModel(TextToolbar model)
        {
            base.SetModel(model);

            CommonButtons = new CommonButtons(model);
        }

        public override ButtonMap DefaultButtons
        {
            get
            {
                var bold = CommonButtons.Bold;
                bold.Activation = item => Selected.Text = "BOLD!!!";

                return new ButtonMap
                {
                    bold
                };
            }
        }

        private CommonButtons CommonButtons { get; set; }
    }
}