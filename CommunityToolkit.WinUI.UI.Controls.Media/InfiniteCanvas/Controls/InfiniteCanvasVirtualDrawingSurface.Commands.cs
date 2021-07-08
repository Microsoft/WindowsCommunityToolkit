// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input.Inking;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// The virtual Drawing surface renderer used to render the ink and text. This control is used as part of the <see cref="InfiniteCanvas"/>
    /// </summary>
    public partial class InfiniteCanvasVirtualDrawingSurface
    {
        private readonly Stack<IInfiniteCanvasCommand> _undoCommands = new Stack<IInfiniteCanvasCommand>();
        private readonly Stack<IInfiniteCanvasCommand> _redoCommands = new Stack<IInfiniteCanvasCommand>();

        internal event EventHandler CommandExecuted;

        internal void Undo(Rect viewPort, float zoomFactor)
        {
            if (_undoCommands.Count != 0)
            {
                IInfiniteCanvasCommand command = _undoCommands.Pop();
                command.Undo();
                _redoCommands.Push(command);

                ReDraw(viewPort, zoomFactor);
            }
        }

        internal void Redo(Rect viewPort, float zoomFactor)
        {
            if (_redoCommands.Count != 0)
            {
                IInfiniteCanvasCommand command = _redoCommands.Pop();
                command.Execute();
                _undoCommands.Push(command);

                ReDraw(viewPort, zoomFactor);
            }
        }

        internal void ExecuteUpdateTextBoxText(string newText)
        {
            var drawable = GetSelectedTextDrawable();
            var command = new InfiniteCanvasUpdateTextCommand(drawable, drawable.Text, newText);
            ExecuteCommand(command);
        }

        internal void ExecuteUpdateTextBoxColor(Color newColor)
        {
            var drawable = GetSelectedTextDrawable();
            var command = new InfiniteCanvasUpdateTextColorCommand(drawable, drawable.TextColor, newColor);
            ExecuteCommand(command);
        }

        internal void ExecuteUpdateTextBoxStyle(bool newValue)
        {
            var drawable = GetSelectedTextDrawable();
            var command = new InfiniteCanvasUpdateTextStyleCommand(drawable, drawable.IsItalic, newValue);
            ExecuteCommand(command);
        }

        internal void ExecuteUpdateTextBoxWeight(bool newValue)
        {
            var drawable = GetSelectedTextDrawable();
            var command = new InfiniteCanvasUpdateTextWeightCommand(drawable, drawable.IsBold, newValue);
            ExecuteCommand(command);
        }

        internal void ExecuteUpdateTextBoxFontSize(float newValue)
        {
            var drawable = GetSelectedTextDrawable();
            var command = new InfiniteCanvasUpdateTextFontSizeCommand(drawable, drawable.FontSize, newValue);
            ExecuteCommand(command);
        }

        internal void ExecuteCreateTextBox(double x, double y, double width, double height, float textFontSize, string text, Color color, bool isBold, bool isItalic)
        {
            var command = new InfiniteCanvasCreateTextBoxCommand(_drawableList, x, y, width, height, textFontSize, text, color, isBold, isItalic);
            ExecuteCommand(command);
        }

        internal void ExecuteRemoveTextBox()
        {
            var drawable = GetSelectedTextDrawable();
            var command = new InfiniteCanvasRemoveTextBoxCommand(_drawableList, drawable);
            ExecuteCommand(command);
        }

        internal void ExecuteCreateInk(IReadOnlyList<InkStroke> beginDry)
        {
            var command = new InfiniteCanvasCreateInkCommand(_drawableList, beginDry);
            ExecuteCommand(command);
        }

        internal void ExecuteEraseInk(IDrawable drawable)
        {
            var command = new InfiniteCanvasEraseInkCommand(_drawableList, drawable);
            ExecuteCommand(command);
        }

        internal void ExecuteClearAll()
        {
            var command = new InfiniteCanvasClearAllCommand(_drawableList);
            ExecuteCommand(command);
        }

        private void ExecuteCommand(IInfiniteCanvasCommand command)
        {
            _undoCommands.Push(command);
            _redoCommands.Clear();
            command.Execute();

            CommandExecuted?.Invoke(this, EventArgs.Empty);
        }
    }
}