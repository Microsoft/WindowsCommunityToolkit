// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// The virtual Drawing surface renderer used to render the ink and text. This control is used as part of the <see cref="InfiniteCanvas"/>
    /// </summary>
    public partial class InfiniteCanvasVirtualDrawingSurface
    {
        private const int DrawableNullIndex = -1;

        private int _selectedTextDrawableIndex = DrawableNullIndex;

        internal void UpdateSelectedTextDrawableIfSelected(Point point, Rect viewPort)
        {
            for (var i = _drawableList.Count - 1; i >= 0; i--)
            {
                var drawable = _drawableList[i];
                if (drawable is TextDrawable && drawable.IsActive && drawable.Bounds.Contains(point))
                {
                    _selectedTextDrawableIndex = i;
                    return;
                }
            }

            _selectedTextDrawableIndex = DrawableNullIndex;
        }

        internal TextDrawable GetSelectedTextDrawable()
        {
            if (_selectedTextDrawableIndex == DrawableNullIndex)
            {
                return null;
            }

            return (TextDrawable)_drawableList.ElementAt(_selectedTextDrawableIndex);
        }

        internal void ResetSelectedTextDrawable()
        {
            _selectedTextDrawableIndex = DrawableNullIndex;
        }

        internal void UpdateSelectedTextDrawable()
        {
            _selectedTextDrawableIndex = _drawableList.Count - 1;
        }

        internal List<string> ExportText()
        {
            return _drawableList.OfType<TextDrawable>().Select(td => td.Text).ToList();
        }
    }
}