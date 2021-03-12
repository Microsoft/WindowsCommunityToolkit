// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Input.Preview;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.Input.GazeInteraction
{
    /// <summary>
    /// Class of singleton object coordinating gaze input.
    /// </summary>
    public class GazePointer
    {
        // units in microseconds
        private static readonly TimeSpan DEFAULT_FIXATION_DELAY = new TimeSpan(3500000);
        private static readonly TimeSpan DEFAULT_DWELL_DELAY = new TimeSpan(4000000);
        private static readonly TimeSpan DEFAULT_DWELL_REPEAT_DELAY = new TimeSpan(4000000);
        private static readonly TimeSpan DEFAULT_REPEAT_DELAY = new TimeSpan(4000000);
        private static readonly TimeSpan DEFAULT_THRESHOLD_DELAY = new TimeSpan(500000);
        private static readonly TimeSpan DEFAULT_MAX_HISTORY_DURATION = new TimeSpan(30000000);
        private static readonly TimeSpan MAX_SINGLE_SAMPLE_DURATION = new TimeSpan(1000000);

        private static readonly TimeSpan GAZE_IDLE_TIME = new TimeSpan(250000000);

        /// <summary>
        /// Finalizes an instance of the <see cref="GazePointer"/> class.
        /// </summary>
        ~GazePointer()
        {
            _watcher.Added -= OnDeviceAdded;
            _watcher.Removed -= OnDeviceRemoved;

            if (_gazeInputSource != null)
            {
                _gazeInputSource.GazeEntered -= OnGazeEntered;
                _gazeInputSource.GazeMoved -= OnGazeMoved;
                _gazeInputSource.GazeExited -= OnGazeExited;
            }
        }

        /// <summary>
        /// Loads a settings collection into GazePointer.
        /// </summary>
        public void LoadSettings(ValueSet settings)
        {
            _gazeCursor.LoadSettings(settings);
            Filter.LoadSettings(settings);

            // TODO Add logic to protect against missing settings
            if (settings.TryGetValue("GazePointer.FixationDelay", out var fixationDelay))
            {
                _defaultFixation = new TimeSpan((int)fixationDelay * 10);
                SetElementStateDelay(_offScreenElement, PointerState.Fixation, new TimeSpan((int)fixationDelay * 10));
            }

            if (settings.TryGetValue("GazePointer.DwellDelay", out var dwellDelay))
            {
                _defaultDwell = new TimeSpan((int)dwellDelay * 10);
                SetElementStateDelay(_offScreenElement, PointerState.Dwell, new TimeSpan((int)dwellDelay * 10));
            }

            if (settings.TryGetValue("GazePointer.DwellRepeatDelay", out var dwellRepeatDelay))
            {
                _defaultDwellRepeatDelay = new TimeSpan((int)dwellRepeatDelay * 10);
            }

            if (settings.TryGetValue("GazePointer.RepeatDelay", out var repeatDelay))
            {
                _defaultRepeatDelay = new TimeSpan((int)repeatDelay * 10);
            }

            if (settings.TryGetValue("GazePointer.ThresholdDelay", out var thresholdDelay))
            {
                _defaultThreshold = new TimeSpan((int)thresholdDelay * 10);
            }

            // TODO need to set fixation and dwell for all elements
            if (settings.TryGetValue("GazePointer.GazeIdleTime", out var gazeIdleTime))
            {
                EyesOffDelay = new TimeSpan((int)gazeIdleTime * 10);
            }

            if (settings.TryGetValue("GazePointer.IsSwitchEnabled", out var isSwitchEnabled))
            {
                IsSwitchEnabled = (bool)isSwitchEnabled;
            }
        }

        /// <summary>
        /// When in switch mode, will issue a click on the currently fixated element
        /// </summary>
        public void Click()
        {
            if (IsSwitchEnabled &&
                _currentlyFixatedElement != null)
            {
                _currentlyFixatedElement.Invoke();
            }
        }

        /// <summary>
        /// Run device calibration.
        /// </summary>
        /// <returns>Task that returns true, if calibration completes successfully; otherwise, false.</returns>
        public Task<bool> RequestCalibrationAsync()
        {
            return _devices.Count == 1 ?
                _devices[0].RequestCalibrationAsync().AsTask() :
                Task.FromResult(false);
        }

        /// <summary>
        /// Event handler for Gaze related events, of with a <see cref="GazeEventArgs"/> object.
        /// </summary>
        public event EventHandler<GazeEventArgs> GazeEvent
        {
            add
            {
                _gazeEventCount++;
                _gazeEvent += value;
            }

            remove
            {
                _gazeEventCount--;
                _gazeEvent -= value;
            }
        }

        /// <summary>
        /// Gets or sets the UIElement representing the cursor.
        /// </summary>
        public UIElement CursorElement
        {
            get { return _gazeCursor.PopupChild; }
            set { _gazeCursor.PopupChild = value; }
        }

        private EventHandler<GazeEventArgs> _gazeEvent;

        private int _gazeEventCount = 0;

        internal Brush EnterBrush { get; set; } = null;

        internal Brush ProgressBrush { get; set; } = new SolidColorBrush(Colors.Green);

        internal Brush CompleteBrush { get; set; } = new SolidColorBrush(Colors.Red);

        internal double DwellStrokeThickness { get; set; } = 2;

        internal Interaction Interaction { get; set; } = Interaction.Disabled;

        internal GazeTargetItem NonInvokeGazeTargetItem { get; }

        internal GazeFeedbackPopupFactory GazeFeedbackPopupFactory { get; } = new GazeFeedbackPopupFactory();

        internal void Reset()
        {
            _activeHitTargetTimes.Clear();
            _gazeHistory.Clear();

            _maxHistoryTime = DEFAULT_MAX_HISTORY_DURATION;
        }

        internal void SetElementStateDelay(UIElement element, PointerState relevantState, TimeSpan stateDelay)
        {
            var property = GetProperty(relevantState);
            element.SetValue(property, stateDelay);

            // fix up _maxHistoryTime in case the new param exceeds the history length we are currently tracking
            var dwellTime = GetElementStateDelay(element, PointerState.Dwell);
            var repeatTime = GetElementStateDelay(element, PointerState.DwellRepeat);
            _maxHistoryTime = new TimeSpan(2 * Math.Max(dwellTime.Ticks, repeatTime.Ticks));
        }

        /// <summary>
        /// Find the parent to inherit properties from.
        /// </summary>
        private static UIElement GetInheritenceParent(UIElement child)
        {
            // The result value.
            object parent = null;

            // Get the automation peer...
            var peer = FrameworkElementAutomationPeer.FromElement(child);
            if (peer != null)
            {
                // ...if it exists, get the peer's parent...
                var peerParent = peer.Navigate(AutomationNavigationDirection.Parent) as FrameworkElementAutomationPeer;
                if (peerParent != null)
                {
                    // ...and if it has a parent, get the corresponding object.
                    parent = peerParent.Owner;
                }
            }

            // If the above failed to find a parent...
            if (parent == null)
            {
                // ...use the visual parent.
                parent = VisualTreeHelper.GetParent(child);
            }

            // Safely pun the value we found to a UIElement reference.
            return parent as UIElement;
        }

        internal TimeSpan GetElementStateDelay(UIElement element, DependencyProperty property, TimeSpan defaultValue)
        {
            UIElement walker = element;
            object valueAtWalker = walker.GetValue(property);

            while (GazeInput.UnsetTimeSpan.Equals(valueAtWalker) && walker != null)
            {
                walker = GetInheritenceParent(walker);

                if (walker != null)
                {
                    valueAtWalker = walker.GetValue(property);
                }
            }

            var ticks = GazeInput.UnsetTimeSpan.Equals(valueAtWalker) ? defaultValue : (TimeSpan)valueAtWalker;

            return ticks;
        }

        internal TimeSpan GetElementStateDelay(UIElement element, PointerState pointerState)
        {
            var property = GetProperty(pointerState);
            var defaultValue = GetDefaultPropertyValue(pointerState);
            var ticks = GetElementStateDelay(element, property, defaultValue);

            switch (pointerState)
            {
                case PointerState.Dwell:
                case PointerState.DwellRepeat:
                    _maxHistoryTime = new TimeSpan(Math.Max(_maxHistoryTime.Ticks, 2 * ticks.Ticks));
                    break;
            }

            return ticks;
        }

        // Provide a configurable delay for when the EyesOffDelay event is fired
        // GOTCHA: this value requires that _eyesOffTimer is instantiated so that it
        // can update the timer interval
        internal TimeSpan EyesOffDelay
        {
            get
            {
                return _eyesOffDelay;
            }

            set
            {
                _eyesOffDelay = value;

                // convert GAZE_IDLE_TIME units (microseconds) to 100-nanosecond units used
                // by TimeSpan struct
                _eyesOffTimer.Interval = EyesOffDelay;
            }
        }

        // Pluggable filter for eye tracking sample data. This defaults to being set to the
        // NullFilter which performs no filtering of input samples.
        internal IGazeFilter Filter { get; set; }

        internal bool IsCursorVisible
        {
            get { return _gazeCursor.IsCursorVisible; }
            set { _gazeCursor.IsCursorVisible = value; }
        }

        internal int CursorRadius
        {
            get { return _gazeCursor.CursorRadius; }
            set { _gazeCursor.CursorRadius = value; }
        }

        internal bool IsSwitchEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this GazePointer should always be activated or not.
        /// </summary>
        public bool IsAlwaysActivated { get; set; }

        private static ThreadLocal<GazePointer> _instance = new ThreadLocal<GazePointer>(() => new GazePointer());

        internal static GazePointer Instance => _instance.Value;

        internal void AddRoot(int proxyId)
        {
            _roots.Insert(0, proxyId);

            if (_roots.Count == 1)
            {
                _isShuttingDown = false;
                InitializeGazeInputSource();
            }
        }

        internal void RemoveRoot(int proxyId)
        {
            int index;
            if ((index = _roots.IndexOf(proxyId)) != -1)
            {
                _roots.RemoveAt(index);
            }
            else
            {
                Debug.Assert(false, "Invalid proxyId when trying to remove a Root from the GazeProxy");
            }

            if (_roots.Count == 0)
            {
                _isShuttingDown = true;
                _gazeCursor.IsGazeEntered = false;
                DeinitializeGazeInputSource();
            }
        }

        internal bool IsDeviceAvailable
        {
            get
            {
                return _devices.Count != 0;
            }
        }

        internal event EventHandler<object> IsDeviceAvailableChanged;

        private GazePointer()
        {
            NonInvokeGazeTargetItem = new NonInvokeGazeTargetItem();

            // Default to not filtering sample data
            Filter = new NullFilter();

            _gazeCursor = new GazeCursor();

            // timer that gets called back if there gaze samples haven't been received in a while
            _eyesOffTimer = DispatcherQueue.GetForCurrentThread().CreateTimer();
            _eyesOffTimer.Tick += OnEyesOff;

            // provide a default of GAZE_IDLE_TIME microseconds to fire eyes off
            EyesOffDelay = GAZE_IDLE_TIME;

            InitializeHistogram();

            _devices = new List<GazeDevicePreview>();
            _watcher = GazeInputSourcePreview.CreateWatcher();
            _watcher.Added += OnDeviceAdded;
            _watcher.Removed += OnDeviceRemoved;
            _watcher.Start();
        }

        private bool _initialized;
        private bool _isShuttingDown;

        private static DependencyProperty GetProperty(PointerState state)
        {
            switch (state)
            {
                case PointerState.Fixation: return GazeInput.FixationDurationProperty;
                case PointerState.Dwell: return GazeInput.DwellDurationProperty;
                case PointerState.DwellRepeat: return GazeInput.DwellRepeatDurationProperty;
                case PointerState.Enter: return GazeInput.ThresholdDurationProperty;
                case PointerState.Exit: return GazeInput.ThresholdDurationProperty;
                default: return null;
            }
        }

        private TimeSpan GetDefaultPropertyValue(PointerState state)
        {
            switch (state)
            {
                case PointerState.Fixation: return _defaultFixation;
                case PointerState.Dwell: return _defaultDwell;
                case PointerState.DwellRepeat: return _defaultRepeatDelay;
                case PointerState.Enter: return _defaultThreshold;
                case PointerState.Exit: return _defaultThreshold;
                default: throw new NotImplementedException();
            }
        }

        private void InitializeHistogram()
        {
            _activeHitTargetTimes = new List<GazeTargetItem>();

            _offScreenElement = new UserControl();
            SetElementStateDelay(_offScreenElement, PointerState.Fixation, _defaultFixation);
            SetElementStateDelay(_offScreenElement, PointerState.Dwell, _defaultDwell);

            _maxHistoryTime = DEFAULT_MAX_HISTORY_DURATION;    // maintain about 3 seconds of history (in microseconds)
            _gazeHistory = new List<GazeHistoryItem>();
        }

        private void InitializeGazeInputSource()
        {
            if (!_initialized)
            {
                if (_roots.Count != 0 && _devices.Count != 0)
                {
                    if (_gazeInputSource == null)
                    {
                        _gazeInputSource = GazeInputSourcePreview.GetForCurrentView();
                    }

                    if (_gazeInputSource != null)
                    {
                        _gazeInputSource.GazeEntered += OnGazeEntered;
                        _gazeInputSource.GazeMoved += OnGazeMoved;
                        _gazeInputSource.GazeExited += OnGazeExited;

                        _initialized = true;
                    }
                }
            }
        }

        private void DeinitializeGazeInputSource()
        {
            if (_initialized)
            {
                _initialized = false;

                _gazeInputSource.GazeEntered -= OnGazeEntered;
                _gazeInputSource.GazeMoved -= OnGazeMoved;
                _gazeInputSource.GazeExited -= OnGazeExited;
            }
        }

        private void ActivateGazeTargetItem(GazeTargetItem target)
        {
            if (_activeHitTargetTimes.IndexOf(target) == -1)
            {
                _activeHitTargetTimes.Add(target);

                // calculate the time that the first DwellRepeat needs to be fired after. this will be updated every time a DwellRepeat is
                // fired to keep track of when the next one is to be fired after that.
                var nextStateTime = GetElementStateDelay(target.TargetElement, PointerState.Enter);

                target.Reset(nextStateTime);
            }
        }

        private GazeTargetItem GetHitTarget(Point gazePoint)
        {
            GazeTargetItem invokable = null;

            if (!IsAlwaysActivated)
            {
                invokable = NonInvokeGazeTargetItem;
            }

            switch (Window.Current.CoreWindow.ActivationMode)
            {
                case CoreWindowActivationMode.ActivatedInForeground:
                case CoreWindowActivationMode.ActivatedNotForeground:
                    var elements = VisualTreeHelper.FindElementsInHostCoordinates(gazePoint, null, false);
                    var element = elements.FirstOrDefault();

                    invokable = null;

                    if (element != null)
                    {
                        invokable = GazeTargetItem.GetOrCreate(element);

                        while (element != null && !invokable.IsInvokable)
                        {
                            element = VisualTreeHelper.GetParent(element) as UIElement;

                            if (element != null)
                            {
                                invokable = GazeTargetItem.GetOrCreate(element);
                            }
                        }
                    }

                    if (element == null || !invokable.IsInvokable)
                    {
                        invokable = NonInvokeGazeTargetItem;
                    }
                    else
                    {
                        Interaction interaction;
                        do
                        {
                            interaction = GazeInput.GetInteraction(element);
                            if (interaction == Interaction.Inherited)
                            {
                                element = GetInheritenceParent(element);
                            }
                        }
                        while (interaction == Interaction.Inherited && element != null);

                        if (interaction == Interaction.Inherited)
                        {
                            interaction = GazeInput.Interaction;
                        }

                        if (interaction != Interaction.Enabled)
                        {
                            invokable = NonInvokeGazeTargetItem;
                        }
                    }

                    break;
            }

            return invokable;
        }

        private GazeTargetItem ResolveHitTarget(Point gazePoint, TimeSpan timestamp)
        {
            // TODO: The existence of a GazeTargetItem should be used to indicate that
            // the target item is invokable. The method of invocation should be stored
            // within the GazeTargetItem when it is created and not recalculated when
            // subsequently needed.

            // create GazeHistoryItem to deal with this sample
            var target = GetHitTarget(gazePoint);
            GazeHistoryItem historyItem = default;
            historyItem.HitTarget = target;
            historyItem.Timestamp = timestamp;
            historyItem.Duration = TimeSpan.Zero;
            Debug.Assert(historyItem.HitTarget != null, "historyItem.HitTarget should not be null");

            // create new GazeTargetItem with a (default) total elapsed time of zero if one does not exist already.
            // this ensures that there will always be an entry for target elements in the code below.
            ActivateGazeTargetItem(target);
            target.LastTimestamp = timestamp;

            // find elapsed time since we got the last hit target
            historyItem.Duration = timestamp - _lastTimestamp;
            if (historyItem.Duration > MAX_SINGLE_SAMPLE_DURATION)
            {
                historyItem.Duration = MAX_SINGLE_SAMPLE_DURATION;
            }

            _gazeHistory.Add(historyItem);

            // update the time this particular hit target has accumulated
            target.DetailedTime += historyItem.Duration;

            // drop the oldest samples from the list until we have samples only
            // within the window we are monitoring
            //
            // historyItem is the last item we just appended a few lines above.
            for (var evOldest = _gazeHistory[0];
                historyItem.Timestamp - evOldest.Timestamp > _maxHistoryTime;
                evOldest = _gazeHistory[0])
            {
                _gazeHistory.RemoveAt(0);

                // subtract the duration obtained from the oldest sample in _gazeHistory
                var targetItem = evOldest.HitTarget;
                Debug.Assert(targetItem.DetailedTime - evOldest.Duration >= TimeSpan.Zero, "DetailedTime of targetItem should be less than oldest history Duration");
                targetItem.DetailedTime -= evOldest.Duration;
                if (targetItem.ElementState != PointerState.PreEnter)
                {
                    targetItem.OverflowTime += evOldest.Duration;
                }
            }

            _lastTimestamp = timestamp;

            // Return the most recent hit target
            // Intuition would tell us that we should return NOT the most recent
            // hitTarget, but the one with the most accumulated time in
            // in the maintained history. But the effect of that is that
            // the user will feel that they have clicked on the wrong thing
            // when they are looking at something else.
            // That is why we return the most recent hitTarget so that
            // when its dwell time has elapsed, it will be invoked
            return target;
        }

        private void CheckIfExiting(TimeSpan curTimestamp)
        {
            for (int index = 0; index < _activeHitTargetTimes.Count; index++)
            {
                var targetItem = _activeHitTargetTimes[index];
                var targetElement = targetItem.TargetElement;
                var exitDelay = GetElementStateDelay(targetElement, PointerState.Exit);

                var idleDuration = curTimestamp - targetItem.LastTimestamp;
                if (targetItem.ElementState != PointerState.PreEnter && idleDuration > exitDelay)
                {
                    targetItem.ElementState = PointerState.PreEnter;

                    // Transitioning to exit - clear the cached fixated element
                    _currentlyFixatedElement = null;

                    RaiseGazePointerEvent(targetItem, PointerState.Exit, targetItem.ElapsedTime);
                    targetItem.GiveFeedback();

                    _activeHitTargetTimes.RemoveAt(index);

                    // remove all history samples referring to deleted hit target
                    for (int i = 0; i < _gazeHistory.Count;)
                    {
                        var hitTarget = _gazeHistory[i].HitTarget;
                        if (hitTarget.TargetElement == targetElement)
                        {
                            _gazeHistory.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }
                    }

                    // return because only one element can be exited at a time and at this point
                    // we have done everything that we can do
                    return;
                }
            }
        }

        /*
        private const string[] pointerStates =
        {
            "Exit",
            "PreEnter",
            "Enter",
            "Fixation",
            "Dwel",
            "DwellRepeat"
        };
        */

        private void RaiseGazePointerEvent(GazeTargetItem target, PointerState state, TimeSpan elapsedTime)
        {
            var control = target?.TargetElement;

            // assert(target != _rootElement);
            var gpea = new StateChangedEventArgs(control, state, elapsedTime);

            /*
             * if (control is Button buttonObj && buttonObj.Content != null)
             * {
             *     var buttonText = buttonObj.Content as string;
             *     Debug.WriteLine("GPE: {0} . {1}, {2}", buttonText, PointerStates[(int)state], elapsedTime);
             * }
             * else
             * {
             *     Debug.WriteLine("GPE: 0x%08x . %s, %d", target != null ? target.GetHashCode() : 0, PointerStates[(int)state], elapsedTime);
             * }
             */

            var gazeElement = target != null ? GazeInput.GetGazeElement(control) : null;

            if (gazeElement != null)
            {
                gazeElement.RaiseStateChanged(control, gpea);
            }

            if (state == PointerState.Dwell)
            {
                var handled = false;

                if (gazeElement != null)
                {
                    var args = new DwellInvokedRoutedEventArgs();
                    gazeElement.RaiseInvoked(control, args);
                    handled = args.Handled;
                }

                if (!handled)
                {
                    target.Invoke();
                }
            }
        }

        private void OnGazeEntered(
            GazeInputSourcePreview provider,
            GazeEnteredPreviewEventArgs args)
        {
            // Debug.WriteLine("Entered at %ld", args.CurrentPoint.Timestamp);
            _gazeCursor.IsGazeEntered = true;
        }

        private void OnGazeMoved(
            GazeInputSourcePreview provider,
            GazeMovedPreviewEventArgs args)
        {
            if (!_isShuttingDown)
            {
                var intermediatePoints = args.GetIntermediatePoints();
                foreach (var point in intermediatePoints)
                {
                    var position = point.EyeGazePosition;
                    if (position != null)
                    {
                        _gazeCursor.IsGazeEntered = true;
                        ProcessGazePoint(new TimeSpan((long)point.Timestamp * 10), position.Value);
                    }
                    else
                    {
                        // Debug.WriteLine("Null position eaten at %ld", point.Timestamp);
                    }
                }
            }
        }

        private void OnGazeExited(
            GazeInputSourcePreview provider,
            GazeExitedPreviewEventArgs args)
        {
            // Debug.WriteLine("Exited at %ld", args.CurrentPoint.Timestamp);
            _gazeCursor.IsGazeEntered = false;
        }

        private void ProcessGazePoint(TimeSpan timestamp, Point position)
        {
            var ea = new GazeFilterArgs(position, timestamp);

            var fa = Filter.Update(ea);
            _gazeCursor.Position = fa.Location;

            if (_gazeEventCount != 0)
            {
                _gazeEventArgs.Set(fa.Location, timestamp);
                _gazeEvent?.Invoke(this, _gazeEventArgs);
                if (_gazeEventArgs.Handled)
                {
                    return;
                }
            }

            var targetItem = ResolveHitTarget(fa.Location, fa.Timestamp);
            Debug.Assert(targetItem != null, "targetItem is null when processing gaze point");

            // Debug.WriteLine("ProcessGazePoint. [{0}, {1}], {2}", (int)fa.Location.X, (int)fa.Location.Y, fa.Timestamp);

            // check to see if any element in _hitTargetTimes needs an exit event fired.
            // this ensures that all exit events are fired before enter event
            CheckIfExiting(fa.Timestamp);

            PointerState nextState = (PointerState)((int)targetItem.ElementState + 1);

            // Debug.WriteLine(targetItem.TargetElement.ToString());
            // Debug.WriteLine("\tState={0}, Elapsed={1}, NextStateTime={2}", targetItem.ElementState, targetItem.ElapsedTime, targetItem.NextStateTime);
            if (targetItem.ElapsedTime > targetItem.NextStateTime)
            {
                var prevStateTime = targetItem.NextStateTime;
                ////Debug.WriteLine(prevStateTime);

                // prevent targetItem from ever actually transitioning into the DwellRepeat state so as
                // to continuously emit the DwellRepeat event
                if (nextState != PointerState.DwellRepeat)
                {
                    targetItem.ElementState = nextState;
                    nextState = (PointerState)((int)nextState + 1);     // nextState++
                    targetItem.NextStateTime += GetElementStateDelay(targetItem.TargetElement, nextState);

                    if (targetItem.ElementState == PointerState.Dwell)
                    {
                        targetItem.NextStateTime += GetElementStateDelay(targetItem.TargetElement, GazeInput.RepeatDelayDurationProperty, _defaultDwellRepeatDelay);
                    }
                }
                else
                {
                    // move the NextStateTime by one dwell period, while continuing to stay in Dwell state
                    targetItem.NextStateTime += GetElementStateDelay(targetItem.TargetElement, PointerState.DwellRepeat);
                }

                if (targetItem.ElementState == PointerState.Dwell)
                {
                    targetItem.RepeatCount++;
                    if (targetItem.MaxDwellRepeatCount < targetItem.RepeatCount)
                    {
                        targetItem.NextStateTime = new TimeSpan(long.MaxValue);
                    }
                }

                if (targetItem.ElementState == PointerState.Fixation)
                {
                    // Cache the fixated item
                    _currentlyFixatedElement = targetItem;

                    // We are about to transition into the Dwell state
                    // If switch input is enabled, make sure dwell never completes
                    // via eye gaze
                    if (IsSwitchEnabled)
                    {
                        // Don't allow the next state (Dwell) to progress
                        targetItem.NextStateTime = new TimeSpan(long.MaxValue);
                    }
                }

                RaiseGazePointerEvent(targetItem, targetItem.ElementState, targetItem.ElapsedTime);
            }

            targetItem.GiveFeedback();

            _eyesOffTimer.Start();
            _lastTimestamp = fa.Timestamp;
        }

        private void OnEyesOff(object sender, object ea)
        {
            _eyesOffTimer.Stop();

            CheckIfExiting(_lastTimestamp + EyesOffDelay);
            RaiseGazePointerEvent(null, PointerState.Enter, EyesOffDelay);
        }

        private void OnDeviceAdded(GazeDeviceWatcherPreview sender, GazeDeviceWatcherAddedPreviewEventArgs args)
        {
            _devices.Add(args.Device);

            if (_devices.Count == 1)
            {
                IsDeviceAvailableChanged?.Invoke(null, null);

                InitializeGazeInputSource();
            }
        }

        private void OnDeviceRemoved(GazeDeviceWatcherPreview sender, GazeDeviceWatcherRemovedPreviewEventArgs args)
        {
            var index = 0;
            while (index < _devices.Count && _devices.ElementAt(index).Id != args.Device.Id)
            {
                index++;
            }

            if (index < _devices.Count)
            {
                _devices.RemoveAt(index);
            }
            else
            {
                _devices.RemoveAt(0);
            }

            if (_devices.Count == 0)
            {
                IsDeviceAvailableChanged?.Invoke(null, null);
            }
        }

        private readonly List<int> _roots = new List<int>();

        private readonly DispatcherQueueTimer _eyesOffTimer;

        private readonly GazeCursor _gazeCursor;

        private readonly GazeDeviceWatcherPreview _watcher;
        private readonly List<GazeDevicePreview> _devices;

        private readonly GazeEventArgs _gazeEventArgs = new GazeEventArgs();

        private TimeSpan _eyesOffDelay;

        // _offScreenElement is a pseudo-element that represents the area outside
        // the screen so we can track how long the user has been looking outside
        // the screen and appropriately trigger the EyesOff event
        private Control _offScreenElement;

        // The value is the total time that FrameworkElement has been gazed at
        private List<GazeTargetItem> _activeHitTargetTimes;

        // A vector to track the history of observed gaze targets
        private List<GazeHistoryItem> _gazeHistory;
        private TimeSpan _maxHistoryTime;

        // Used to determine if exit events need to be fired by adding GAZE_IDLE_TIME to the last
        // saved timestamp
        private TimeSpan _lastTimestamp;

        private GazeInputSourcePreview _gazeInputSource;

        private TimeSpan _defaultFixation = DEFAULT_FIXATION_DELAY;
        private TimeSpan _defaultDwell = DEFAULT_DWELL_DELAY;
        private TimeSpan _defaultDwellRepeatDelay = DEFAULT_DWELL_REPEAT_DELAY;
        private TimeSpan _defaultRepeatDelay = DEFAULT_REPEAT_DELAY;
        private TimeSpan _defaultThreshold = DEFAULT_THRESHOLD_DELAY;
        private GazeTargetItem _currentlyFixatedElement;
    }
}