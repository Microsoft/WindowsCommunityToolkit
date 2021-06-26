// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    internal class SerializableStrokeConverter : JsonConverter<SerializableStroke>
    {
        public override SerializableStroke Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var matrixOptions = new JsonSerializerOptions();
            matrixOptions.Converters.Add(new Matrix3x2Converter());
            SerializableStroke serializableStroke = JsonSerializer.Deserialize<SerializableStroke>(ref reader, matrixOptions);

            serializableStroke.OnDeserialized();

            return serializableStroke;
        }

        public override void Write(Utf8JsonWriter writer, SerializableStroke value, JsonSerializerOptions options)
        {
            var matrixOptions = new JsonSerializerOptions();
            matrixOptions.Converters.Add(new Matrix3x2Converter());
            value.Write(writer, matrixOptions);
        }
    }
}