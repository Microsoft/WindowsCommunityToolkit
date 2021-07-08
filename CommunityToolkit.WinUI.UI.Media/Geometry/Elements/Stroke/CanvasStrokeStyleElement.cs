// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.WinUI.UI.Media.Geometry.Core;
using Microsoft.Graphics.Canvas.Geometry;

namespace CommunityToolkit.WinUI.UI.Media.Geometry.Elements.Stroke
{
    /// <summary>
    /// Represents a CanvasStrokeStyle Element.
    /// </summary>
    internal sealed class CanvasStrokeStyleElement : ICanvasStrokeStyleElement
    {
        /// <summary>
        /// Gets the Stroke data defining the Brush Element
        /// </summary>
        public string Data { get; private set; }

        /// <summary>
        /// Gets the number of non-whitespace characters in
        /// the Stroke Data
        /// </summary>
        public int ValidationCount { get; private set; }

        /// <summary>
        /// Gets the CanvasStrokeStyle obtained by parsing
        /// the style data.
        /// </summary>
        public CanvasStrokeStyle Style { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasStrokeStyleElement"/> class.
        /// </summary>
        /// <param name="match">The matching data</param>
        public CanvasStrokeStyleElement(Match match)
        {
            Data = match.Groups["CanvasStrokeStyle"].Value;

            Style = new CanvasStrokeStyle();

            Initialize(match);

            // Get the number of non-whitespace characters in the data
            ValidationCount = RegexFactory.ValidationRegex.Replace(Data, string.Empty).Length;
        }

        /// <summary>
        /// Initializes the Stroke Element with the given Capture.
        /// </summary>
        /// <param name="match">Match object</param>
        public void Initialize(Match match)
        {
            var group = match.Groups["CanvasStrokeStyle"];
            if (group.Success)
            {
                // DashStyle
                group = match.Groups["DashStyle"];
                if (group.Success)
                {
                    Enum.TryParse(group.Value, out CanvasDashStyle dashStyle);
                    Style.DashStyle = dashStyle;
                }

                // LineJoin
                group = match.Groups["LineJoin"];
                if (group.Success)
                {
                    Enum.TryParse(group.Value, out CanvasLineJoin lineJoin);
                    Style.LineJoin = lineJoin;
                }

                // MiterLimit
                group = match.Groups["MiterLimit"];
                if (group.Success)
                {
                    float.TryParse(group.Value, out var miterLimit);

                    // Sanitize by taking the absolute value
                    Style.MiterLimit = Math.Abs(miterLimit);
                }

                // DashOffset
                group = match.Groups["DashOffset"];
                if (group.Success)
                {
                    float.TryParse(group.Value, out var dashOffset);
                    Style.DashOffset = dashOffset;
                }

                // StartCap
                group = match.Groups["StartCap"];
                if (group.Success)
                {
                    Enum.TryParse(group.Value, out CanvasCapStyle capStyle);
                    Style.StartCap = capStyle;
                }

                // EndCap
                group = match.Groups["EndCap"];
                if (group.Success)
                {
                    Enum.TryParse(group.Value, out CanvasCapStyle capStyle);
                    Style.EndCap = capStyle;
                }

                // DashCap
                group = match.Groups["DashCap"];
                if (group.Success)
                {
                    Enum.TryParse(group.Value, out CanvasCapStyle capStyle);
                    Style.DashCap = capStyle;
                }

                // TransformBehavior
                group = match.Groups["TransformBehavior"];
                if (group.Success)
                {
                    Enum.TryParse(group.Value, out CanvasStrokeTransformBehavior transformBehavior);
                    Style.TransformBehavior = transformBehavior;
                }

                // CustomDashStyle
                group = match.Groups["CustomDashStyle"];
                if (group.Success)
                {
                    List<float> dashes = new List<float>();
                    group = match.Groups["Main"];
                    if (group.Success)
                    {
                        if (float.TryParse(match.Groups["DashSize"].Value, out var dashSize))
                        {
                            // Sanitize by taking the absolute value
                            dashes.Add(Math.Abs(dashSize));
                        }

                        if (float.TryParse(match.Groups["SpaceSize"].Value, out var spaceSize))
                        {
                            // Sanitize by taking the absolute value
                            dashes.Add(Math.Abs(spaceSize));
                        }
                    }

                    group = match.Groups["Additional"];
                    if (group.Success)
                    {
                        foreach (Capture capture in group.Captures)
                        {
                            var dashMatch = RegexFactory.CustomDashAttributeRegex.Match(capture.Value);
                            if (!dashMatch.Success)
                            {
                                continue;
                            }

                            if (float.TryParse(dashMatch.Groups["DashSize"].Value, out var dashSize))
                            {
                                // Sanitize by taking the absolute value
                                dashes.Add(Math.Abs(dashSize));
                            }

                            if (float.TryParse(dashMatch.Groups["SpaceSize"].Value, out var spaceSize))
                            {
                                // Sanitize by taking the absolute value
                                dashes.Add(Math.Abs(spaceSize));
                            }
                        }
                    }

                    // Any valid dashes?
                    if (dashes.Any())
                    {
                        Style.CustomDashStyle = dashes.ToArray();
                    }
                }
            }
        }
    }
}