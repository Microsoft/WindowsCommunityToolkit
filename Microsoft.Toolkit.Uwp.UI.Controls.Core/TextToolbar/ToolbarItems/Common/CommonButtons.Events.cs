// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarButtons.Common
{
    /// <summary>
    /// Provides access to Generic Buttons that activate Formatter Methods
    /// </summary>
    public partial class CommonButtons
    {
        private void MakeBold(ToolbarButton button)
        {
            Model.Formatter.ButtonActions.FormatBold(button);
        }

        private void MakeItalics(ToolbarButton button)
        {
            Model.Formatter.ButtonActions.FormatItalics(button);
        }

        private void MakeStrike(ToolbarButton button)
        {
            Model.Formatter.ButtonActions.FormatStrikethrough(button);
        }

        private void MakeLink(ToolbarButton button)
        {
            Model.Formatter.ButtonActions.FormatLink(button, string.Empty, string.Empty, string.Empty);
        }

        private void MakeList(ToolbarButton button)
        {
            Model.Formatter.ButtonActions.FormatList(button);
        }

        private void MakeOList(ToolbarButton button)
        {
            Model.Formatter.ButtonActions.FormatOrderedList(button);
        }

        /// <summary>
        /// Opens a <see cref="ContentDialog"/> for the user to enter a URL
        /// </summary>
        /// <param name="button">The <see cref="ToolbarButton"/> invoked</param>
        public async void OpenLinkCreator(ToolbarButton button)
        {
            var selection = button.Model.Editor.Document.Selection;

            var labelBox = new RichEditBox
            {
                PlaceholderText = "WCT_TextToolbar_LabelLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources"),
                Margin = new Thickness(0, 0, 0, 5),
                AcceptsReturn = false
            };
            var linkBox = new TextBox
            {
                PlaceholderText = "WCT_TextToolbar_UrlLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources")
            };

            CheckBox relativeBox = null;

            var contentPanel = new StackPanel
            {
                Children =
                    {
                        labelBox,
                        linkBox
                    }
            };

            if (Model.UseURIChecker)
            {
                relativeBox = new CheckBox
                {
                    Content = "WCT_TextToolbar_RelativeLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources")
                };
                contentPanel.Children.Add(relativeBox);
            }

            labelBox.Document.SetDefaultCharacterFormat(selection.CharacterFormat);
            selection.GetText(Windows.UI.Text.TextGetOptions.FormatRtf, out string labeltext);
            labelBox.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, labeltext);

            var contentDialog = new ContentDialog
            {
                Title = "WCT_TextToolbar_CreateLinkLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources"),
                Content = contentPanel,
                PrimaryButtonText = "WCT_TextToolbar_OkLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources"),
                SecondaryButtonText = "WCT_TextToolbar_CancelLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources")
            };

            if (ControlHelpers.IsXamlRootAvailable && button.XamlRoot != null)
            {
                contentDialog.XamlRoot = button.XamlRoot;
            }

            var result = await contentDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                labelBox.Document.GetText(Windows.UI.Text.TextGetOptions.None, out string labelText);
                labelBox.Document.GetText(Windows.UI.Text.TextGetOptions.FormatRtf, out string formattedlabelText);

                string linkInvalidLabel = "WCT_TextToolbar_LinkInvalidLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources");
                string okLabel = "WCT_TextToolbar_OkLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources");
                string warningLabel = "WCT_TextToolbar_WarningLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources");
                string linkText = linkBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(linkText))
                {
                    ShowContentDialog(warningLabel, linkInvalidLabel, okLabel, button);
                    return;
                }

                if (Model.UseURIChecker && !string.IsNullOrWhiteSpace(linkText))
                {
                    var wellFormed = Uri.IsWellFormedUriString(linkText, relativeBox?.IsChecked == true ? UriKind.RelativeOrAbsolute : UriKind.Absolute);
                    if (!wellFormed)
                    {
                        ShowContentDialog(warningLabel, linkInvalidLabel, okLabel, button);
                        return;
                    }
                }

                Model.Formatter.ButtonActions.FormatLink(button, labelText.Trim(), formattedlabelText.Trim(), linkText);
            }
        }

        private async void ShowContentDialog(string title, string content, string primaryButtonText, ToolbarButton button)
        {
            var contentDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                PrimaryButtonText = primaryButtonText
            };

            if (ControlHelpers.IsXamlRootAvailable && button.XamlRoot != null)
            {
                contentDialog.XamlRoot = button.XamlRoot;
            }

            await contentDialog.ShowAsync();
        }
    }
}