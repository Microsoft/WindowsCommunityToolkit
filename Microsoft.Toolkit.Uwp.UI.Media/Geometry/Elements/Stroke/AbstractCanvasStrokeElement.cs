// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using Microsoft.Graphics.Canvas;
using Microsoft.Toolkit.Uwp.UI.Media.Geometry.Core;

namespace Microsoft.Toolkit.Uwp.UI.Media.Geometry.Elements.Stroke
{
    /// <summary>
    /// Abstract base class for Stroke Element.
    /// </summary>
    internal abstract class AbstractCanvasStrokeElement : ICanvasStrokeElement
    {
        /// <summary>
        /// Gets or sets the Stroke data defining the Brush Element
        /// </summary>
        public string Data { get; protected set; }

        /// <summary>
        /// Gets or sets the number of non-whitespace characters in the Stroke Data.
        /// </summary>
        public int ValidationCount { get; protected set; }

        /// <summary>
        /// Initializes the Stroke Element with the given Capture.
        /// </summary>
        /// <param name="match">Match object</param>
        public virtual void Initialize(Match match)
        {
            Data = match.Value;

            if (!match.Success)
            {
                return;
            }

            GetAttributes(match);

            // Update the validation count
            Validate();
        }

        /// <summary>
        /// Gets the number of non-whitespace characters in the data.
        /// </summary>
        protected virtual void Validate()
        {
            ValidationCount = RegexFactory.ValidationRegex.Replace(Data, string.Empty).Length;
        }

        /// <summary>
        /// Creates the ICanvasStroke from the parsed data.
        /// </summary>
        /// <returns>ICanvasStroke</returns>
        public abstract ICanvasStroke CreateStroke(ICanvasResourceCreator resourceCreator);

        /// <summary>
        /// Gets the Stroke Element Attributes from the Match.
        /// </summary>
        /// <param name="match">Match object</param>
        protected abstract void GetAttributes(Match match);
    }
}