// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarButtons;
using Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarButtons.Common;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Controls.TextToolbarFormats.MarkDown
{
    /// <summary>
    /// Formatter implementation for MarkDown
    /// </summary>
    public class MarkDownFormatter : Formatter
    {
        internal const string QuoteElement = "Quote";
        internal const string HeadersElement = "Headers";
        internal const string CodeElement = "Code";

        /// <inheritdoc/>
        public override void SetModel(TextToolbar model)
        {
            base.SetModel(model);

            CommonButtons = new CommonButtons(model);
            ButtonActions = new MarkDownButtonActions(this);
        }

        /// <summary>
        /// Invoked the flyout to style a header
        /// </summary>
        /// <param name="button">The button pressed</param>
        public void StyleHeader(ToolbarButton button)
        {
            var list = new ListBox { Margin = new Thickness(0), Padding = new Thickness(0) };
            headerFlyout = new Flyout { Content = list };

            if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Xaml.UIElement", "XamlRoot") && button.XamlRoot != null)
            {
                headerFlyout.XamlRoot = button.XamlRoot;
            }

            string headerVal = "#";
            for (int i = 1; i <= 5; i++)
            {
                string val = string.Concat(Enumerable.Repeat(headerVal, i));
                var item = new ListBoxItem
                {
                    Content = new MarkdownTextBlock
                    {
                        Text = val + "WCT_TextToolbar_HeaderLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources"),
                        IsTextSelectionEnabled = false
                    },
                    Tag = val,
                    Padding = new Thickness(5, 2, 5, 2),
                    Margin = new Thickness(0)
                };
                item.Tapped += HeaderSelected;
                list.Items.Add(item);
            }

            headerFlyout.ShowAt(button);
        }

        private void HeaderSelected(object sender, TappedRoutedEventArgs e)
        {
            var item = sender as FrameworkElement;

            EnsureAtStartOfCurrentLine();
            string linesStart = (item.Tag as string) + " ";

            if (Selected.Text.StartsWith(linesStart))
            {
                // Revert Header.
                Selected.Text = Selected.Text.Substring(linesStart.Length);
            }
            else
            {
                // Clear Header before replacing.
                Selected.Text = Selected.Text.TrimStart('#', ' ');
                SetSelection(linesStart, string.Empty, false);
            }

            Selected.StartPosition = Selected.EndPosition;
            headerFlyout?.Hide();
        }

        /// <summary>
        /// Formats a string as code
        /// </summary>
        /// <param name="button">Button invoking the action</param>
        public void FormatCode(ToolbarButton button)
        {
            if (DetermineSimpleReverse("`", "`"))
            {
                return;
            }
            else if (!Selected.Text.Contains(Return))
            {
                SetSelection("`", "`");
            }
            else
            {
                Func<string> codeLines = () =>
                {
                    return ListLineIterator == 1 || ReachedEndLine ? "```" : string.Empty;
                };

                SetList(codeLines, button, wrapNewLines: true);
            }
        }

        /// <summary>
        /// Formats a string as quote
        /// </summary>
        /// <param name="button">Button invoking the actions</param>
        public void FormatQuote(ToolbarButton button)
        {
            SetList(() => "> ", button);
        }

        /// <summary>
        /// Applies formatting to Selected Text, or Removes formatting if already applied.
        /// </summary>
        /// <param name="start">Formatting in front of Text</param>
        /// <param name="end">Formatting at end of Text</param>
        /// <param name="reversible">Is the Text reversible?</param>
        /// <param name="contents">Text to insert between Start and End (Overwrites Current Text)</param>
        public virtual void SetSelection(string start, string end, bool reversible = true, string contents = null)
        {
            if (Model.Editor == null)
            {
                return;
            }

            if (!reversible || !DetermineSimpleReverse(start, end))
            {
                int originalStartPos = Selected.StartPosition;

                string originalText = contents ?? Selected.Text;

                if (!string.IsNullOrWhiteSpace(originalText) && originalText.Last() == Return.First())
                {
                    originalText = originalText.Remove(originalText.Length - 1, 1);
                }

                string originalNoWhiteSpace = originalText.Trim();

                if (!string.IsNullOrWhiteSpace(originalText))
                {
                    string beginningNoWhiteSpace = originalText.TrimStart();
                    string beginningWhiteSpace = originalText.Replace(beginningNoWhiteSpace, string.Empty);

                    string endNoWhiteSpace = originalText.TrimEnd();
                    string endWhiteSpace = originalText.Replace(endNoWhiteSpace, string.Empty);

                    Selected.Text = beginningWhiteSpace + start + originalNoWhiteSpace + end + endWhiteSpace;
                }
                else
                {
                    Selected.Text = start + end;
                }

                if (string.IsNullOrWhiteSpace(originalText))
                {
                    Selected.StartPosition = originalStartPos + start.Length;
                    Selected.EndPosition = Selected.StartPosition;
                }
                else
                {
                    Selected.StartPosition = originalStartPos + start.Length;
                    Selected.EndPosition = Selected.StartPosition + originalNoWhiteSpace.Length;
                }
            }
        }

        /// <summary>
        /// Determines if formatting is to be reversed
        /// </summary>
        /// <param name="start">Formatting in front of Text</param>
        /// <param name="end">Formatting at end of Text</param>
        /// <returns>True if formatting is reversing, otherwise false</returns>
        protected virtual bool DetermineSimpleReverse(string start, string end)
        {
            if (!DetermineSimpleInlineReverse(start, end))
            {
                try
                {
                    int startpos = Selected.StartPosition - start.Length;
                    int endpos = Selected.EndPosition;

                    string text = string.Empty;
                    Model.Editor.Document.GetText(TextGetOptions.NoHidden, out text);
                    if (text.Substring(startpos, start.Length) == start)
                    {
                        string endofstring = text.Substring(endpos, end.Length);
                        if (endofstring == end)
                        {
                            text = text.Remove(startpos, start.Length);
                            endpos -= start.Length;
                            Model.Editor.Document.SetText(TextSetOptions.None, text.Remove(endpos, end.Length));

                            Selected.StartPosition = startpos;
                            Selected.EndPosition = endpos;
                            return true;
                        }
                    }
                }
                catch
                {
                }

                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Determines if formatting is to be reversed, when the formatting is located inside the Selectors.
        /// </summary>
        /// <param name="start">Formatting in front of Text</param>
        /// <param name="end">Formatting at end of Text</param>
        /// <returns>True if formatting is reversing, otherwise false</returns>
        protected virtual bool DetermineSimpleInlineReverse(string start, string end)
        {
            try
            {
                if (Selected.Text.Substring(0, start.Length) == start)
                {
                    if (Selected.Text.Substring(Selected.Text.Length - end.Length, end.Length) == end)
                    {
                        Selected.Text = Selected.Text.Substring(start.Length, Selected.Text.Length - end.Length - start.Length);
                        return true;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        ///  This function will either add List Characters to lines of text, or Remove List Characters from Lines of Text, if already applied.
        /// </summary>
        /// <param name="listChar">A function for generating a List Character, use ListLineIterator to generate a Numbered Style List, or return a string Result, e.g. () => "- "</param>
        /// <param name="button">Button that activated the Set List</param>
        /// <param name="wrapNewLines">Adds New Lines to Start and End of Selected Text</param>
        public virtual void SetList(Func<string> listChar, ToolbarButton button, bool wrapNewLines = false)
        {
            if (Model.Editor == null)
            {
                return;
            }

            ListLineIterator = 1;
            ReachedEndLine = false;
            if (!DetermineListReverse(listChar, wrapNewLines))
            {
                ListLineIterator = 1;
                ReachedEndLine = false;

                EnsureAtNewLine();
                string text = listChar();

                var lines = Selected.Text.Split(new string[] { Return }, StringSplitOptions.None).ToList();
                if (wrapNewLines)
                {
                    lines.Insert(0, string.Empty);
                }
                else if (lines.Count > 1)
                {
                    lines.RemoveAt(lines.Count - 1); // remove last escape as selected end of last line
                }

                for (int i = 0; i < lines.Count; i++)
                {
                    ListLineIterator++;

                    var element = lines[i];
                    text += element;

                    ReachedEndLine = i + 1 >= lines.Count;

                    if (lines.Count > 1 && !ReachedEndLine)
                    {
                        text += Return;
                    }

                    if (!ReachedEndLine || wrapNewLines)
                    {
                        text += listChar();

                        if (ReachedEndLine)
                        {
                            text += Return;
                        }
                    }
                }

                Selected.Text = text;

                if (!lines.Any(line => !string.IsNullOrWhiteSpace(line)))
                {
                    Selected.StartPosition = Selected.EndPosition;
                }
            }
        }

        /// <summary>
        /// Determines whether a List already has the formatting applied.
        /// </summary>
        /// <param name="listChar">Function to generate the List Character</param>
        /// <param name="wrapNewLines">Adds New Lines to Start and End of Selected Text</param>
        /// <returns>True if List formatting is reversing, otherwise false</returns>
        protected virtual bool DetermineListReverse(Func<string> listChar, bool wrapNewLines)
        {
            if (wrapNewLines && DetermineInlineWrapListReverse(listChar))
            {
                return true;
            }

            string text = string.Empty;
            int startpos = Selected.StartPosition;

            // Test if line contains List Char, if so, reverse. For Single Line Operations Only.
            if (!Selected.Text.Contains("\r"))
            {
                var testChar = listChar();
                Selected.StartPosition -= testChar.Length;
                if (Selected.Text.StartsWith(testChar))
                {
                    Selected.Text = Selected.Text.Substring(testChar.Length);
                    return true;
                }
                else
                {
                    Selected.StartPosition = startpos;
                }
            }

            var lines = Selected.Text.Split(new string[] { Return }, StringSplitOptions.None).ToList();
            if (wrapNewLines)
            {
                lines.RemoveAt(lines.Count - 1); // removes the line kept from Wrapping with NewLines.
            }

            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                ReachedEndLine = i + 1 >= lines.Count;
                string listchar = listChar();

                try
                {
                    if (line.Substring(0, listchar.Length) == listchar)
                    {
                        text += line.Remove(0, listchar.Length);
                        if (lines.Count > 1)
                        {
                            text += Return;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

                ListLineIterator++;
            }

            if (wrapNewLines)
            {
                text = text.Remove(0, 1);
                text = text.Remove(text.Length - 1);
            }

            Selected.Text = text;
            return true;
        }

        /// <summary>
        /// Determines if a reverse is requested, if the list characters are inside the Selection.
        /// </summary>
        /// <param name="listChar">List character generating function</param>
        /// <returns>Is it reversing?</returns>
        protected virtual bool DetermineInlineWrapListReverse(Func<string> listChar)
        {
            try
            {
                ListLineIterator = 1;
                string start = listChar();
                int startpos = Selected.StartPosition - start.Length - 1; // removing newline char as well

                ReachedEndLine = true;
                string end = listChar();

                string text = string.Empty;
                Model.Editor.Document.GetText(TextGetOptions.None, out text);

                string startText = text.Substring(startpos, start.Length);
                if (startText == start)
                {
                    string endText = text.Substring(Selected.EndPosition + end.Length - 3, end.Length);
                    if (endText == end)
                    {
                        return true; // works if Line Chars are only on the first and last lines, this would need to check all the other line chars for other NewLine Wrap methods that change the line char for all lines.
                    }
                }
            }
            catch
            {
            }

            ListLineIterator = 1;
            ReachedEndLine = false;

            return false;
        }

        internal string OrderedListIterate()
        {
            return ListLineIterator + ". ";
        }

        private CommonButtons CommonButtons { get; set; }

        /// <inheritdoc/>
        public override string Text
        {
            get
            {
                Model.Editor.Document.GetText(TextGetOptions.UseCrlf, out var currentvalue);
                return currentvalue.Replace('\n', '\r'); // Converts CRLF into double Return for Markdown new line.
            }
        }

        /// <inheritdoc/>
        public override ButtonMap DefaultButtons
        {
            get
            {
                ListButton = ListButton ?? CommonButtons.List;
                OrderedListButton = OrderedListButton ?? CommonButtons.OrderedList;
                QuoteButton = new ToolbarButton
                {
                    Name = QuoteElement,
                    ToolTip = "WCT_TextToolbar_QuoteLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources"),
                    Icon = new SymbolIcon { Symbol = Symbol.Message },
                    Activation = FormatQuote
                };

                return new ButtonMap
                {
                    CommonButtons.Bold,
                    CommonButtons.Italics,
                    CommonButtons.Strikethrough,

                    new ToolbarSeparator(),

                    new ToolbarButton
                    {
                        Name = HeadersElement,
                        Icon = new SymbolIcon { Symbol = Symbol.FontSize },
                        ToolTip = "WCT_TextToolbar_HeaderLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources"),
                        Activation = StyleHeader
                    },
                    new ToolbarButton
                    {
                        Name = CodeElement,
                        ToolTip = "WCT_TextToolbar_CodeLabel".GetLocalized("Microsoft.Toolkit.Uwp.UI.Controls.Core/Resources"),
                        Icon = new FontIcon { Glyph = "{}", FontFamily = new FontFamily("Segoe UI"), Margin = new Thickness(0, -5, 0, 0) },
                        Activation = FormatCode
                    },
                    QuoteButton,

                    CommonButtons.Link,

                    new ToolbarSeparator(),

                    ListButton,
                    OrderedListButton
                };
            }
        }

        /// <summary>
        /// Gets the value of the Line Number Iterator. Use this for generating Numbered Lists.
        /// </summary>
        public int ListLineIterator { get; internal set; } = 1;

        /// <summary>
        /// Gets a value indicating whether gets whether it is the last line of the list.
        /// </summary>
        public bool ReachedEndLine { get; private set; } = false;

        internal ToolbarButton QuoteButton { get; set; }

        internal ToolbarButton ListButton { get; set; }

        internal ToolbarButton OrderedListButton { get; set; }

        /// <inheritdoc/>
        public override string NewLineChars => "\r\r";

        private Flyout headerFlyout;
    }
}