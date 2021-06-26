// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Media.Pipelines;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Microsoft.Toolkit.Uwp.UI.Media
{
    /// <summary>
    /// A builder type for <see cref="SpriteVisual"/> instance to apply to UI elements.
    /// </summary>
    [ContentProperty(Name = nameof(Effects))]
    public sealed class PipelineVisualFactory : PipelineVisualFactoryBase
    {
        /// <summary>
        /// Gets or sets the source for the current pipeline (defaults to a <see cref="BackdropSourceExtension"/> with <see cref="AcrylicBackgroundSource.Backdrop"/> source).
        /// </summary>
        public PipelineBuilder Source { get; set; }

        /// <summary>
        /// Gets or sets the collection of effects to use in the current pipeline.
        /// </summary>
        public IList<PipelineEffect> Effects
        {
            get
            {
                if (GetValue(EffectsProperty) is not IList<PipelineEffect> effects)
                {
                    effects = new List<PipelineEffect>();

                    SetValue(EffectsProperty, effects);
                }

                return effects;
            }
            set => SetValue(EffectsProperty, value);
        }

        /// <summary>
        /// Identifies the <seealso cref="Effects"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EffectsProperty = DependencyProperty.Register(
            nameof(Effects),
            typeof(IList<PipelineEffect>),
            typeof(PipelineVisualFactory),
            new PropertyMetadata(null));

        /// <inheritdoc/>
        public override async ValueTask<Visual> GetAttachedVisualAsync(UIElement element)
        {
            var visual = (SpriteVisual)await base.GetAttachedVisualAsync(element);

            foreach (IPipelineEffect effect in Effects)
            {
                effect.NotifyCompositionBrushInUse(visual.Brush);
            }

            return visual;
        }

        /// <inheritdoc/>
        protected override PipelineBuilder OnPipelineRequested()
        {
            PipelineBuilder builder = Source ?? PipelineBuilder.FromBackdrop();

            foreach (IPipelineEffect effect in Effects)
            {
                builder = effect.AppendToBuilder(builder);
            }

            return builder;
        }
    }
}