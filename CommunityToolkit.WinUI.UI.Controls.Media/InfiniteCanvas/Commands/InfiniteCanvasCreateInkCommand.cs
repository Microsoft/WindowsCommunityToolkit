// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Windows.UI.Input.Inking;

namespace CommunityToolkit.WinUI.UI.Controls
{
    internal class InfiniteCanvasCreateInkCommand : IInfiniteCanvasCommand
    {
        private readonly List<IDrawable> _drawableList;
        private readonly InkDrawable _drawable;

        public InfiniteCanvasCreateInkCommand(List<IDrawable> drawableList, IReadOnlyList<InkStroke> strokes)
        {
            _drawable = new InkDrawable(strokes);
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