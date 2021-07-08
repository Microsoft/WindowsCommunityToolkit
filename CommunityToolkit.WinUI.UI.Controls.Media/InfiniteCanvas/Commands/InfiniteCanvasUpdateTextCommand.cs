// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Controls
{
    internal class InfiniteCanvasUpdateTextCommand : IInfiniteCanvasCommand
    {
        private readonly string _oldText;
        private readonly string _newText;
        private readonly TextDrawable _drawable;

        public InfiniteCanvasUpdateTextCommand(TextDrawable drawable, string oldText, string newText)
        {
            _oldText = oldText;
            _newText = newText;
            _drawable = drawable;
        }

        public void Execute()
        {
            _drawable.Text = _newText;
        }

        public void Undo()
        {
            _drawable.Text = _oldText;
        }
    }
}