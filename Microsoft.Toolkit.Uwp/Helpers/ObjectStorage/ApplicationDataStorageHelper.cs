// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Helpers;
using Windows.Storage;
using Windows.System;

namespace Microsoft.Toolkit.Uwp.Helpers
{
    /// <summary>
    /// Storage helper for files and folders living in Windows.Storage.ApplicationData storage endpoints.
    /// </summary>
    public class ApplicationDataStorageHelper : IFileStorageHelper, ISettingsStorageHelper
    {
        /// <summary>
        /// Get a new instance using ApplicationData.Current and the provided serializer.
        /// </summary>
        /// <param name="objectSerializer">Serializer for converting stored values.</param>
        /// <returns>A new instance of ApplicationDataStorageHelper.</returns>
        public static ApplicationDataStorageHelper GetCurrent(Toolkit.Helpers.IObjectSerializer objectSerializer)
        {
            var appData = ApplicationData.Current;
            return new ApplicationDataStorageHelper(appData, objectSerializer);
        }

        /// <summary>
        /// Get a new instance using the ApplicationData for the provided user and serializer.
        /// </summary>
        /// <param name="user">App data user owner.</param>
        /// <param name="objectSerializer">Serializer for converting stored values.</param>
        /// <returns>A new instance of ApplicationDataStorageHelper.</returns>
        public static async Task<ApplicationDataStorageHelper> GetForUserAsync(User user, Toolkit.Helpers.IObjectSerializer objectSerializer)
        {
            var appData = await ApplicationData.GetForUserAsync(user);
            return new ApplicationDataStorageHelper(appData, objectSerializer);
        }

        /// <summary>
        /// Gets the settings container.
        /// </summary>
        protected ApplicationData AppData { get; private set; }

        private ApplicationDataContainer DefaultSettings => AppData.LocalSettings;

        private StorageFolder DefaultFolder => AppData.LocalFolder;

        private readonly Toolkit.Helpers.IObjectSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationDataStorageHelper"/> class.
        /// </summary>
        /// <param name="appData">The data store to interact with.</param>
        /// <param name="objectSerializer">Serializer for converting stored values.</param>
        public ApplicationDataStorageHelper(ApplicationData appData, Toolkit.Helpers.IObjectSerializer objectSerializer)
        {
            AppData = appData ?? throw new ArgumentNullException(nameof(appData));
            _serializer = objectSerializer ?? throw new ArgumentNullException(nameof(objectSerializer));
        }

        /// <inheritdoc />
        public bool KeyExists(string key)
        {
            return DefaultSettings.Values.ContainsKey(key);
        }

        /// <inheritdoc />
        public bool KeyExists(string compositeKey, string key)
        {
            if (KeyExists(compositeKey))
            {
                ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)DefaultSettings.Values[compositeKey];
                if (composite != null)
                {
                    return composite.ContainsKey(key);
                }
            }

            return false;
        }

        /// <inheritdoc />
        public T Read<T>(string key, T @default = default)
        {
            if (!DefaultSettings.Values.TryGetValue(key, out var value) || value == null)
            {
                return @default;
            }

            return _serializer.Deserialize<T>(value);
        }

        /// <inheritdoc />
        public T Read<T>(string compositeKey, string key, T @default = default)
        {
            ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)DefaultSettings.Values[compositeKey];
            if (composite != null)
            {
                string value = (string)composite[key];
                if (value != null)
                {
                    return _serializer.Deserialize<T>(value);
                }
            }

            return @default;
        }

        /// <inheritdoc />
        public void Save<T>(string key, T value)
        {
            DefaultSettings.Values[key] = _serializer.Serialize(value);
        }

        /// <inheritdoc />
        public void Save<T>(string compositeKey, IDictionary<string, T> values)
        {
            if (KeyExists(compositeKey))
            {
                ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)DefaultSettings.Values[compositeKey];

                foreach (KeyValuePair<string, T> setting in values)
                {
                    if (composite.ContainsKey(setting.Key))
                    {
                        composite[setting.Key] = _serializer.Serialize(setting.Value);
                    }
                    else
                    {
                        composite.Add(setting.Key, _serializer.Serialize(setting.Value));
                    }
                }
            }
            else
            {
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
                foreach (KeyValuePair<string, T> setting in values)
                {
                    composite.Add(setting.Key, _serializer.Serialize(setting.Value));
                }

                DefaultSettings.Values[compositeKey] = composite;
            }
        }

        /// <inheritdoc />
        public void Delete(string key)
        {
            DefaultSettings.Values.Remove(key);
        }

        /// <inheritdoc />
        public void Delete(string compositeKey, string key)
        {
            if (KeyExists(compositeKey))
            {
                ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)DefaultSettings.Values[compositeKey];
                composite.Remove(key);
            }
        }

        /// <inheritdoc />
        public Task<bool> FileExistsAsync(string filePath)
        {
            return FileExistsAsync(DefaultFolder, filePath);
        }

        /// <inheritdoc />
        public Task<T> ReadFileAsync<T>(string filePath, T @default = default)
        {
            return ReadFileAsync<T>(DefaultFolder, filePath, @default);
        }

        /// <inheritdoc />
        public Task<IList<string>> ReadFolderAsync(string folderPath)
        {
            return ReadFolderAsync(DefaultFolder, folderPath);
        }

        /// <inheritdoc />
        public Task SaveFileAsync<T>(string filePath, T value)
        {
            return SaveFileAsync<T>(DefaultFolder, filePath, value);
        }

        /// <inheritdoc />
        public Task SaveFolderAsync(string folderPath)
        {
            return SaveFolderAsync(DefaultFolder, folderPath);
        }

        /// <inheritdoc />
        public Task DeleteItemAsync(string itemPath)
        {
            return DeleteItemAsync(DefaultFolder, itemPath);
        }

        private Task<bool> FileExistsAsync(StorageFolder folder, string filePath)
        {
            return folder.FileExistsAsync(filePath);
        }

        private async Task<T> ReadFileAsync<T>(StorageFolder folder, string filePath, T @default = default)
        {
            string value = await StorageFileHelper.ReadTextFromFileAsync(folder, filePath);
            return (value != null) ? _serializer.Deserialize<T>(value) : @default;
        }

        private async Task<IList<string>> ReadFolderAsync(StorageFolder folder, string folderPath)
        {
            var targetFolder = await folder.GetFolderAsync(folderPath);
            var files = await targetFolder.GetFilesAsync();
            return files.Select((f) => f.Path + f.Name).ToList();
        }

        private Task SaveFileAsync<T>(StorageFolder folder, string filePath, T value)
        {
            return StorageFileHelper.WriteTextToFileAsync(folder, _serializer.Serialize(value)?.ToString(), filePath, CreationCollisionOption.ReplaceExisting);
        }

        private async Task SaveFolderAsync(StorageFolder folder, string folderPath)
        {
            await folder.CreateFolderAsync(folderPath, CreationCollisionOption.OpenIfExists);
        }

        private async Task DeleteItemAsync(StorageFolder folder, string itemPath)
        {
            var item = await folder.GetItemAsync(itemPath);
            await item.DeleteAsync();
        }
    }
}
