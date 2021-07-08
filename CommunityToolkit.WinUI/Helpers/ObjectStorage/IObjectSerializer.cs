// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.Helpers
{
    /// <summary>
    /// A basic serialization service.
    /// </summary>
    public interface IObjectSerializer
    {
        /// <summary>
        /// Serialize an object into a string. It is recommended to use strings as the final format for objects if you plan to use the <see cref="BaseObjectStorageHelper.SaveFileAsync{T}(string, T)"/> method.
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The serialized object.</returns>
        object Serialize<T>(T value);

        /// <summary>
        /// Deserialize a primitive or string into an object of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized object.</typeparam>
        /// <param name="value">The string to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize<T>(object value);
    }
}