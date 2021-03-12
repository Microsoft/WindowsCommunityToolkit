// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarButtons;

namespace Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarFormats.MarkDown
{
    /// <summary>
    /// Default button Actions for MarkDown Formatter
    /// </summary>
    public class MarkDownButtonActions : ButtonActions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarkDownButtonActions"/> class.
        /// </summary>
        /// <param name="formatter">The Formatter to use</param>
        public MarkDownButtonActions(MarkDownFormatter formatter)
        {
            Formatter = formatter;
        }

        /// <inheritdoc/>
        public override void FormatBold(ToolbarButton button)
        {
            Formatter.SetSelection("**", "**");
        }

        /// <inheritdoc/>
        public override void FormatItalics(ToolbarButton button)
        {
            Formatter.SetSelection("_", "_");
        }

        /// <inheritdoc/>
        public override void FormatStrikethrough(ToolbarButton button)
        {
            Formatter.SetSelection("~~", "~~");
        }

        /// <inheritdoc/>
        public override void FormatLink(ToolbarButton button, string label, string formattedText, string link)
        {
            var select = Formatter.Selected;
            int originalStart = Formatter.Selected.StartPosition;
            string urlLabel = "WCT_TextToolbar_UrlLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources");
            string labelLabel = "WCT_TextToolbar_LabelLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources");

            // Replaces Selection of first Line only.
            if (select.Text.Contains("\r"))
            {
                select.EndPosition = select.Text.IndexOf("\r");
            }

            if (string.IsNullOrWhiteSpace(label))
            {
                if (!string.IsNullOrWhiteSpace(link))
                {
                    Formatter.SetSelection($"[{labelLabel}](", ")", false, link);
                    select.StartPosition = select.EndPosition;
                    select.EndPosition = select.StartPosition;
                }
                else
                {
                    string startChars = $"[{labelLabel}](";
                    string filler = urlLabel;
                    Formatter.SetSelection(startChars, ")", false, filler);
                    select.StartPosition = originalStart + startChars.Length;
                    select.EndPosition = select.StartPosition + filler.Length;
                }
            }
            else if (string.IsNullOrWhiteSpace(link))
            {
                Formatter.SetSelection($"[{label}](", ")", false, urlLabel);
            }
            else
            {
                select.Text = $"[{label}]({link})";
                select.StartPosition = select.EndPosition;
            }
        }

        /// <inheritdoc/>
        public override void FormatList(ToolbarButton button)
        {
            Formatter.SetList(() => "- ", button);
        }

        /// <inheritdoc/>
        public override void FormatOrderedList(ToolbarButton button)
        {
            Formatter.SetList(Formatter.OrderedListIterate, button);
        }

        /// <summary>
        /// Gets the Formatter used
        /// </summary>
        public MarkDownFormatter Formatter { get; }
    }
}
