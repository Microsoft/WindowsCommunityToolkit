// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using CommunityToolkit.WinUI.Helpers;

namespace UnitTests.Helpers
{
#pragma warning disable CS0612, CS0618 // Type or member is obsolete
    [TestClass]
    public class Test_DispatcherHelper
    {
        [TestCategory("Helpers")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_DispatcherHelper_Action_Null()
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(default(Action));
        }

        [TestCategory("Helpers")]
        [UITestMethod]
        public void Test_DispatcherHelper_Action_Ok_UIThread()
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                var textBlock = new TextBlock { Text = nameof(Test_DispatcherHelper_Action_Ok_UIThread) };

                Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherHelper_Action_Ok_UIThread));
            }).Wait();
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_DispatcherHelper_Action_Ok_NonUIThread()
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                var textBlock = new TextBlock { Text = nameof(Test_DispatcherHelper_Action_Ok_NonUIThread) };

                Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherHelper_Action_Ok_NonUIThread));
            });
        }

        [TestCategory("Helpers")]
        [UITestMethod]
        public void Test_DispatcherHelper_Action_Exception()
        {
            var task = DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                throw new ArgumentException(nameof(this.Test_DispatcherHelper_Action_Exception));
            });

            Assert.IsNotNull(task);
            Assert.AreEqual(task.Status, TaskStatus.Faulted);
            Assert.IsNotNull(task.Exception);
            Assert.IsInstanceOfType(task.Exception.InnerExceptions.FirstOrDefault(), typeof(ArgumentException));
        }

        [TestCategory("Helpers")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_DispatcherHelper_FuncOfT_Null()
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(default(Func<int>));
        }

        [TestCategory("Helpers")]
        [UITestMethod]
        public void Test_DispatcherHelper_FuncOfT_Ok_UIThread()
        {
            var textBlock = DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                return new TextBlock { Text = nameof(Test_DispatcherHelper_FuncOfT_Ok_UIThread) };
            }).Result;

            Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherHelper_FuncOfT_Ok_UIThread));
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_DispatcherHelper_FuncOfT_Ok_NonUIThread()
        {
            var textBlock = await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                return new TextBlock { Text = nameof(Test_DispatcherHelper_FuncOfT_Ok_NonUIThread) };
            });

            await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherHelper_FuncOfT_Ok_NonUIThread));
            });
        }

        [TestCategory("Helpers")]
        [UITestMethod]
        public void Test_DispatcherHelper_FuncOfT_Exception()
        {
            var task = DispatcherHelper.ExecuteOnUIThreadAsync(new Func<int>(() =>
            {
                throw new ArgumentException(nameof(this.Test_DispatcherHelper_FuncOfT_Exception));
            }));

            Assert.IsNotNull(task);
            Assert.AreEqual(task.Status, TaskStatus.Faulted);
            Assert.IsNotNull(task.Exception);
            Assert.IsInstanceOfType(task.Exception.InnerExceptions.FirstOrDefault(), typeof(ArgumentException));
        }

        [TestCategory("Helpers")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [SuppressMessage("Style", "IDE0034", Justification = "Explicit overload for clarity")]
        public void Test_DispatcherHelper_FuncOfTask_Null()
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(default(Func<Task>));
        }

        [TestCategory("Helpers")]
        [UITestMethod]
        public void Test_DispatcherHelper_FuncOfTask_Ok_UIThread()
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                var textBlock = new TextBlock { Text = nameof(Test_DispatcherHelper_FuncOfTask_Ok_UIThread) };

                Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherHelper_FuncOfTask_Ok_UIThread));

                return Task.CompletedTask;
            }).Wait();
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_DispatcherHelper_FuncOfTask_Ok_NonUIThread()
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
            {
                await Task.Yield();

                var textBlock = new TextBlock { Text = nameof(Test_DispatcherHelper_FuncOfTask_Ok_NonUIThread) };

                Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherHelper_FuncOfTask_Ok_NonUIThread));
            });
        }

        [TestCategory("Helpers")]
        [UITestMethod]
        public void Test_DispatcherHelper_FuncOfTask_Exception()
        {
            var task = DispatcherHelper.ExecuteOnUIThreadAsync(new Func<Task>(() =>
            {
                throw new ArgumentException(nameof(this.Test_DispatcherHelper_FuncOfTask_Exception));
            }));

            Assert.IsNotNull(task);
            Assert.AreEqual(task.Status, TaskStatus.Faulted);
            Assert.IsNotNull(task.Exception);
            Assert.IsInstanceOfType(task.Exception.InnerExceptions.FirstOrDefault(), typeof(ArgumentException));
        }

        [TestCategory("Helpers")]
        [UITestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Test_DispatcherHelper_FuncOfTaskOfT_Null()
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(default(Func<Task<int>>));
        }

        [TestCategory("Helpers")]
        [UITestMethod]
        public void Test_DispatcherHelper_FuncOfTaskOfT_Ok_UIThread()
        {
            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                var textBlock = new TextBlock { Text = nameof(Test_DispatcherHelper_FuncOfTaskOfT_Ok_UIThread) };

                Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherHelper_FuncOfTaskOfT_Ok_UIThread));

                return Task.FromResult(1);
            }).Wait();
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_DispatcherHelper_FuncOfTaskOfT_Ok_NonUIThread()
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
            {
                await Task.Yield();

                var textBlock = new TextBlock { Text = nameof(Test_DispatcherHelper_FuncOfTaskOfT_Ok_NonUIThread) };

                Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherHelper_FuncOfTaskOfT_Ok_NonUIThread));

                return textBlock;
            });
        }

        [TestCategory("Helpers")]
        [UITestMethod]
        public void Test_DispatcherHelper_FuncOfTaskOfT_Exception()
        {
            var task = DispatcherHelper.ExecuteOnUIThreadAsync(new Func<Task<int>>(() =>
            {
                throw new ArgumentException(nameof(this.Test_DispatcherHelper_FuncOfTaskOfT_Exception));
            }));

            Assert.IsNotNull(task);
            Assert.AreEqual(task.Status, TaskStatus.Faulted);
            Assert.IsNotNull(task.Exception);
            Assert.IsInstanceOfType(task.Exception.InnerExceptions.FirstOrDefault(), typeof(ArgumentException));
        }
    }
#pragma warning restore CS0612 // Type or member is obsolete
}