// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Controls
{
    internal class InfiniteCanvasUpdateTextColorCommand : IInfiniteCanvasCommand
    {
        private readonly Color _oldColor;
        private readonly Color _newColor;
        private readonly TextDrawable _drawable;

        public InfiniteCanvasUpdateTextColorCommand(TextDrawable drawable, Color oldText, Color newText)
        {
            _oldColor = oldText;
            _newColor = newText;
            _drawable = drawable;
        }

        public void Execute()
        {
            _drawable.TextColor = _newColor;
        }

        public void Undo()
        {
            _drawable.TextColor = _oldColor;
        }
    }
}