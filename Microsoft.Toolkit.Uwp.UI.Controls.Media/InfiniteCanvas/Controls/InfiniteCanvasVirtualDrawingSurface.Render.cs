// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Storage;
using Windows.UI;

namespace Microsoft.Toolkit.Uwp.UI.Controls
{
    /// <summary>
    /// The virtual Drawing surface renderer used to render the ink and text. This control is used as part of the <see cref="InfiniteCanvas"/>
    /// </summary>
    public partial class InfiniteCanvasVirtualDrawingSurface
    {
        private const float BaseCanvasDPI = 96;

        private readonly List<IDrawable> _visibleList = new List<IDrawable>();
        private readonly List<IDrawable> _drawableList = new List<IDrawable>();

        internal void ReDraw(Rect viewPort, float zoom)
        {
            var toDraw = GetDrawingBoundaries(viewPort);
            var scale = _screenScale * zoom;
            var rect = ScaleRect(toDraw, scale);
            var dpi = BaseCanvasDPI * (float)scale;

            try
            {
                using (var drawingSession = CanvasComposition.CreateDrawingSession(_drawingSurface, rect, dpi))
                {
                    drawingSession.Clear(Colors.White);
                    foreach (var drawable in _visibleList)
                    {
                        drawable.Draw(drawingSession, toDraw);
                    }
                }
            }
            catch (ArgumentException)
            {
                /* CanvasComposition.CreateDrawingSession has an internal
                 * limit on the size of the updateRectInPixels parameter,
                 * which we don't know, so we can get an ArgumentException
                 * if there is a lot of extreme zooming and panning
                 * Therefore, the only solution is to silently catch the
                 * exception and allow the app to continue
                 */
            }
        }

        private Rect ScaleRect(Rect rect, double scale)
        {
            return new Rect(rect.X * scale, rect.Y * scale, rect.Width * scale, rect.Height * scale);
        }

        internal CanvasRenderTarget ExportMaxOffScreenDrawings()
        {
            var toDraw = GetMaxDrawingsBoundaries();
            var offScreen = DrawOffScreen(toDraw, _drawableList);
            return offScreen;
        }

        internal void ClearAll(Rect viewPort)
        {
            _visibleList.Clear();
            ExecuteClearAll();
            _drawingSurface.Trim(new RectInt32[0]);
        }

        internal string GetSerializedList()
        {
            var exportModel = new InkCanvasExportModel { DrawableList = _drawableList, Version = 1 };
            return JsonSerializer.Serialize(exportModel, GetJsonSerializerOptions());
        }

        private static JsonSerializerOptions GetJsonSerializerOptions()
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            // This will be needed until These two issues are fixed:
            // https://github.com/dotnet/runtime/issues/30083
            // https://github.com/dotnet/runtime/issues/29937
            jsonSerializerOptions.Converters.Add(new IDrawableConverter());
            return jsonSerializerOptions;
        }

        internal static List<IDrawable> LoadJson(string json)
        {
            var token = JsonDocument.Parse(json);
            List<IDrawable> newList;
            if (token.RootElement.ValueKind == JsonValueKind.Array)
            {
                // first sin, because of creating a file without versioning so we have to be able to import without breaking changes.
                newList = JsonSerializer.Deserialize<List<IDrawable>>(json, GetJsonSerializerOptions());
            }
            else
            {
                newList = JsonSerializer.Deserialize<InkCanvasExportModel>(json, GetJsonSerializerOptions()).DrawableList;
            }

            return newList;
        }

        internal void RenderFromJsonAndDraw(Rect viewPort, string json, float zoom)
        {
            _visibleList.Clear();
            _drawableList.Clear();
            _undoCommands.Clear();
            _redoCommands.Clear();

            var newList = LoadJson(json);

            foreach (var drawable in newList)
            {
                _drawableList.Add(drawable);
            }

            ReDraw(viewPort, zoom);
        }

        private Rect GetDrawingBoundaries(Rect viewPort)
        {
            _visibleList.Clear();
            double top = double.MaxValue, bottom = double.MinValue, left = double.MaxValue, right = double.MinValue;

            foreach (var drawable in _drawableList)
            {
                if (drawable.IsVisible(viewPort))
                {
                    _visibleList.Add(drawable);

                    bottom = Math.Max(drawable.Bounds.Bottom, bottom);
                    right = Math.Max(drawable.Bounds.Right, right);
                    top = Math.Min(drawable.Bounds.Top, top);
                    left = Math.Min(drawable.Bounds.Left, left);
                }
            }

            Rect toDraw;
            if (_visibleList.Any())
            {
                toDraw = new Rect(Math.Max(left, 0), Math.Max(top, 0), Math.Max(right - left, 0), Math.Max(bottom - top, 0));

                toDraw.Union(viewPort);
            }
            else
            {
                toDraw = viewPort;
            }

            if (toDraw.Height > Height)
            {
                toDraw.Height = Height;
            }

            if (toDraw.Width > Width)
            {
                toDraw.Width = Width;
            }

            return toDraw;
        }

        private Rect GetMaxDrawingsBoundaries()
        {
            double top = double.MaxValue, bottom = double.MinValue, left = double.MaxValue, right = double.MinValue;

            foreach (var drawable in _drawableList)
            {
                bottom = Math.Max(drawable.Bounds.Bottom, bottom);
                right = Math.Max(drawable.Bounds.Right, right);
                top = Math.Min(drawable.Bounds.Top, top);
                left = Math.Min(drawable.Bounds.Left, left);
            }

            // if the width or height are greater than _win2DDevice.MaximumBitmapSizeInPixels drawing session will through an exception.
            var toDraw = new Rect(
                Math.Max(left, 0),
                Math.Max(top, 0),
                Math.Min(Math.Max(right - left, 0), _win2DDevice.MaximumBitmapSizeInPixels),
                Math.Min(Math.Max(bottom - top, 0), _win2DDevice.MaximumBitmapSizeInPixels));

            return toDraw;
        }

        private CanvasRenderTarget DrawOffScreen(Rect toDraw, List<IDrawable> visibleList)
        {
            const int dpi = 96;
            var offScreen = new CanvasRenderTarget(_win2DDevice, (float)toDraw.Width, (float)toDraw.Height, dpi);
            using (var drawingSession = offScreen.CreateDrawingSession())
            {
                drawingSession.Clear(Colors.White);
                foreach (var drawable in visibleList)
                {
                    drawable.Draw(drawingSession, toDraw);
                }
            }

            return offScreen;
        }
    }
}