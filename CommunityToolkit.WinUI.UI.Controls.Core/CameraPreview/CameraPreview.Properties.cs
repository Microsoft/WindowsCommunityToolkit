// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// Camera Control to preview video. Can subscribe to video frames, software bitmap when they arrive.
    /// </summary>
    public partial class CameraPreview
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for <see cref="IsFrameSourceGroupButtonVisible "/>.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsFrameSourceGroupButtonVisibleProperty =
            DependencyProperty.Register("IsFrameSourceGroupButtonVisible", typeof(bool), typeof(CameraPreview), new PropertyMetadata(true, IsFrameSourceGroupButtonVisibleChanged));

        /// <summary>
        /// Gets or sets a value indicating whether Frame Source Group Button is visible or not
        /// </summary>
        public bool IsFrameSourceGroupButtonVisible
        {
            get { return (bool)GetValue(IsFrameSourceGroupButtonVisibleProperty); }
            set { SetValue(IsFrameSourceGroupButtonVisibleProperty, value); }
        }

        private static void IsFrameSourceGroupButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cameraPreview = d as CameraPreview;
            if (cameraPreview._frameSourceGroupButton != null)
            {
                cameraPreview.SetFrameSourceGroupButtonVisibility();
            }
        }
    }
}