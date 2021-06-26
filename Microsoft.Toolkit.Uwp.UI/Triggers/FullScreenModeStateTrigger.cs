// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.UI.Triggers
{
    /// <summary>
    /// Trigger for switching when in full screen mode.
    /// </summary>
    public class FullScreenModeStateTrigger : StateTriggerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FullScreenModeStateTrigger"/> class.
        /// </summary>
        public FullScreenModeStateTrigger()
        {
            if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                var weakEvent =
                    new WeakEventListener<FullScreenModeStateTrigger, ApplicationView, object>(this)
                    {
                        OnEventAction = (instance, source, eventArgs) => instance.FullScreenModeTrigger_VisibleBoundsChanged(source, eventArgs),
                        OnDetachAction = (weakEventListener) => ApplicationView.GetForCurrentView().VisibleBoundsChanged -= weakEventListener.OnEvent
                    };
                ApplicationView.GetForCurrentView().VisibleBoundsChanged += weakEvent.OnEvent;
            }
        }

        private void FullScreenModeTrigger_VisibleBoundsChanged(ApplicationView sender, object args) => UpdateTrigger(sender.IsFullScreenMode);

        private bool _isFullScreen;

        /// <summary>
        /// Gets or sets a value indicating whether to trigger on full screen or not.
        /// </summary>
        public bool IsFullScreen
        {
            get => _isFullScreen;
            set
            {
                _isFullScreen = value;
                if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
                {
                    var isFullScreenMode = ApplicationView.GetForCurrentView().IsFullScreenMode;
                    UpdateTrigger(isFullScreenMode);
                }
            }
        }

        private void UpdateTrigger(bool isFullScreenMode) => SetActive(IsFullScreen == isFullScreenMode);
    }
}