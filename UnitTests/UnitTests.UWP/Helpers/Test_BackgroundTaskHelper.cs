// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using CommunityToolkit.WinUI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.ApplicationModel.Background;

namespace UnitTests.Helpers
{
    [TestClass]
    public class Test_BackgroundTaskHelper
    {
        [TestCleanup]
        public void TestCleanUp()
        {
            if (BackgroundTaskHelper.IsBackgroundTaskRegistered("TaskName"))
            {
                BackgroundTaskHelper.Unregister("TaskName");
            }
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_BackgroundTaskHelper_Register_NoConditions()
        {
            Assert.IsNotNull(BackgroundTaskHelper.Register("TaskName", new TimeTrigger(15, true)));
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_BackgroundTaskHelper_Register_WithConditions()
        {
            Assert.IsNotNull(BackgroundTaskHelper.Register("TaskName", new TimeTrigger(15, true), false, true, new SystemCondition(SystemConditionType.InternetAvailable), new SystemCondition(SystemConditionType.UserPresent)));
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_BackgroundTaskHelper_Unregister()
        {
            BackgroundTaskHelper.Register("TaskName", new TimeTrigger(15, true));

            if (!BackgroundTaskHelper.IsBackgroundTaskRegistered("TaskName"))
            {
                Assert.Inconclusive("Task failed to register!");
            }

            BackgroundTaskHelper.Unregister("TaskName");

            Assert.IsFalse(BackgroundTaskHelper.IsBackgroundTaskRegistered("TaskName"));
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_BackgroundTaskHelper_IsBackgroundTaskRegistered_NoTask()
        {
            Assert.IsFalse(BackgroundTaskHelper.IsBackgroundTaskRegistered("TaskName"));
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_BackgroundTaskHelper_IsBackgroundTaskRegistered_WithValidTask()
        {
            BackgroundTaskRegistration registeredTask = BackgroundTaskHelper.Register("TaskName", new TimeTrigger(15, true));

            if (registeredTask == null)
            {
                Assert.Inconclusive("Task failed to register");
            }

            Assert.IsTrue(BackgroundTaskHelper.IsBackgroundTaskRegistered("TaskName"));
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_BackgroundTaskHelper_GetBackgroundTask_NoTask()
        {
            Assert.IsNull(BackgroundTaskHelper.GetBackgroundTask("TaskName"));
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_BackgroundTaskHelper_GetBackgroundTask_WithValidTask()
        {
            BackgroundTaskRegistration registeredTask = BackgroundTaskHelper.Register("TaskName", new TimeTrigger(15, true));

            if (registeredTask == null)
            {
                Assert.Inconclusive("Task failed to register");
            }

            Assert.IsNotNull(BackgroundTaskHelper.GetBackgroundTask("TaskName"));
        }
    }
}