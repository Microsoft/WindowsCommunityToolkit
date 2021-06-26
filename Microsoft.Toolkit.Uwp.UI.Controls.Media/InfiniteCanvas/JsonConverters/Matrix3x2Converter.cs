// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    internal class Matrix3x2Converter : JsonConverter<Matrix3x2>
    {
        public override Matrix3x2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            string propertyName;
            Matrix3x2 matrix = default;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return matrix;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "M11":
                            matrix.M11 = reader.GetSingle();
                            break;
                        case "M12":
                            matrix.M12 = reader.GetSingle();
                            break;
                        case "M21":
                            matrix.M21 = reader.GetSingle();
                            break;
                        case "M22":
                            matrix.M22 = reader.GetSingle();
                            break;
                        case "M31":
                            matrix.M31 = reader.GetSingle();
                            break;
                        case "M32":
                            matrix.M32 = reader.GetSingle();
                            break;
                        case "IsIdentity":
                            // Ignore, as it is readonly, and from v1
                            break;
                        case "Translation":
                            var translation = JsonSerializer.Deserialize<Vector2>(ref reader);
                            matrix.Translation = translation;
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Matrix3x2 value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber("M11", value.M11);
            writer.WriteNumber("M12", value.M12);
            writer.WriteNumber("M21", value.M21);
            writer.WriteNumber("M22", value.M22);
            writer.WriteNumber("M31", value.M31);
            writer.WriteNumber("M32", value.M32);

            writer.WriteEndObject();
        }
    }
}