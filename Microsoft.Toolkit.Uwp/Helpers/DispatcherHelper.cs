// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Microsoft.Toolkit.Uwp.Helpers
{
    /// <summary>
    /// This class provides static methods helper for executing code in UI thread of the main window.
    /// </summary>
    [Obsolete("Replace calls to APIs in this class with extensions for the Windows.System.DispatcherQueue type (see https://docs.microsoft.com/uwp/api/windows.system.dispatcherqueue).")]
    public static class DispatcherHelper
    {
        /// <summary>
        /// Executes the given function on the main view's UI thread.
        /// </summary>
        /// <param name="function">Synchronous function to be executed on UI thread.</param>
        /// <param name="priority">Dispatcher execution priority, default is normal.</param>
        /// <returns>An awaitable <see cref="Task"/> for the operation.</returns>
        /// <remarks>If the current thread has UI access, <paramref name="function"/> will be invoked directly.</remarks>
        [Obsolete("This method should be replaced with dispatcherQueue.EnqueueAsync(function, priority), where dispatcherQueue is a DispatcherQueue instance that was retrieved from the UI thread and stored for later use.")]
        public static Task ExecuteOnUIThreadAsync(Action function, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            return ExecuteOnUIThreadAsync(CoreApplication.MainView, function, priority);
        }

        /// <summary>
        /// Executes the given function on the main view's UI thread and returns its result.
        /// </summary>
        /// <typeparam name="T">Returned data type of the function.</typeparam>
        /// <param name="function">Synchronous function to be executed on UI thread.</param>
        /// <param name="priority">Dispatcher execution priority, default is normal.</param>
        /// <returns>An awaitable <see cref="Task{T}"/> for the operation.</returns>
        /// <remarks>If the current thread has UI access, <paramref name="function"/> will be invoked directly.</remarks>
        [Obsolete("This method should be replaced with dispatcherQueue.EnqueueAsync(function, priority), where dispatcherQueue is a DispatcherQueue instance that was retrieved from the UI thread and stored for later use.")]
        public static Task<T> ExecuteOnUIThreadAsync<T>(Func<T> function, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            return ExecuteOnUIThreadAsync(CoreApplication.MainView, function, priority);
        }

        /// <summary>
        /// Executes the given <see cref="Task"/>-returning function on the main view's UI thread and returns either that <see cref="Task"/>
        /// or a proxy <see cref="Task"/> that completes when the one produced by the given function completes.
        /// </summary>
        /// <param name="function">Asynchronous function to be executed asynchronously on UI thread.</param>
        /// <param name="priority">Dispatcher execution priority, default is normal.</param>
        /// <returns>An awaitable <see cref="Task"/> for the operation.</returns>
        [Obsolete("This method should be replaced with dispatcherQueue.EnqueueAsync(function, priority), where dispatcherQueue is a DispatcherQueue instance that was retrieved from the UI thread and stored for later use.")]
        public static Task ExecuteOnUIThreadAsync(Func<Task> function, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            return ExecuteOnUIThreadAsync(CoreApplication.MainView, function, priority);
        }

        /// <summary>
        /// Executes the given <see cref="Task{TResult}"/>-returning function on the main view's UI thread and returns either that <see cref="Task{TResult}"/>
        /// or a proxy <see cref="Task{TResult}"/> that completes when the one produced by the given function completes.
        /// </summary>
        /// <typeparam name="T">Returned data type of the function.</typeparam>
        /// <param name="function">Asynchronous function to be executed asynchronously on UI thread.</param>
        /// <param name="priority">Dispatcher execution priority, default is normal.</param>
        /// <returns>An awaitable <see cref="Task{T}"/> for the operation.</returns>
        [Obsolete("This method should be replaced with dispatcherQueue.EnqueueAsync(function, priority), where dispatcherQueue is a DispatcherQueue instance that was retrieved from the UI thread and stored for later use.")]
        public static Task<T> ExecuteOnUIThreadAsync<T>(Func<Task<T>> function, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            return ExecuteOnUIThreadAsync(CoreApplication.MainView, function, priority);
        }

        /// <summary>
        /// Executes the given function on a given view's UI thread.
        /// </summary>
        /// <param name="viewToExecuteOn">View for the <paramref name="function"/> to be executed on.</param>
        /// <param name="function">Synchronous function to be executed on UI thread.</param>
        /// <param name="priority">Dispatcher execution priority, default is normal</param>
        /// <returns>An awaitable <see cref="Task"/> for the operation.</returns>
        /// <remarks>If the current thread has UI access, <paramref name="function"/> will be invoked directly.</remarks>
        [Obsolete("This method should be replaced with viewToExecuteOn.DispatcherQueue.EnqueueAsync(function, priority).")]
        public static Task ExecuteOnUIThreadAsync(this CoreApplicationView viewToExecuteOn, Action function, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (viewToExecuteOn is null)
            {
                throw new ArgumentNullException(nameof(viewToExecuteOn));
            }

            return viewToExecuteOn.Dispatcher.AwaitableRunAsync(function, priority);
        }

        /// <summary>
        /// Executes the given function on a given view's UI thread.
        /// </summary>
        /// <typeparam name="T">Returned data type of the function.</typeparam>
        /// <param name="viewToExecuteOn">View for the <paramref name="function"/> to be executed on.</param>
        /// <param name="function">Synchronous function with return type <typeparamref name="T"/> to be executed on UI thread.</param>
        /// <param name="priority">Dispatcher execution priority, default is normal.</param>
        /// <returns>An awaitable <see cref="Task{T}"/> for the operation.</returns>
        /// <remarks>If the current thread has UI access, <paramref name="function"/> will be invoked directly.</remarks>
        [Obsolete("This method should be replaced with viewToExecuteOn.DispatcherQueue.EnqueueAsync(function, priority).")]
        public static Task<T> ExecuteOnUIThreadAsync<T>(this CoreApplicationView viewToExecuteOn, Func<T> function, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (viewToExecuteOn is null)
            {
                throw new ArgumentNullException(nameof(viewToExecuteOn));
            }

            return viewToExecuteOn.Dispatcher.AwaitableRunAsync(function, priority);
        }

        /// <summary>
        /// Executes the given function on a given view's UI thread.
        /// </summary>
        /// <param name="viewToExecuteOn">View for the <paramref name="function"/> to be executed on.</param>
        /// <param name="function">Asynchronous function to be executed asynchronously on UI thread.</param>
        /// <param name="priority">Dispatcher execution priority, default is normal.</param>
        /// <returns>An awaitable <see cref="Task"/> for the operation.</returns>
        /// <remarks>If the current thread has UI access, <paramref name="function"/> will be invoked directly.</remarks>
        [Obsolete("This method should be replaced with viewToExecuteOn.DispatcherQueue.EnqueueAsync(function, priority).")]
        public static Task ExecuteOnUIThreadAsync(this CoreApplicationView viewToExecuteOn, Func<Task> function, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (viewToExecuteOn is null)
            {
                throw new ArgumentNullException(nameof(viewToExecuteOn));
            }

            return viewToExecuteOn.Dispatcher.AwaitableRunAsync(function, priority);
        }

        /// <summary>
        /// Executes the given function on a given view's UI thread.
        /// </summary>
        /// <typeparam name="T">Returned data type of the function.</typeparam>
        /// <param name="viewToExecuteOn">View for the <paramref name="function"/>  to be executed on.</param>
        /// <param name="function">Asynchronous function to be executed asynchronously on UI thread.</param>
        /// <param name="priority">Dispatcher execution priority, default is normal.</param>
        /// <returns>An awaitable <see cref="Task"/> for the operation.</returns>
        /// <remarks>If the current thread has UI access, <paramref name="function"/> will be invoked directly.</remarks>
        [Obsolete("This method should be replaced with viewToExecuteOn.DispatcherQueue.EnqueueAsync(function, priority).")]
        public static Task<T> ExecuteOnUIThreadAsync<T>(this CoreApplicationView viewToExecuteOn, Func<Task<T>> function, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (viewToExecuteOn is null)
            {
                throw new ArgumentNullException(nameof(viewToExecuteOn));
            }

            return viewToExecuteOn.Dispatcher.AwaitableRunAsync(function, priority);
        }

        /// <summary>
        /// Extension method for <see cref="CoreDispatcher"/>. Offering an actual awaitable <see cref="Task"/> with optional result that will be executed on the given dispatcher.
        /// </summary>
        /// <param name="dispatcher">Dispatcher of a thread to run <paramref name="function"/>.</param>
        /// <param name="function"> Function to be executed on the given dispatcher.</param>
        /// <param name="priority">Dispatcher execution priority, default is normal.</param>
        /// <returns>An awaitable <see cref="Task"/> for the operation.</returns>
        /// <remarks>If the current thread has UI access, <paramref name="function"/> will be invoked directly.</remarks>
        [Obsolete("This method should be replaced with dispatcherQueue.EnqueueAsync(function, priority). A queue can be retrieved with DispatcherQueue.GetForCurrentThread().")]
        public static Task AwaitableRunAsync(this CoreDispatcher dispatcher, Action function, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (function is null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            /* Run the function directly when we have thread access.
             * Also reuse Task.CompletedTask in case of success,
             * to skip an unnecessary heap allocation for every invocation. */
            if (dispatcher.HasThreadAccess)
            {
                try
                {
                    function();

                    return Task.CompletedTask;
                }
                catch (Exception e)
                {
                    return Task.FromException(e);
                }
            }

            var taskCompletionSource = new TaskCompletionSource<object>();

            _ = dispatcher.RunAsync(priority, () =>
            {
                try
                {
                    function();

                    taskCompletionSource.SetResult(null);
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                }
            });

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Extension method for <see cref="CoreDispatcher"/>. Offering an actual awaitable <see cref="Task{T}"/> with optional result that will be executed on the given dispatcher.
        /// </summary>
        /// <typeparam name="T">Returned data type of the function.</typeparam>
        /// <param name="dispatcher">Dispatcher of a thread to run <paramref name="function"/>.</param>
        /// <param name="function"> Function to be executed on the given dispatcher.</param>
        /// <param name="priority">Dispatcher execution priority, default is normal.</param>
        /// <returns>An awaitable <see cref="Task{T}"/> for the operation.</returns>
        /// <remarks>If the current thread has UI access, <paramref name="function"/> will be invoked directly.</remarks>
        [Obsolete("This method should be replaced with dispatcherQueue.EnqueueAsync(function, priority). A queue can be retrieved with DispatcherQueue.GetForCurrentThread().")]
        public static Task<T> AwaitableRunAsync<T>(this CoreDispatcher dispatcher, Func<T> function, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (function is null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            // Skip the dispatch, if possible
            if (dispatcher.HasThreadAccess)
            {
                try
                {
                    return Task.FromResult(function());
                }
                catch (Exception e)
                {
                    return Task.FromException<T>(e);
                }
            }

            var taskCompletionSource = new TaskCompletionSource<T>();

            _ = dispatcher.RunAsync(priority, () =>
            {
                try
                {
                    taskCompletionSource.SetResult(function());
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                }
            });

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Extension method for <see cref="CoreDispatcher"/>. Offering an actual awaitable <see cref="Task"/> with optional result that will be executed on the given dispatcher.
        /// </summary>
        /// <param name="dispatcher">Dispatcher of a thread to run <paramref name="function"/>.</param>
        /// <param name="function">Asynchronous function to be executed on the given dispatcher.</param>
        /// <param name="priority">Dispatcher execution priority, default is normal.</param>
        /// <returns>An awaitable <see cref="Task"/> for the operation.</returns>
        /// <remarks>If the current thread has UI access, <paramref name="function"/> will be invoked directly.</remarks>
        [Obsolete("This method should be replaced with dispatcherQueue.EnqueueAsync(function, priority). A queue can be retrieved with DispatcherQueue.GetForCurrentThread().")]
        public static Task AwaitableRunAsync(this CoreDispatcher dispatcher, Func<Task> function, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (function is null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            /* If we have thread access, we can retrieve the task directly.
             * We don't use ConfigureAwait(false) in this case, in order
             * to let the caller continue its execution on the same thread
             * after awaiting the task returned by this function. */
            if (dispatcher.HasThreadAccess)
            {
                try
                {
                    if (function() is Task awaitableResult)
                    {
                        return awaitableResult;
                    }

                    return Task.FromException(new InvalidOperationException("The Task returned by function cannot be null."));
                }
                catch (Exception e)
                {
                    return Task.FromException(e);
                }
            }

            var taskCompletionSource = new TaskCompletionSource<object>();

            _ = dispatcher.RunAsync(priority, async () =>
            {
                try
                {
                    if (function() is Task awaitableResult)
                    {
                        await awaitableResult.ConfigureAwait(false);

                        taskCompletionSource.SetResult(null);
                    }
                    else
                    {
                        taskCompletionSource.SetException(new InvalidOperationException("The Task returned by function cannot be null."));
                    }
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                }
            });

            return taskCompletionSource.Task;
        }

        /// <summary>
        /// Extension method for <see cref="CoreDispatcher"/>. Offering an actual awaitable <see cref="Task{T}"/> with optional result that will be executed on the given dispatcher.
        /// </summary>
        /// <typeparam name="T">Returned data type of the function.</typeparam>
        /// <param name="dispatcher">Dispatcher of a thread to run <paramref name="function"/>.</param>
        /// <param name="function">Asynchronous function to be executed asynchronously on the given dispatcher.</param>
        /// <param name="priority">Dispatcher execution priority, default is normal.</param>
        /// <returns>An awaitable <see cref="Task{T}"/> for the operation.</returns>
        /// <remarks>If the current thread has UI access, <paramref name="function"/> will be invoked directly.</remarks>
        [Obsolete("This method should be replaced with dispatcherQueue.EnqueueAsync(function, priority). A queue can be retrieved with DispatcherQueue.GetForCurrentThread().")]
        public static Task<T> AwaitableRunAsync<T>(this CoreDispatcher dispatcher, Func<Task<T>> function, CoreDispatcherPriority priority = CoreDispatcherPriority.Normal)
        {
            if (function is null)
            {
                throw new ArgumentNullException(nameof(function));
            }

            // Skip the dispatch, if possible
            if (dispatcher.HasThreadAccess)
            {
                try
                {
                    if (function() is Task<T> awaitableResult)
                    {
                        return awaitableResult;
                    }

                    return Task.FromException<T>(new InvalidOperationException("The Task returned by function cannot be null."));
                }
                catch (Exception e)
                {
                    return Task.FromException<T>(e);
                }
            }

            var taskCompletionSource = new TaskCompletionSource<T>();

            _ = dispatcher.RunAsync(priority, async () =>
            {
                try
                {
                    if (function() is Task<T> awaitableResult)
                    {
                        var result = await awaitableResult.ConfigureAwait(false);

                        taskCompletionSource.SetResult(result);
                    }
                    else
                    {
                        taskCompletionSource.SetException(new InvalidOperationException("The Task returned by function cannot be null."));
                    }
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                }
            });

            return taskCompletionSource.Task;
        }
    }
}