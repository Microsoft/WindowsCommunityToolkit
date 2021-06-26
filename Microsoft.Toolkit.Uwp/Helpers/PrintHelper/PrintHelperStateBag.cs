// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.System;
using Windows.UI.Xaml;

namespace Microsoft.Toolkit.Uwp.Helpers
{
    /// <summary>
    /// Internal class used to store values updated by <see cref="PrintHelper"/>.
    /// </summary>
    internal class PrintHelperStateBag
    {
        private readonly DispatcherQueue _dispatcherQueue;

        internal PrintHelperStateBag(DispatcherQueue dispatcherQueue)
        {
            _dispatcherQueue = dispatcherQueue;
        }

        /// <summary>
        /// Gets or sets the stored horizontal alignment.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment { get; set; }

        /// <summary>
        /// Gets or sets the stored vertical alignment.
        /// </summary>
        public VerticalAlignment VerticalAlignment { get; set; }

        /// <summary>
        /// Gets or sets the stored width.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets the stored height.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the stored margin.
        /// </summary>
        public Thickness Margin { get; set; }

        /// <summary>
        /// Captures the current element state.
        /// </summary>
        /// <param name="element">Element to capture state from</param>
        public void Capture(FrameworkElement element)
        {
            HorizontalAlignment = element.HorizontalAlignment;
            VerticalAlignment = element.VerticalAlignment;
            Width = element.Width;
            Height = element.Height;
            Margin = element.Margin;
        }

        /// <summary>
        /// Restores stored state to given element.
        /// </summary>
        /// <param name="element">Element to restore state to</param>
        public void Restore(FrameworkElement element)
        {
            _dispatcherQueue.EnqueueAsync(() =>
            {
                element.HorizontalAlignment = HorizontalAlignment;
                element.VerticalAlignment = VerticalAlignment;
                element.Width = Width;
                element.Height = Height;
                element.Margin = Margin;
            });
        }
    }
}