// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Microsoft.Toolkit.Helpers
{
    /// <summary>
    /// Service interface used to store data using key value pairs.
    /// </summary>
    public interface ISettingsStorageHelper
    {
        /// <summary>
        /// Determines whether a setting already exists.
        /// </summary>
        /// <param name="key">Key of the setting (that contains object).</param>
        /// <returns>True if a value exists.</returns>
        bool KeyExists(string key);

        /// <summary>
        /// Determines whether a setting already exists in composite.
        /// </summary>
        /// <param name="compositeKey">Key of the composite (that contains settings).</param>
        /// <param name="key">Key of the setting (that contains object).</param>
        /// <returns>True if a value exists.</returns>
        bool KeyExists(string compositeKey, string key);

        /// <summary>
        /// Retrieves a single item by its key.
        /// </summary>
        /// <typeparam name="T">Type of object retrieved.</typeparam>
        /// <param name="key">Key of the object.</param>
        /// <param name="default">Default value of the object.</param>
        /// <returns>The T object</returns>
        T Read<T>(string key, T? @default = default(T));

        /// <summary>
        /// Retrieves a single item by its key in composite.
        /// </summary>
        /// <typeparam name="T">Type of object retrieved.</typeparam>
        /// <param name="compositeKey">Key of the composite (that contains settings).</param>
        /// <param name="key">Key of the object.</param>
        /// <param name="default">Default value of the object.</param>
        /// <returns>The T object.</returns>
        T Read<T>(string compositeKey, string key, T? @default = default(T));

        /// <summary>
        /// Saves a single item by its key.
        /// </summary>
        /// <typeparam name="T">Type of object saved.</typeparam>
        /// <param name="key">Key of the value saved.</param>
        /// <param name="value">Object to save.</param>
        void Save<T>(string key, T value);

        /// <summary>
        /// Saves a group of items by its key in a composite.
        /// This method should be considered for objects that do not exceed 8k bytes during the lifetime of the application
        /// and for groups of settings which need to be treated in an atomic way.
        /// </summary>
        /// <typeparam name="T">Type of object saved.</typeparam>
        /// <param name="compositeKey">Key of the composite (that contains settings).</param>
        /// <param name="values">Objects to save.</param>
        void Save<T>(string compositeKey, IDictionary<string, T> values);

        /// <summary>
        /// Deletes a single item by its key.
        /// </summary>
        /// <param name="key">Key of the object.</param>
        void Delete(string key);

        /// <summary>
        /// Deletes a single item by its key in composite.
        /// </summary>
        /// <param name="compositeKey">Key of the composite (that contains settings).</param>
        /// <param name="key">Key of the object.</param>
        void Delete(string compositeKey, string key);
    }
}
