// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// RangeSelector is a "double slider" control for range values.
    /// </summary>
    public partial class RangeSelector : Control
    {
        private readonly DispatcherQueueTimer keyDebounceTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();

        private void MinThumb_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Left:
                    RangeStart -= StepFrequency;
                    SyncThumbs(fromMinKeyDown: true);
                    if (_toolTip != null)
                    {
                        _toolTip.Visibility = Visibility.Visible;
                    }

                    e.Handled = true;
                    break;
                case VirtualKey.Right:
                    RangeStart += StepFrequency;
                    SyncThumbs(fromMinKeyDown: true);
                    if (_toolTip != null)
                    {
                        _toolTip.Visibility = Visibility.Visible;
                    }

                    e.Handled = true;
                    break;
            }
        }

        private void MaxThumb_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Left:
                    RangeEnd -= StepFrequency;
                    SyncThumbs(fromMaxKeyDown: true);
                    if (_toolTip != null)
                    {
                        _toolTip.Visibility = Visibility.Visible;
                    }

                    e.Handled = true;
                    break;
                case VirtualKey.Right:
                    RangeEnd += StepFrequency;
                    SyncThumbs(fromMaxKeyDown: true);
                    if (_toolTip != null)
                    {
                        _toolTip.Visibility = Visibility.Visible;
                    }

                    e.Handled = true;
                    break;
            }
        }

        private void Thumb_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case VirtualKey.Left:
                case VirtualKey.Right:
                    if (_toolTip != null)
                    {
                        keyDebounceTimer.Debounce(
                            () => _toolTip.Visibility = Visibility.Collapsed,
                            TimeToHideToolTipOnKeyUp);
                    }

                    e.Handled = true;
                    break;
            }
        }
    }
}
