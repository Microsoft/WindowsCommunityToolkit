// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// An items control that presents enumerable content similar to the live tiles on the
    /// start menu.
    /// </summary>
    [TemplatePart(Name = ScrollerPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = CurrentPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = NextPartName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = TranslatePartName, Type = typeof(TranslateTransform))]
    [TemplatePart(Name = StackPartName, Type = typeof(StackPanel))]
    public partial class RotatorTile : Control
    {
        private const string ScrollerPartName = "Scroller";
        private const string CurrentPartName = "Current";
        private const string NextPartName = "Next";
        private const string TranslatePartName = "Translate";
        private const string StackPartName = "Stack";

        private static readonly Random Randomizer = new Random();
        private int _currentIndex = -1; // current index in the items displayed
        private DispatcherQueueTimer _timer;  // timer for triggering when to flip the content
        private FrameworkElement _currentElement; // FrameworkElement holding a reference to the current element being display
        private FrameworkElement _nextElement; // FrameworkElement holding a reference to the next element being display
        private FrameworkElement _scroller;  // Container Element that's being translated to animate from one item to the next
        private TranslateTransform _translate; // Translate Transform used when animating the transition
        private StackPanel _stackPanel; // StackPanel that contains the live tile elements
        private bool _suppressFlipOnSet; // Prevents the SelectedItem change handler to cause a flip
        private WeakEventListener<RotatorTile, object, NotifyCollectionChangedEventArgs> _inccWeakEventListener;

        /// <summary>
        /// Identifies the <see cref="ExtraRandomDuration"/> property.
        /// </summary>
        public static readonly DependencyProperty ExtraRandomDurationProperty =
            DependencyProperty.Register(nameof(ExtraRandomDuration), typeof(TimeSpan), typeof(RotatorTile), new PropertyMetadata(default(TimeSpan)));

        /// <summary>
        /// Identifies the <see cref="RotationDelay"/> property.
        /// </summary>
        public static readonly DependencyProperty RotationDelayProperty =
            DependencyProperty.Register(nameof(RotationDelay), typeof(TimeSpan), typeof(RotatorTile), new PropertyMetadata(default(TimeSpan), OnRotationDelayInSecondsPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(RotatorTile), new PropertyMetadata(null, OnItemsSourcePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(RotatorTile), null);

        /// <summary>
        /// Identifies the <see cref="RotateDirection"/> property.
        /// </summary>
        public static readonly DependencyProperty RotateDirectionProperty =
            DependencyProperty.Register(nameof(RotateDirection), typeof(RotateDirection), typeof(RotatorTile), new PropertyMetadata(RotateDirection.Up));

        /// <summary>
        /// Identifies the <see cref="CurrentItem"/> property.
        /// </summary>
        public static readonly DependencyProperty CurrentItemProperty =
            DependencyProperty.Register(nameof(CurrentItem), typeof(object), typeof(RotatorTile), new PropertyMetadata(null, OnCurrentItemPropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="RotatorTile"/> class.
        /// </summary>
        public RotatorTile()
        {
            DefaultStyleKey = typeof(RotatorTile);

            Unloaded += RotatorTile_Unloaded;
            Loaded += RotatorTile_Loaded;
            SizeChanged += RotatorTile_SizeChanged;
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            _scroller = GetTemplateChild(ScrollerPartName) as FrameworkElement;
            _currentElement = GetTemplateChild(CurrentPartName) as FrameworkElement;
            _nextElement = GetTemplateChild(NextPartName) as FrameworkElement;
            _translate = GetTemplateChild(TranslatePartName) as TranslateTransform;
            _stackPanel = GetTemplateChild(StackPartName) as StackPanel;

            // set the correct defaults for translate transform
            UpdateTranslateXY();

            if (_stackPanel != null)
            {
                if (Direction == RotateDirection.Down || Direction == RotateDirection.Right)
                {
                    // reverse the order of elements in the _stackpanel
                    _stackPanel.Children.Move(1, 0);
                }

                _stackPanel.Orientation = Direction == RotateDirection.Up || Direction == RotateDirection.Down ? Orientation.Vertical : Orientation.Horizontal;
            }

            if (ItemsSource != null)
            {
                Start();
            }

            base.OnApplyTemplate();
        }

        private void RotatorTile_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_currentElement != null && _nextElement != null)
            {
                _currentElement.Width = _nextElement.Width = e.NewSize.Width;
                _currentElement.Height = _nextElement.Height = e.NewSize.Height;
            }

            // Set content area to twice the size in the slide direction
            if (_scroller != null)
            {
                if (Direction == RotateDirection.Up || Direction == RotateDirection.Down)
                {
                    _scroller.Height = e.NewSize.Height * 2;
                }
                else
                {
                    _scroller.Width = e.NewSize.Width * 2;
                }
            }

            // Set clip to control
            Clip = new RectangleGeometry { Rect = e.NewSize.ToRect() };
        }

        private void RotatorTile_Loaded(object sender, RoutedEventArgs e)
        {
            // Start timer after control has loaded
            _timer?.Start();
        }

        private void RotatorTile_Unloaded(object sender, RoutedEventArgs e)
        {
            // Stop timer and reset animation when control unloads
            _timer?.Stop();

            if (_translate != null)
            {
                _translate.Y = 0;
            }
        }

        /// <summary>
        /// Triggered when it's time to flip to the next live tile.
        /// </summary>
        private void Timer_Tick(object sender, object e)
        {
            _timer.Interval = GetTileDuration();
            UpdateNextItem();
        }

        private void UpdateNextItem()
        {
            _currentIndex++;
            CurrentItem = GetItemAt(_currentIndex);
        }

        private void RotateToNextItem()
        {
            // Check if there's more than one item. if not, don't start animation
            bool hasTwoOrMoreItems = false;
            if (ItemsSource is IEnumerable)
            {
                var enumerator = (ItemsSource as IEnumerable).GetEnumerator();
                int count = 0;
                while (enumerator.MoveNext())
                {
                    count++;
                    if (count > 1)
                    {
                        hasTwoOrMoreItems = true;
                        break;
                    }
                }
            }

            if (!hasTwoOrMoreItems)
            {
                return;
            }

            var sb = new Storyboard();
            if (_translate != null)
            {
                var anim = new DoubleAnimation
                {
                    Duration = new Duration(TimeSpan.FromMilliseconds(500)),
                    From = 0
                };
                if (Direction == RotateDirection.Up)
                {
                    anim.To = -ActualHeight;
                }
                else if (Direction == RotateDirection.Down)
                {
                    anim.From = -1 * ActualHeight;
                    anim.To = 0;
                }
                else if (Direction == RotateDirection.Right)
                {
                    anim.From = -1 * ActualWidth;
                    anim.To = 0;
                }
                else if (Direction == RotateDirection.Left)
                {
                    anim.To = -ActualWidth;
                }

                anim.FillBehavior = FillBehavior.HoldEnd;
                anim.EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut };
                Storyboard.SetTarget(anim, _translate);
                if (Direction == RotateDirection.Up || Direction == RotateDirection.Down)
                {
                    Storyboard.SetTargetProperty(anim, "Y");
                }
                else
                {
                    Storyboard.SetTargetProperty(anim, "X");
                }

                sb.Children.Add(anim);
            }

            sb.Completed += async (a, b) =>
            {
                if (_currentElement != null && _nextElement != null)
                {
                    _currentElement.DataContext = _nextElement.DataContext;
                }

                // make sure DataContext on _currentElement has had a chance to update the binding
                // avoids flicker on rotation
                await System.Threading.Tasks.Task.Delay(50);

                // Reset back and swap images, getting the next image ready
                sb.Stop();
                if (_translate != null)
                {
                    UpdateTranslateXY();
                }

                if (_nextElement != null)
                {
                    _nextElement.DataContext = GetNext(); // Preload the next tile
                }
            };
            sb.Begin();
        }

        private void UpdateTranslateXY()
        {
            if (_translate == null)
            {
                return;
            }

            if (Direction == RotateDirection.Left || Direction == RotateDirection.Up)
            {
                _translate.X = _translate.Y = 0;
            }
            else if (Direction == RotateDirection.Right)
            {
                _translate.X = -1 * ActualWidth;
            }
            else if (Direction == RotateDirection.Down)
            {
                _translate.Y = -1 * ActualHeight;
            }
        }

        private object GetCurrent()
        {
            return GetItemAt(_currentIndex);
        }

        private object GetNext()
        {
            if (_currentIndex < 0)
            {
                return null;
            }

            return GetItemAt(_currentIndex + 1);
        }

        private object GetItemAt(int index)
        {
            if (ItemsSource != null && index > -1)
            {
                if (ItemsSource is IList)
                {
                    var list = ItemsSource as IList;
                    if (list.Count > 0)
                    {
                        index = index % list.Count;
                        return list[index];
                    }
                }
                else if (ItemsSource is IEnumerable)
                {
                    var ienum = ((IEnumerable)ItemsSource).GetEnumerator();
                    int count = 0;
                    while (ienum.MoveNext())
                    {
                        count++;
                    }

                    if (count > 0)
                    {
                        index = index % count;
                        ienum.Reset();
                        for (int i = 0; i < index; i++)
                        {
                            ienum.MoveNext();
                        }

                        return ienum.Current;
                    }
                }
            }

            return null;
        }

        private int GetIndexOf(object item)
        {
            if (ItemsSource is IEnumerable)
            {
                int i = 0;
                var ienum = ((IEnumerable)ItemsSource).GetEnumerator();
                while (ienum.MoveNext())
                {
                    if (ienum.Current == item)
                    {
                        return i;
                    }

                    i++;
                }
            }

            return -1;
        }

        private void Start()
        {
            _currentIndex = 0;
            var currentItem = GetCurrent();
            if (_currentElement != null)
            {
                _currentElement.DataContext = currentItem;
            }

            if (_nextElement != null)
            {
                _nextElement.DataContext = GetNext();
            }

            if (currentItem == null)
            {
                _currentIndex = -1;
                _timer?.Stop();
                return;
            }

            if (_timer == null)
            {
                _timer = DispatcherQueue.GetForCurrentThread().CreateTimer();
                _timer.Interval = GetTileDuration();
                _timer.Tick += Timer_Tick;
            }

            _timer.Start();
            _suppressFlipOnSet = true;
            CurrentItem = currentItem;
            _suppressFlipOnSet = false;
        }

        /// <summary>
        /// A method to get duration for tile.
        /// </summary>
        /// <returns>Returns the duration for the tile based on RotationDelay.</returns>
        private TimeSpan GetTileDuration()
        {
            return RotationDelay + TimeSpan.FromSeconds(Randomizer.Next(0, (int)(ExtraRandomDuration - TimeSpan.Zero).TotalSeconds));
        }

        /// <summary>
        /// Gets or sets the ItemsSource
        /// </summary>
        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (RotatorTile)d;
            ctrl.OnCollectionChanged(e.OldValue, e.NewValue);
        }

        private void OnCollectionChanged(object oldValue, object newValue)
        {
            if (oldValue is INotifyCollectionChanged)
            {
                var incc = (INotifyCollectionChanged)oldValue;
                incc.CollectionChanged -= Incc_CollectionChanged;
                _inccWeakEventListener?.Detach();
                _inccWeakEventListener = null;
            }

            if (newValue is IEnumerable)
            {
                if (newValue is INotifyCollectionChanged)
                {
                    var incc = (INotifyCollectionChanged)newValue;
                    _inccWeakEventListener = new WeakEventListener<RotatorTile, object, NotifyCollectionChangedEventArgs>(this)
                    {
                        OnEventAction = (instance, source, eventArgs) => instance.Incc_CollectionChanged(source, eventArgs),
                        OnDetachAction = (listener) => incc.CollectionChanged -= listener.OnEvent
                    };
                    incc.CollectionChanged += _inccWeakEventListener.OnEvent;
                }

                Start();
            }
            else
            {
                _timer?.Stop();
            }
        }

        private void Incc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems?.Count > 0)
                {
                    int endIndex = e.OldStartingIndex + e.OldItems.Count;
                    if (_currentIndex >= e.NewStartingIndex && _currentIndex < endIndex)
                    {
                        // Current item was removed. Replace with the next one
                        UpdateNextItem();
                    }
                    else if (_currentIndex > endIndex)
                    {
                        // Items were removed before the current item. Just update the changed index
                        _currentIndex -= (endIndex - e.NewStartingIndex) - 1;
                    }
                    else if (e.NewStartingIndex == _currentIndex + 1)
                    {
                        // Upcoming item was changed, so update the datacontext
                        if (_nextElement != null)
                        {
                            _nextElement.DataContext = GetNext();
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems?.Count > 0)
                {
                    if (_currentIndex < 0)
                    {
                        // First item loaded. Start the rotator
                        Start();
                    }
                    else if (_currentIndex >= e.NewStartingIndex)
                    {
                        // Items were inserted before the current item. Update the index
                        _currentIndex += e.NewItems.Count;
                    }
                    else if (_currentIndex + 1 == e.NewStartingIndex)
                    {
                        // Upcoming item was changed, so update the datacontext
                        if (_nextElement != null)
                        {
                            _nextElement.DataContext = GetNext();
                        }
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                int endIndex = e.OldStartingIndex + e.OldItems.Count;
                if (_currentIndex >= e.OldStartingIndex && _currentIndex < endIndex + 1)
                {
                    // Current item was removed. Replace with the next one
                    UpdateNextItem();
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Move)
            {
                int endIndex = e.OldStartingIndex + e.OldItems.Count;
                if (_currentIndex >= e.OldStartingIndex && _currentIndex < endIndex)
                {
                    // The current item was moved. Get its new location
                    _currentIndex = GetIndexOf(CurrentItem);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                // Significant change or clear. Restart.
                Start();
            }
        }

        /// <summary>
        /// Gets or sets the currently selected visible item
        /// </summary>
        public object CurrentItem
        {
            get { return GetValue(CurrentItemProperty); }
            set { SetValue(CurrentItemProperty, value); }
        }

        private static void OnRotationDelayInSecondsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Update new timer value.
            var ctrl = d as RotatorTile;
            if (ctrl?._timer != null)
            {
                ctrl._timer.Interval = ctrl.GetTileDuration();
            }
        }

        private static void OnCurrentItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (RotatorTile)d;
            if (ctrl._suppressFlipOnSet)
            {
                return;
            }

            int index = ctrl.GetIndexOf(e.NewValue);
            if (index > -1)
            {
                ctrl._currentIndex = index;
                if (ctrl._nextElement != null)
                {
                    ctrl._nextElement.DataContext = e.NewValue;
                }

                ctrl.RotateToNextItem();
                ctrl._timer.Stop();
                ctrl._timer.Start();
            }
        }

        /// <summary>
        /// Gets or sets the item template
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the direction the tile slides in.
        /// </summary>
        public RotateDirection Direction
        {
            get { return (RotateDirection)GetValue(RotateDirectionProperty); }
            set { SetValue(RotateDirectionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the duration for tile rotation.
        /// </summary>
        public TimeSpan RotationDelay
        {
            get { return (TimeSpan)GetValue(RotationDelayProperty); }
            set { SetValue(RotationDelayProperty, value); }
        }

        /// <summary>
        /// Tile Slide Direction
        /// </summary>
        public enum RotateDirection
        {
            /// <summary>Up</summary>
            Up,

            /// <summary>Left</summary>
            Left,

            /// <summary>Down</summary>
            Down,

            /// <summary>Right</summary>
            Right,
        }

        /// <summary>
        /// Gets or sets the extra randomized duration to be added to the <see cref="RotationDelay"/> property.
        /// A value between zero and this value *in seconds* will be added to the <see cref="RotationDelay"/>.
        /// </summary>
        public TimeSpan ExtraRandomDuration
        {
            get { return (TimeSpan)GetValue(ExtraRandomDurationProperty); }
            set { SetValue(ExtraRandomDurationProperty, value); }
        }
    }
}
