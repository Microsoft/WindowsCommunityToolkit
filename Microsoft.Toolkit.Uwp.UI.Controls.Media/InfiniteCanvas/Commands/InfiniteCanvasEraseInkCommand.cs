// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    internal class InfiniteCanvasEraseInkCommand : IInfiniteCanvasCommand
    {
        private readonly List<IDrawable> _drawableList;
        private readonly IDrawable _drawable;

        public InfiniteCanvasEraseInkCommand(List<IDrawable> drawableList, IDrawable drawable)
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