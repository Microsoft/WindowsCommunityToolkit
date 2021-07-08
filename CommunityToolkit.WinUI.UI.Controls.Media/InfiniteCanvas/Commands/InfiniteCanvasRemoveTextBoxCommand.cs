// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace CommunityToolkit.WinUI.UI.Controls
{
    internal class InfiniteCanvasRemoveTextBoxCommand : IInfiniteCanvasCommand
    {
        private readonly List<IDrawable> _drawableList;
        private readonly TextDrawable _drawable;

        public InfiniteCanvasRemoveTextBoxCommand(List<IDrawable> drawableList, TextDrawable drawable)
        {
            _drawable = drawable;
            _drawableList = drawableList;
        }

        public void Execute()
        {
            _drawableList.Remove(_drawable);
        }

        public void Undo()
        {
            _drawableList.Add(_drawable);
        }
    }
}