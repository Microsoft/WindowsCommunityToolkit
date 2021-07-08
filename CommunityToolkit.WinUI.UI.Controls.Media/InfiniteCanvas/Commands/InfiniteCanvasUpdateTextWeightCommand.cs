// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Controls
{
    internal class InfiniteCanvasUpdateTextWeightCommand : IInfiniteCanvasCommand
    {
        private readonly bool _oldValue;
        private readonly bool _newValue;
        private readonly TextDrawable _drawable;

        public InfiniteCanvasUpdateTextWeightCommand(TextDrawable drawable, bool oldValue, bool newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
            _drawable = drawable;
        }

        public void Execute()
        {
            _drawable.IsBold = _newValue;
        }

        public void Undo()
        {
            _drawable.IsBold = _oldValue;
        }
    }
}