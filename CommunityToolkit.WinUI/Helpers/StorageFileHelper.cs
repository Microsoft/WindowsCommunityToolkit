// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;

namespace CommunityToolkit.WinUI.Helpers
{
    /// <summary>
    /// This class provides static helper methods for <see cref="StorageFile" />.
    /// </summary>
    public static class StorageFileHelper
    {
        /// <summary>
        /// Saves a string value to a <see cref="StorageFile"/> in application local folder/>.
        /// </summary>
        /// <param name="text">
        /// The <see cref="string"/> value to save to the file.
        /// </param>
        /// <param name="fileName">
        /// The <see cref="string"/> name for the file.
        /// </param>
        /// <param name="options">
        /// The creation collision options. Default is ReplaceExisting.
        /// </param>
        /// <returns>
        /// The saved <see cref="StorageFile"/> containing the text.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the file location or file name are null or empty.
        /// </exception>
        public static Task<StorageFile> WriteTextToLocalFileAsync(
            string text,
            string fileName,
            CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var folder = ApplicationData.Current.LocalFolder;
            return folder.WriteTextToFileAsync(text, fileName, options);
        }

        /// <summary>
        /// Saves a string value to a <see cref="StorageFile"/> in application local cache folder/>.
        /// </summary>
        /// <param name="text">
        /// The <see cref="string"/> value to save to the file.
        /// </param>
        /// <param name="fileName">
        /// The <see cref="string"/> name for the file.
        /// </param>
        /// <param name="options">
        /// The creation collision options. Default is ReplaceExisting.
        /// </param>
        /// <returns>
        /// The saved <see cref="StorageFile"/> containing the text.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the file location or file name are null or empty.
        /// </exception>
        public static Task<StorageFile> WriteTextToLocalCacheFileAsync(
            string text,
            string fileName,
            CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var folder = ApplicationData.Current.LocalCacheFolder;
            return folder.WriteTextToFileAsync(text, fileName, options);
        }

        /// <summary>
        /// Saves a string value to a <see cref="StorageFile"/> in well known folder/>.
        /// </summary>
        /// <param name="knownFolderId">
        /// The well known folder ID to use.
        /// </param>
        /// <param name="text">
        /// The <see cref="string"/> value to save to the file.
        /// </param>
        /// <param name="fileName">
        /// The <see cref="string"/> name for the file.
        /// </param>
        /// <param name="options">
        /// The creation collision options. Default is ReplaceExisting.
        /// </param>
        /// <returns>
        /// The saved <see cref="StorageFile"/> containing the text.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the file location or file name are null or empty.
        /// </exception>
        public static Task<StorageFile> WriteTextToKnownFolderFileAsync(
            KnownFolderId knownFolderId,
            string text,
            string fileName,
            CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var folder = GetFolderFromKnownFolderId(knownFolderId);
            return folder.WriteTextToFileAsync(text, fileName, options);
        }

