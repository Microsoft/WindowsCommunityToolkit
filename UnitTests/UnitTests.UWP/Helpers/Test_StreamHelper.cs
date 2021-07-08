// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.ApplicationModel;
using Windows.Storage;
using CommunityToolkit.WinUI.Helpers;

namespace UnitTests.Helpers
{
    [TestClass]
    public class Test_StreamHelper
    {
        private const string SampleText = "Lorem ipsum dolor sit amet";
        private const string Filename = "Test_StreamHelper_file.txt";
        private const string PackagedFilePath = @"Assets\Samples\lorem.txt";

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StreamHelper_GetHttpStream()
        {
            using (var stream = await new Uri("http://dev.windows.com").GetHttpStreamAsync(default(CancellationToken)))
            {
                Assert.IsNotNull(stream);
            }
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StreamHelper_GetHttpStreamToStorageFile()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync(Filename, CreationCollisionOption.ReplaceExisting);

            await new Uri("http://dev.windows.com").GetHttpStreamToStorageFileAsync(file);

            var properties = await file.GetBasicPropertiesAsync();
            Assert.IsTrue(properties.Size > 0);

            await file.DeleteAsync(StorageDeleteOption.Default);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StreamHelper_PackagedFile()
        {
            var packageFolder = Package.Current.InstalledLocation;
            Assert.IsTrue(await packageFolder.FileExistsAsync(PackagedFilePath));

            using (var stream = await StreamHelper.GetPackagedFileStreamAsync(PackagedFilePath))
            {
                var loadedText = await stream.ReadTextAsync(Encoding.Unicode);
                StringAssert.Contains(loadedText, SampleText);
            }
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StreamHelper_LocalFolder()
        {
            var localFolder = ApplicationData.Current.LocalFolder;
            Assert.IsFalse(await localFolder.FileExistsAsync(Filename));

            var storageFile = await StorageFileHelper.WriteTextToLocalFileAsync(SampleText, Filename);
            Assert.IsNotNull(storageFile);

            Assert.IsTrue(await localFolder.FileExistsAsync(Filename));

            using (var stream = await StreamHelper.GetLocalFileStreamAsync(Filename))
            {
                var loadedText = await stream.ReadTextAsync(Encoding.UTF8);
                StringAssert.Contains(loadedText, SampleText);
            }

            await storageFile.DeleteAsync(StorageDeleteOption.Default);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StreamHelper_LocalCacheFolder()
        {
            var localCacheFolder = ApplicationData.Current.LocalCacheFolder;
            Assert.IsFalse(await localCacheFolder.FileExistsAsync(Filename));

            var storageFile = await StorageFileHelper.WriteTextToLocalCacheFileAsync(SampleText, Filename);
            Assert.IsNotNull(storageFile);

            Assert.IsTrue(await localCacheFolder.FileExistsAsync(Filename));

            using (var stream = await StreamHelper.GetLocalCacheFileStreamAsync(Filename))
            {
                var loadedText = await stream.ReadTextAsync(Encoding.UTF8);
                StringAssert.Contains(loadedText, SampleText);
            }

            await storageFile.DeleteAsync(StorageDeleteOption.Default);
        }
    }
}