// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Microsoft.Toolkit.Uwp.UI
{
    /// <summary>
    /// Provides attached dependency properties and methods for the <see cref="ScrollViewer"/> control.
    /// </summary>
    public static partial class ScrollViewerExtensions
    {
        private static double _threshold = 50;
        private static bool _isPressed = false;
        private static bool _isMoved = false;
        private static Point _startPosition;
        private static bool _isDeferredMovingStarted = false;
        private static double _factor = 50;
        private static Point _currentPosition;
        private static Timer _timer;
        private static ScrollViewer _scrollViewer;
        private static uint _oldCursorID = 100;
        private static uint _maxSpeed = 200;
        private static bool _isCursorAvailable = false;

        /// <summary>
        /// Function will be called when <see cref="EnableMiddleClickScrollingProperty"/> is updated
        /// </summary>
        /// <param name="d">Holds the dependency object</param>
        /// <param name="e">Holds the dependency object args</param>
        private static void OnEnableMiddleClickScrollingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                _scrollViewer = scrollViewer;
            }
            else
            {
                _scrollViewer = (d as FrameworkElement).FindDescendant<ScrollViewer>();

                if (_scrollViewer == null)
                {
                    (d as FrameworkElement).Loaded += (sender, arg) =>
                    {
                        _scrollViewer = (sender as FrameworkElement).FindDescendant<ScrollViewer>();

                        if (_scrollViewer != null)
                        {
                            UpdateChange((bool)e.NewValue);
                        }
                    };
                }
            }

            if (_scrollViewer == null)
            {
                return;
            }

            UpdateChange((bool)e.NewValue);
        }

        /// <summary>
        /// Function to update changes in <see cref="EnableMiddleClickScrollingProperty"/>
        /// </summary>
        /// <param name="newValue">New value from the <see cref="EnableMiddleClickScrollingProperty"/></param>
        private static void UpdateChange(bool newValue)
        {
            if (newValue)
            {
                _scrollViewer.PointerPressed -= ScrollViewer_PointerPressed;
                _scrollViewer.PointerPressed += ScrollViewer_PointerPressed;
            }
            else
            {
                _scrollViewer.PointerPressed -= ScrollViewer_PointerPressed;
                UnsubscribeMiddleClickScrolling();
            }
        }

        /// <summary>
        /// Function to set default value and subscribe to events
        /// </summary>
        private static void SubscribeMiddleClickScrolling(DispatcherQueue dispatcherQueue)
        {
            _isPressed = true;
            _isMoved = false;
            _startPosition = default(Point);
            _currentPosition = default(Point);
            _isDeferredMovingStarted = false;
            _oldCursorID = 100;
            _isCursorAvailable = IsCursorResourceAvailable();

            _timer?.Dispose();
            _timer = new Timer(Scroll, dispatcherQueue, 5, 5);

            Window.Current.CoreWindow.PointerMoved -= CoreWindow_PointerMoved;
            Window.Current.CoreWindow.PointerReleased -= CoreWindow_PointerReleased;

            Window.Current.CoreWindow.PointerMoved += CoreWindow_PointerMoved;
            Window.Current.CoreWindow.PointerReleased += CoreWindow_PointerReleased;
        }

        /// <summary>
        /// Function to set default value and unsubscribe to events
        /// </summary>
        private static void UnsubscribeMiddleClickScrolling()
        {
            _isPressed = false;
            _isMoved = false;
            _startPosition = default(Point);
            _currentPosition = default(Point);
            _isDeferredMovingStarted = false;
            _oldCursorID = 100;
            _timer?.Dispose();

            Window.Current.CoreWindow.PointerMoved -= CoreWindow_PointerMoved;
            Window.Current.CoreWindow.PointerReleased -= CoreWindow_PointerReleased;

            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
        }

        /// <summary>
        /// This function will be called for every small interval by <see cref="Timer"/>
        /// </summary>
        /// <param name="state">Default param for <see cref="Timer"/>. In this function it will be `null`</param>
        private static void Scroll(object state)
        {
            var dispatcherQueue = state as DispatcherQueue;
            if (dispatcherQueue == null)
            {
                return;
            }

            var offsetX = _currentPosition.X - _startPosition.X;
            var offsetY = _currentPosition.Y - _startPosition.Y;

            SetCursorType(dispatcherQueue, offsetX, offsetY);

            if (Math.Abs(offsetX) > _threshold || Math.Abs(offsetY) > _threshold)
            {
                offsetX = Math.Abs(offsetX) < _threshold ? 0 : offsetX;
                offsetY = Math.Abs(offsetY) < _threshold ? 0 : offsetY;

                offsetX /= _factor;
                offsetY /= _factor;

                offsetX = offsetX > 0 ? Math.Pow(offsetX, 2) : -Math.Pow(offsetX, 2);
                offsetY = offsetY > 0 ? Math.Pow(offsetY, 2) : -Math.Pow(offsetY, 2);

                offsetX = offsetX > _maxSpeed ? _maxSpeed : offsetX;
                offsetY = offsetY > _maxSpeed ? _maxSpeed : offsetY;

                dispatcherQueue.EnqueueAsync(() => _scrollViewer?.ChangeView(_scrollViewer.HorizontalOffset + offsetX, _scrollViewer.VerticalOffset + offsetY, null, true));
            }
        }

        /// <summary>
        /// Function to check the status of scrolling
        /// </summary>
        /// <returns>Return true if the scrolling is started</returns>
        private static bool CanScroll()
        {
            return _isDeferredMovingStarted || (_isPressed && !_isDeferredMovingStarted);
        }

        private static void ScrollViewer_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // Unsubscribe if deferred moving is started
            if (_isDeferredMovingStarted)
            {
                UnsubscribeMiddleClickScrolling();
                return;
            }

            Pointer pointer = e.Pointer;

            if (pointer.PointerDeviceType == PointerDeviceType.Mouse)
            {
                _scrollViewer = sender as ScrollViewer;

                PointerPoint pointerPoint = e.GetCurrentPoint(_scrollViewer);

                // SubscribeMiddle if middle button is pressed
                if (pointerPoint.Properties.IsMiddleButtonPressed)
                {
                    SubscribeMiddleClickScrolling(DispatcherQueue.GetForCurrentThread());

                    _startPosition = Window.Current.CoreWindow.PointerPosition;
                    _currentPosition = Window.Current.CoreWindow.PointerPosition;
                }
            }
        }

        private static void CoreWindow_PointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            // If condition that occurs before scrolling begins
            if (_isPressed && !_isMoved)
            {
                PointerPoint pointerPoint = args.CurrentPoint;

                if (pointerPoint.Properties.IsMiddleButtonPressed)
                {
                    _currentPosition = Window.Current.CoreWindow.PointerPosition;

                    var offsetX = _currentPosition.X - _startPosition.X;
                    var offsetY = _currentPosition.Y - _startPosition.Y;

                    // Setting _isMoved if pointer goes out of threshold value
                    if (Math.Abs(offsetX) > _threshold || Math.Abs(offsetY) > _threshold)
                    {
                        _isMoved = true;
                    }
                }
            }

            // Update current position of the pointer if scrolling started
            if (CanScroll())
            {
                _currentPosition = Window.Current.CoreWindow.PointerPosition;
            }
        }

        private static void CoreWindow_PointerReleased(CoreWindow sender, PointerEventArgs args)
        {
            // Start deferred moving if the pointer is pressed and not moved
            if (_isPressed && !_isMoved)
            {
                _isDeferredMovingStarted = true;

                // Event to stop deferred scrolling if pointer exited
                Window.Current.CoreWindow.PointerExited -= CoreWindow_PointerExited;
                Window.Current.CoreWindow.PointerExited += CoreWindow_PointerExited;

                // Event to stop deferred scrolling if pointer pressed
                Window.Current.CoreWindow.PointerPressed -= CoreWindow_PointerPressed;
                Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;

                SetCursorType(DispatcherQueue.GetForCurrentThread(), 0, 0);
            }
            else
            {
                _isDeferredMovingStarted = false;
            }

            // Unsubscribe if the pointer is pressed and not DeferredMoving
            if (_isPressed && !_isDeferredMovingStarted)
            {
                UnsubscribeMiddleClickScrolling();
            }
        }

        private static void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs args)
        {
            Window.Current.CoreWindow.PointerPressed -= CoreWindow_PointerPressed;
            Window.Current.CoreWindow.PointerExited -= CoreWindow_PointerExited;
            UnsubscribeMiddleClickScrolling();
        }

        private static void CoreWindow_PointerExited(CoreWindow sender, PointerEventArgs args)
        {
            Window.Current.CoreWindow.PointerPressed -= CoreWindow_PointerPressed;
            Window.Current.CoreWindow.PointerExited -= CoreWindow_PointerExited;
            UnsubscribeMiddleClickScrolling();
        }

        private static void SetCursorType(DispatcherQueue dispatcherQueue, double offsetX, double offsetY)
        {
            if (!_isCursorAvailable)
            {
                return;
            }

            uint cursorID = 101;

            if (Math.Abs(offsetX) < _threshold && Math.Abs(offsetY) < _threshold)
            {
                cursorID = 101;
            }
            else if (Math.Abs(offsetX) < _threshold && offsetY < -_threshold)
            {
                cursorID = 102;
            }
            else if (offsetX > _threshold && offsetY < -_threshold)
            {
                cursorID = 103;
            }
            else if (offsetX > _threshold && Math.Abs(offsetY) < _threshold)
            {
                cursorID = 104;
            }
            else if (offsetX > _threshold && offsetY > _threshold)
            {
                cursorID = 105;
            }
            else if (Math.Abs(offsetX) < _threshold && offsetY > _threshold)
            {
                cursorID = 106;
            }
            else if (offsetX < -_threshold && offsetY > _threshold)
            {
                cursorID = 107;
            }
            else if (offsetX < -_threshold && Math.Abs(offsetY) < _threshold)
            {
                cursorID = 108;
            }
            else if (offsetX < -_threshold && offsetY < -_threshold)
            {
                cursorID = 109;
            }

            if (_oldCursorID != cursorID)
            {
                dispatcherQueue.EnqueueAsync(() => Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Custom, cursorID));

                _oldCursorID = cursorID;
            }
        }

        /// <summary>
        /// Function to check the availability of cursor resource
        /// </summary>
        /// <returns>Returns `true` if the cursor resource is available</returns>
        private static bool IsCursorResourceAvailable()
        {
            var isCursorAvailable = true;

            try
            {
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Custom, 101);
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Custom, 102);
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Custom, 103);
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Custom, 104);
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Custom, 105);
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Custom, 106);
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Custom, 107);
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Custom, 108);
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Custom, 109);
            }
            catch (Exception)
            {
                isCursorAvailable = false;
            }
            finally
            {
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
            }

            return isCursorAvailable;
        }
    }
}
