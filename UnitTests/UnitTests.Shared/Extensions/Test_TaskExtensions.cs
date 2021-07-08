// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using CommunityToolkit.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Extensions
{
    [TestClass]
    public class Test_TaskExtensions
    {
        [TestCategory("TaskExtensions")]
        [TestMethod]
        public void Test_TaskExtensions_ResultOrDefault()
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            Assert.AreEqual(null, ((Task)tcs.Task).GetResultOrDefault());

            tcs.SetCanceled();

            Assert.AreEqual(null, ((Task)tcs.Task).GetResultOrDefault());

            tcs = new TaskCompletionSource<int>();

            tcs.SetException(new InvalidOperationException("Test"));

            Assert.AreEqual(null, ((Task)tcs.Task).GetResultOrDefault());

            tcs = new TaskCompletionSource<int>();

            tcs.SetResult(42);

            Assert.AreEqual(42, ((Task)tcs.Task).GetResultOrDefault());
        }

        [TestCategory("TaskExtensions")]
        [TestMethod]
        public void Test_TaskExtensions_ResultOrDefault_FromTaskCompleted()
        {
            Assert.AreEqual(null, Task.CompletedTask.GetResultOrDefault());
        }

        [TestCategory("TaskExtensions")]
        [TestMethod]
        public async Task Test_TaskExtensions_ResultOrDefault_FromAsyncTaskMethodBuilder()
        {
            var tcs = new TaskCompletionSource<object>();

            Task<string> taskFromBuilder = GetTaskFromAsyncMethodBuilder("Test", tcs);

            Assert.IsNull(((Task)taskFromBuilder).GetResultOrDefault());
            Assert.IsNull(taskFromBuilder.GetResultOrDefault());

            tcs.SetResult(null);

            await taskFromBuilder;

            Assert.AreEqual(((Task)taskFromBuilder).GetResultOrDefault(), "Test");
            Assert.AreEqual(taskFromBuilder.GetResultOrDefault(), "Test");
        }

        [TestCategory("TaskExtensions")]
        [TestMethod]
        public void Test_TaskExtensions_ResultOrDefault_OfT_Int32()
        {
            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();

            Assert.AreEqual(0, tcs.Task.GetResultOrDefault());

            tcs.SetCanceled();

            Assert.AreEqual(0, tcs.Task.GetResultOrDefault());

            tcs = new TaskCompletionSource<int>();

            tcs.SetException(new InvalidOperationException("Test"));

            Assert.AreEqual(0, tcs.Task.GetResultOrDefault());

            tcs = new TaskCompletionSource<int>();

            tcs.SetResult(42);

            Assert.AreEqual(42, tcs.Task.GetResultOrDefault());
        }

        [TestCategory("TaskExtensions")]
        [TestMethod]
        public void Test_TaskExtensions_ResultOrDefault_OfT_String()
        {
            TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

            Assert.AreEqual(null, tcs.Task.GetResultOrDefault());

            tcs.SetCanceled();

            Assert.AreEqual(null, tcs.Task.GetResultOrDefault());

            tcs = new TaskCompletionSource<string>();

            tcs.SetException(new InvalidOperationException("Test"));

            Assert.AreEqual(null, tcs.Task.GetResultOrDefault());

            tcs = new TaskCompletionSource<string>();

            tcs.SetResult("Hello world");

            Assert.AreEqual("Hello world", tcs.Task.GetResultOrDefault());
        }

        // Creates a Task<T> of a given type which is actually an instance of
        // System.Runtime.CompilerServices.AsyncTaskMethodBuilder<TResult>.AsyncStateMachineBox<TStateMachine>.
        // See https://source.dot.net/#System.Private.CoreLib/AsyncTaskMethodBuilderT.cs,f8f35fd356112b30.
        // This is needed to verify that the extension also works when the input Task<T> is of a derived type.
        private static async Task<T> GetTaskFromAsyncMethodBuilder<T>(T result, TaskCompletionSource<object> tcs)
        {
            await tcs.Task;

            return result;
        }
    }
}