// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.UI.Controls.TextToolbarButtons;
using Microsoft.UI.Text;

namespace CommunityToolkit.WinUI.UI.Controls.TextToolbarFormats
{
    /// <summary>
    /// Manipulates Selected Text into an applied format according to default buttons.
    /// </summary>
    public abstract class Formatter
    {
        /// <summary>
        /// Called when text editor has changed
        /// </summary>
        /// <param name="sender"><see cref="TextToolbar"/> invoking the event</param>
        /// <param name="e"><see cref="EditorChangedArgs"/></param>
        protected virtual void Model_EditorChanged(object sender, EditorChangedArgs e)
        {
            if (e.Old != null)
            {
                e.Old.SelectionChanged -= Editor_SelectionChanged;
            }

            if (e.New != null)
            {
                e.New.SelectionChanged += Editor_SelectionChanged;
            }
        }

        /// <summary>
        /// Called for Changes to Selection (Requires unhook if switching RichEditBox).
        /// </summary>
        /// <param name="sender">Editor</param>
        /// <param name="e">Args</param>
        private void Editor_SelectionChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            OnSelectionChanged();
        }

        /// <summary>
        /// Decrements the selected position until it is at the start of the current line.
        /// </summary>
        public virtual void EnsureAtStartOfCurrentLine()
        {
            while (!Selected.Text.StartsWith(NewLineChars))
            {
                Selected.StartPosition -= 1;
                if (Selected.StartPosition == 0)
                {
                    break;
                }
            }

            if (Selected.StartPosition != 0)
            {
                Selected.StartPosition += NewLineChars.Length;
            }
        }

        /// <summary>
        /// Determines the Position of the Selector, if not at a New Line, it will move the Selector to a new line.
        /// </summary>
        public virtual void EnsureAtNewLine()
        {
            int val = Selected.StartPosition;
            int counter = 0;
            bool atNewLine = false;

            Model.Editor.Document.GetText(TextGetOptions.NoHidden, out var docText);
            var lines = docText.Split(new string[] { Return }, StringSplitOptions.None);

            foreach (var line in lines)
            {
                if (counter == val)
                {
                    atNewLine = true;
                }

                foreach (var c in line)
                {
                    counter++;
                    if (counter >= val)
                    {
                        break;
                    }
                }

                counter++;
            }

            if (!atNewLine)
            {
                bool selectionEmpty = string.IsNullOrWhiteSpace(Selected.Text);
                Selected.Text = Selected.Text.Insert(0, Return);
                Selected.StartPosition += 1;

                if (selectionEmpty)
                {
                    Selected.EndPosition = Selected.StartPosition;
                }
            }
        }

        /// <summary>
        /// Gets an array of the Lines of Text in the Editor.
        /// </summary>
        /// <returns>Text Array</returns>
        public virtual string[] GetLines()
        {
            Model.Editor.Document.GetText(TextGetOptions.None, out string doc);
            var lines = doc.Split(new string[] { NewLineChars }, StringSplitOptions.None);
            return lines;
        }

        /// <summary>
        /// Gets the line from the index provided (Skips last Carriage Return)
        /// </summary>
        /// <returns>Last line text</returns>
        public virtual string GetLine(int index)
        {
            return GetLines()[index];
        }

        /// <summary>
        /// Gets the last line (Skips last Carriage Return)
        /// </summary>
        /// <returns>Last line text</returns>
        public virtual string GetLastLine()
        {
            var lines = GetLines();
            return lines[lines.Length - 2];
        }

        /// <summary>
        /// Called after the Selected Text changes.
        /// </summary>
        public virtual void OnSelectionChanged()
        {
        }

        /// <summary>
        /// Gets the source Toolbar
        /// </summary>
        public TextToolbar Model { get; private set; }

        /// <summary>
        /// This method is called to unset event handlers that might have been registers by <see cref="SetModel(TextToolbar)"/>
        /// </summary>
        /// <param name="model">The old <see cref="TextToolbar"/> the Formatter was associated with</param>
        public virtual void UnsetModel(TextToolbar model)
        {
            model.EditorChanged -= Model_EditorChanged;
        }

        /// <summary>
        /// Sets the <see cref="TextToolbar"/> where the Formatter is used
        /// </summary>
        /// <param name="model">The <see cref="TextToolbar"/> where Formatter is used</param>
        public virtual void SetModel(TextToolbar model)
        {
            if (Model != null)
            {
                Model.EditorChanged -= Model_EditorChanged;
            }

            Model = model;
            Model.EditorChanged += Model_EditorChanged;
        }

        /// <summary>
        /// Gets or sets a map of the Actions taken when a button is pressed. Required for Common Button actions (Unless you override both Activation and ShiftActivation)
        /// </summary>
        public ButtonActions ButtonActions { get; protected set; }

        /// <summary>
        /// Gets the default list of buttons
        /// </summary>
        public abstract ButtonMap DefaultButtons { get; }

        /// <summary>
        /// Gets the formatted version of the Editor's Text
        /// </summary>
        public virtual string Text
        {
            get
            {
                Model.Editor.Document.GetText(TextGetOptions.FormatRtf, out var currentvalue);
                return currentvalue;
            }
        }

        /// <summary>
        /// Gets the Characters used to indicate a New Line
        /// </summary>
        public virtual string NewLineChars
        {
            get
            {
                return "\r\n";
            }
        }

        /// <summary>
        /// Gets the current Editor Selection
        /// </summary>
        public ITextSelection Selected
        {
            get { return Model.Editor.Document.Selection; }
        }

        /// <summary>
        /// Shortcut to Carriage Return
        /// </summary>
        protected const string Return = "\r";
    }
}