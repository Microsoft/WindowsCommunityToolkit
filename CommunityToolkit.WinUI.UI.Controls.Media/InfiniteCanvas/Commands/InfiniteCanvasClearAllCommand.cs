// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace CommunityToolkit.WinUI.UI.Controls
{
    internal class InfiniteCanvasClearAllCommand : IInfiniteCanvasCommand
    {
        private readonly List<IDrawable> _drawableList;
        private IDrawable[] _storeList;

        public InfiniteCanvasClearAllCommand(List<IDrawable> drawableList)
        {
            _drawableList = drawableList;
        }

        public void Execute()
        {
            _storeList = new IDrawable[_drawableList.Count];
            for (int i = 0; i < _drawableList.Count; i++)
            {
                _storeList[i] = _drawableList[i];
            }

            _drawableList.Clear();
        }

        public void Undo()
        {
            foreach (var drawable in _storeList)
            {
                _drawableList.Add(drawable);
            }
        }
    }
}