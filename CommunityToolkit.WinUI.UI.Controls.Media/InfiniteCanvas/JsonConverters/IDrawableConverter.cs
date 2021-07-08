// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.Foundation;

namespace CommunityToolkit.WinUI.UI.Controls
{
    internal class IDrawableConverter : JsonConverter<IDrawable>
    {
        private const string OldInkDrawableDiscriminator = "CommunityToolkit.WinUI.UI.Controls.InkDrawable, CommunityToolkit.WinUI.UI.Controls";
        private const string OldTextDrawableDiscriminator = "CommunityToolkit.WinUI.UI.Controls.TextDrawable, CommunityToolkit.WinUI.UI.Controls";

        public override bool CanConvert(Type typeToConvert) => typeof(IDrawable).IsAssignableFrom(typeToConvert);

        public override IDrawable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string propertyName = reader.GetString();
            if (propertyName != "$type")
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var typeDiscriminator = reader.GetString();
            IDrawable drawable;
            if (typeDiscriminator == GetDiscriminator(typeof(InkDrawable)) || typeDiscriminator == OldInkDrawableDiscriminator)
            {
                drawable = new InkDrawable();
            }
            else if (typeDiscriminator == GetDiscriminator(typeof(TextDrawable)) || typeDiscriminator == OldTextDrawableDiscriminator)
            {
                drawable = new TextDrawable();
            }
            else
            {
                throw new JsonException();
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    drawable.OnDeserialized();
                    return drawable;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "IsActive":
                            drawable.IsActive = reader.GetBoolean();
                            break;
                        case "Bounds":
                            drawable.Bounds = JsonSerializer.Deserialize<Rect>(ref reader);
                            break;
                        default:
                            drawable.ReadProperty(propertyName, ref reader);
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, IDrawable drawable, JsonSerializerOptions options)
        {
            drawable.WriteJson(writer);
        }

        internal static string GetDiscriminator(Type type)
        {
            return $"{type.FullName}, {type.Assembly.GetName().Name}";
        }
    }
}