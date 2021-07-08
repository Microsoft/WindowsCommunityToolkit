// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Numerics;
using System.Threading.Tasks;
using CommunityToolkit.WinUI.UI.Media.Helpers.Cache;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using Microsoft.Graphics.DirectX;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Media.Helpers
{
    /// <summary>
    /// A <see langword="class"/> that can load and draw images and other objects to Win2D surfaces and brushes
    /// </summary>
    public sealed partial class SurfaceLoader
    {
        /// <summary>
        /// Synchronization mutex to access the cache and load Win2D images concurrently
        /// </summary>
        private static readonly AsyncMutex Win2DMutex = new AsyncMutex();

        /// <summary>
        /// Gets the local cache mapping for previously loaded Win2D images
        /// </summary>
        private static readonly CompositionObjectCache<Uri, CompositionBrush> Cache = new CompositionObjectCache<Uri, CompositionBrush>();

        /// <summary>
        /// Loads a <see cref="CompositionBrush"/> instance with the target image from the shared <see cref="CanvasDevice"/> instance
        /// </summary>
        /// <param name="uri">The path to the image to load</param>
        /// <param name="dpiMode">Indicates the desired DPI mode to use when loading the image</param>
        /// <param name="dpi">Indicates the current display DPI used to load the image</param>
        /// <param name="cacheMode">Indicates the cache option to use to load the image</param>
        /// <returns>A <see cref="Task{T}"/> that returns the loaded <see cref="CompositionBrush"/> instance</returns>
        public static async Task<CompositionBrush> LoadImageAsync(Uri uri, DpiMode dpiMode, float dpi, CacheMode cacheMode = CacheMode.Default)
        {
            var compositor = CompositionTarget.GetCompositorForCurrentThread();

            // Lock and check the cache first
            using (await Win2DMutex.LockAsync())
            {
                uri = uri.ToAppxUri();

                if (cacheMode == CacheMode.Default &&
                    Cache.TryGetValue(compositor, uri, out var cached))
                {
                    return cached;
                }

                // Load the image
                CompositionBrush brush;
                try
                {
                    // This will throw and the canvas will re-initialize the Win2D device if needed
                    var sharedDevice = CanvasDevice.GetSharedDevice();
                    brush = await LoadSurfaceBrushAsync(sharedDevice, compositor, uri, dpiMode, dpi);
                }
                catch
                {
                    // Device error
                    brush = null;
                }

                // Cache when needed and return the result
                if (brush != null &&
                    cacheMode != CacheMode.Disabled)
                {
                    Cache.AddOrUpdate(compositor, uri, brush);
                }

                return brush;
            }
        }

        /// <summary>
        /// Loads a <see cref="CompositionBrush"/> from the input <see cref="global::System.Uri"/>, and prepares it to be used in a tile effect
        /// </summary>
        /// <param name="canvasDevice">The device to use to process the Win2D image</param>
        /// <param name="compositor">The compositor instance to use to create the final brush</param>
        /// <param name="uri">The path to the image to load</param>
        /// <param name="dpiMode">Indicates the desired DPI mode to use when loading the image</param>
        /// <param name="dpi">Indicates the current display DPI used to load the image</param>
        /// <returns>A <see cref="Task{T}"/> that returns the loaded <see cref="CompositionBrush"/> instance</returns>
        private static async Task<CompositionBrush> LoadSurfaceBrushAsync(
            CanvasDevice canvasDevice,
            Compositor compositor,
            Uri uri,
            DpiMode dpiMode,
            float dpi)
        {
            // WinUI3/Win2D bug: switch back to CanvasBitmap once it works.
            // Load the bitmap with the appropriate settings
            using CanvasVirtualBitmap bitmap = dpiMode switch
            {
                DpiMode.UseSourceDpi => await CanvasVirtualBitmap.LoadAsync(canvasDevice, uri),
                DpiMode.Default96Dpi => await CanvasVirtualBitmap.LoadAsync(canvasDevice, uri), // , 96),
                DpiMode.DisplayDpi => await CanvasVirtualBitmap.LoadAsync(canvasDevice, uri), // , dpi),
                DpiMode.DisplayDpiWith96AsLowerBound => await CanvasVirtualBitmap.LoadAsync(canvasDevice, uri), // , dpi >= 96 ? dpi : 96),
                _ => throw new ArgumentOutOfRangeException(nameof(dpiMode), dpiMode, $"Invalid DPI mode: {dpiMode}")
            };

            // Calculate the surface size
            Size
                size = bitmap.Size,
                sizeInPixels = new Size(bitmap.SizeInPixels.Width, bitmap.SizeInPixels.Height);

            // Get the device and the target surface
            using CompositionGraphicsDevice graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(compositor, canvasDevice);

            // Create the drawing surface
            var drawingSurface = graphicsDevice.CreateDrawingSurface(
                sizeInPixels,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                DirectXAlphaMode.Premultiplied);

            // Create a drawing session for the target surface
            using (var drawingSession = CanvasComposition.CreateDrawingSession(drawingSurface, new Rect(0, 0, sizeInPixels.Width, sizeInPixels.Height), dpi))
            {
                // Fill the target surface
                drawingSession.Clear(Color.FromArgb(0, 0, 0, 0));
                drawingSession.DrawImage(bitmap, new Rect(0, 0, size.Width, size.Height), new Rect(0, 0, size.Width, size.Height));
                drawingSession.EffectTileSize = new BitmapSize { Width = (uint)size.Width, Height = (uint)size.Height };
            }

            // Setup the effect brush to use
            var surfaceBrush = compositor.CreateSurfaceBrush(drawingSurface);
            surfaceBrush.Stretch = CompositionStretch.None;

            var rasterizationScale = dpi / 96.0;

            // Adjust the scale if the DPI scaling is greater than 100%
            if (rasterizationScale > 1)
            {
                surfaceBrush.Scale = new Vector2((float)(1 / rasterizationScale));
                surfaceBrush.BitmapInterpolationMode = CompositionBitmapInterpolationMode.NearestNeighbor;
            }

            return surfaceBrush;
        }
    }
}