        /// <summary>
        /// Saves a string value to a <see cref="StorageFile"/> in the given <see cref="StorageFolder"/>.
        /// </summary>
        /// <param name="fileLocation">
        /// The <see cref="StorageFolder"/> to save the file in.
        /// </param>
        /// <param name="text">
        /// The <see cref="string"/> value to save to the file.
        /// </param>
        /// <param name="fileName">
        /// The <see cref="string"/> name for the file.
        /// </param>
        /// <param name="options">
        /// The creation collision options. Default is ReplaceExisting.
        /// </param>
        /// <returns>
        /// The saved <see cref="StorageFile"/> containing the text.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the file location or file name are null or empty.
        /// </exception>
        public static async Task<StorageFile> WriteTextToFileAsync(
            this StorageFolder fileLocation,
            string text,
            string fileName,
            CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
        {
            if (fileLocation == null)
            {
                throw new ArgumentNullException(nameof(fileLocation));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var storageFile = await fileLocation.CreateFileAsync(fileName, options);
            await FileIO.WriteTextAsync(storageFile, text);

            return storageFile;
        }

        /// <summary>
        /// Saves an array of bytes to a <see cref="StorageFile"/> to application local folder/>.
        /// </summary>
        /// <param name="bytes">
        /// The <see cref="byte"/> array to save to the file.
        /// </param>
        /// <param name="fileName">
        /// The <see cref="string"/> name for the file.
        /// </param>
        /// <param name="options">
        /// The creation collision options. Default is ReplaceExisting.
        /// </param>
        /// <returns>
        /// The saved <see cref="StorageFile"/> containing the bytes.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the file location or file name are null or empty.
        /// </exception>
        public static Task<StorageFile> WriteBytesToLocalFileAsync(
            byte[] bytes,
            string fileName,
            CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var folder = ApplicationData.Current.LocalFolder;
            return folder.WriteBytesToFileAsync(bytes, fileName, options);
        }

        /// <summary>
        /// Saves an array of bytes to a <see cref="StorageFile"/> to application local cache folder/>.
        /// </summary>
        /// <param name="bytes">
        /// The <see cref="byte"/> array to save to the file.
        /// </param>
        /// <param name="fileName">
        /// The <see cref="string"/> name for the file.
        /// </param>
        /// <param name="options">
        /// The creation collision options. Default is ReplaceExisting.
        /// </param>
        /// <returns>
        /// The saved <see cref="StorageFile"/> containing the bytes.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the file location or file name are null or empty.
        /// </exception>
        public static Task<StorageFile> WriteBytesToLocalCacheFileAsync(
            byte[] bytes,
            string fileName,
            CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var folder = ApplicationData.Current.LocalCacheFolder;
            return folder.WriteBytesToFileAsync(bytes, fileName, options);
        }

        /// <summary>
        /// Saves an array of bytes to a <see cref="StorageFile"/> to well known folder/>.
        /// </summary>
        /// <param name="knownFolderId">
        /// The well known folder ID to use.
        /// </param>
        /// <param name="bytes">
        /// The <see cref="byte"/> array to save to the file.
        /// </param>
        /// <param name="fileName">
        /// The <see cref="string"/> name for the file.
        /// </param>
        /// <param name="options">
        /// The creation collision options. Default is ReplaceExisting.
        /// </param>
        /// <returns>
        /// The saved <see cref="StorageFile"/> containing the bytes.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the file location or file name are null or empty.
        /// </exception>
        public static Task<StorageFile> WriteBytesToKnownFolderFileAsync(
            KnownFolderId knownFolderId,
            byte[] bytes,
            string fileName,
            CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var folder = GetFolderFromKnownFolderId(knownFolderId);
            return folder.WriteBytesToFileAsync(bytes, fileName, options);
        }

        /// <summary>
        /// Saves an array of bytes to a <see cref="StorageFile"/> in the given <see cref="StorageFolder"/>.
        /// </summary>
        /// <param name="fileLocation">
        /// The <see cref="StorageFolder"/> to save the file in.
        /// </param>
        /// <param name="bytes">
        /// The <see cref="byte"/> array to save to the file.
        /// </param>
        /// <param name="fileName">
        /// The <see cref="string"/> name for the file.
        /// </param>
        /// <param name="options">
        /// The creation collision options. Default is ReplaceExisting.
        /// </param>
        /// <returns>
        /// The saved <see cref="StorageFile"/> containing the bytes.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the file location or file name are null or empty.
        /// </exception>
        public static async Task<StorageFile> WriteBytesToFileAsync(
            this StorageFolder fileLocation,
            byte[] bytes,
            string fileName,
            CreationCollisionOption options = CreationCollisionOption.ReplaceExisting)
        {
            if (fileLocation == null)
            {
                throw new ArgumentNullException(nameof(fileLocation));
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var storageFile = await fileLocation.CreateFileAsync(fileName, options);
            await FileIO.WriteBytesAsync(storageFile, bytes);

            return storageFile;
        }

        /// <summary>
        /// Gets a string value from a <see cref="StorageFile"/> located in the application installation folder.
        /// </summary>
        /// <param name="fileName">
        /// The relative <see cref="string"/> file path.
        /// </param>
        /// <returns>
        /// The stored <see cref="string"/> value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the <paramref name="fileName"/> is null or empty.
        /// </exception>
        public static Task<string> ReadTextFromPackagedFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var folder = Package.Current.InstalledLocation;
            return folder.ReadTextFromFileAsync(fileName);
        }

        /// <summary>
        /// Gets a string value from a <see cref="StorageFile"/> located in the application local cache folder.
        /// </summary>
        /// <param name="fileName">
        /// The relative <see cref="string"/> file path.
        /// </param>
        /// <returns>
        /// The stored <see cref="string"/> value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the <paramref name="fileName"/> is null or empty.
        /// </exception>
        public static Task<string> ReadTextFromLocalCacheFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var folder = ApplicationData.Current.LocalCacheFolder;
            return folder.ReadTextFromFileAsync(fileName);
        }

        /// <summary>
        /// Gets a string value from a <see cref="StorageFile"/> located in the application local folder.
        /// </summary>
        /// <param name="fileName">
        /// The relative <see cref="string"/> file path.
        /// </param>
        /// <returns>
        /// The stored <see cref="string"/> value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the <paramref name="fileName"/> is null or empty.
        /// </exception>
        public static Task<string> ReadTextFromLocalFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var folder = ApplicationData.Current.LocalFolder;
            return folder.ReadTextFromFileAsync(fileName);
        }

