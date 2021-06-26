// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.Toolkit.HighPerformance.Helpers
{
    /// <summary>
    /// Helpers to work with parallel code in a highly optimized manner.
    /// </summary>
    public static partial class ParallelHelper
    {
        /// <summary>
        /// Executes a specified action in an optimized parallel loop over the input data.
        /// </summary>
        /// <typeparam name="TItem">The type of items to iterate over.</typeparam>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IRefAction{T}"/> of <typeparamref name="TItem"/>) to invoke over each item.</typeparam>
        /// <param name="memory">The input <see cref="Memory{T}"/> representing the data to process.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<TItem, TAction>(Memory<TItem> memory)
            where TAction : struct, IRefAction<TItem>
        {
            ForEach(memory, default(TAction), 1);
        }

        /// <summary>
        /// Executes a specified action in an optimized parallel loop over the input data.
        /// </summary>
        /// <typeparam name="TItem">The type of items to iterate over.</typeparam>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IInAction{T}"/> of <typeparamref name="TItem"/>) to invoke over each item.</typeparam>
        /// <param name="memory">The input <see cref="Memory{T}"/> representing the data to process.</param>
        /// <param name="minimumActionsPerThread">
        /// The minimum number of actions to run per individual thread. Set to 1 if all invocations
        /// should be parallelized, or to a greater number if each individual invocation is fast
        /// enough that it is more efficient to set a lower bound per each running thread.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<TItem, TAction>(Memory<TItem> memory, int minimumActionsPerThread)
            where TAction : struct, IRefAction<TItem>
        {
            ForEach(memory, default(TAction), minimumActionsPerThread);
        }

        /// <summary>
        /// Executes a specified action in an optimized parallel loop over the input data.
        /// </summary>
        /// <typeparam name="TItem">The type of items to iterate over.</typeparam>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IInAction{T}"/> of <typeparamref name="TItem"/>) to invoke over each item.</typeparam>
        /// <param name="memory">The input <see cref="Memory{T}"/> representing the data to process.</param>
        /// <param name="action">The <typeparamref name="TAction"/> instance representing the action to invoke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForEach<TItem, TAction>(Memory<TItem> memory, in TAction action)
            where TAction : struct, IRefAction<TItem>
        {
            ForEach(memory, action, 1);
        }

        /// <summary>
        /// Executes a specified action in an optimized parallel loop over the input data.
        /// </summary>
        /// <typeparam name="TItem">The type of items to iterate over.</typeparam>
        /// <typeparam name="TAction">The type of action (implementing <see cref="IInAction{T}"/> of <typeparamref name="TItem"/>) to invoke over each item.</typeparam>
        /// <param name="memory">The input <see cref="Memory{T}"/> representing the data to process.</param>
        /// <param name="action">The <typeparamref name="TAction"/> instance representing the action to invoke.</param>
        /// <param name="minimumActionsPerThread">
        /// The minimum number of actions to run per individual thread. Set to 1 if all invocations
        /// should be parallelized, or to a greater number if each individual invocation is fast
        /// enough that it is more efficient to set a lower bound per each running thread.
        /// </param>
        public static void ForEach<TItem, TAction>(Memory<TItem> memory, in TAction action, int minimumActionsPerThread)
            where TAction : struct, IRefAction<TItem>
        {
            if (minimumActionsPerThread <= 0)
            {
                ThrowArgumentOutOfRangeExceptionForInvalidMinimumActionsPerThread();
            }

            if (memory.IsEmpty)
            {
                return;
            }

            int
                maxBatches = 1 + ((memory.Length - 1) / minimumActionsPerThread),
                cores = Environment.ProcessorCount,
                numBatches = Math.Min(maxBatches, cores);

            // Skip the parallel invocation when a single batch is needed
            if (numBatches == 1)
            {
                foreach (ref var item in memory.Span)
                {
                    Unsafe.AsRef(action).Invoke(ref item);
                }

                return;
            }

            int batchSize = 1 + ((memory.Length - 1) / numBatches);

            var actionInvoker = new RefActionInvoker<TItem, TAction>(batchSize, memory, action);

            // Run the batched operations in parallel
            Parallel.For(
                0,
                numBatches,
                new ParallelOptions { MaxDegreeOfParallelism = numBatches },
                actionInvoker.Invoke);
        }

        // Wrapping struct acting as explicit closure to execute the processing batches
        private readonly struct RefActionInvoker<TItem, TAction>
            where TAction : struct, IRefAction<TItem>
        {
            private readonly int batchSize;
            private readonly ReadOnlyMemory<TItem> memory;
            private readonly TAction action;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public RefActionInvoker(
                int batchSize,
                ReadOnlyMemory<TItem> memory,
                in TAction action)
            {
                this.batchSize = batchSize;
                this.memory = memory;
                this.action = action;
            }

            /// <summary>
            /// Processes the batch of actions at a specified index
            /// </summary>
            /// <param name="i">The index of the batch to process</param>
            public void Invoke(int i)
            {
                int
                    low = i * this.batchSize,
                    high = low + this.batchSize,
                    end = Math.Min(high, this.memory.Length);

                ref TItem r0 = ref MemoryMarshal.GetReference(this.memory.Span);
                ref TItem rStart = ref Unsafe.Add(ref r0, low);
                ref TItem rEnd = ref Unsafe.Add(ref r0, end);

                while (Unsafe.IsAddressLessThan(ref rStart, ref rEnd))
                {
                    Unsafe.AsRef(this.action).Invoke(ref rStart);

                    rStart = ref Unsafe.Add(ref rStart, 1);
                }
            }
        }
    }

    /// <summary>
    /// A contract for actions being executed on items of a specific type, with side effect.
    /// </summary>
    /// <typeparam name="T">The type of items to process.</typeparam>
    /// <remarks>If the <see cref="Invoke"/> method is small enough, it is highly recommended to mark it with <see cref="MethodImplOptions.AggressiveInlining"/>.</remarks>
    public interface IRefAction<T>
    {
        /// <summary>
        /// Executes the action on a specified <typeparamref name="T"/> item.
        /// </summary>
        /// <param name="item">The current item to process.</param>
        void Invoke(ref T item);
    }
}