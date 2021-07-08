// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.UI.Controls.TextToolbarButtons;
using Microsoft.UI.Text;

namespace CommunityToolkit.WinUI.UI.Controls.TextToolbarFormats.RichText
{
    /// <summary>
    /// Class defining the RichText formatter actions
    /// </summary>
    public class RichTextButtonActions : ButtonActions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextButtonActions"/> class.
        /// </summary>
        /// <param name="formatter">The <see cref="RichTextFormatter"/></param>
        public RichTextButtonActions(RichTextFormatter formatter)
        {
            Formatter = formatter;
        }

        /// <inheritdoc/>
        public override void FormatBold(ToolbarButton button)
        {
            var format = Formatter.SelectionFormat;
            if (!button.IsToggled)
            {
                format.Bold = FormatEffect.On;
                Formatter.SelectionFormat = format;
            }
            else
            {
                format.Bold = FormatEffect.Off;
                Formatter.SelectionFormat = format;
            }

            button.IsToggled = button.IsToggled != true;
        }

        /// <inheritdoc/>
        public override void FormatItalics(ToolbarButton button)
        {
            var format = Formatter.SelectionFormat;
            if (!button.IsToggled)
            {
                format.Italic = FormatEffect.On;
                Formatter.SelectionFormat = format;
            }
            else
            {
                format.Italic = FormatEffect.Off;
                Formatter.SelectionFormat = format;
            }

            button.IsToggled = button.IsToggled != true;
        }

        /// <inheritdoc/>
        public override void FormatStrikethrough(ToolbarButton button)
        {
            var format = Formatter.SelectionFormat;
            if (!button.IsToggled)
            {
                format.Strikethrough = FormatEffect.On;
                Formatter.SelectionFormat = format;
            }
            else
            {
                format.Strikethrough = FormatEffect.Off;
                Formatter.SelectionFormat = format;
            }

            button.IsToggled = button.IsToggled != true;
        }

        /// <inheritdoc/>
        public override void FormatLink(ToolbarButton button, string label, string formattedText, string link)
        {
            var selected = Formatter.Selected;
            if (!string.IsNullOrWhiteSpace(label))
            {
                selected.SetText(TextSetOptions.FormatRtf, formattedText);
            }
            else
            {
                selected.Text = link;
            }

            // Fixes Link Replacement
            if (!string.IsNullOrWhiteSpace(selected.Link))
            {
                selected.Link = string.Empty;
            }

            selected.Link = $"\"{link}\"";

            var doc = Formatter.Model.Editor.Document;
            doc.ApplyDisplayUpdates(); // doesn't seem to work
        }

        /// <inheritdoc/>
        public override void FormatList(ToolbarButton button)
        {
            var selected = Formatter.Selected;
            if (!button.IsToggled)
            {
                Formatter.OrderedListButton.IsToggled = false;
                selected.ParagraphFormat.ListType = MarkerType.Bullet;
                selected.ParagraphFormat.ListStyle = MarkerStyle.Plain;
            }
            else
            {
                selected.ParagraphFormat.ListType = MarkerType.None;
            }

            button.IsToggled = button.IsToggled != true;
        }

        /// <inheritdoc/>
        public override void FormatOrderedList(ToolbarButton button)
        {
            var selected = Formatter.Selected;
            if (!button.IsToggled)
            {
                Formatter.ListButton.IsToggled = false;
                selected.ParagraphFormat.ListType = MarkerType.UnicodeSequence;
                selected.ParagraphFormat.ListStyle = MarkerStyle.Period;
            }
            else
            {
                selected.ParagraphFormat.ListType = MarkerType.None;
            }

            button.IsToggled = button.IsToggled != true;
        }

        /// <summary>
        /// Formats the selection as underline
        /// </summary>
        /// <param name="button">The button invoking the action</param>
        public void FormatUnderline(ToolbarButton button)
        {
            var format = Formatter.SelectionFormat;
            if (!button.IsToggled)
            {
                format.Underline = UnderlineType.Single;
                Formatter.SelectionFormat = format;
            }
            else
            {
                format.Underline = UnderlineType.None;
                Formatter.SelectionFormat = format;
            }

            button.IsToggled = button.IsToggled != true;
        }

        /// <summary>
        /// Gets the <see cref="RichTextFormatter"/>
        /// </summary>
        public RichTextFormatter Formatter { get; }
    }
}