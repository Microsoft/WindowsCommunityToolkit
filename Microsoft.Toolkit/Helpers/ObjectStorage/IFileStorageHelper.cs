// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Toolkit.Helpers
{
    /// <summary>
    /// Service interface used to store data in files and folders.
    /// </summary>
    public interface IFileStorageHelper
    {
        /// <summary>
        /// Determines whether a file already exists.
        /// </summary>
        /// <param name="filePath">Key of the file (that contains object).</param>
        /// <returns>True if a value exists.</returns>
        Task<bool> FileExistsAsync(string filePath);

        /// <summary>
        /// Retrieves an object from a file.
        /// </summary>
        /// <typeparam name="T">Type of object retrieved.</typeparam>
        /// <param name="filePath">Path to the file that contains the object.</param>
        /// <param name="default">Default value of the object.</param>
        /// <returns>Waiting task until completion with the object in the file.</returns>
        Task<T> ReadFileAsync<T>(string filePath, T? @default = default);

        /// <summary>
        /// Retrieves all file listings for a folder.
        /// </summary>
        /// <param name="folderPath">The path to the target folder.</param>
        /// <returns>A list of file names in the target folder.</returns>
        Task<IList<string>> ReadFolderAsync(string folderPath);

        /// <summary>
        /// Saves an object inside a file.
        /// </summary>
        /// <typeparam name="T">Type of object saved.</typeparam>
        /// <param name="filePath">Path to the file that will contain the object.</param>
        /// <param name="value">Object to save.</param>
        /// <returns>Waiting task until completion.</returns>
        Task SaveFileAsync<T>(string filePath, T value);

        /// <summary>
        /// Saves a folder.
        /// </summary>
        /// <param name="folderPath">The path and name of the target folder.</param>
        /// <returns>Waiting task until completion.</returns>
        Task SaveFolderAsync(string folderPath);

        /// <summary>
        /// Deletes a file or folder item.
        /// </summary>
        /// <param name="itemPath">The path to the item for deletion.</param>
        /// <returns>Waiting task until completion.</returns>
        Task DeleteItemAsync(string itemPath);
    }
}