        /// <summary>
        /// Gets a string value from a <see cref="StorageFile"/> located in a well known folder.
        /// </summary>
        /// <param name="knownFolderId">
        /// The well known folder ID to use.
        /// </param>
        /// <param name="fileName">
        /// The relative <see cref="string"/> file path.
        /// </param>
        /// <returns>
        /// The stored <see cref="string"/> value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the <paramref name="fileName"/> is null or empty.
        /// </exception>
        public static Task<string> ReadTextFromKnownFoldersFileAsync(
            KnownFolderId knownFolderId,
            string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var folder = GetFolderFromKnownFolderId(knownFolderId);
            return folder.ReadTextFromFileAsync(fileName);
        }

        /// <summary>
        /// Gets a string value from a <see cref="StorageFile"/> located in the given <see cref="StorageFolder"/>.
        /// </summary>
        /// <param name="fileLocation">
        /// The <see cref="StorageFolder"/> to save the file in.
        /// </param>
        /// <param name="fileName">
        /// The relative <see cref="string"/> file path.
        /// </param>
        /// <returns>
        /// The stored <see cref="string"/> value.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the <paramref name="fileName"/> is null or empty.
        /// </exception>
        public static async Task<string> ReadTextFromFileAsync(
            this StorageFolder fileLocation,
            string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var file = await fileLocation.GetFileAsync(fileName);
            return await FileIO.ReadTextAsync(file);
        }

        /// <summary>
        /// Gets an array of bytes from a <see cref="StorageFile"/> located in the application installation folder.
        /// </summary>
        /// <param name="fileName">
        /// The relative <see cref="string"/> file path.
        /// </param>
        /// <returns>
        /// The stored <see cref="byte"/> array.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the <paramref name="fileName"/> is null or empty.
        /// </exception>
        public static Task<byte[]> ReadBytesFromPackagedFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var folder = Package.Current.InstalledLocation;
            return folder.ReadBytesFromFileAsync(fileName);
        }

