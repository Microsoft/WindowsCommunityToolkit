// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using Microsoft.Graphics.DirectX;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Hosting;
using Windows.Graphics;
using Windows.Graphics.Display;

namespace CommunityToolkit.WinUI.UI.Controls
{
    /// <summary>
    /// The virtual Drawing surface renderer used to render the ink and text. This control is used as part of the <see cref="InfiniteCanvas"/>
    /// </summary>
    public partial class InfiniteCanvasVirtualDrawingSurface : Panel
    {
        private Compositor _compositor;

        private CanvasDevice _win2DDevice;

        private CompositionGraphicsDevice _compositionGraphicsDevice;
        private SpriteVisual _myDrawingVisual;

        private CompositionVirtualDrawingSurface _drawingSurface;
        private CompositionSurfaceBrush _surfaceBrush;
        private double _screenScale;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfiniteCanvasVirtualDrawingSurface"/> class.
        /// </summary>
        public InfiniteCanvasVirtualDrawingSurface()
        {
            InitializeComposition();
            SizeChanged += TheSurface_SizeChanged;
        }

        private void TheSurface_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _myDrawingVisual.Size = new Vector2((float)ActualWidth, (float)ActualHeight);
        }

        internal void InitializeComposition()
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

            _win2DDevice = CanvasDevice.GetSharedDevice();

            _compositionGraphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, _win2DDevice);
            _myDrawingVisual = _compositor.CreateSpriteVisual();
            ElementCompositionPreview.SetElementChildVisual(this, _myDrawingVisual);
        }

        internal void ConfigureSpriteVisual(double width, double height, float zoomFactor)
        {
            var size = new SizeInt32
            {
                Height = (int)width,
                Width = (int)height
            };

            _drawingSurface = _compositionGraphicsDevice.CreateVirtualDrawingSurface(
                size,
                DirectXPixelFormat.B8G8R8A8UIntNormalized,
                DirectXAlphaMode.Premultiplied);

            _surfaceBrush = _compositor.CreateSurfaceBrush(_drawingSurface);
            _surfaceBrush.Stretch = CompositionStretch.None;
            _surfaceBrush.HorizontalAlignmentRatio = 0;
            _surfaceBrush.VerticalAlignmentRatio = 0;
            _surfaceBrush.TransformMatrix = Matrix3x2.CreateTranslation(0, 0);

            SetScale(zoomFactor);

            _myDrawingVisual.Brush = _surfaceBrush;
            _surfaceBrush.Offset = new Vector2(0, 0);
        }

        internal void SetScale(float zoomFactor)
        {
            if (XamlRoot != null)
            {
                _screenScale = XamlRoot.RasterizationScale;
            }
            else
            {
                _screenScale = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            }

            var scale = _screenScale * zoomFactor;
            _surfaceBrush.Scale = new Vector2((float)(1 / scale));
            _surfaceBrush.BitmapInterpolationMode = CompositionBitmapInterpolationMode.NearestNeighbor;
        }
    }
}