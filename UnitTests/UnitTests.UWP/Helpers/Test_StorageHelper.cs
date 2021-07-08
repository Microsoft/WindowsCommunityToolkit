// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using UnitTests.UWP.Helpers;
using Windows.Storage;

namespace UnitTests.Helpers
{
    [TestClass]
    public class Test_StorageHelper
    {
        private LocalObjectStorageHelper _localStorageHelperSystem = new LocalObjectStorageHelper(new SystemSerializer());
        private LocalObjectStorageHelper _localStorageHelperJsonCompat = new LocalObjectStorageHelper(new JsonObjectSerializer());

        private LocalObjectStorageHelper _localStorageHelperJsonNew = new LocalObjectStorageHelper(new SystemTextJsonSerializer());

        /// <summary>
        /// Checks that we're running 10.0.3 version of Newtonsoft.Json package which we used in 6.1.1.
        /// </summary>
        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_StorageHelper_CheckNewtonsoftVersion()
        {
            var version = typeof(JsonSerializer).Assembly.GetName().Version;
            Assert.AreEqual(10, version.Major);
            Assert.AreEqual(0, version.Minor);
            Assert.AreEqual(0, version.Revision); // Apparently the file revision was not updated for the updated package
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_StorageHelper_LegacyIntTest()
        {
            string key = "LifeUniverseAndEverything";

            int input = 42;

            // Use our previous Json layer to store value
            _localStorageHelperJsonCompat.Save(key, input);

            // But try and read from our new system to see if it works
            int output = _localStorageHelperSystem.Read(key, 0);

            Assert.AreEqual(input, output);
        }

        /// <summary>
        /// If we try and deserialize a complex type with the <see cref="SystemSerializer"/>, we do a check ourselves and will throw our own exception.
        /// </summary>
        [TestCategory("Helpers")]
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void Test_StorageHelper_LegacyDateTestFailure()
        {
            string key = "ChristmasDay";

            DateTime input = new DateTime(2017, 12, 25);

            _localStorageHelperJsonCompat.Save(key, input);

            // now read it as int to valid that the change works
            DateTime output = _localStorageHelperSystem.Read(key, DateTime.Today);
        }

        /// <summary>
        /// The <see cref="SystemSerializer"/> doesn't support complex types, since it just passes through directly.
        /// We'll get the argument exception from the <see cref="ApplicationDataContainer"/> API.
        /// </summary>
        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_StorageHelper_DateTestFailure()
        {
            Exception expectedException = null;

            // We can't use standard exception checking here like Assert.Throws or ExpectedException
            // as local and online platforms seem to throw different exception types :(
            try
            {
                _localStorageHelperSystem.Save("Today", DateTime.Today);
            }
            catch (Exception exception)
            {
                expectedException = exception;
            }

            Assert.IsNotNull(expectedException, "Was expecting an Exception.");
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_StorageHelper_LegacyInternalClassTest()
        {
            string key = "Contact";

            UI.Person input = new UI.Person() { Name = "Joe Bloggs", Age = 42 };

            // simulate previous version by generating json and manually inserting it as string
            _localStorageHelperJsonCompat.Save(key, input);

            // now read it as int to valid that the change works
            UI.Person output = _localStorageHelperJsonCompat.Read<UI.Person>(key, null);

            Assert.IsNotNull(output);
            Assert.AreEqual(input.Name, output.Name);
            Assert.AreEqual(input.Age, output.Age);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_StorageHelper_LegacyPublicClassTest()
        {
            string key = "Contact";

            // Here's we're serializing a different class which has the same properties as our other class below.
            UI.Person input = new UI.Person() { Name = "Joe Bloggs", Age = 42 };

            // simulate previous version by generating json and manually inserting it as string
            _localStorageHelperJsonCompat.Save(key, input);

            // now read it as int to valid that the change works
            Person output = _localStorageHelperJsonCompat.Read<Person>(key, null);

            Assert.IsNotNull(output);
            Assert.AreEqual(input.Name, output.Name);
            Assert.AreEqual(input.Age, output.Age);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_StorageHelper_IntTest()
        {
            string key = "NewLifeUniverseAndEverything";

            int input = 42;

            _localStorageHelperSystem.Save<int>(key, input);

            // now read it as int to valid that the change works
            int output = _localStorageHelperSystem.Read<int>(key, 0);

            Assert.AreEqual(input, output);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_StorageHelper_NewDateTest()
        {
            string key = "NewChristmasDay";

            DateTime input = new DateTime(2017, 12, 25);

            _localStorageHelperJsonNew.Save(key, input);

            // now read it as int to valid that the change works
            DateTime output = _localStorageHelperJsonNew.Read(key, DateTime.Today);

            Assert.AreEqual(input, output);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_StorageHelper_NewPersonTest()
        {
            string key = "Contact";

            Person input = new Person() { Name = "Joe Bloggs", Age = 42 };

            _localStorageHelperJsonNew.Save(key, input);

            // now read it as int to valid that the change works
            Person output = _localStorageHelperJsonNew.Read<Person>(key, null);

            Assert.IsNotNull(output);
            Assert.AreEqual(input.Name, output.Name);
            Assert.AreEqual(input.Age, output.Age);
        }

        public class Person
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }
    }
}