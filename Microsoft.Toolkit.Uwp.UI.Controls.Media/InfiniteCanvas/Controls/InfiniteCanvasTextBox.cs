// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// This is the infiniteCanvas custom textbox that is used to write to the canvas. This control is used as part of the <see cref="InfiniteCanvas"/>
    /// </summary>
    public class InfiniteCanvasTextBox : Control
    {
        private TextBox _editZone;

        internal event EventHandler<string> TextChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfiniteCanvasTextBox"/> class.
        /// </summary>
        public InfiniteCanvasTextBox()
        {
            DefaultStyleKey = typeof(InfiniteCanvasTextBox);
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            _editZone = (TextBox)GetTemplateChild("EditZone");
            _editZone.TextChanged -= EditZone_TextChanged;
            _editZone.TextChanged += EditZone_TextChanged;
            _editZone.FontSize = FontSize;
            base.OnApplyTemplate();
        }

        private void EditZone_TextChanged(object sender, RoutedEventArgs e)
        {
            TextChanged?.Invoke(this, _editZone.Text);
        }

        internal double GetEditZoneWidth()
        {
            return _editZone.ActualWidth;
        }

        internal double GetEditZoneHeight()
        {
            return _editZone.ActualHeight;
        }

        internal void Clear()
        {
            if (_editZone == null)
            {
                return;
            }

            _editZone.TextChanged -= EditZone_TextChanged;
            _editZone.Text = string.Empty;
            _editZone.TextChanged += EditZone_TextChanged;
        }

        internal void SetText(string text)
        {
            if (_editZone == null)
            {
                if (!ApplyTemplate())
                {
                    return;
                }
            }

            _editZone.Text = text;
            _editZone.SelectionStart = text.Length;
        }

        internal void UpdateFontSize(float textFontSize)
        {
            FontSize = textFontSize;

            if (_editZone != null)
            {
                _editZone.FontSize = textFontSize;
            }
        }

        internal void UpdateFontStyle(bool isItalic)
        {
            if (_editZone != null)
            {
                _editZone.FontStyle = isItalic ? FontStyle.Italic : FontStyle.Normal;
            }
        }

        internal void UpdateFontWeight(bool isBold)
        {
            if (_editZone != null)
            {
                _editZone.FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal;
            }
        }

        internal bool CannotGoRight()
        {
            return (_editZone.SelectionStart + _editZone.SelectionLength) == _editZone.Text.Length;
        }

        internal bool CannotGoLeft()
        {
            return _editZone.SelectionStart == 0;
        }

        internal bool CannotGoUp()
        {
            var lines = _editZone.Text.Split('\r');
            if (lines.Count() == 1)
            {
                return true;
            }

            var firstLine = lines.First();
            if (firstLine.Length >= _editZone.SelectionStart)
            {
                return true;
            }

            return false;
        }

        internal bool CannotGoDown()
        {
            var lines = _editZone.Text.Split('\r');
            if (lines.Count() == 1)
            {
                return true;
            }

            var lastLine = lines.ElementAt(lines.Length - 1);
            if ((_editZone.Text.Length - lastLine.Length) <= (_editZone.SelectionStart + _editZone.SelectionLength))
            {
                return true;
            }

            return false;
        }
    }
}