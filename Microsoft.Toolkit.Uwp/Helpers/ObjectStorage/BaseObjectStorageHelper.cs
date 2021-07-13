// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Microsoft.Toolkit.Uwp.Helpers
{
    /// <summary>
    /// Shared implementation of ObjectStorageHelper.
    /// </summary>
    [Obsolete("BaseObjectStorageHelper is deprecated and has been superceded by ApplicationDataStorageHelper.")]
    public abstract class BaseObjectStorageHelper : IObjectStorageHelper
    {
        private readonly IObjectSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseObjectStorageHelper"/> class,
        /// which can read and write data using the provided <see cref="IObjectSerializer"/>;
        /// In 6.1 and older the default Serializer was based on Newtonsoft.Json.
        /// To implement an <see cref="IObjectSerializer"/> based on System.Text.Json, Newtonsoft.Json, or DataContractJsonSerializer see https://aka.ms/wct/storagehelper-migration
        /// </summary>
        /// <param name="objectSerializer">The serializer to use.</param>
        public BaseObjectStorageHelper(IObjectSerializer objectSerializer)
        {
            serializer = objectSerializer ?? throw new ArgumentNullException(nameof(objectSerializer));
        }

        /// <summary>
        /// Gets or sets the settings container.
        /// </summary>
        protected ApplicationDataContainer Settings { get; set; }

        /// <summary>
        /// Gets or sets the storage folder.
        /// </summary>
        protected StorageFolder Folder { get; set; }

        /// <summary>
        /// Determines whether a setting already exists.
        /// </summary>
        /// <param name="key">Key of the setting (that contains object)</param>
        /// <returns><c>true</c> if the setting exists; otherwise, <c>false</c>.</returns>
        public bool KeyExists(string key)
        {
            return Settings.Values.ContainsKey(key);
        }

        /// <summary>
        /// Determines whether a setting already exists in composite.
        /// </summary>
        /// <param name="compositeKey">Key of the composite (that contains settings)</param>
        /// <param name="key">Key of the setting (that contains object)</param>
        /// <returns><c>true</c> if the setting exists; otherwise, <c>false</c>.</returns>
        public bool KeyExists(string compositeKey, string key)
        {
            if (KeyExists(compositeKey))
            {
                ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)Settings.Values[compositeKey];
                if (composite != null)
                {
                    return composite.ContainsKey(key);
                }
            }

            return false;
        }

        /// <summary>
        /// Retrieves a single item by its key.
        /// </summary>
        /// <typeparam name="T">Type of object retrieved</typeparam>
        /// <param name="key">Key of the object</param>
        /// <param name="default">Default value of the object</param>
        /// <returns>The T object</returns>
        public T Read<T>(string key, T @default = default)
        {
            if (!Settings.Values.TryGetValue(key, out var value) || value == null)
            {
                return @default;
            }

            return serializer.Deserialize<T>(value);
        }

        /// <summary>
        /// Retrieves a single item by its key in composite.
        /// </summary>
        /// <typeparam name="T">Type of object retrieved</typeparam>
        /// <param name="compositeKey">Key of the composite (that contains settings)</param>
        /// <param name="key">Key of the object</param>
        /// <param name="default">Default value of the object</param>
        /// <returns>The T object</returns>
        public T Read<T>(string compositeKey, string key, T @default = default)
        {
            ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)Settings.Values[compositeKey];
            if (composite != null)
            {
                string value = (string)composite[key];
                if (value != null)
                {
                    return serializer.Deserialize<T>(value);
                }
            }

            return @default;
        }

        /// <summary>
        /// Saves a single item by its key.
        /// This method should be considered for objects that do not exceed 8k bytes during the lifetime of the application
        /// (refers to <see cref="SaveFileAsync{T}(string, T)"/> for complex/large objects).
        /// </summary>
        /// <typeparam name="T">Type of object saved</typeparam>
        /// <param name="key">Key of the value saved</param>
        /// <param name="value">Object to save</param>
        public void Save<T>(string key, T value)
        {
            Settings.Values[key] = serializer.Serialize(value);
        }

        /// <summary>
        /// Saves a group of items by its key in a composite.
        /// This method should be considered for objects that do not exceed 8k bytes during the lifetime of the application
        /// (refers to <see cref="SaveFileAsync{T}(string, T)"/> for complex/large objects) and for groups of settings which
        /// need to be treated in an atomic way.
        /// </summary>
        /// <typeparam name="T">Type of object saved</typeparam>
        /// <param name="compositeKey">Key of the composite (that contains settings)</param>
        /// <param name="values">Objects to save</param>
        public void Save<T>(string compositeKey, IDictionary<string, T> values)
        {
            if (KeyExists(compositeKey))
            {
                ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)Settings.Values[compositeKey];

                foreach (KeyValuePair<string, T> setting in values)
                {
                    if (composite.ContainsKey(setting.Key))
                    {
                        composite[setting.Key] = serializer.Serialize(setting.Value);
                    }
                    else
                    {
                        composite.Add(setting.Key, serializer.Serialize(setting.Value));
                    }
                }
            }
            else
            {
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
                foreach (KeyValuePair<string, T> setting in values)
                {
                    composite.Add(setting.Key, serializer.Serialize(setting.Value));
                }

                Settings.Values[compositeKey] = composite;
            }
        }

        /// <summary>
        /// Determines whether a file already exists.
        /// </summary>
        /// <param name="filePath">Key of the file (that contains object)</param>
        /// <returns><c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
        public Task<bool> FileExistsAsync(string filePath)
        {
            return Folder.FileExistsAsync(filePath);
        }

        /// <summary>
        /// Retrieves an object from a file.
        /// </summary>
        /// <typeparam name="T">Type of object retrieved</typeparam>
        /// <param name="filePath">Path to the file that contains the object</param>
        /// <param name="default">Default value of the object</param>
        /// <returns>Waiting task until completion with the object in the file</returns>
        public async Task<T> ReadFileAsync<T>(string filePath, T @default = default)
        {
            string value = await StorageFileHelper.ReadTextFromFileAsync(Folder, filePath);
            return (value != null) ? serializer.Deserialize<T>(value) : @default;
        }

        /// <summary>
        /// Saves an object inside a file.
        /// There is no limitation to use this method (refers to <see cref="Save{T}(string, T)"/> method for simple objects).
        /// </summary>
        /// <typeparam name="T">Type of object saved</typeparam>
        /// <param name="filePath">Path to the file that will contain the object</param>
        /// <param name="value">Object to save</param>
        /// <returns>The <see cref="StorageFile"/> where the object was saved</returns>
        public Task<StorageFile> SaveFileAsync<T>(string filePath, T value)
        {
            return StorageFileHelper.WriteTextToFileAsync(Folder, serializer.Serialize(value)?.ToString(), filePath, CreationCollisionOption.ReplaceExisting);
        }
    }
}