// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Windows.UI.Input.Inking;

namespace CommunityToolkit.WinUI.UI.Controls
{
    internal class SerializableStroke
    {
        [JsonIgnore]
        public List<InkPoint> FinalPointList { get; set; }

        [JsonIgnore]
        public InkDrawingAttributes DrawingAttributesIgnored { get; set; }

        // This class is created to avoid breaking Changes
        public CustomInkDrawingAttribute DrawingAttributes { get; set; }

        public List<SerializablePoint> SerializableFinalPointList { get; set; }

        public short? SerializableDrawingAttributesKind { get; set; }

        public double? SerializableDrawingAttributesPencilProperties { get; set; }

        public Matrix3x2 PointTransform { get; set; }

        internal void Write(Utf8JsonWriter writer, JsonSerializerOptions options)
        {
            SerializableFinalPointList = new List<SerializablePoint>(FinalPointList.Count);
            foreach (var point in FinalPointList)
            {
                var serializablePoint = new SerializablePoint();
                serializablePoint.Position = point.Position;
                serializablePoint.Pressure = point.Pressure;
                serializablePoint.TiltX = point.TiltX;
                serializablePoint.TiltY = point.TiltY;
                serializablePoint.Pressure = point.Pressure;
                SerializableFinalPointList.Add(serializablePoint);
            }

            if (DrawingAttributesIgnored != null)
            {
                SerializableDrawingAttributesKind = (short)DrawingAttributesIgnored.Kind;
                SerializableDrawingAttributesPencilProperties = DrawingAttributesIgnored.PencilProperties?.Opacity;
                DrawingAttributes = new CustomInkDrawingAttribute
                {
                    Color = DrawingAttributesIgnored.Color,
                    FitToCurve = DrawingAttributesIgnored.FitToCurve,
                    IgnorePressure = DrawingAttributesIgnored.IgnorePressure,
                    IgnoreTilt = DrawingAttributesIgnored.IgnoreTilt,
                    Size = DrawingAttributesIgnored.Size,
                    PenTip = DrawingAttributesIgnored.PenTip,
                    PenTipTransform = DrawingAttributesIgnored.PenTipTransform,
                    DrawAsHighlighter = DrawingAttributesIgnored.DrawAsHighlighter
                };
            }

            JsonSerializer.Serialize(writer, this, options);

            SerializableFinalPointList = null;
        }

        internal void OnDeserialized()
        {
            var finalPointList = new List<InkPoint>(SerializableFinalPointList.Count);

            foreach (var point in SerializableFinalPointList)
            {
                finalPointList.Add(new InkPoint(point.Position, point.Pressure, point.TiltX, point.TiltY, point.Timestamp));
            }

            FinalPointList = finalPointList;

            InkDrawingAttributes pencilAttributes;
            if (SerializableDrawingAttributesKind.HasValue &&
                SerializableDrawingAttributesKind == (short)InkDrawingAttributesKind.Pencil)
            {
                pencilAttributes = InkDrawingAttributes.CreateForPencil();
            }
            else
            {
                pencilAttributes = new InkDrawingAttributes
                {
                    PenTip = DrawingAttributes.PenTip,
                    PenTipTransform = DrawingAttributes.PenTipTransform,
                    DrawAsHighlighter = DrawingAttributes.DrawAsHighlighter
                };
            }

            pencilAttributes.Color = DrawingAttributes.Color;
            pencilAttributes.FitToCurve = DrawingAttributes.FitToCurve;
            pencilAttributes.IgnorePressure = DrawingAttributes.IgnorePressure;
            pencilAttributes.IgnoreTilt = DrawingAttributes.IgnoreTilt;
            pencilAttributes.Size = DrawingAttributes.Size;

            if (SerializableDrawingAttributesPencilProperties.HasValue)
            {
                pencilAttributes.PencilProperties.Opacity = SerializableDrawingAttributesPencilProperties.Value;
            }

            DrawingAttributesIgnored = pencilAttributes;

            // Empty unused values
            SerializableDrawingAttributesPencilProperties = null;
            SerializableFinalPointList = null;
            SerializableDrawingAttributesKind = null;
        }
    }
}