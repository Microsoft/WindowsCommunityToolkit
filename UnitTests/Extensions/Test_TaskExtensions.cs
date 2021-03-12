// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.Toolkit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Extensions
{
    [TestClass]
    public class Test_TaskExtensions
    {
        [TestCategory("TaskExtensions")]
        [TestMethod]
        public void Test_TaskExtensions_Nongeneric()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Assert.ThrowsException<NotImplementedException>(() => default(Task).ResultOrDefault());
            Assert.ThrowsException<NotImplementedException>(() => Task.CompletedTask.ResultOrDefault());
#pragma warning restore CS0618
        }

        [TestCategory("TaskExtensions")]
        [TestMethod]
        public void Test_TaskExtensions_Generic_ValueType()
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            Assert.AreEqual(0, tcs.Task.ResultOrDefault());

            tcs.SetResult(42);

            Assert.AreEqual(42, tcs.Task.ResultOrDefault());
        }

        [TestCategory("TaskExtensions")]
        [TestMethod]
        public void Test_TaskExtensions_Generic_ReferenceType()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Assert.AreEqual(null, tcs.Task.ResultOrDefault());

            tcs.SetResult(nameof(Test_TaskExtensions_Generic_ReferenceType));

            Assert.AreEqual(nameof(Test_TaskExtensions_Generic_ReferenceType), tcs.Task.ResultOrDefault());
        }
    }
}
