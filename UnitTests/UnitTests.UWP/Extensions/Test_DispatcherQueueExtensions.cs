// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using CommunityToolkit.WinUI;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace UnitTests.Extensions
{
    [TestClass]
    [Ignore("Ignored until issue on .Net Native is fixed. These are working.")]
    public class Test_DispatcherQueueExtensions
    {
        private const int TIME_OUT = 5000;

        [TestCategory("DispatcherQueueExtensions")]
        [UITestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Test_DispatcherQueueHelper_Action_Null()
        {
            DispatcherQueue.GetForCurrentThread().EnqueueAsync(default(Action)!);
        }

        [TestCategory("DispatcherQueueExtensions")]
        [UITestMethod]
        public void Test_DispatcherQueueHelper_Action_Ok_UIThread()
        {
            DispatcherQueue.GetForCurrentThread().EnqueueAsync(() =>
            {
                var textBlock = new TextBlock { Text = nameof(Test_DispatcherQueueHelper_Action_Ok_UIThread) };

                Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherQueueHelper_Action_Ok_UIThread));
            }).Wait();
        }

        [TestCategory("DispatcherQueueExtensions")]
        [TestMethod]
        public async Task Test_DispatcherQueueHelper_Action_Ok_NonUIThread()
        {
            var taskSource = new TaskCompletionSource<object>();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
                        await Task.Run(async () =>
                        {
                            await dispatcherQueue.EnqueueAsync(() =>
                            {
                                var textBlock = new TextBlock { Text = nameof(Test_DispatcherQueueHelper_Action_Ok_NonUIThread) };

                                Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherQueueHelper_Action_Ok_NonUIThread));

                                taskSource.SetResult(null);
                            });
                        });
                    }
                    catch (Exception e)
                    {
                        taskSource.SetException(e);
                    }
                });
            await taskSource.Task;
        }

        [TestCategory("DispatcherQueueExtensions")]
        [UITestMethod]
        public void Test_DispatcherQueueHelper_Action_Exception()
        {
            var task = DispatcherQueue.GetForCurrentThread().EnqueueAsync(() =>
            {
                throw new ArgumentException(nameof(this.Test_DispatcherQueueHelper_Action_Exception));
            });

            Assert.IsNotNull(task);
            Assert.AreEqual(task.Status, TaskStatus.Faulted);
            Assert.IsNotNull(task.Exception);
            Assert.IsInstanceOfType(task.Exception.InnerExceptions.FirstOrDefault(), typeof(ArgumentException));
        }

        [TestCategory("DispatcherQueueExtensions")]
        [UITestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Test_DispatcherQueueHelper_FuncOfT_Null()
        {
            DispatcherQueue.GetForCurrentThread().EnqueueAsync(default(Func<int>)!);
        }

        [TestCategory("DispatcherQueueExtensions")]
        [UITestMethod]
        public void Test_DispatcherQueueHelper_FuncOfT_Ok_UIThread()
        {
            var textBlock = DispatcherQueue.GetForCurrentThread().EnqueueAsync(() =>
            {
                return new TextBlock { Text = nameof(Test_DispatcherQueueHelper_FuncOfT_Ok_UIThread) };
            }).Result;

            Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherQueueHelper_FuncOfT_Ok_UIThread));
        }

        [TestCategory("DispatcherQueueExtensions")]
        [TestMethod]
        public async Task Test_DispatcherQueueHelper_FuncOfT_Ok_NonUIThread()
        {
            var taskSource = new TaskCompletionSource<object>();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
                        await Task.Run(async () =>
                        {
                            var textBlock = await dispatcherQueue.EnqueueAsync(() =>
                            {
                                return new TextBlock { Text = nameof(Test_DispatcherQueueHelper_FuncOfT_Ok_NonUIThread) };
                            });
                            await dispatcherQueue.EnqueueAsync(() =>
                            {
                                Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherQueueHelper_FuncOfT_Ok_NonUIThread));
                                taskSource.SetResult(null);
                            });
                        });
                    }
                    catch (Exception e)
                    {
                        taskSource.SetException(e);
                    }
                });
            await taskSource.Task;
        }

        [TestCategory("DispatcherQueueExtensions")]
        [UITestMethod]
        public void Test_DispatcherQueueHelper_FuncOfT_Exception()
        {
            var task = DispatcherQueue.GetForCurrentThread().EnqueueAsync(new Func<int>(() =>
            {
                throw new ArgumentException(nameof(this.Test_DispatcherQueueHelper_FuncOfT_Exception));
            }));

            Assert.IsNotNull(task);
            Assert.AreEqual(task.Status, TaskStatus.Faulted);
            Assert.IsNotNull(task.Exception);
            Assert.IsInstanceOfType(task.Exception.InnerExceptions.FirstOrDefault(), typeof(ArgumentException));
        }

        [TestCategory("DispatcherQueueExtensions")]
        [UITestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Test_DispatcherQueueHelper_FuncOfTask_Null()
        {
            DispatcherQueue.GetForCurrentThread().EnqueueAsync(default(Func<Task>)!);
        }

        [TestCategory("Helpers")]
        [UITestMethod]
        public void Test_DispatcherQueueHelper_FuncOfTask_Ok_UIThread()
        {
            DispatcherQueue.GetForCurrentThread().EnqueueAsync(() =>
            {
                var textBlock = new TextBlock { Text = nameof(Test_DispatcherQueueHelper_FuncOfTask_Ok_UIThread) };

                Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherQueueHelper_FuncOfTask_Ok_UIThread));

                return Task.CompletedTask;
            }).Wait();
        }

        [TestCategory("DispatcherQueueExtensions")]
        [TestMethod]
        public async Task Test_DispatcherQueueHelper_FuncOfTask_Ok_NonUIThread()
        {
            var taskSource = new TaskCompletionSource<object>();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        await DispatcherQueue.GetForCurrentThread().EnqueueAsync(async () =>
                        {
                            await Task.Yield();

                            var textBlock = new TextBlock { Text = nameof(Test_DispatcherQueueHelper_FuncOfTask_Ok_NonUIThread) };

                            Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherQueueHelper_FuncOfTask_Ok_NonUIThread));

                            taskSource.SetResult(null);
                        });
                    }
                    catch (Exception e)
                    {
                        taskSource.SetException(e);
                    }
                });
            await taskSource.Task;
        }

        [TestCategory("DispatcherQueueExtensions")]
        [UITestMethod]
        public void Test_DispatcherQueueHelper_FuncOfTask_Exception()
        {
            var task = DispatcherQueue.GetForCurrentThread().EnqueueAsync(new Func<Task>(() =>
            {
                throw new ArgumentException(nameof(this.Test_DispatcherQueueHelper_FuncOfTask_Exception));
            }));

            Assert.IsNotNull(task);
            Assert.AreEqual(task.Status, TaskStatus.Faulted);
            Assert.IsNotNull(task.Exception);
            Assert.IsInstanceOfType(task.Exception.InnerExceptions.FirstOrDefault(), typeof(ArgumentException));
        }

        [TestCategory("DispatcherQueueExtensions")]
        [UITestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Test_DispatcherQueueHelper_FuncOfTaskOfT_Null()
        {
            DispatcherQueue.GetForCurrentThread().EnqueueAsync(default(Func<Task<int>>)!);
        }

        [TestCategory("DispatcherQueueExtensions")]
        [UITestMethod]
        public void Test_DispatcherQueueHelper_FuncOfTaskOfT_Ok_UIThread()
        {
            DispatcherQueue.GetForCurrentThread().EnqueueAsync(() =>
            {
                var textBlock = new TextBlock { Text = nameof(Test_DispatcherQueueHelper_FuncOfTaskOfT_Ok_UIThread) };

                Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherQueueHelper_FuncOfTaskOfT_Ok_UIThread));

                return Task.FromResult(1);
            }).Wait();
        }

        [TestCategory("DispatcherQueueExtensions")]
        [TestMethod]
        public async Task Test_DispatcherQueueHelper_FuncOfTaskOfT_Ok_NonUIThread()
        {
            var taskSource = new TaskCompletionSource<object>();
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, async () =>
                {
                    try
                    {
                        await DispatcherQueue.GetForCurrentThread().EnqueueAsync(async () =>
                        {
                            await Task.Yield();

                            var textBlock = new TextBlock { Text = nameof(Test_DispatcherQueueHelper_FuncOfTaskOfT_Ok_NonUIThread) };

                            Assert.AreEqual(textBlock.Text, nameof(Test_DispatcherQueueHelper_FuncOfTaskOfT_Ok_NonUIThread));

                            taskSource.SetResult(null);

                            return textBlock;
                        });
                    }
                    catch (Exception e)
                    {
                        taskSource.SetException(e);
                    }
                });
            await taskSource.Task;
        }

        [TestCategory("DispatcherQueueExtensions")]
        [UITestMethod]
        public void Test_DispatcherQueueHelper_FuncOfTaskOfT_Exception()
        {
            var task = DispatcherQueue.GetForCurrentThread().EnqueueAsync(new Func<Task<int>>(() =>
            {
                throw new ArgumentException(nameof(this.Test_DispatcherQueueHelper_FuncOfTaskOfT_Exception));
            }));

            Assert.IsNotNull(task);
            Assert.AreEqual(task.Status, TaskStatus.Faulted);
            Assert.IsNotNull(task.Exception);
            Assert.IsInstanceOfType(task.Exception.InnerExceptions.FirstOrDefault(), typeof(ArgumentException));
        }
    }
}