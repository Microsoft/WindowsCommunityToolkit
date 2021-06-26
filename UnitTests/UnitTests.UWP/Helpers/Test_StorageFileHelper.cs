// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Windows.Storage;
using Microsoft.Toolkit.Uwp.Helpers;

namespace UnitTests.Helpers
{
    [TestClass]
    public class Test_StorageFileHelper
    {
        private const string Sampletext = "Lorem ipsum dolor sit amet";
        private const string Filename = "Test_StorageFileHelper_file.txt";
        private const string PackagedFilePath = @"Assets\Samples\lorem.txt";

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StorageFileHelper_Text_PackagedFile()
        {
            string loadedText = await StorageFileHelper.ReadTextFromPackagedFileAsync(PackagedFilePath);
            StringAssert.Contains(loadedText, Sampletext);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StorageFileHelper_Text_LocalFolder()
        {
            var storageFile = await StorageFileHelper.WriteTextToLocalFileAsync(Sampletext, Filename);
            Assert.IsNotNull(storageFile);

            string loadedText = await StorageFileHelper.ReadTextFromLocalFileAsync(Filename);
            Assert.AreEqual(Sampletext, loadedText);

            await storageFile.DeleteAsync(StorageDeleteOption.Default);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StorageFileHelper_Text_LocalCacheFolder()
        {
            var storageFile = await StorageFileHelper.WriteTextToLocalCacheFileAsync(Sampletext, Filename);
            Assert.IsNotNull(storageFile);

            string loadedText = await StorageFileHelper.ReadTextFromLocalCacheFileAsync(Filename);
            Assert.AreEqual(Sampletext, loadedText);

            await storageFile.DeleteAsync(StorageDeleteOption.Default);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StorageFileHelper_Text_KnownFolder()
        {
            var storageFile = await StorageFileHelper.WriteTextToKnownFolderFileAsync(KnownFolderId.PicturesLibrary, Sampletext, Filename);
            Assert.IsNotNull(storageFile);

            string loadedText = await StorageFileHelper.ReadTextFromKnownFoldersFileAsync(KnownFolderId.PicturesLibrary, Filename);
            Assert.AreEqual(Sampletext, loadedText);

            await storageFile.DeleteAsync(StorageDeleteOption.Default);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StorageFileHelper_Text_StorageFolder()
        {
            var folder = ApplicationData.Current.LocalFolder;
            var storageFile = await folder.WriteTextToFileAsync(Sampletext, Filename);
            Assert.IsNotNull(storageFile);

            var loadedText = await folder.ReadTextFromFileAsync(Filename);
            Assert.AreEqual(Sampletext, loadedText);

            await storageFile.DeleteAsync(StorageDeleteOption.Default);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StorageFileHelper_Bytes_PackagedFile()
        {
            byte[] loadedBytes = await StorageFileHelper.ReadBytesFromPackagedFileAsync(PackagedFilePath);

            string loadedText = Encoding.Unicode.GetString(loadedBytes);
            StringAssert.Contains(loadedText, Sampletext);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StorageFileHelper_Bytes_LocalFolder()
        {
            byte[] unicodeBytes = Encoding.Unicode.GetBytes(Sampletext);

            var storageFile = await StorageFileHelper.WriteBytesToLocalFileAsync(unicodeBytes, Filename);
            Assert.IsNotNull(storageFile);

            byte[] loadedBytes = await StorageFileHelper.ReadBytesFromLocalFileAsync(Filename);

            string loadedText = Encoding.Unicode.GetString(loadedBytes);
            Assert.AreEqual(Sampletext, loadedText);

            await storageFile.DeleteAsync(StorageDeleteOption.Default);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StorageFileHelper_Bytes_LocalCacheFolder()
        {
            byte[] unicodeBytes = Encoding.Unicode.GetBytes(Sampletext);

            var storageFile = await StorageFileHelper.WriteBytesToLocalCacheFileAsync(unicodeBytes, Filename);
            Assert.IsNotNull(storageFile);

            byte[] loadedBytes = await StorageFileHelper.ReadBytesFromLocalCacheFileAsync(Filename);

            string loadedText = Encoding.Unicode.GetString(loadedBytes);
            Assert.AreEqual(Sampletext, loadedText);

            await storageFile.DeleteAsync(StorageDeleteOption.Default);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StorageFileHelper_Bytes_KnownFolder()
        {
            byte[] unicodeBytes = Encoding.Unicode.GetBytes(Sampletext);

            var storageFile = await StorageFileHelper.WriteBytesToKnownFolderFileAsync(KnownFolderId.PicturesLibrary, unicodeBytes, Filename);
            Assert.IsNotNull(storageFile);

            byte[] loadedBytes = await StorageFileHelper.ReadBytesFromKnownFoldersFileAsync(KnownFolderId.PicturesLibrary, Filename);

            string loadedText = Encoding.Unicode.GetString(loadedBytes);
            Assert.AreEqual(Sampletext, loadedText);

            await storageFile.DeleteAsync(StorageDeleteOption.Default);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StorageFileHelper_Bytes_StorageFolder()
        {
            byte[] unicodeBytes = Encoding.Unicode.GetBytes(Sampletext);

            var folder = ApplicationData.Current.LocalFolder;
            var storageFile = await folder.WriteBytesToFileAsync(unicodeBytes, Filename);
            Assert.IsNotNull(storageFile);

            byte[] loadedBytes = await folder.ReadBytesFromFileAsync(Filename);

            string loadedText = Encoding.Unicode.GetString(loadedBytes);
            Assert.AreEqual(Sampletext, loadedText);

            await storageFile.DeleteAsync(StorageDeleteOption.Default);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StorageFileHelper_FileExists()
        {
            byte[] unicodeBytes = Encoding.Unicode.GetBytes(Sampletext);

            var folder = ApplicationData.Current.LocalFolder;
            var storageFile = await folder.WriteBytesToFileAsync(unicodeBytes, Filename);
            Assert.IsNotNull(storageFile);

            var exists = await folder.FileExistsAsync(Filename);
            Assert.IsTrue(exists);

            await storageFile.DeleteAsync(StorageDeleteOption.Default);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public async Task Test_StorageFileHelper_FileExists_Recursive()
        {
            byte[] unicodeBytes = Encoding.Unicode.GetBytes(Sampletext);

            var folder = ApplicationData.Current.LocalFolder;
            var subfolder = await folder.CreateFolderAsync("subfolder");
            Assert.IsNotNull(subfolder);

            var storageFile = await subfolder.WriteBytesToFileAsync(unicodeBytes, Filename);
            Assert.IsNotNull(storageFile);

            var exists = await folder.FileExistsAsync(Filename, true);
            Assert.IsTrue(exists);

            await storageFile.DeleteAsync(StorageDeleteOption.Default);
            await subfolder.DeleteAsync(StorageDeleteOption.Default);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_StorageFileHelper_IsFileNameValid_WithCorrectFileName()
        {
            string filename = "my_file.txt";

            bool result = StorageFileHelper.IsFileNameValid(filename);

            Assert.IsTrue(result);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_StorageFileHelper_IsFileNameValid_WithIllegalCharacters()
        {
            string filename = "my|file.txt";

            bool result = StorageFileHelper.IsFileNameValid(filename);

            Assert.IsFalse(result);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_StorageFileHelper_IsFilePathValid_WithCorrectFilePath()
        {
            string filepath = "my_folder/my_file.txt";

            bool result = StorageFileHelper.IsFilePathValid(filepath);

            Assert.IsTrue(result);
        }

        [TestCategory("Helpers")]
        [TestMethod]
        public void Test_StorageFileHelper_IsFilePathValid_WithIllegalCharacters()
        {
            string filepath = "my>folder|my<file\t.txt";

            bool result = StorageFileHelper.IsFilePathValid(filepath);

            Assert.IsFalse(result);
        }
    }
}