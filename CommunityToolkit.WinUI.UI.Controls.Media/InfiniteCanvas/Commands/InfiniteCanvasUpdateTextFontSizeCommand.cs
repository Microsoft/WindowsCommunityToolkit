// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Controls
{
    internal class InfiniteCanvasUpdateTextFontSizeCommand : IInfiniteCanvasCommand
    {
        private readonly float _oldValue;
        private readonly float _newValue;
        private readonly TextDrawable _drawable;

        public InfiniteCanvasUpdateTextFontSizeCommand(TextDrawable drawable, float oldValue, float newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
            _drawable = drawable;
        }

        public void Execute()
        {
            _drawable.FontSize = _newValue;
        }

        public void Undo()
        {
            _drawable.FontSize = _oldValue;
        }
    }
}