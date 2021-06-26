// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.Toolkit.HighPerformance.Helpers
{
    /// <summary>
    /// Helpers to work with parallel code in a highly optimized manner.
    /// </summary>
    public static partial class ParallelHelper
    {
#if NETSTANDARD2_1_OR_GREATER
        /// <summary>
        /// Executes a specified action in an optimized parallel loop.
        /// </summary>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IAction2D"/>) to invoke for each pair of iteration indices.</typeparam>
        /// <param name="i">The <see cref="Range"/> value indicating the iteration range for the outer loop.</param>
        /// <param name="j">The <see cref="Range"/> value indicating the iteration range for the inner loop.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void For2D<TAction>(Range i, Range j)
            where TAction : struct, IAction2D
        {
            For2D(i, j, default(TAction), 1);
        }

        /// <summary>
        /// Executes a specified action in an optimized parallel loop.
        /// </summary>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IAction2D"/>) to invoke for each pair of iteration indices.</typeparam>
        /// <param name="i">The <see cref="Range"/> value indicating the iteration range for the outer loop.</param>
        /// <param name="j">The <see cref="Range"/> value indicating the iteration range for the inner loop.</param>
        /// <param name="minimumActionsPerThread">
        /// The minimum number of actions to run per individual thread. Set to 1 if all invocations
        /// should be parallelized, or to a greater number if each individual invocation is fast
        /// enough that it is more efficient to set a lower bound per each running thread.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void For2D<TAction>(Range i, Range j, int minimumActionsPerThread)
            where TAction : struct, IAction2D
        {
            For2D(i, j, default(TAction), minimumActionsPerThread);
        }

        /// <summary>
        /// Executes a specified action in an optimized parallel loop.
        /// </summary>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IAction2D"/>) to invoke for each pair of iteration indices.</typeparam>
        /// <param name="i">The <see cref="Range"/> value indicating the iteration range for the outer loop.</param>
        /// <param name="j">The <see cref="Range"/> value indicating the iteration range for the inner loop.</param>
        /// <param name="action">The <typeparamref name="TAction"/> instance representing the action to invoke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void For2D<TAction>(Range i, Range j, in TAction action)
            where TAction : struct, IAction2D
        {
            For2D(i, j, action, 1);
        }

        /// <summary>
        /// Executes a specified action in an optimized parallel loop.
        /// </summary>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IAction2D"/>) to invoke for each pair of iteration indices.</typeparam>
        /// <param name="i">The <see cref="Range"/> value indicating the iteration range for the outer loop.</param>
        /// <param name="j">The <see cref="Range"/> value indicating the iteration range for the inner loop.</param>
        /// <param name="action">The <typeparamref name="TAction"/> instance representing the action to invoke.</param>
        /// <param name="minimumActionsPerThread">
        /// The minimum number of actions to run per individual thread. Set to 1 if all invocations
        /// should be parallelized, or to a greater number if each individual invocation is fast
        /// enough that it is more efficient to set a lower bound per each running thread.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void For2D<TAction>(Range i, Range j, in TAction action, int minimumActionsPerThread)
            where TAction : struct, IAction2D
        {
            if (i.Start.IsFromEnd || i.End.IsFromEnd)
            {
                ThrowArgumentExceptionForRangeIndexFromEnd(nameof(i));
            }

            if (j.Start.IsFromEnd || j.End.IsFromEnd)
            {
                ThrowArgumentExceptionForRangeIndexFromEnd(nameof(j));
            }

            int
                top = i.Start.Value,
                bottom = i.End.Value,
                left = j.Start.Value,
                right = j.End.Value;

            For2D(top, bottom, left, right, action, minimumActionsPerThread);
        }
#endif

        /// <summary>
        /// Executes a specified action in an optimized parallel loop.
        /// </summary>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IAction2D"/>) to invoke for each pair of iteration indices.</typeparam>
        /// <param name="area">The <see cref="Rectangle"/> value indicating the 2D iteration area to use.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void For2D<TAction>(Rectangle area)
            where TAction : struct, IAction2D
        {
            For2D(area.Top, area.Bottom, area.Left, area.Right, default(TAction), 1);
        }

        /// <summary>
        /// Executes a specified action in an optimized parallel loop.
        /// </summary>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IAction2D"/>) to invoke for each pair of iteration indices.</typeparam>
        /// <param name="area">The <see cref="Rectangle"/> value indicating the 2D iteration area to use.</param>
        /// <param name="minimumActionsPerThread">
        /// The minimum number of actions to run per individual thread. Set to 1 if all invocations
        /// should be parallelized, or to a greater number if each individual invocation is fast
        /// enough that it is more efficient to set a lower bound per each running thread.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void For2D<TAction>(Rectangle area, int minimumActionsPerThread)
            where TAction : struct, IAction2D
        {
            For2D(area.Top, area.Bottom, area.Left, area.Right, default(TAction), minimumActionsPerThread);
        }

        /// <summary>
        /// Executes a specified action in an optimized parallel loop.
        /// </summary>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IAction2D"/>) to invoke for each pair of iteration indices.</typeparam>
        /// <param name="area">The <see cref="Rectangle"/> value indicating the 2D iteration area to use.</param>
        /// <param name="action">The <typeparamref name="TAction"/> instance representing the action to invoke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void For2D<TAction>(Rectangle area, in TAction action)
            where TAction : struct, IAction2D
        {
            For2D(area.Top, area.Bottom, area.Left, area.Right, action, 1);
        }

        /// <summary>
        /// Executes a specified action in an optimized parallel loop.
        /// </summary>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IAction2D"/>) to invoke for each pair of iteration indices.</typeparam>
        /// <param name="area">The <see cref="Rectangle"/> value indicating the 2D iteration area to use.</param>
        /// <param name="action">The <typeparamref name="TAction"/> instance representing the action to invoke.</param>
        /// <param name="minimumActionsPerThread">
        /// The minimum number of actions to run per individual thread. Set to 1 if all invocations
        /// should be parallelized, or to a greater number if each individual invocation is fast
        /// enough that it is more efficient to set a lower bound per each running thread.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void For2D<TAction>(Rectangle area, in TAction action, int minimumActionsPerThread)
            where TAction : struct, IAction2D
        {
            For2D(area.Top, area.Bottom, area.Left, area.Right, action, minimumActionsPerThread);
        }

        /// <summary>
        /// Executes a specified action in an optimized parallel loop.
        /// </summary>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IAction2D"/>) to invoke for each pair of iteration indices.</typeparam>
        /// <param name="top">The starting iteration value for the outer loop.</param>
        /// <param name="bottom">The final iteration value for the outer loop (exclusive).</param>
        /// <param name="left">The starting iteration value for the inner loop.</param>
        /// <param name="right">The final iteration value for the inner loop (exclusive).</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void For2D<TAction>(int top, int bottom, int left, int right)
            where TAction : struct, IAction2D
        {
            For2D(top, bottom, left, right, default(TAction), 1);
        }

        /// <summary>
        /// Executes a specified action in an optimized parallel loop.
        /// </summary>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IAction2D"/>) to invoke for each pair of iteration indices.</typeparam>
        /// <param name="top">The starting iteration value for the outer loop.</param>
        /// <param name="bottom">The final iteration value for the outer loop (exclusive).</param>
        /// <param name="left">The starting iteration value for the inner loop.</param>
        /// <param name="right">The final iteration value for the inner loop (exclusive).</param>
        /// <param name="minimumActionsPerThread">
        /// The minimum number of actions to run per individual thread. Set to 1 if all invocations
        /// should be parallelized, or to a greater number if each individual invocation is fast
        /// enough that it is more efficient to set a lower bound per each running thread.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void For2D<TAction>(int top, int bottom, int left, int right, int minimumActionsPerThread)
            where TAction : struct, IAction2D
        {
            For2D(top, bottom, left, right, default(TAction), minimumActionsPerThread);
        }

        /// <summary>
        /// Executes a specified action in an optimized parallel loop.
        /// </summary>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IAction2D"/>) to invoke for each pair of iteration indices.</typeparam>
        /// <param name="top">The starting iteration value for the outer loop.</param>
        /// <param name="bottom">The final iteration value for the outer loop (exclusive).</param>
        /// <param name="left">The starting iteration value for the inner loop.</param>
        /// <param name="right">The final iteration value for the inner loop (exclusive).</param>
        /// <param name="action">The <typeparamref name="TAction"/> instance representing the action to invoke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void For2D<TAction>(int top, int bottom, int left, int right, in TAction action)
            where TAction : struct, IAction2D
        {
            For2D(top, bottom, left, right, action, 1);
        }

        /// <summary>
        /// Executes a specified action in an optimized parallel loop.
        /// </summary>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IAction2D"/>) to invoke for each pair of iteration indices.</typeparam>
        /// <param name="top">The starting iteration value for the outer loop.</param>
        /// <param name="bottom">The final iteration value for the outer loop (exclusive).</param>
        /// <param name="left">The starting iteration value for the inner loop.</param>
        /// <param name="right">The final iteration value for the inner loop (exclusive).</param>
        /// <param name="action">The <typeparamref name="TAction"/> instance representing the action to invoke.</param>
        /// <param name="minimumActionsPerThread">
        /// The minimum number of actions to run per individual thread. Set to 1 if all invocations
        /// should be parallelized, or to a greater number if each individual invocation is fast
        /// enough that it is more efficient to set a lower bound per each running thread.
        /// </param>
        public static void For2D<TAction>(int top, int bottom, int left, int right, in TAction action, int minimumActionsPerThread)
            where TAction : struct, IAction2D
        {
            if (minimumActionsPerThread <= 0)
            {
                ThrowArgumentOutOfRangeExceptionForInvalidMinimumActionsPerThread();
            }

            if (top > bottom)
            {
                ThrowArgumentOutOfRangeExceptionForTopGreaterThanBottom();
            }

            if (left > right)
            {
                ThrowArgumentOutOfRangeExceptionForLeftGreaterThanRight();
            }

            // If either side of the target area is empty, no iterations are performed
            if (top == bottom || left == right)
            {
                return;
            }

            int
                height = Math.Abs(top - bottom),
                width = Math.Abs(left - right),
                count = height * width,
                maxBatches = 1 + ((count - 1) / minimumActionsPerThread),
                clipBatches = Math.Min(maxBatches, height),
                cores = Environment.ProcessorCount,
                numBatches = Math.Min(clipBatches, cores);

            // Skip the parallel invocation when a single batch is needed
            if (numBatches == 1)
            {
                for (int y = top; y < bottom; y++)
                {
                    for (int x = left; x < right; x++)
                    {
                        Unsafe.AsRef(action).Invoke(y, x);
                    }
                }

                return;
            }

            int batchHeight = 1 + ((height - 1) / numBatches);

            var actionInvoker = new Action2DInvoker<TAction>(top, bottom, left, right, batchHeight, action);

            // Run the batched operations in parallel
            Parallel.For(
                0,
                numBatches,
                new ParallelOptions { MaxDegreeOfParallelism = numBatches },
                actionInvoker.Invoke);
        }

        // Wrapping struct acting as explicit closure to execute the processing batches
        private readonly struct Action2DInvoker<TAction>
            where TAction : struct, IAction2D
        {
            private readonly int startY;
            private readonly int endY;
            private readonly int startX;
            private readonly int endX;
            private readonly int batchHeight;
            private readonly TAction action;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Action2DInvoker(
                int startY,
                int endY,
                int startX,
                int endX,
                int batchHeight,
                in TAction action)
            {
                this.startY = startY;
                this.endY = endY;
                this.startX = startX;
                this.endX = endX;
                this.batchHeight = batchHeight;
                this.action = action;
            }

            /// <summary>
            /// Processes the batch of actions at a specified index
            /// </summary>
            /// <param name="i">The index of the batch to process</param>
            public void Invoke(int i)
            {
                int
                    heightOffset = i * this.batchHeight,
                    lowY = this.startY + heightOffset,
                    highY = lowY + this.batchHeight,
                    stopY = Math.Min(highY, this.endY);

                for (int y = lowY; y < stopY; y++)
                {
                    for (int x = this.startX; x < this.endX; x++)
                    {
                        Unsafe.AsRef(this.action).Invoke(y, x);
                    }
                }
            }
        }
    }

    /// <summary>
    /// A contract for actions being executed with two input indices.
    /// </summary>
    /// <remarks>If the <see cref="Invoke"/> method is small enough, it is highly recommended to mark it with <see cref="MethodImplOptions.AggressiveInlining"/>.</remarks>
    public interface IAction2D
    {
        /// <summary>
        /// Executes the action associated with two specified indices.
        /// </summary>
        /// <param name="i">The first index for the action to execute.</param>
        /// <param name="j">The second index for the action to execute.</param>
        void Invoke(int i, int j);
    }
}