// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Provides the base implementation for all controls that contain single content and have a header.
    /// </summary>
    public partial class HeaderedContentControl : ContentControl
    {
        private const string PartHeaderPresenter = "HeaderPresenter";

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderedContentControl"/> class.
        /// </summary>
        public HeaderedContentControl()
        {
            DefaultStyleKey = typeof(HeaderedContentControl);
        }

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(object),
            typeof(HeaderedContentControl),
            new PropertyMetadata(null, OnHeaderChanged));

        /// <summary>
        /// Identifies the <see cref="HeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate",
            typeof(DataTemplate),
            typeof(HeaderedContentControl),
            new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            nameof(Orientation),
            typeof(Orientation),
            typeof(HeaderedContentControl),
            new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        /// <summary>
        /// Gets or sets the <see cref="Orientation"/> used for the header.
        /// </summary>
        /// <remarks>
        /// If set to <see cref="Orientation.Vertical"/> the header will be above the content.
        /// If set to <see cref="Orientation.Horizontal"/> the header will be to the left of the content.
        /// </remarks>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// Gets or sets the data used for the header of each control.
        /// </summary>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the template used to display the content of the control's header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            SetHeaderVisibility();
            SetOrientation();
        }

        /// <summary>
        /// Called when the <see cref="Header"/> property changes.
        /// </summary>
        /// <param name="oldValue">The old value of the <see cref="Header"/> property.</param>
        /// <param name="newValue">The new value of the <see cref="Header"/> property.</param>
        protected virtual void OnHeaderChanged(object oldValue, object newValue)
        {
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (HeaderedContentControl)d;
            control.SetOrientation();
        }

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (HeaderedContentControl)d;
            control.SetHeaderVisibility();
            control.OnHeaderChanged(e.OldValue, e.NewValue);
        }

        private void SetHeaderVisibility()
        {
            if (GetTemplateChild(PartHeaderPresenter) is FrameworkElement headerPresenter)
            {
                if (Header is string headerText)
                {
                    headerPresenter.Visibility = string.IsNullOrEmpty(headerText)
                        ? Visibility.Collapsed
                        : Visibility.Visible;
                }
                else
                {
                    headerPresenter.Visibility = Header != null
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }

        private void SetOrientation()
        {
            var orientation = this.Orientation == Orientation.Vertical
                ? nameof(Orientation.Vertical)
                : nameof(Orientation.Horizontal);

            VisualStateManager.GoToState(this, orientation, true);
        }
    }
}