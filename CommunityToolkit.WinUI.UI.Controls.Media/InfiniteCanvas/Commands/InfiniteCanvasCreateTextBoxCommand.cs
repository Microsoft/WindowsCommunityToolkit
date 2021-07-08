// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Controls
{
    internal class InfiniteCanvasCreateTextBoxCommand : IInfiniteCanvasCommand
    {
        private readonly List<IDrawable> _drawableList;
        private readonly TextDrawable _drawable;

        public InfiniteCanvasCreateTextBoxCommand(List<IDrawable> drawableList, double x, double y, double width, double height, float textFontSize, string text, Color color, bool isBold, bool isItalic)
        {
            _drawable = new TextDrawable(
                x,
                y,
                width,
                height,
                textFontSize,
                text,
                color,
                isBold,
                isItalic);
            _drawableList = drawableList;
        }

        public void Execute()
        {
            _drawableList.Add(_drawable);
        }

        public void Undo()
        {
            _drawableList.Remove(_drawable);
        }
    }
}