// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Media.Pipelines;
using Windows.System;

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// A <see langword="delegate"/> that represents a custom effect setter that can be applied to a <see cref="XamlCompositionBrush"/> instance
    /// </summary>
    /// <typeparam name="T">The type of property value to set</typeparam>
    /// <param name="value">The effect target value</param>
    public delegate void XamlEffectSetter<in T>(T value)
        where T : unmanaged;

    /// <summary>
    /// A <see langword="delegate"/> that represents a custom effect animation that can be applied to a <see cref="XamlCompositionBrush"/> instance
    /// </summary>
    /// <typeparam name="T">The type of property value to animate</typeparam>
    /// <param name="value">The animation target value</param>
    /// <param name="duration">The animation duration</param>
    /// <returns>A <see cref="Task"/> that completes when the target animation completes</returns>
    public delegate Task XamlEffectAnimation<in T>(T value, TimeSpan duration)
        where T : unmanaged;

    /// <summary>
    /// A simple <see langword="class"/> that can be used to quickly create XAML brushes from arbitrary <see cref="PipelineBuilder"/> pipelines
    /// </summary>
    public sealed class XamlCompositionBrush : XamlCompositionEffectBrushBase
    {
        /// <summary>
        /// Gets the <see cref="PipelineBuilder"/> pipeline for the current instance
        /// </summary>
        public PipelineBuilder Pipeline { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="XamlCompositionBrush"/> class.
        /// </summary>
        /// <param name="pipeline">The <see cref="PipelineBuilder"/> instance to create the effect</param>
        public XamlCompositionBrush(PipelineBuilder pipeline) => this.Pipeline = pipeline;

        /// <summary>
        /// Binds an <see cref="EffectSetter{T}"/> to the composition brush in the current instance
        /// </summary>
        /// <typeparam name="T">The type of property value to set</typeparam>
        /// <param name="setter">The input setter</param>
        /// <param name="bound">The resulting setter</param>
        /// <returns>The current <see cref="XamlCompositionBrush"/> instance</returns>
        [Pure]
        public XamlCompositionBrush Bind<T>(EffectSetter<T> setter, out XamlEffectSetter<T> bound)
            where T : unmanaged
        {
            bound = value => setter(this.CompositionBrush, value);

            return this;
        }

        /// <summary>
        /// Binds an <see cref="EffectAnimation{T}"/> to the composition brush in the current instance
        /// </summary>
        /// <typeparam name="T">The type of property value to animate</typeparam>
        /// <param name="animation">The input animation</param>
        /// <param name="bound">The resulting animation</param>
        /// <returns>The current <see cref="XamlCompositionBrush"/> instance</returns>
        [Pure]
        public XamlCompositionBrush Bind<T>(EffectAnimation<T> animation, out XamlEffectAnimation<T> bound)
            where T : unmanaged
        {
            bound = (value, duration) => animation(this.CompositionBrush, value, duration);

            return this;
        }

        /// <inheritdoc cref="XamlCompositionEffectBrushBase"/>
        protected override PipelineBuilder OnPipelineRequested() => this.Pipeline;

        /// <summary>
        /// Clones the current instance by rebuilding the source <see cref="Windows.UI.Xaml.Media.Brush"/>. Use this method to reuse the same effects pipeline on a different <see cref="DispatcherQueue"/>
        /// </summary>
        /// <remarks>
        /// If your code is already on the same thread, you can just assign this brush to an arbitrary number of controls and it will still work correctly.
        /// This method is only meant to be used to create a new instance of this brush using the same pipeline, on threads that can't access the current instance, for example in secondary app windows.
        /// </remarks>
        /// <returns>A <see cref="XamlCompositionBrush"/> instance using the current effects pipeline</returns>
        [Pure]
        public XamlCompositionBrush Clone()
        {
            return new XamlCompositionBrush(this.Pipeline);
        }
    }
}