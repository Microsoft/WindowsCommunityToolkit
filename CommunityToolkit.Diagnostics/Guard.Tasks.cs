// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CommunityToolkit.Diagnostics
{
    /// <summary>
    /// Helper methods to verify conditions when running code.
    /// </summary>
    public static partial class Guard
    {
        /// <summary>
        /// Asserts that the input <see cref="Task"/> instance is in a completed state.
        /// </summary>
        /// <param name="task">The input <see cref="Task"/> instance to test.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> is not in a completed state.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsCompleted(Task task, string name)
        {
            if (task.IsCompleted)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsCompleted(task, name);
        }

        /// <summary>
        /// Asserts that the input <see cref="Task"/> instance is not in a completed state.
        /// </summary>
        /// <param name="task">The input <see cref="Task"/> instance to test.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> is in a completed state.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotCompleted(Task task, string name)
        {
            if (!task.IsCompleted)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotCompleted(task, name);
        }

        /// <summary>
        /// Asserts that the input <see cref="Task"/> instance has been completed successfully.
        /// </summary>
        /// <param name="task">The input <see cref="Task"/> instance to test.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> has not been completed successfully.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsCompletedSuccessfully(Task task, string name)
        {
            if (task.Status == TaskStatus.RanToCompletion)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsCompletedSuccessfully(task, name);
        }

        /// <summary>
        /// Asserts that the input <see cref="Task"/> instance has not been completed successfully.
        /// </summary>
        /// <param name="task">The input <see cref="Task"/> instance to test.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> has been completed successfully.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotCompletedSuccessfully(Task task, string name)
        {
            if (task.Status != TaskStatus.RanToCompletion)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotCompletedSuccessfully(task, name);
        }

        /// <summary>
        /// Asserts that the input <see cref="Task"/> instance is faulted.
        /// </summary>
        /// <param name="task">The input <see cref="Task"/> instance to test.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> is not faulted.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsFaulted(Task task, string name)
        {
            if (task.IsFaulted)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsFaulted(task, name);
        }

        /// <summary>
        /// Asserts that the input <see cref="Task"/> instance is not faulted.
        /// </summary>
        /// <param name="task">The input <see cref="Task"/> instance to test.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> is faulted.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotFaulted(Task task, string name)
        {
            if (!task.IsFaulted)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotFaulted(task, name);
        }

        /// <summary>
        /// Asserts that the input <see cref="Task"/> instance is canceled.
        /// </summary>
        /// <param name="task">The input <see cref="Task"/> instance to test.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> is not canceled.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsCanceled(Task task, string name)
        {
            if (task.IsCanceled)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsCanceled(task, name);
        }

        /// <summary>
        /// Asserts that the input <see cref="Task"/> instance is not canceled.
        /// </summary>
        /// <param name="task">The input <see cref="Task"/> instance to test.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> is canceled.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNotCanceled(Task task, string name)
        {
            if (!task.IsCanceled)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForIsNotCanceled(task, name);
        }

        /// <summary>
        /// Asserts that the input <see cref="Task"/> instance has a specific status.
        /// </summary>
        /// <param name="task">The input <see cref="Task"/> instance to test.</param>
        /// <param name="status">The task status that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> doesn't match <paramref name="status"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HasStatusEqualTo(Task task, TaskStatus status, string name)
        {
            if (task.Status == status)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForHasStatusEqualTo(task, status, name);
        }

        /// <summary>
        /// Asserts that the input <see cref="Task"/> instance has not a specific status.
        /// </summary>
        /// <param name="task">The input <see cref="Task"/> instance to test.</param>
        /// <param name="status">The task status that is accepted.</param>
        /// <param name="name">The name of the input parameter being tested.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="task"/> matches <paramref name="status"/>.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HasStatusNotEqualTo(Task task, TaskStatus status, string name)
        {
            if (task.Status != status)
            {
                return;
            }

            ThrowHelper.ThrowArgumentExceptionForHasStatusNotEqualTo(task, status, name);
        }
    }
}