// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.Json;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    internal interface IDrawable
    {
        void Draw(CanvasDrawingSession drawingSession, Rect sessionBounds);

        bool IsVisible(Rect viewPort);

        void WriteJson(Utf8JsonWriter writer);

        void ReadProperty(string propertyName, ref Utf8JsonReader reader);

        void OnDeserialized();

        bool IsActive { get; set; }

        Rect Bounds { get; set; }
    }
}