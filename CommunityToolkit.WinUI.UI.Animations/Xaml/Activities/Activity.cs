// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// Base class to use when creating activities which accept a <see cref="Delay"/>.
    /// </summary>
    public abstract class Activity : DependencyObject, IActivity
    {
        /// <summary>
        /// Gets or sets the <see cref="TimeSpan"/> to wait before running the activity.
        /// </summary>
        public TimeSpan? Delay
        {
            get => (TimeSpan?)GetValue(DelayProperty);
            set => SetValue(DelayProperty, value);
        }

        /// <summary>
        /// Identifies the <seealso cref="Delay"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DelayProperty = DependencyProperty.Register(
            nameof(Delay),
            typeof(TimeSpan?),
            typeof(Activity),
            new PropertyMetadata(null));

        /// <inheritdoc/>
        public virtual Task InvokeAsync(UIElement element, CancellationToken token)
        {
            if (Delay is not null)
            {
                return Task.Delay(Delay.Value, token);
            }

            return Task.CompletedTask;
        }
    }
}