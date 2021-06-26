// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using Microsoft.Toolkit.Uwp.Helpers;
using Newtonsoft.Json;

namespace UnitTests.UWP.Helpers
{
    /// <summary>
    /// This is a Serializer which should mimic the previous functionality of 6.1.1 release of the Toolkit with Newtonsoft.Json.
    /// </summary>
    internal class JsonObjectSerializer : IObjectSerializer
    {
        public T Deserialize<T>(object value)
        {
            var type = typeof(T);
            var typeInfo = type.GetTypeInfo();

            // Note: If you're creating a new app, you could just use the serializer directly.
            // This if/return combo is to maintain compatibility with 6.1.1
            if (typeInfo.IsPrimitive || type == typeof(string))
            {
                return (T)Convert.ChangeType(value, type);
            }

            return JsonConvert.DeserializeObject<T>((string)value);
        }

        public object Serialize<T>(T value)
        {
            var type = typeof(T);
            var typeInfo = type.GetTypeInfo();

            // Note: If you're creating a new app, you could just use the serializer directly.
            // This if/return combo is to maintain compatibility with 6.1.1
            if (typeInfo.IsPrimitive || type == typeof(string))
            {
                return value;
            }

            return JsonConvert.SerializeObject(value);
        }
    }
}