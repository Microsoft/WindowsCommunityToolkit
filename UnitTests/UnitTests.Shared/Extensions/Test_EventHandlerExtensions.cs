// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.Toolkit.Deferred;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Extensions
{
    [TestClass]
    public class Test_EventHandlerExtensions
    {
        [TestCategory("Deferred")]
        [TestMethod]
        public void Test_EventHandlerExtensions_GettingDeferralCausesAwait()
        {
            var tsc = new TaskCompletionSource<bool>();

            var testClass = new TestClass();

            testClass.TestEvent += async (s, e) =>
            {
                var deferral = e.GetDeferral();

                await tsc.Task;

                deferral.Complete();
            };

            var handlersTask = testClass.RaiseTestEvent();

            Assert.IsFalse(handlersTask.IsCompleted);

            tsc.SetResult(true);

            Assert.IsTrue(handlersTask.IsCompleted);
        }

        [TestCategory("Deferred")]
        [TestMethod]
        public void Test_EventHandlerExtensions_NotGettingDeferralCausesNoAwait()
        {
            var tsc = new TaskCompletionSource<bool>();

            var testClass = new TestClass();

            testClass.TestEvent += async (s, e) =>
            {
                await tsc.Task;
            };

            var handlersTask = testClass.RaiseTestEvent();

            Assert.IsTrue(handlersTask.IsCompleted);

            tsc.SetResult(true);
        }

        [TestCategory("Deferred")]
        [TestMethod]
        public void Test_EventHandlerExtensions_UsingDeferralCausesAwait()
        {
            var tsc = new TaskCompletionSource<bool>();

            var testClass = new TestClass();

            testClass.TestEvent += async (s, e) =>
            {
                using (e.GetDeferral())
                {
                    await tsc.Task;
                }
            };

            var handlersTask = testClass.RaiseTestEvent();

            Assert.IsFalse(handlersTask.IsCompleted);

            tsc.SetResult(true);

            Assert.IsTrue(handlersTask.IsCompleted);
        }

        [TestCategory("Deferred")]
        [DataTestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        public void Test_EventHandlerExtensions_MultipleHandlersCauseAwait(int firstToReleaseDeferral, int lastToReleaseDeferral)
        {
            var tsc = new[]
            {
                new TaskCompletionSource<bool>(),
                new TaskCompletionSource<bool>()
            };

            var testClass = new TestClass();

            testClass.TestEvent += async (s, e) =>
            {
                var deferral = e.GetDeferral();

                await tsc[0].Task;

                deferral.Complete();
            };

            testClass.TestEvent += async (s, e) =>
            {
                var deferral = e.GetDeferral();

                await tsc[1].Task;

                deferral.Complete();
            };

            var handlersTask = testClass.RaiseTestEvent();

            Assert.IsFalse(handlersTask.IsCompleted);

            tsc[firstToReleaseDeferral].SetResult(true);

            Assert.IsFalse(handlersTask.IsCompleted);

            tsc[lastToReleaseDeferral].SetResult(true);

            Assert.IsTrue(handlersTask.IsCompleted);
        }

        private class TestClass
        {
            public event EventHandler<DeferredEventArgs> TestEvent;

            public Task RaiseTestEvent() => TestEvent.InvokeAsync(this, new DeferredEventArgs());
        }
    }
}