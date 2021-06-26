// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.ApplicationModel;
using Windows.Storage;

namespace UnitTests.Helpers
{
    [TestClass]
    public class Test_SystemInformation
    {
        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_SystemInformation_ConsistentInfoForFirstStartup()
        {
            ApplicationData.Current.LocalSettings.Values.Clear();

            var systemInformation = (SystemInformation)Activator.CreateInstance(typeof(SystemInformation), nonPublic: true);
            var currentAppVersion = Package.Current.Id.Version;

            Assert.IsTrue(systemInformation.IsFirstRun);
            Assert.IsFalse(systemInformation.IsAppUpdated);
            Assert.AreEqual(systemInformation.ApplicationVersion, currentAppVersion);
            Assert.AreEqual(systemInformation.PreviousVersionInstalled, currentAppVersion);
            Assert.AreEqual(systemInformation.FirstVersionInstalled, currentAppVersion);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_SystemInformation_ConsistentInfoForStartupWithNoUpdate()
        {
            ApplicationData.Current.LocalSettings.Values.Clear();

            // Simulate a first app startup
            _ = (SystemInformation)Activator.CreateInstance(typeof(SystemInformation), nonPublic: true);

            var systemInformation = (SystemInformation)Activator.CreateInstance(typeof(SystemInformation), nonPublic: true);
            var currentAppVersion = Package.Current.Id.Version;

            Assert.IsFalse(systemInformation.IsFirstRun);
            Assert.IsFalse(systemInformation.IsAppUpdated);
            Assert.AreEqual(systemInformation.ApplicationVersion, currentAppVersion);
            Assert.AreEqual(systemInformation.PreviousVersionInstalled, currentAppVersion);
            Assert.AreEqual(systemInformation.FirstVersionInstalled, currentAppVersion);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_SystemInformation_ConsistentInfoForStartupWithUpdate()
        {
            ApplicationData.Current.LocalSettings.Values.Clear();

            // Simulate a first app startup
            _ = (SystemInformation)Activator.CreateInstance(typeof(SystemInformation), nonPublic: true);

            LocalObjectStorageHelper localObjectStorageHelper = new(new SystemSerializer());
            PackageVersion previousVersion = new() { Build = 42, Major = 1111, Minor = 2222, Revision = 12345 };

            localObjectStorageHelper.Save("currentVersion", previousVersion.ToFormattedString());

            var systemInformation = (SystemInformation)Activator.CreateInstance(typeof(SystemInformation), nonPublic: true);
            var currentAppVersion = Package.Current.Id.Version;

            Assert.IsFalse(systemInformation.IsFirstRun);
            Assert.IsTrue(systemInformation.IsAppUpdated);
            Assert.AreEqual(systemInformation.ApplicationVersion, currentAppVersion);
            Assert.AreEqual(systemInformation.PreviousVersionInstalled, previousVersion);
            Assert.AreEqual(systemInformation.FirstVersionInstalled, currentAppVersion);
        }
    }
}