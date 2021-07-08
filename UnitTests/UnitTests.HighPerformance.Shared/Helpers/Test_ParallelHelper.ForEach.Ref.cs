// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.HighPerformance.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.HighPerformance.Shared.Buffers.Internals;

namespace UnitTests.HighPerformance.Helpers
{
    public partial class Test_ParallelHelper
    {
        [TestCategory("ParallelHelper")]
        [TestMethod]
        public void Test_ParallelHelper_ForEach_Ref()
        {
            foreach (int count in TestForCounts)
            {
                using UnmanagedSpanOwner<int> data = CreateRandomData(count);
                using UnmanagedSpanOwner<int> copy = new UnmanagedSpanOwner<int>(count);

                data.GetSpan().CopyTo(copy.GetSpan());

                foreach (ref int n in copy.GetSpan())
                {
                    n = unchecked(n * 397);
                }

                ParallelHelper.ForEach(data.Memory, new Multiplier(397));

                Span<int> dataSpan = data.GetSpan();
                Span<int> copySpan = copy.GetSpan();

                for (int i = 0; i < data.Length; i++)
                {
                    if (dataSpan[i] != copySpan[i])
                    {
                        Assert.Fail($"Item #{i} was not a match, was {dataSpan[i]} instead of {copySpan[i]}");
                    }
                }
            }
        }

        /// <summary>
        /// A type implementing <see cref="IRefAction{T}"/> to multiply array elements.
        /// </summary>
        private readonly struct Multiplier : IRefAction<int>
        {
            private readonly int factor;

            public Multiplier(int factor) => this.factor = factor;

            /// <inheritdoc/>
            public void Invoke(ref int i) => i = unchecked(i * this.factor);
        }
    }
}