        /// <summary>
        /// Gets an array of bytes from a <see cref="StorageFile"/> located in the application local cache folder.
        /// </summary>
        /// <param name="fileName">
        /// The relative <see cref="string"/> file path.
        /// </param>
        /// <returns>
        /// The stored <see cref="byte"/> array.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the <paramref name="fileName"/> is null or empty.
        /// </exception>
        public static Task<byte[]> ReadBytesFromLocalCacheFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var folder = ApplicationData.Current.LocalCacheFolder;
            return folder.ReadBytesFromFileAsync(fileName);
        }

        /// <summary>
        /// Gets an array of bytes from a <see cref="StorageFile"/> located in the application local folder.
        /// </summary>
        /// <param name="fileName">
        /// The relative <see cref="string"/> file path.
        /// </param>
        /// <returns>
        /// The stored <see cref="byte"/> array.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the <paramref name="fileName"/> is null or empty.
        /// </exception>
        public static Task<byte[]> ReadBytesFromLocalFileAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var folder = ApplicationData.Current.LocalFolder;
            return folder.ReadBytesFromFileAsync(fileName);
        }

        /// <summary>
        /// Gets an array of bytes from a <see cref="StorageFile"/> located in a well known folder.
        /// </summary>
        /// <param name="knownFolderId">
        /// The well known folder ID to use.
        /// </param>
        /// <param name="fileName">
        /// The relative <see cref="string"/> file path.
        /// </param>
        /// <returns>
        /// The stored <see cref="byte"/> array.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the <paramref name="fileName"/> is null or empty.
        /// </exception>
        public static Task<byte[]> ReadBytesFromKnownFoldersFileAsync(
            KnownFolderId knownFolderId,
            string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var folder = GetFolderFromKnownFolderId(knownFolderId);
            return folder.ReadBytesFromFileAsync(fileName);
        }

        /// <summary>
        /// Gets an array of bytes from a <see cref="StorageFile"/> located in the given <see cref="StorageFolder"/>.
        /// </summary>
        /// <param name="fileLocation">
        /// The <see cref="StorageFolder"/> to save the file in.
        /// </param>
        /// <param name="fileName">
        /// The relative <see cref="string"/> file path.
        /// </param>
        /// <returns>
        /// The stored <see cref="byte"/> array.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Exception thrown if the <paramref name="fileName"/> is null or empty.
        /// </exception>
        public static async Task<byte[]> ReadBytesFromFileAsync(
            this StorageFolder fileLocation,
            string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            var file = await fileLocation.GetFileAsync(fileName).AsTask().ConfigureAwait(false);
            return await file.ReadBytesAsync();
        }

        /// <summary>
        /// Gets an array of bytes from a <see cref="StorageFile"/>.
        /// </summary>
        /// <param name="file">
        /// The <see cref="StorageFile"/>.
        /// </param>
        /// <returns>
        /// The stored <see cref="byte"/> array.
        /// </returns>
        public static async Task<byte[]> ReadBytesAsync(this StorageFile file)
        {
            if (file == null)
            {
                return null;
            }

            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                using (var reader = new DataReader(stream.GetInputStreamAt(0)))
                {
                    await reader.LoadAsync((uint)stream.Size);
                    var bytes = new byte[stream.Size];
                    reader.ReadBytes(bytes);
                    return bytes;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether a file exists in the current folder.
        /// </summary>
        /// <param name="folder">
        /// The <see cref="StorageFolder"/> to look for the file in.
        /// </param>
        /// <param name="fileName">
        /// The <see cref="string"/> filename of the file to search for. Must include the file extension and is not case-sensitive.
        /// </param>
        /// <param name="isRecursive">
        /// The <see cref="bool"/>, indicating if the subfolders should also be searched through.
        /// </param>
        /// <returns>
        /// <c>true</c> if the file exists; otherwise, <c>false</c>.
        /// </returns>
        public static Task<bool> FileExistsAsync(this StorageFolder folder, string fileName, bool isRecursive = false)
            => isRecursive
                ? FileExistsInSubtreeAsync(folder, fileName)
                : FileExistsInFolderAsync(folder, fileName);

        /// <summary>
        /// Gets a value indicating whether a filename is correct or not using the Storage feature.
        /// </summary>
        /// <param name="fileName">The filename to test. Must include the file extension and is not case-sensitive.</param>
        /// <returns><c>true</c> if the filename is valid; otherwise, <c>false</c>.</returns>
        public static bool IsFileNameValid(string fileName)
        {
            var illegalChars = Path.GetInvalidFileNameChars();
            return fileName.All(c => !illegalChars.Contains(c));
        }

        /// <summary>
        /// Gets a value indicating whether a file path is correct or not using the Storage feature.
        /// </summary>
        /// <param name="filePath">The file path to test. Must include the file extension and is not case-sensitive.</param>
        /// <returns><c>true</c> if the file path is valid; otherwise, <c>false</c>.</returns>
        public static bool IsFilePathValid(string filePath)
        {
            var illegalChars = Path.GetInvalidPathChars();
            return filePath.All(c => !illegalChars.Contains(c));
        }

        /// <summary>
        /// Gets a value indicating whether a file exists in the current folder.
        /// </summary>
        /// <param name="folder">
        /// The <see cref="StorageFolder"/> to look for the file in.
        /// </param>
        /// <param name="fileName">
        /// The <see cref="string"/> filename of the file to search for. Must include the file extension and is not case-sensitive.
        /// </param>
        /// <returns>
        /// <c>true</c> if the file exists; otherwise, <c>false</c>.
        /// </returns>
        internal static async Task<bool> FileExistsInFolderAsync(StorageFolder folder, string fileName)
        {
            var item = await folder.TryGetItemAsync(fileName).AsTask().ConfigureAwait(false);
            return (item != null) && item.IsOfType(StorageItemTypes.File);
        }

        /// <summary>
        /// Gets a value indicating whether a file exists in the current folder or in one of its subfolders.
        /// </summary>
        /// <param name="rootFolder">
        /// The <see cref="StorageFolder"/> to look for the file in.
        /// </param>
        /// <param name="fileName">
        /// The <see cref="string"/> filename of the file to search for. Must include the file extension and is not case-sensitive.
        /// </param>
        /// <returns>
        /// <c>true</c> if the file exists; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Exception thrown if the <paramref name="fileName"/> contains a quotation mark.
        /// </exception>
        internal static async Task<bool> FileExistsInSubtreeAsync(StorageFolder rootFolder, string fileName)
        {
            if (fileName.IndexOf('"') >= 0)
            {
                throw new ArgumentException(nameof(fileName));
            }

            var options = new QueryOptions
            {
                FolderDepth = FolderDepth.Deep,
                UserSearchFilter = $"filename:=\"{fileName}\"" // “:=” is the exact-match operator
            };

            var files = await rootFolder.CreateFileQueryWithOptions(options).GetFilesAsync().AsTask().ConfigureAwait(false);
            return files.Count > 0;
        }

        /// <summary>
        /// Returns a <see cref="StorageFolder"/> from a <see cref="KnownFolderId"/>
        /// </summary>
        /// <param name="knownFolderId">Folder Id</param>
        /// <returns>The <see cref="StorageFolder"/></returns>
        internal static StorageFolder GetFolderFromKnownFolderId(KnownFolderId knownFolderId)
        {
            StorageFolder workingFolder;

            switch (knownFolderId)
            {
                case KnownFolderId.AppCaptures:
                    workingFolder = KnownFolders.AppCaptures;
                    break;
                case KnownFolderId.CameraRoll:
                    workingFolder = KnownFolders.CameraRoll;
                    break;
                case KnownFolderId.DocumentsLibrary:
                    workingFolder = KnownFolders.DocumentsLibrary;
                    break;
                case KnownFolderId.HomeGroup:
                    workingFolder = KnownFolders.HomeGroup;
                    break;
                case KnownFolderId.MediaServerDevices:
                    workingFolder = KnownFolders.MediaServerDevices;
                    break;
                case KnownFolderId.MusicLibrary:
                    workingFolder = KnownFolders.MusicLibrary;
                    break;
                case KnownFolderId.Objects3D:
                    workingFolder = KnownFolders.Objects3D;
                    break;
                case KnownFolderId.PicturesLibrary:
                    workingFolder = KnownFolders.PicturesLibrary;
                    break;
                case KnownFolderId.Playlists:
                    workingFolder = KnownFolders.Playlists;
                    break;
                case KnownFolderId.RecordedCalls:
                    workingFolder = KnownFolders.RecordedCalls;
                    break;
                case KnownFolderId.RemovableDevices:
                    workingFolder = KnownFolders.RemovableDevices;
                    break;
                case KnownFolderId.SavedPictures:
                    workingFolder = KnownFolders.SavedPictures;
                    break;
                case KnownFolderId.VideosLibrary:
                    workingFolder = KnownFolders.VideosLibrary;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(knownFolderId), knownFolderId, null);
            }

            return workingFolder;
        }
    }
}