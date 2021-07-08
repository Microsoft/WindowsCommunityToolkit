// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;

// TODO: Fix tests for WinUI3
// [assembly: InternalsVisibleTo("UnitTests.XamlIslands.UWPApp")]
namespace CommunityToolkit.WinUI.UI.Helpers
{
    /// <summary>
    /// The Delegate for a ThemeChanged Event.
    /// </summary>
    /// <param name="sender">Sender ThemeListener</param>
    public delegate void ThemeChangedEvent(ThemeListener sender);

    /// <summary>
    /// Class which listens for changes to Application Theme or High Contrast Modes
    /// and Signals an Event when they occur.
    /// </summary>
    [AllowForWeb]
    public sealed class ThemeListener : IDisposable
    {
        /// <summary>
        /// Gets the Name of the Current Theme.
        /// </summary>
        public string CurrentThemeName
        {
            get { return this.CurrentTheme.ToString(); }
        }

        /// <summary>
        /// Gets or sets the Current Theme.
        /// </summary>
        public ApplicationTheme CurrentTheme { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the current theme is high contrast.
        /// </summary>
        public bool IsHighContrast { get; set; }

        /// <summary>
        /// Gets or sets which DispatcherQueue is used to dispatch UI updates.
        /// </summary>
        public DispatcherQueue DispatcherQueue { get; set; }

        /// <summary>
        /// An event that fires if the Theme changes.
        /// </summary>
        public event ThemeChangedEvent ThemeChanged;

        private AccessibilitySettings _accessible = new AccessibilitySettings();
        private UISettings _settings = new UISettings();

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeListener"/> class.
        /// </summary>
        /// <param name="dispatcherQueue">The DispatcherQueue that should be used to dispatch UI updates, or null if this is being called from the UI thread.</param>
        public ThemeListener(DispatcherQueue dispatcherQueue = null)
        {
            CurrentTheme = Application.Current.RequestedTheme;
            IsHighContrast = _accessible.HighContrast;

            DispatcherQueue = dispatcherQueue ?? DispatcherQueue.GetForCurrentThread();

            if (Window.Current != null)
            {
                _accessible.HighContrastChanged += Accessible_HighContrastChanged;
                _settings.ColorValuesChanged += Settings_ColorValuesChanged;

                Window.Current.CoreWindow.Activated += CoreWindow_Activated;
            }
        }

        private void Accessible_HighContrastChanged(AccessibilitySettings sender, object args)
        {
#if DEBUG
            global::System.Diagnostics.Debug.WriteLine("HighContrast Changed");
#endif

            UpdateProperties();
        }

        // Note: This can get called multiple times during HighContrast switch, do we care?
        private async void Settings_ColorValuesChanged(UISettings sender, object args)
        {
            await OnColorValuesChanged();
        }

        // Internal abstraction is used by the Unit Tests
        internal Task OnColorValuesChanged()
        {
            // Getting called off thread, so we need to dispatch to request value.
            return DispatcherQueue.EnqueueAsync(
                () =>
                {
                    // TODO: This doesn't stop the multiple calls if we're in our faked 'White' HighContrast Mode below.
                    if (CurrentTheme != Application.Current.RequestedTheme ||
                        IsHighContrast != _accessible.HighContrast)
                    {
#if DEBUG
                        global::System.Diagnostics.Debug.WriteLine("Color Values Changed");
#endif

                        UpdateProperties();
                    }
                }, DispatcherQueuePriority.Normal);
        }

        private void CoreWindow_Activated(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.WindowActivatedEventArgs args)
        {
            if (CurrentTheme != Application.Current.RequestedTheme ||
                IsHighContrast != _accessible.HighContrast)
            {
#if DEBUG
                global::System.Diagnostics.Debug.WriteLine("CoreWindow Activated Changed");
#endif

                UpdateProperties();
            }
        }

        /// <summary>
        /// Set our current properties and fire a change notification.
        /// </summary>
        private void UpdateProperties()
        {
            // TODO: Not sure if HighContrastScheme names are localized?
            if (_accessible.HighContrast && _accessible.HighContrastScheme.IndexOf("white", StringComparison.OrdinalIgnoreCase) != -1)
            {
                // If our HighContrastScheme is ON & a lighter one, then we should remain in 'Light' theme mode for Monaco Themes Perspective
                IsHighContrast = false;
                CurrentTheme = ApplicationTheme.Light;
            }
            else
            {
                // Otherwise, we just set to what's in the system as we'd expect.
                IsHighContrast = _accessible.HighContrast;
                CurrentTheme = Application.Current.RequestedTheme;
            }

            ThemeChanged?.Invoke(this);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _accessible.HighContrastChanged -= Accessible_HighContrastChanged;
            _settings.ColorValuesChanged -= Settings_ColorValuesChanged;
            if (Window.Current != null)
            {
                Window.Current.CoreWindow.Activated -= CoreWindow_Activated;
            }
        }
    }
}