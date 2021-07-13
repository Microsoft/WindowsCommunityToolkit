// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json;
using Microsoft.Toolkit.Helpers;

namespace UnitTests.UWP.Helpers
{
    /// <summary>
    /// Example class of writing a new <see cref="IObjectSerializer"/> that uses System.Text.Json.
    /// </summary>
    internal class SystemTextJsonSerializer : IObjectSerializer
    {
        public T Deserialize<T>(object value) => JsonSerializer.Deserialize<T>(value as string);

        public object Serialize<T>(T value) => JsonSerializer.Serialize(value);
    }
}