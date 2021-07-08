// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using CommunityToolkit.HighPerformance.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArgumentOutOfRangeException = System.ArgumentOutOfRangeException;

namespace UnitTests.HighPerformance.Helpers
{
    public partial class Test_ParallelHelper
    {
        [TestCategory("ParallelHelper")]
        [TestMethod]
        public void Test_ParallelHelper_ParameterName_ThrowArgumentOutOfRangeExceptionForInvalidMinimumActionsPerThread()
        {
            try
            {
                ParallelHelper.For<DummyAction>(0, 1, -1);
            }
            catch (ArgumentOutOfRangeException e) when (e.GetType() == typeof(ArgumentOutOfRangeException))
            {
                var name = (
                    from method in typeof(ParallelHelper).GetMethods()
                    where
                        method.Name == nameof(ParallelHelper.For) &&
                        method.IsGenericMethodDefinition
                    let typeParams = method.GetGenericArguments()
                    let normalParams = method.GetParameters()
                    where
                        typeParams.Length == 1 &&
                        normalParams.Length == 3 &&
                        normalParams.All(p => p.ParameterType == typeof(int))
                    select normalParams[2].Name).Single();

                Assert.AreEqual(e.ParamName, name);

                return;
            }

            Assert.Fail("Failed to raise correct exception");
        }

        [TestCategory("ParallelHelper")]
        [TestMethod]
        public void Test_ParallelHelper_ParameterName_ThrowArgumentOutOfRangeExceptionForStartGreaterThanEnd()
        {
            try
            {
                ParallelHelper.For<DummyAction>(1, 0);
            }
            catch (ArgumentOutOfRangeException e) when (e.GetType() == typeof(ArgumentOutOfRangeException))
            {
                var name = (
                    from method in typeof(ParallelHelper).GetMethods()
                    where
                        method.Name == nameof(ParallelHelper.For) &&
                        method.IsGenericMethodDefinition
                    let typeParams = method.GetGenericArguments()
                    let normalParams = method.GetParameters()
                    where
                        typeParams.Length == 1 &&
                        normalParams.Length == 2 &&
                        normalParams.All(p => p.ParameterType == typeof(int))
                    select normalParams[0].Name).Single();

                Assert.AreEqual(e.ParamName, name);

                return;
            }

            Assert.Fail("Failed to raise correct exception");
        }

        [TestCategory("ParallelHelper")]
        [TestMethod]
        public void Test_ParallelHelper_ParameterName_ThrowArgumentOutOfRangeExceptionForTopGreaterThanBottom()
        {
            try
            {
                ParallelHelper.For2D<DummyAction2D>(1, 0, 0, 1);
            }
            catch (ArgumentOutOfRangeException e) when (e.GetType() == typeof(ArgumentOutOfRangeException))
            {
                var name = (
                    from method in typeof(ParallelHelper).GetMethods()
                    where
                        method.Name == nameof(ParallelHelper.For2D) &&
                        method.IsGenericMethodDefinition
                    let typeParams = method.GetGenericArguments()
                    let normalParams = method.GetParameters()
                    where
                        typeParams.Length == 1 &&
                        normalParams.Length == 4 &&
                        normalParams.All(p => p.ParameterType == typeof(int))
                    select normalParams[0].Name).Single();

                Assert.AreEqual(e.ParamName, name);

                return;
            }

            Assert.Fail("Failed to raise correct exception");
        }

        [TestCategory("ParallelHelper")]
        [TestMethod]
        public void Test_ParallelHelper_ParameterName_ThrowArgumentOutOfRangeExceptionForLeftGreaterThanRight()
        {
            try
            {
                ParallelHelper.For2D<DummyAction2D>(0, 1, 1, 0);
            }
            catch (ArgumentOutOfRangeException e) when (e.GetType() == typeof(ArgumentOutOfRangeException))
            {
                var name = (
                    from method in typeof(ParallelHelper).GetMethods()
                    where
                        method.Name == nameof(ParallelHelper.For2D) &&
                        method.IsGenericMethodDefinition
                    let typeParams = method.GetGenericArguments()
                    let normalParams = method.GetParameters()
                    where
                        typeParams.Length == 1 &&
                        normalParams.Length == 4 &&
                        normalParams.All(p => p.ParameterType == typeof(int))
                    select normalParams[2].Name).Single();

                Assert.AreEqual(e.ParamName, name);

                return;
            }

            Assert.Fail("Failed to raise correct exception");
        }

        /// <summary>
        /// A dummy type implementing <see cref="IAction"/>
        /// </summary>
        private readonly struct DummyAction : IAction
        {
            /// <inheritdoc/>
            public void Invoke(int i)
            {
            }
        }

        /// <summary>
        /// A dummy type implementing <see cref="IAction2D"/>
        /// </summary>
        private readonly struct DummyAction2D : IAction2D
        {
            /// <inheritdoc/>
            public void Invoke(int i, int j)
            {
            }
        }
    }
}