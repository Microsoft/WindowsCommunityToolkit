// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace CommunityToolkit.WinUI.Input.GazeInteraction
{
    internal class GazeCursor
    {
        private const int DEFAULT_CURSOR_RADIUS = 5;
        private const bool DEFAULT_CURSOR_VISIBILITY = true;

        public void LoadSettings(ValueSet settings)
        {
            if (settings.ContainsKey("GazeCursor.CursorRadius"))
            {
                CursorRadius = (int)settings["GazeCursor.CursorRadius"];
            }

            if (settings.ContainsKey("GazeCursor.CursorVisibility"))
            {
                IsCursorVisible = (bool)settings["GazeCursor.CursorVisibility"];
            }
        }

        public int CursorRadius
        {
            get
            {
                return _cursorRadius;
            }

            set
            {
                _cursorRadius = value;
                var gazeCursor = CursorElement;
                if (gazeCursor != null)
                {
                    gazeCursor.Width = 2 * _cursorRadius;
                    gazeCursor.Height = 2 * _cursorRadius;
                    gazeCursor.Margin = new Thickness(-_cursorRadius, -_cursorRadius, 0, 0);
                }
            }
        }

        public bool IsCursorVisible
        {
            get
            {
                return _isCursorVisible;
            }

            set
            {
                _isCursorVisible = value;
                SetVisibility();
            }
        }

        public bool IsGazeEntered
        {
            get
            {
                return _isGazeEntered;
            }

            set
            {
                _isGazeEntered = value;
                SetVisibility();
            }
        }

        public Point Position
        {
            get
            {
                return _cursorPosition;
            }

            set
            {
                _cursorPosition = value;
                _gazePopup.HorizontalOffset = value.X;
                _gazePopup.VerticalOffset = value.Y;
                SetVisibility();
            }
        }

        public UIElement PopupChild
        {
            get
            {
                return _gazePopup.Child;
            }

            set
            {
                _gazePopup.Child = value;
            }
        }

        public FrameworkElement CursorElement
        {
            get
            {
                return _gazePopup.Child as FrameworkElement;
            }
        }

        internal GazeCursor()
        {
            _gazePopup = new Popup
            {
                IsHitTestVisible = false
            };

            var gazeCursor = new Microsoft.UI.Xaml.Shapes.Ellipse
            {
                Fill = new SolidColorBrush(Colors.IndianRed),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Width = 2 * CursorRadius,
                Height = 2 * CursorRadius,
                Margin = new Thickness(-CursorRadius, -CursorRadius, 0, 0),
                IsHitTestVisible = false
            };

            _gazePopup.Child = gazeCursor;
        }

        private void SetVisibility()
        {
            var isOpen = _isCursorVisible && _isGazeEntered;
            if (_gazePopup.IsOpen != isOpen)
            {
                _gazePopup.IsOpen = isOpen;
            }
            else if (isOpen)
            {
                Popup topmost;

                if (_gazePopup.XamlRoot != null)
                {
                    topmost = VisualTreeHelper.GetOpenPopupsForXamlRoot(_gazePopup.XamlRoot).First();
                }
                else
                {
                    topmost = VisualTreeHelper.GetOpenPopups(Window.Current).First();
                }

                if (_gazePopup != topmost)
                {
                    _gazePopup.IsOpen = false;
                    _gazePopup.IsOpen = true;
                }
            }
        }

        private readonly Popup _gazePopup;
        private Point _cursorPosition = default;
        private int _cursorRadius = DEFAULT_CURSOR_RADIUS;
        private bool _isCursorVisible = DEFAULT_CURSOR_VISIBILITY;
        private bool _isGazeEntered;
    }
}