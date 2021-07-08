// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using CommunityToolkit.WinUI.UI.Media.Pipelines;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;

namespace CommunityToolkit.WinUI.UI.Media
{
    /// <summary>
    /// A backdrop effect that can sample from a specified source
    /// </summary>
    [MarkupExtensionReturnType(ReturnType = typeof(PipelineBuilder))]
    public sealed class BackdropSourceExtension : MarkupExtension
    {
        /*
        /// <summary>
        /// Gets or sets the background source mode for the effect (the default is <see cref="AcrylicBackgroundSource.Backdrop"/>).
        /// </summary>
        public AcrylicBackgroundSource BackgroundSource { get; set; } = AcrylicBackgroundSource.Backdrop;
        */

        /// <inheritdoc/>
        protected override object ProvideValue()
        {
            /*
            return BackgroundSource switch
            {
                AcrylicBackgroundSource.Backdrop => PipelineBuilder.FromBackdrop(),
                AcrylicBackgroundSource.HostBackdrop => PipelineBuilder.FromHostBackdrop(),
                _ => throw new ArgumentException($"Invalid source for backdrop effect: {BackgroundSource}")
            };
            */
            return PipelineBuilder.FromBackdrop();
        }
    }
}