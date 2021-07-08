// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation.Collections;

namespace CommunityToolkit.WinUI.Input.GazeInteraction
{
    /// <summary>
    /// Static class primarily providing access to attached properties controlling gaze behavior.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public class GazeInput
    {
        internal static readonly TimeSpan UnsetTimeSpan = new TimeSpan(-1);

        /// <summary>
        /// Gets the Interaction dependency property
        /// </summary>
        public static DependencyProperty InteractionProperty { get; } = DependencyProperty.RegisterAttached("Interaction", typeof(Interaction), typeof(GazeInput), new PropertyMetadata(Interaction.Inherited, new PropertyChangedCallback(OnInteractionChanged)));

        private static void OnInteractionChanged(DependencyObject ob, DependencyPropertyChangedEventArgs args)
        {
            var element = ob as FrameworkElement;
            var interaction = (Interaction)args.NewValue;
            GazePointerProxy.SetInteraction(element, interaction);
        }

        /// <summary>
        /// Gets the IsCursorVisible dependency property
        /// </summary>
        public static DependencyProperty IsCursorVisibleProperty { get; } = DependencyProperty.RegisterAttached("IsCursorVisible", typeof(bool), typeof(GazeInput), new PropertyMetadata(true, new PropertyChangedCallback(OnIsCursorVisibleChanged)));

        private static void OnIsCursorVisibleChanged(DependencyObject ob, DependencyPropertyChangedEventArgs args)
        {
            GazePointer.Instance.IsCursorVisible = (bool)args.NewValue;
        }

        /// <summary>
        /// Gets the CursorRadius dependency property
        /// </summary>
        public static DependencyProperty CursorRadiusProperty { get; } = DependencyProperty.RegisterAttached("CursorRadius", typeof(int), typeof(GazeInput), new PropertyMetadata(6, new PropertyChangedCallback(OnCursorRadiusChanged)));

        private static void OnCursorRadiusChanged(DependencyObject ob, DependencyPropertyChangedEventArgs args)
        {
            GazePointer.Instance.CursorRadius = (int)args.NewValue;
        }

        /// <summary>
        /// Gets the GazeElement dependency property
        /// </summary>
        public static DependencyProperty GazeElementProperty { get; } = DependencyProperty.RegisterAttached("GazeElement", typeof(GazeElement), typeof(GazeInput), new PropertyMetadata(null));

        /// <summary>
        /// Gets the FixationDuration dependency property
        /// </summary>
        public static DependencyProperty FixationDurationProperty { get; } = DependencyProperty.RegisterAttached("FixationDuration", typeof(TimeSpan), typeof(GazeInput), new PropertyMetadata(UnsetTimeSpan));

        /// <summary>
        /// Gets the DwellDuration dependency property
        /// </summary>
        public static DependencyProperty DwellDurationProperty { get; } = DependencyProperty.RegisterAttached("DwellDuration", typeof(TimeSpan), typeof(GazeInput), new PropertyMetadata(UnsetTimeSpan));

        /// <summary>
        /// Gets the RepeatDelayDuration dependency property
        /// </summary>
        public static DependencyProperty RepeatDelayDurationProperty { get; } = DependencyProperty.RegisterAttached("RepeatDelayDuration", typeof(TimeSpan), typeof(GazeInput), new PropertyMetadata(UnsetTimeSpan));

        /// <summary>
        /// Gets the DwellRepeatDuration dependency property
        /// </summary>
        public static DependencyProperty DwellRepeatDurationProperty { get; } = DependencyProperty.RegisterAttached("DwellRepeatDuration", typeof(TimeSpan), typeof(GazeInput), new PropertyMetadata(UnsetTimeSpan));

        /// <summary>
        /// Gets the ThresholdDuration dependency property
        /// </summary>
        public static DependencyProperty ThresholdDurationProperty { get; } = DependencyProperty.RegisterAttached("ThresholdDuration", typeof(TimeSpan), typeof(GazeInput), new PropertyMetadata(UnsetTimeSpan));

        /// <summary>
        /// Gets the MaxDwellRepeatCount dependency property
        /// </summary>
        public static DependencyProperty MaxDwellRepeatCountProperty { get; } = DependencyProperty.RegisterAttached("MaxDwellRepeatCount", typeof(int), typeof(GazeInput), new PropertyMetadata(0));

        /// <summary>
        /// Gets the IsSwitchEnabled dependency property
        /// </summary>
        public static DependencyProperty IsSwitchEnabledProperty { get; } = DependencyProperty.RegisterAttached("IsSwitchEnabled", typeof(bool), typeof(GazeInput), new PropertyMetadata(false, new PropertyChangedCallback(OnIsSwitchEnabledChanged)));

        private static void OnIsSwitchEnabledChanged(DependencyObject ob, DependencyPropertyChangedEventArgs args)
        {
            GazePointer.Instance.IsSwitchEnabled = (bool)args.NewValue;
        }

        /// <summary>
        /// Gets or sets the brush to use when displaying the default indication that gaze entered a control
        /// </summary>
        public static Brush DwellFeedbackEnterBrush
        {
            get
            {
                return GazePointer.Instance.EnterBrush;
            }

            set
            {
                GazePointer.Instance.EnterBrush = value;
            }
        }

        /// <summary>
        /// Gets or sets the brush to use when displaying the default animation for dwell press
        /// </summary>
        public static Brush DwellFeedbackProgressBrush
        {
            get
            {
                return GazePointer.Instance.ProgressBrush;
            }

            set
            {
                GazePointer.Instance.ProgressBrush = value;
            }
        }

        /// <summary>
        /// Gets or sets the brush to use when displaying the default animation for dwell complete
        /// </summary>
        public static Brush DwellFeedbackCompleteBrush
        {
            get
            {
                return GazePointer.Instance.CompleteBrush;
            }

            set
            {
                GazePointer.Instance.CompleteBrush = value;
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the lines animated for dwell.
        /// </summary>
        public static double DwellStrokeThickness
        {
            get
            {
                return GazePointer.Instance.DwellStrokeThickness;
            }

            set
            {
                GazePointer.Instance.DwellStrokeThickness = value;
            }
        }

        /// <summary>
        /// Gets or sets the interaction default
        /// </summary>
        public static Interaction Interaction
        {
            get
            {
                return GazePointer.Instance.Interaction;
            }

            set
            {
                if (GazePointer.Instance.Interaction != value)
                {
                    if (value == Interaction.Enabled)
                    {
                        GazePointer.Instance.AddRoot(0);
                    }
                    else if (GazePointer.Instance.Interaction == Interaction.Enabled)
                    {
                        GazePointer.Instance.RemoveRoot(0);
                    }

                    GazePointer.Instance.Interaction = value;
                }
            }
        }

        /// <summary>
        /// Gets the status of gaze interaction over that particular XAML element.
        /// </summary>
        /// <returns>The status of gaze interaction over that particular XAML element.</returns>
        public static Interaction GetInteraction(UIElement element)
        {
            return (Interaction)element.GetValue(InteractionProperty);
        }

        /// <summary>
        /// Gets a boolean indicating whether cursor is shown while user is looking at the school.
        /// </summary>
        /// <returns>True the cursor is shown while user is looking at the school; otherwise, false.</returns>
        public static bool GetIsCursorVisible(UIElement element)
        {
            return (bool)element.GetValue(IsCursorVisibleProperty);
        }

        /// <summary>
        /// Gets the size of the gaze cursor radius.
        /// </summary>
        /// <returns>The size of the gaze cursor radius.</returns>
        public static int GetCursorRadius(UIElement element)
        {
            return (int)element.GetValue(CursorRadiusProperty);
        }

        /// <summary>
        /// Gets the GazeElement associated with an UIElement.
        /// </summary>
        /// <returns>The GazeElement associated with an UIElement.</returns>
        public static GazeElement GetGazeElement(UIElement element)
        {
            return (GazeElement)element.GetValue(GazeElementProperty);
        }

        /// <summary>
        /// Gets the duration for the control to transition from the Enter state to the Fixation state. At this point, a StateChanged event is fired with PointerState set to Fixation. This event should be used to control the earliest visual feedback the application needs to provide to the user about the gaze location. The default is 350ms.
        /// </summary>
        /// <returns>Duration for the control to transition from the Enter state to the Fixation state.</returns>
        public static TimeSpan GetFixationDuration(UIElement element)
        {
            return (TimeSpan)element.GetValue(FixationDurationProperty);
        }

        /// <summary>
        /// Gets the duration for the control to transition from the Fixation state to the Dwell state. At this point, a StateChanged event is fired with PointerState set to Dwell. The Enter and Fixation states are typically achieved too rapidly for the user to have much control over. In contrast Dwell is conscious event. This is the point at which the control is invoked, e.g. a button click. The application can modify this property to control when a gaze enabled UI element gets invoked after a user starts looking at it.
        /// </summary>
        /// <returns>The duration for the control to transition from the Fixation state to the Dwell state.</returns>
        public static TimeSpan GetDwellDuration(UIElement element)
        {
            return (TimeSpan)element.GetValue(DwellDurationProperty);
        }

        /// <summary>
        /// Gets the additional duration for the first repeat to occur. This prevents inadvertent repeated invocation.
        /// </summary>
        /// <returns>The additional duration for the first repeat to occur.</returns>
        public static TimeSpan GetRepeatDelayDuration(UIElement element)
        {
            return (TimeSpan)element.GetValue(RepeatDelayDurationProperty);
        }

        /// <summary>
        /// Gets the duration of repeated dwell invocations, should the user continue to dwell on the control. The first repeat will occur after an additional delay specified by RepeatDelayDuration. Subsequent repeats happen after every period of DwellRepeatDuration. A control is invoked repeatedly only if MaxDwellRepeatCount is set to greater than zero.
        /// </summary>
        /// <returns>The duration of repeated dwell invocations.</returns>
        public static TimeSpan GetDwellRepeatDuration(UIElement element)
        {
            return (TimeSpan)element.GetValue(DwellRepeatDurationProperty);
        }

        /// <summary>
        /// Gets the duration that controls when the PointerState moves to either the Enter state or the Exit state. When this duration has elapsed after the user's gaze first enters a control, the PointerState is set to Enter. And when this duration has elapsed after the user's gaze has left the control, the PointerState is set to Exit. In both cases, a StateChanged event is fired. The default is 50ms.
        /// </summary>
        /// <returns>The duration that controls when the PointerState moves to either the Enter state or the Exit state.</returns>
        public static TimeSpan GetThresholdDuration(UIElement element)
        {
            return (TimeSpan)element.GetValue(ThresholdDurationProperty);
        }

        /// <summary>
        /// Gets the maximum times the control will invoked repeatedly without the user's gaze having to leave and re-enter the control. The default value is zero which disables repeated invocation of a control. Developers can set a higher value to enable repeated invocation.
        /// </summary>
        /// <returns>The maximum times the control will invoked repeatedly without the user's gaze having to leave and re-enter the control.</returns>
        public static int GetMaxDwellRepeatCount(UIElement element)
        {
            return (int)element.GetValue(MaxDwellRepeatCountProperty);
        }

        /// <summary>
        /// Gets a boolean indicating whether gaze plus switch is enabled.
        /// </summary>
        /// <returns>A boolean indicating whether gaze plus switch is enabled.</returns>
        public static bool GetIsSwitchEnabled(UIElement element)
        {
            return (bool)element.GetValue(IsSwitchEnabledProperty);
        }

        /// <summary>
        /// Sets the status of gaze interaction over that particular XAML element.
        /// </summary>
        public static void SetInteraction(UIElement element, Interaction value)
        {
            element.SetValue(InteractionProperty, value);
        }

        /// <summary>
        /// Sets a boolean indicating whether cursor is shown while user is looking at the school.
        /// </summary>
        public static void SetIsCursorVisible(UIElement element, bool value)
        {
            element.SetValue(IsCursorVisibleProperty, value);
        }

        /// <summary>
        /// Sets the size of the gaze cursor radius.
        /// </summary>
        public static void SetCursorRadius(UIElement element, int value)
        {
            element.SetValue(CursorRadiusProperty, value);
        }

        /// <summary>
        /// Sets the GazeElement associated with an UIElement.
        /// </summary>
        public static void SetGazeElement(UIElement element, GazeElement value)
        {
            element.SetValue(GazeElementProperty, value);
        }

        /// <summary>
        /// Sets the duration for the control to transition from the Enter state to the Fixation state. At this point, a StateChanged event is fired with PointerState set to Fixation. This event should be used to control the earliest visual feedback the application needs to provide to the user about the gaze location. The default is 350ms.
        /// </summary>
        public static void SetFixationDuration(UIElement element, TimeSpan span)
        {
            element.SetValue(FixationDurationProperty, span);
        }

        /// <summary>
        /// Sets the duration for the control to transition from the Fixation state to the Dwell state. At this point, a StateChanged event is fired with PointerState set to Dwell. The Enter and Fixation states are typically achieved too rapidly for the user to have much control over. In contrast Dwell is conscious event. This is the point at which the control is invoked, e.g. a button click. The application can modify this property to control when a gaze enabled UI element gets invoked after a user starts looking at it.
        /// </summary>
        public static void SetDwellDuration(UIElement element, TimeSpan span)
        {
            element.SetValue(DwellDurationProperty, span);
        }

        /// <summary>
        /// Sets the additional duration for the first repeat to occur.This prevents inadvertent repeated invocation.
        /// </summary>
        public static void SetRepeatDelayDuration(UIElement element, TimeSpan span)
        {
            element.SetValue(RepeatDelayDurationProperty, span);
        }

        /// <summary>
        /// Sets the duration of repeated dwell invocations, should the user continue to dwell on the control. The first repeat will occur after an additional delay specified by RepeatDelayDuration. Subsequent repeats happen after every period of DwellRepeatDuration. A control is invoked repeatedly only if MaxDwellRepeatCount is set to greater than zero.
        /// </summary>
        public static void SetDwellRepeatDuration(UIElement element, TimeSpan span)
        {
            element.SetValue(DwellRepeatDurationProperty, span);
        }

        /// <summary>
        /// Sets the duration that controls when the PointerState moves to either the Enter state or the Exit state. When this duration has elapsed after the user's gaze first enters a control, the PointerState is set to Enter. And when this duration has elapsed after the user's gaze has left the control, the PointerState is set to Exit. In both cases, a StateChanged event is fired. The default is 50ms.
        /// </summary>
        public static void SetThresholdDuration(UIElement element, TimeSpan span)
        {
            element.SetValue(ThresholdDurationProperty, span);
        }

        /// <summary>
        /// Sets the maximum times the control will invoked repeatedly without the user's gaze having to leave and re-enter the control. The default value is zero which disables repeated invocation of a control. Developers can set a higher value to enable repeated invocation.
        /// </summary>
        public static void SetMaxDwellRepeatCount(UIElement element, int value)
        {
            element.SetValue(MaxDwellRepeatCountProperty, value);
        }

        /// <summary>
        /// Sets the boolean indicating whether gaze plus switch is enabled.
        /// </summary>
        public static void SetIsSwitchEnabled(UIElement element, bool value)
        {
            element.SetValue(IsSwitchEnabledProperty, value);
        }

        /// <summary>
        /// Gets the GazePointer object.
        /// </summary>
        /// <returns>The GazePointer associated with that particular page.</returns>
        public static GazePointer GetGazePointer(Page page)
        {
            return GazePointer.Instance;
        }

        /// <summary>
        /// Invoke the default action of the specified UIElement.
        /// </summary>
        public static void Invoke(UIElement element)
        {
            var item = GazeTargetItem.GetOrCreate(element);
            item.Invoke();
        }

        /// <summary>
        /// Gets a value indicating whether a gaze input device is available, and hence whether there is any possibility of gaze events occurring in the application.
        /// </summary>
        public static bool IsDeviceAvailable
        {
            get
            {
                return GazePointer.Instance.IsDeviceAvailable;
            }
        }

        /// <summary>
        /// Event triggered whenever IsDeviceAvailable changes value.
        /// </summary>
        public static event EventHandler<object> IsDeviceAvailableChanged
        {
            add
            {
                GazePointer.Instance.IsDeviceAvailableChanged += value;
            }

            remove
            {
                GazePointer.Instance.IsDeviceAvailableChanged -= value;
            }
        }

        /// <summary>
        /// Loads a settings collection into GazeInput.
        /// Note: This must be loaded from a UI thread to be valid, since the GazeInput
        /// instance is tied to the UI thread.
        /// </summary>
        public static void LoadSettings(ValueSet settings)
        {
            GazePointer.Instance.LoadSettings(settings);
        }
    }
}