// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Toolkit.Uwp.UI.Media.Geometry.Core;

namespace Microsoft.Toolkit.Uwp.UI.Media.Geometry.Elements.Brush
{
    /// <summary>
    /// Abstract base class for all Brush Elements
    /// </summary>
    internal abstract class AbstractCanvasBrushElement : ICanvasBrushElement
    {
#pragma warning disable SA1401 // Fields should be private
        protected float _opacity;
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Gets or sets the Brush data defining the Brush Element
        /// </summary>
        public string Data { get; protected set; }

        /// <summary>
        /// Gets  or sets the number of non-whitespace characters in
        /// the Brush Data
        /// </summary>
        public int ValidationCount { get; protected set; }

        /// <summary>
        /// Initializes the Brush Element with the given Capture
        /// </summary>
        /// <param name="capture">Capture object</param>
        public virtual void Initialize(Capture capture)
        {
            Data = capture.Value;

            var regex = GetAttributesRegex();
            var match = regex.Match(Data);
            if (!match.Success)
            {
                return;
            }

            GetAttributes(match);

            // Get the number of non-whitespace characters in the data
            ValidationCount = RegexFactory.ValidationRegex.Replace(Data, string.Empty).Length;
        }

        /// <summary>
        /// Creates the ICanvasBrush from the parsed data
        /// </summary>
        /// <param name="resourceCreator">ICanvasResourceCreator object</param>
        /// <returns>ICanvasBrush</returns>
        public abstract ICanvasBrush CreateBrush(ICanvasResourceCreator resourceCreator);

        /// <summary>
        /// Gets the Regex for extracting Brush Element Attributes
        /// </summary>
        /// <returns>Regex</returns>
        protected abstract Regex GetAttributesRegex();

        /// <summary>
        /// Gets the Brush Element Attributes from the Match
        /// </summary>
        /// <param name="match">Match object</param>
        protected abstract void GetAttributes(Match match);
    }
}