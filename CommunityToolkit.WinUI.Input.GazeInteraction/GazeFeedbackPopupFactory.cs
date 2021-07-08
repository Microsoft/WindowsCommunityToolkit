// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace CommunityToolkit.WinUI.Input.GazeInteraction
{
    internal class GazeFeedbackPopupFactory
    {
        private readonly List<Popup> _cache = new List<Popup>();

        public Popup Get()
        {
            Popup popup;
            Microsoft.UI.Xaml.Shapes.Rectangle rectangle;

            if (_cache.Count != 0)
            {
                popup = _cache[0];
                _cache.RemoveAt(0);

                rectangle = popup.Child as Microsoft.UI.Xaml.Shapes.Rectangle;
            }
            else
            {
                popup = new Popup();

                rectangle = new Microsoft.UI.Xaml.Shapes.Rectangle
                {
                    IsHitTestVisible = false
                };

                popup.Child = rectangle;
            }

            rectangle.StrokeThickness = GazeInput.DwellStrokeThickness;

            return popup;
        }

        public void Return(Popup popup)
        {
            popup.IsOpen = false;
            _cache.Add(popup);
        }
    }
}