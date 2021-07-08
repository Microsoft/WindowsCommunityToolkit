// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace CommunityToolkit.WinUI.UI.Animations
{
    /// <summary>
    /// An interface representing a XAML model for a custom activity or action within an <see cref="AnimationSet"/>.
    /// </summary>
    public interface IActivity
    {
        /// <summary>
        /// Invokes the current activity.
        /// </summary>
        /// <param name="element">The target <see cref="UIElement"/> to invoke the activity for.</param>
        /// <param name="token">A cancellation token to cancel the activity before it completes.</param>
        /// <returns>A <see cref="Task"/> that indicates when the activity has completed its execution.</returns>
        Task InvokeAsync(UIElement element, CancellationToken token);
    }
}