// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.Foundation;

namespace CommunityToolkit.WinUI.Input.GazeInteraction
{
    internal abstract class GazeTargetItem
    {
        private static readonly DependencyProperty _gazeTargetItemProperty = DependencyProperty.RegisterAttached("_GazeTargetItem", typeof(GazeTargetItem), typeof(GazeTargetItem), new PropertyMetadata(null));

        internal TimeSpan DetailedTime { get; set; }

        internal TimeSpan OverflowTime { get; set; }

        internal TimeSpan ElapsedTime
        {
            get { return DetailedTime + OverflowTime; }
        }

        internal TimeSpan NextStateTime { get; set; }

        internal TimeSpan LastTimestamp { get; set; }

        internal PointerState ElementState { get; set; }

        internal UIElement TargetElement { get; set; }

        internal int RepeatCount { get; set; }

        internal int MaxDwellRepeatCount { get; set; }

        internal GazeTargetItem(UIElement target)
        {
            TargetElement = target;
        }

        internal static GazeTargetItem GetOrCreate(UIElement element)
        {
            GazeTargetItem item;

            var value = element.ReadLocalValue(_gazeTargetItemProperty);

            if (value != DependencyProperty.UnsetValue)
            {
                item = (GazeTargetItem)value;
            }
            else
            {
                var peer = FrameworkElementAutomationPeer.FromElement(element);

                if (peer == null)
                {
                    item = GazePointer.Instance.NonInvokeGazeTargetItem;
                }
                else if (peer.GetPattern(PatternInterface.Invoke) is IInvokeProvider)
                {
                    item = new InvokePatternGazeTargetItem(element);
                }
                else if (peer.GetPattern(PatternInterface.Toggle) is IToggleProvider)
                {
                    item = new TogglePatternGazeTargetItem(element);
                }
                else if (peer.GetPattern(PatternInterface.SelectionItem) is ISelectionItemProvider)
                {
                    item = new SelectionItemPatternGazeTargetItem(element);
                }
                else if (peer.GetPattern(PatternInterface.ExpandCollapse) is IExpandCollapseProvider)
                {
                    item = new ExpandCollapsePatternGazeTargetItem(element);
                }
                else if (peer is ComboBoxItemAutomationPeer)
                {
                    item = new ComboBoxItemGazeTargetItem(element);
                }
                else
                {
                    item = GazePointer.Instance.NonInvokeGazeTargetItem;
                }

                element.SetValue(_gazeTargetItemProperty, item);
            }

            return item;
        }

        internal abstract void Invoke();

        internal virtual bool IsInvokable
        {
            get { return true; }
        }

        internal void Reset(TimeSpan nextStateTime)
        {
            ElementState = PointerState.PreEnter;
            DetailedTime = TimeSpan.Zero;
            OverflowTime = TimeSpan.Zero;
            NextStateTime = nextStateTime;
            RepeatCount = 0;
            MaxDwellRepeatCount = GazeInput.GetMaxDwellRepeatCount(TargetElement);
        }

        internal void GiveFeedback()
        {
            if (_nextStateTime != NextStateTime)
            {
                _prevStateTime = _nextStateTime;
                _nextStateTime = NextStateTime;
            }

            if (ElementState != _notifiedPointerState)
            {
                switch (ElementState)
                {
                    case PointerState.Enter:
                        RaiseProgressEvent(DwellProgressState.Fixating);
                        break;

                    case PointerState.Dwell:
                    case PointerState.Fixation:
                        RaiseProgressEvent(DwellProgressState.Progressing);
                        break;

                    case PointerState.Exit:
                    case PointerState.PreEnter:
                        RaiseProgressEvent(DwellProgressState.Idle);
                        break;
                }

                _notifiedPointerState = ElementState;
            }
            else if (ElementState == PointerState.Dwell || ElementState == PointerState.Fixation)
            {
                if (RepeatCount <= MaxDwellRepeatCount)
                {
                    RaiseProgressEvent(DwellProgressState.Progressing);
                }
                else
                {
                    RaiseProgressEvent(DwellProgressState.Complete);
                }
            }
        }

        private void RaiseProgressEvent(DwellProgressState state)
        {
            // TODO: We should eliminate non-invokable controls before we arrive here!
            if (TargetElement is Page)
            {
                return;
            }

            if (_notifiedProgressState != state || state == DwellProgressState.Progressing)
            {
                var handled = false;

                var gazeElement = GazeInput.GetGazeElement(TargetElement);
                if (gazeElement != null)
                {
                    handled = gazeElement.RaiseProgressFeedback(TargetElement, state, ElapsedTime - _prevStateTime, _nextStateTime - _prevStateTime);
                }

                if (!handled && state != DwellProgressState.Idle)
                {
                    if (_feedbackPopup == null)
                    {
                        _feedbackPopup = GazePointer.Instance.GazeFeedbackPopupFactory.Get();
                    }

                    var control = TargetElement as FrameworkElement;

                    var transform = control.TransformToVisual(_feedbackPopup);
                    var bounds = transform.TransformBounds(new Rect(
                        new Point(0, 0),
                        new Size((float)control.ActualWidth, (float)control.ActualHeight)));
                    var rectangle = (Microsoft.UI.Xaml.Shapes.Rectangle)_feedbackPopup.Child;

                    if (state == DwellProgressState.Progressing)
                    {
                        var progress = ((double)(ElapsedTime - _prevStateTime).Ticks) / (_nextStateTime - _prevStateTime).Ticks;

                        if (progress >= 0 && progress < 1)
                        {
                            rectangle.Stroke = GazeInput.DwellFeedbackProgressBrush;
                            rectangle.Width = (1 - progress) * bounds.Width;
                            rectangle.Height = (1 - progress) * bounds.Height;

                            _feedbackPopup.HorizontalOffset = bounds.Left + (progress * bounds.Width / 2);
                            _feedbackPopup.VerticalOffset = bounds.Top + (progress * bounds.Height / 2);
                        }
                    }
                    else
                    {
                        rectangle.Stroke = state == DwellProgressState.Fixating ?
                            GazeInput.DwellFeedbackEnterBrush : GazeInput.DwellFeedbackCompleteBrush;
                        rectangle.Width = bounds.Width;
                        rectangle.Height = bounds.Height;

                        _feedbackPopup.HorizontalOffset = bounds.Left;
                        _feedbackPopup.VerticalOffset = bounds.Top;
                    }

                    _feedbackPopup.IsOpen = true;
                }
                else
                {
                    if (_feedbackPopup != null)
                    {
                        GazePointer.Instance.GazeFeedbackPopupFactory.Return(_feedbackPopup);
                        _feedbackPopup = null;
                    }
                }
            }

            _notifiedProgressState = state;
        }

        private PointerState _notifiedPointerState = PointerState.Exit;
        private TimeSpan _prevStateTime;
        private TimeSpan _nextStateTime;
        private DwellProgressState _notifiedProgressState = DwellProgressState.Idle;
        private Popup _feedbackPopup;
    }
}