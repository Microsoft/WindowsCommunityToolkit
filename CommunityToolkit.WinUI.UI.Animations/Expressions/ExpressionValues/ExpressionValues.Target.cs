// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Animations.Expressions
{
    // ExpressionValues is a static class instead of a namespace to improve intellisense discoverablity and consistency with the other helper classes.

    /// <summary>
    /// Class ExpressionValues.
    /// </summary>
    public static partial class ExpressionValues
    {
        /// <summary>
        /// Create a reference to the CompositionObject this expression will be connected to.
        /// </summary>
        public static class Target
        {
            /// <summary>
            /// Create a reference to the AmbientLight target that this expression will be connected to.
            /// </summary>
            /// <returns>AmbientLightReferenceNode.</returns>
            public static AmbientLightReferenceNode CreateAmbientLightTarget()
            {
                return AmbientLightReferenceNode.CreateTargetReference();
            }

            /// <summary>
            /// Create a reference to the ColorBrush target that this expression will be connected to.
            /// </summary>
            /// <returns>ColorBrushReferenceNode.</returns>
            public static ColorBrushReferenceNode CreateColorBrushTarget()
            {
                return ColorBrushReferenceNode.CreateTargetReference();
            }

            /// <summary>
            /// Create a reference to the DistantLight target that this expression will be connected to.
            /// </summary>
            /// <returns>DistantLightReferenceNode.</returns>
            public static DistantLightReferenceNode CreateDistantLightTarget()
            {
                return DistantLightReferenceNode.CreateTargetReference();
            }

            /// <summary>
            /// Create a reference to the DropShadow target that this expression will be connected to.
            /// </summary>
            /// <returns>DropShadowReferenceNode.</returns>
            public static DropShadowReferenceNode CreateDropShadowTarget()
            {
                return DropShadowReferenceNode.CreateTargetReference();
            }

            /// <summary>
            /// Create a reference to the InsetClip target that this expression will be connected to.
            /// </summary>
            /// <returns>InsetClipReferenceNode.</returns>
            public static InsetClipReferenceNode CreateInsetClipTarget()
            {
                return InsetClipReferenceNode.CreateTargetReference();
            }

            /// <summary>
            /// Create a reference to the InteractionTracker target that this expression will be connected to.
            /// </summary>
            /// <returns>InteractionTrackerReferenceNode.</returns>
            public static InteractionTrackerReferenceNode CreateInteractionTrackerTarget()
            {
                return InteractionTrackerReferenceNode.CreateTargetReference();
            }

            /// <summary>
            /// Create a reference to the NineGridBrush target that this expression will be connected to.
            /// </summary>
            /// <returns>NineGridBrushReferenceNode.</returns>
            public static NineGridBrushReferenceNode CreateNineGridBrushTarget()
            {
                return NineGridBrushReferenceNode.CreateTargetReference();
            }

            /// <summary>
            /// Create a reference to the PointLight target that this expression will be connected to.
            /// </summary>
            /// <returns>PointLightReferenceNode.</returns>
            public static PointLightReferenceNode CreatePointLightTarget()
            {
                return PointLightReferenceNode.CreateTargetReference();
            }

            /// <summary>
            /// Create a reference to the PropertySet target that this expression will be connected to.
            /// </summary>
            /// <returns>PropertySetReferenceNode.</returns>
            public static PropertySetReferenceNode CreatePropertySetTarget()
            {
                return PropertySetReferenceNode.CreateTargetReference();
            }

            /// <summary>
            /// Create a reference to the SpotLight target that this expression will be connected to.
            /// </summary>
            /// <returns>SpotLightReferenceNode.</returns>
            public static SpotLightReferenceNode CreateSpotLightTarget()
            {
                return SpotLightReferenceNode.CreateTargetReference();
            }

            /// <summary>
            /// Create a reference to the SurfaceBrush target that this expression will be connected to.
            /// </summary>
            /// <returns>SurfaceBrushReferenceNode.</returns>
            public static SurfaceBrushReferenceNode CreateSurfaceBrushTarget()
            {
                return SurfaceBrushReferenceNode.CreateTargetReference();
            }

            /// <summary>
            /// Create a reference to the Visual target that this expression will be connected to.
            /// </summary>
            /// <returns>VisualReferenceNode.</returns>
            public static VisualReferenceNode CreateVisualTarget()
            {
                return VisualReferenceNode.CreateTargetReference();
            }
        }
    }
}