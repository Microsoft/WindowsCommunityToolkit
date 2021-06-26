// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Helpers
{
    /// <summary>
    /// Provides helpers for the <see cref="CompositionTarget"/> class.
    /// </summary>
    public static class CompositionTargetHelper
    {
        /// <summary>
        /// Provides a method to execute code after the rendering pass is completed.
        /// <seealso href="https://github.com/microsoft/microsoft-ui-xaml/blob/c045cde57c5c754683d674634a0baccda34d58c4/dev/dll/SharedHelpers.cpp#L399"/>
        /// <seealso href="https://devblogs.microsoft.com/premier-developer/the-danger-of-taskcompletionsourcet-class/"/>
        /// </summary>
        /// <param name="action">Action to be executed after render pass</param>
        /// <param name="options"><see cref="TaskCreationOptions"/> for how to handle async calls with <see cref="TaskCompletionSource{TResult}"/>.</param>
        /// <returns>Awaitable Task</returns>
        public static Task<bool> ExecuteAfterCompositionRenderingAsync(Action action, TaskCreationOptions? options = null)
        {
            if (action is null)
            {
                ThrowArgumentNullException();
            }

            var taskCompletionSource = options.HasValue ? new TaskCompletionSource<bool>(options.Value)
                : new TaskCompletionSource<bool>();

            try
            {
                void Callback(object sender, object args)
                {
                    CompositionTarget.Rendering -= Callback;

                    action();

                    taskCompletionSource.SetResult(true);
                }

                CompositionTarget.Rendering += Callback;
            }
            catch (Exception e)
            {
                taskCompletionSource.SetException(e); // Note this can just sometimes be a wrong thread exception, see WinUI function notes.
            }

            return taskCompletionSource.Task;

            static void ThrowArgumentNullException() => throw new ArgumentNullException("The parameter \"action\" must not be null.");
        }
    }
}