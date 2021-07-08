// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CommunityToolkit.Common
{
    /// <summary>
    /// Helpers for working with tasks.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Gets the result of a <see cref="Task"/> if available, or <see langword="null"/> otherwise.
        /// </summary>
        /// <param name="task">The input <see cref="Task"/> instance to get the result for.</param>
        /// <returns>The result of <paramref name="task"/> if completed successfully, or <see langword="default"/> otherwise.</returns>
        /// <remarks>
        /// This method does not block if <paramref name="task"/> has not completed yet. Furthermore, it is not generic
        /// and uses reflection to access the <see cref="Task{TResult}.Result"/> property and boxes the result if it's
        /// a value type, which adds overhead. It should only be used when using generics is not possible.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object? GetResultOrDefault(this Task task)
        {
            // Check if the instance is a completed Task
            if (
#if NETSTANDARD2_1
                task.IsCompletedSuccessfully
#else
                task.Status == TaskStatus.RanToCompletion
#endif
            )
            {
                // We need an explicit check to ensure the input task is not the cached
                // Task.CompletedTask instance, because that can internally be stored as
                // a Task<T> for some given T (eg. on .NET 5 it's VoidTaskResult), which
                // would cause the following code to return that result instead of null.
                if (task != Task.CompletedTask)
                {
                    // Try to get the Task<T>.Result property. This method would've
                    // been called anyway after the type checks, but using that to
                    // validate the input type saves some additional reflection calls.
                    // Furthermore, doing this also makes the method flexible enough to
                    // cases whether the input Task<T> is actually an instance of some
                    // runtime-specific type that inherits from Task<T>.
                    PropertyInfo? propertyInfo =
#if NETSTANDARD1_4
                        task.GetType().GetRuntimeProperty(nameof(Task<object>.Result));
#else
                        task.GetType().GetProperty(nameof(Task<object>.Result));
#endif

                    // Return the result, if possible
                    return propertyInfo?.GetValue(task);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the result of a <see cref="Task{TResult}"/> if available, or <see langword="default"/> otherwise.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="Task{TResult}"/> to get the result for.</typeparam>
        /// <param name="task">The input <see cref="Task{TResult}"/> instance to get the result for.</param>
        /// <returns>The result of <paramref name="task"/> if completed successfully, or <see langword="default"/> otherwise.</returns>
        /// <remarks>This method does not block if <paramref name="task"/> has not completed yet.</remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? GetResultOrDefault<T>(this Task<T?> task)
        {
#if NETSTANDARD2_1
            return task.IsCompletedSuccessfully ? task.Result : default;
#else
            return task.Status == TaskStatus.RanToCompletion ? task.Result : default;
#endif
        }
    }
}