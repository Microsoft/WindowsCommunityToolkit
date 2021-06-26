// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using Windows.Graphics.DirectX;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// Helper Brush class to interop with Win2D Canvas calls.
    /// </summary>
    public abstract class CanvasBrushBase : XamlCompositionBrushBase
    {
        private CompositionSurfaceBrush _surfaceBrush;

        /// <summary>
        /// Gets or sets the internal surface render width.  Modify during construction.
        /// </summary>
        protected float SurfaceWidth { get; set; }

        /// <summary>
        /// Gets or sets the internal surface render height.  Modify during construction.
        /// </summary>
        protected float SurfaceHeight { get; set; }

        private CanvasDevice _device;

        private CompositionGraphicsDevice _graphics;

        /// <summary>
        /// Implemented by parent class and called when canvas is being constructed for brush.
        /// </summary>
        /// <param name="device">Canvas device.</param>
        /// <param name="session">Canvas drawing session.</param>
        /// <param name="size">Size of surface to draw on.</param>
        /// <returns>True if drawing was completed and the brush is ready, otherwise return False to not create brush yet.</returns>
        protected abstract bool OnDraw(CanvasDevice device, CanvasDrawingSession session, Vector2 size);

        /// <summary>
        /// Initializes the Composition Brush.
        /// </summary>
        protected override void OnConnected()
        {
            base.OnConnected();

            if (_device != null)
            {
                _device.DeviceLost -= CanvasDevice_DeviceLost;
            }

            _device = CanvasDevice.GetSharedDevice();
            _device.DeviceLost += CanvasDevice_DeviceLost;

            if (_graphics != null)
            {
                _graphics.RenderingDeviceReplaced -= CanvasDevice_RenderingDeviceReplaced;
            }

            _graphics = CanvasComposition.CreateCompositionGraphicsDevice(Window.Current.Compositor, _device);
            _graphics.RenderingDeviceReplaced += CanvasDevice_RenderingDeviceReplaced;

            // Delay creating composition resources until they're required.
            if (CompositionBrush == null)
            {
                // Abort if effects aren't supported.
                if (!CompositionCapabilities.GetForCurrentView().AreEffectsSupported())
                {
                    return;
                }

                var size = new Vector2(SurfaceWidth, SurfaceHeight);
                var surface = _graphics.CreateDrawingSurface(size.ToSize(), DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);

                using (var session = CanvasComposition.CreateDrawingSession(surface))
                {
                    // Call Implementor to draw on session.
                    if (!OnDraw(_device, session, size))
                    {
                        return;
                    }
                }

                _surfaceBrush = Window.Current.Compositor.CreateSurfaceBrush(surface);
                _surfaceBrush.Stretch = CompositionStretch.Fill;

                CompositionBrush = _surfaceBrush;
            }
        }

        private void CanvasDevice_RenderingDeviceReplaced(CompositionGraphicsDevice sender, object args)
        {
            OnDisconnected();
            OnConnected();
        }

        private void CanvasDevice_DeviceLost(CanvasDevice sender, object args)
        {
            OnDisconnected();
            OnConnected();
        }

        /// <summary>
        /// Deconstructs the Composition Brush.
        /// </summary>
        protected override void OnDisconnected()
        {
            base.OnDisconnected();

            if (_device != null)
            {
                _device.DeviceLost -= CanvasDevice_DeviceLost;
                _device = null;
            }

            if (_graphics != null)
            {
                _graphics.RenderingDeviceReplaced -= CanvasDevice_RenderingDeviceReplaced;
                _graphics = null;
            }

            // Dispose of composition resources when no longer in use.
            if (CompositionBrush != null)
            {
                CompositionBrush.Dispose();
                CompositionBrush = null;
            }

            if (_surfaceBrush != null)
            {
                _surfaceBrush.Dispose();
                _surfaceBrush = null;
            }
        }
    }
}