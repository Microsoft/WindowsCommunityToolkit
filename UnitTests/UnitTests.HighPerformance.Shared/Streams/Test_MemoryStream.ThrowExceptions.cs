// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using CommunityToolkit.HighPerformance;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.HighPerformance.Streams
{
    public partial class Test_MemoryStream
    {
        [TestCategory("MemoryStream")]
        [TestMethod]
        public void Test_MemoryStream_ParameterName_ThrowArgumentExceptionForPosition()
        {
            var stream = new byte[10].AsMemory().AsStream();

            try
            {
                stream.Position = -1;
            }
            catch (ArgumentOutOfRangeException e) when (e.GetType() == typeof(ArgumentOutOfRangeException))
            {
                Assert.AreEqual(e.ParamName, nameof(Stream.Position));

                return;
            }

            Assert.Fail("Failed to raise correct exception");
        }

        [TestCategory("MemoryStream")]
        [TestMethod]
        public void Test_MemoryStream_ParameterName_ThrowArgumentExceptionForSeekOrigin()
        {
            var stream = new byte[10].AsMemory().AsStream();

            try
            {
                stream.Seek(0, (SeekOrigin)int.MinValue);
            }
            catch (ArgumentException e) when (e.GetType() == typeof(ArgumentException))
            {
                var method = stream.GetType().GetMethod(nameof(Stream.Seek));
                var name = method!.GetParameters()[1].Name;

                Assert.AreEqual(e.ParamName, name);

                return;
            }

            Assert.Fail("Failed to raise correct exception");
        }

        [TestCategory("MemoryStream")]
        [TestMethod]
        public void Test_MemoryStream_ParameterName_ThrowArgumentNullExceptionForNullBuffer()
        {
            var stream = new byte[10].AsMemory().AsStream();

            try
            {
                stream.Write(null, 0, 10);
            }
            catch (ArgumentNullException e) when (e.GetType() == typeof(ArgumentNullException))
            {
                var name = (
                    from method in typeof(Stream).GetMethods()
                    where method.Name == nameof(Stream.Write)
                    let normalParams = method.GetParameters()
                    where
                        normalParams.Length == 3 &&
                        normalParams[0].ParameterType == typeof(byte[]) &&
                        normalParams[1].ParameterType == typeof(int) &&
                        normalParams[2].ParameterType == typeof(int)
                    select normalParams[0].Name).Single();

                Assert.AreEqual(e.ParamName, name);

                return;
            }

            Assert.Fail("Failed to raise correct exception");
        }

        [TestCategory("MemoryStream")]
        [TestMethod]
        public void Test_MemoryStream_ParameterName_ThrowArgumentOutOfRangeExceptionForNegativeOffset()
        {
            var stream = new byte[10].AsMemory().AsStream();

            try
            {
                stream.Write(new byte[1], -1, 10);
            }
            catch (ArgumentOutOfRangeException e) when (e.GetType() == typeof(ArgumentOutOfRangeException))
            {
                var name = (
                    from method in typeof(Stream).GetMethods()
                    where method.Name == nameof(Stream.Write)
                    let normalParams = method.GetParameters()
                    where
                        normalParams.Length == 3 &&
                        normalParams[0].ParameterType == typeof(byte[]) &&
                        normalParams[1].ParameterType == typeof(int) &&
                        normalParams[2].ParameterType == typeof(int)
                    select normalParams[1].Name).Single();

                Assert.AreEqual(e.ParamName, name);

                return;
            }

            Assert.Fail("Failed to raise correct exception");
        }

        [TestCategory("MemoryStream")]
        [TestMethod]
        public void Test_MemoryStream_ParameterName_ThrowArgumentOutOfRangeExceptionForNegativeCount()
        {
            var stream = new byte[10].AsMemory().AsStream();

            try
            {
                stream.Write(new byte[1], 0, -1);
            }
            catch (ArgumentOutOfRangeException e) when (e.GetType() == typeof(ArgumentOutOfRangeException))
            {
                var name = (
                    from method in typeof(Stream).GetMethods()
                    where method.Name == nameof(Stream.Write)
                    let normalParams = method.GetParameters()
                    where
                        normalParams.Length == 3 &&
                        normalParams[0].ParameterType == typeof(byte[]) &&
                        normalParams[1].ParameterType == typeof(int) &&
                        normalParams[2].ParameterType == typeof(int)
                    select normalParams[2].Name).Single();

                Assert.AreEqual(e.ParamName, name);

                return;
            }

            Assert.Fail("Failed to raise correct exception");
        }

        [TestCategory("MemoryStream")]
        [TestMethod]
        public void Test_MemoryStream_ParameterName_ThrowArgumentExceptionForExceededBufferSize()
        {
            var stream = new byte[10].AsMemory().AsStream();

            try
            {
                stream.Write(new byte[1], 0, 10);
            }
            catch (ArgumentException e) when (e.GetType() == typeof(ArgumentException))
            {
                var name = (
                    from method in typeof(Stream).GetMethods()
                    where method.Name == nameof(Stream.Write)
                    let normalParams = method.GetParameters()
                    where
                        normalParams.Length == 3 &&
                        normalParams[0].ParameterType == typeof(byte[]) &&
                        normalParams[1].ParameterType == typeof(int) &&
                        normalParams[2].ParameterType == typeof(int)
                    select normalParams[0].Name).Single();

                Assert.AreEqual(e.ParamName, name);

                return;
            }

            Assert.Fail("Failed to raise correct exception");
        }
    }
}