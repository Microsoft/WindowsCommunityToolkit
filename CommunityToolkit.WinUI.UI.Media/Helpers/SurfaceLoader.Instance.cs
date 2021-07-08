// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Composition;
using Microsoft.Graphics.DirectX;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;

namespace CommunityToolkit.WinUI.UI.Media.Helpers
{
    /// <summary>
    /// A delegate for load time effects.
    /// </summary>
    /// <param name="bitmap">The bitmap.</param>
    /// <param name="device">The device.</param>
    /// <param name="sizeTarget">The size target.</param>
    /// <returns>A CompositeDrawingSurface</returns>
    public delegate CompositionDrawingSurface LoadTimeEffectHandler(CanvasVirtualBitmap bitmap, CompositionGraphicsDevice device, Size sizeTarget); // WinUI3/Win2D bug: switch back to CanvasBitmap once it works.

    /// <summary>
    /// A <see langword="class"/> that can load and draw images and other objects to Win2D surfaces and brushes
    /// </summary>
    public sealed partial class SurfaceLoader
    {
        /// <summary>
        /// The cache of <see cref="SurfaceLoader"/> instances currently available
        /// </summary>
        private static readonly ConditionalWeakTable<Compositor, SurfaceLoader> Instances = new ConditionalWeakTable<Compositor, SurfaceLoader>();

        /// <summary>
        /// Gets a <see cref="SurfaceLoader"/> instance for the <see cref="Compositor"/> of the current window
        /// </summary>
        /// <returns>A <see cref="SurfaceLoader"/> instance to use in the current window</returns>
        public static SurfaceLoader GetInstance()
        {
            return GetInstance(CompositionTarget.GetCompositorForCurrentThread());
        }

        /// <summary>
        /// Gets a <see cref="SurfaceLoader"/> instance for a given <see cref="Compositor"/>
        /// </summary>
        /// <param name="compositor">The input <see cref="Compositor"/> object to use</param>
        /// <returns>A <see cref="SurfaceLoader"/> instance associated with <paramref name="compositor"/></returns>
        public static SurfaceLoader GetInstance(Compositor compositor)
        {
            lock (Instances)
            {
                if (Instances.TryGetValue(compositor, out var instance))
                {
                    return instance;
                }

                instance = new SurfaceLoader(compositor);

                Instances.Add(compositor, instance);

                return instance;
            }
        }

        /// <summary>
        /// The <see cref="Compositor"/> instance in use.
        /// </summary>
        private readonly Compositor compositor;

        /// <summary>
        /// The <see cref="CanvasDevice"/> instance in use.
        /// </summary>
        private CanvasDevice canvasDevice;

        /// <summary>
        /// The <see cref="CompositionGraphicsDevice"/> instance to determine which GPU is handling the request.
        /// </summary>
        private CompositionGraphicsDevice compositionDevice;

        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceLoader"/> class.
        /// </summary>
        /// <param name="compositor">The <see cref="Compositor"/> instance to use</param>
        private SurfaceLoader(Compositor compositor)
        {
            this.compositor = compositor;

            this.InitializeDevices();
        }

        /// <summary>
        /// Reloads the <see cref="canvasDevice"/> and <see cref="compositionDevice"/> fields.
        /// </summary>
        private void InitializeDevices()
        {
            if (!(this.canvasDevice is null))
            {
                this.canvasDevice.DeviceLost -= CanvasDevice_DeviceLost;
            }

            if (!(this.compositionDevice is null))
            {
                this.compositionDevice.RenderingDeviceReplaced -= CompositionDevice_RenderingDeviceReplaced;
            }

            this.canvasDevice = new CanvasDevice();
            this.compositionDevice = CanvasComposition.CreateCompositionGraphicsDevice(this.compositor, this.canvasDevice);

            this.canvasDevice.DeviceLost += CanvasDevice_DeviceLost;
            this.compositionDevice.RenderingDeviceReplaced += CompositionDevice_RenderingDeviceReplaced;
        }

        /// <summary>
        /// Invokes <see cref="InitializeDevices"/> when the current <see cref="CanvasDevice"/> is lost.
        /// </summary>
        private void CanvasDevice_DeviceLost(CanvasDevice sender, object args)
        {
            InitializeDevices();
        }

        /// <summary>
        /// Invokes <see cref="InitializeDevices"/> when the current <see cref="CompositionGraphicsDevice"/> changes rendering device.
        /// </summary>
        private void CompositionDevice_RenderingDeviceReplaced(CompositionGraphicsDevice sender, RenderingDeviceReplacedEventArgs args)
        {
            InitializeDevices();
        }

        /// <summary>
        /// Loads an image from the URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns><see cref="CompositionDrawingSurface"/></returns>
        public async Task<CompositionDrawingSurface> LoadFromUri(Uri uri)
        {
            return await LoadFromUri(uri, Size.Empty);
        }

        /// <summary>
        /// Loads an image from URI with a specified size.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="sizeTarget">The size target.</param>
        /// <returns><see cref="CompositionDrawingSurface"/></returns>
        public async Task<CompositionDrawingSurface> LoadFromUri(Uri uri, Size sizeTarget)
        {
            // WinUI3/Win2D bug: switch back to CanvasBitmap once it works.
            var bitmap = await CanvasVirtualBitmap.LoadAsync(canvasDevice, uri);
            var sizeSource = bitmap.Size;

            if (sizeTarget.IsEmpty)
            {
                sizeTarget = sizeSource;
            }

            var surface = compositionDevice.CreateDrawingSurface(
                sizeTarget,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                DirectXAlphaMode.Premultiplied);

            using (var ds = CanvasComposition.CreateDrawingSession(surface))
            {
                ds.Clear(Color.FromArgb(0, 0, 0, 0));
                ds.DrawImage(bitmap, new Rect(0, 0, sizeTarget.Width, sizeTarget.Height), new Rect(0, 0, sizeSource.Width, sizeSource.Height));
            }

            return surface;
        }

        /// <summary>
        /// Loads the text on to a <see cref="CompositionDrawingSurface"/>.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="sizeTarget">The size target.</param>
        /// <param name="textFormat">The text format.</param>
        /// <param name="textColor">Color of the text.</param>
        /// <param name="bgColor">Color of the bg.</param>
        /// <returns><see cref="CompositionDrawingSurface"/></returns>
        public CompositionDrawingSurface LoadText(string text, Size sizeTarget, CanvasTextFormat textFormat, Color textColor, Color bgColor)
        {
            var surface = compositionDevice.CreateDrawingSurface(
                sizeTarget,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                DirectXAlphaMode.Premultiplied);

            using (var ds = CanvasComposition.CreateDrawingSession(surface))
            {
                ds.Clear(bgColor);
                ds.DrawText(text, new Rect(0, 0, sizeTarget.Width, sizeTarget.Height), textColor, textFormat);
            }

            return surface;
        }

        /// <summary>
        /// Loads an image from URI, with a specified size.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="sizeTarget">The size target.</param>
        /// <param name="loadEffectHandler">The load effect handler callback.</param>
        /// <returns><see cref="CompositionDrawingSurface"/></returns>
        public async Task<CompositionDrawingSurface> LoadFromUri(Uri uri, Size sizeTarget, LoadTimeEffectHandler loadEffectHandler)
        {
            if (loadEffectHandler != null)
            {
                // WinUI3/Win2D bug: switch back to CanvasBitmap once it works.
                var bitmap = await CanvasVirtualBitmap.LoadAsync(canvasDevice, uri);
                return loadEffectHandler(bitmap, compositionDevice, sizeTarget);
            }

            return await LoadFromUri(uri, sizeTarget);
        }
    }
}