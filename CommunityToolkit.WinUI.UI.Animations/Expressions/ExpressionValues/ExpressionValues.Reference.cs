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
        /// Create a reference to a CompositionObject.
        /// </summary>
        public static class Reference
        {
            /// <summary>
            /// Creates a named reference parameter to an AmbientLight.
            /// </summary>
            /// <param name="parameterName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>AmbientLightReferenceNode.</returns>
            public static AmbientLightReferenceNode CreateAmbientLightReference(string parameterName)
            {
                return new AmbientLightReferenceNode(parameterName);
            }

            /// <summary>
            /// Creates a named reference parameter to a ColorBrush.
            /// </summary>
            /// <param name="parameterName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>ColorBrushReferenceNode.</returns>
            public static ColorBrushReferenceNode CreateColorBrushReference(string parameterName)
            {
                return new ColorBrushReferenceNode(parameterName);
            }

            /// <summary>
            /// Creates a named reference parameter to a DistantLight.
            /// </summary>
            /// <param name="parameterName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>DistantLightReferenceNode.</returns>
            public static DistantLightReferenceNode CreateDistantLightReference(string parameterName)
            {
                return new DistantLightReferenceNode(parameterName);
            }

            /// <summary>
            /// Creates a named reference parameter to a DropShadow.
            /// </summary>
            /// <param name="parameterName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>DropShadowReferenceNode.</returns>
            public static DropShadowReferenceNode CreateDropShadowReference(string parameterName)
            {
                return new DropShadowReferenceNode(parameterName);
            }

            /// <summary>
            /// Creates a named reference parameter to an InsetClip.
            /// </summary>
            /// <param name="parameterName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>InsetClipReferenceNode.</returns>
            public static InsetClipReferenceNode CreateInsetClipReference(string parameterName)
            {
                return new InsetClipReferenceNode(parameterName);
            }

            /// <summary>
            /// Creates a named reference parameter to an InteractionTracker.
            /// </summary>
            /// <param name="parameterName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>InteractionTrackerReferenceNode.</returns>
            public static InteractionTrackerReferenceNode CreateInteractionTrackerReference(string parameterName)
            {
                return new InteractionTrackerReferenceNode(parameterName);
            }

            /// <summary>
            /// Creates a named reference parameter to a NineGridBrush.
            /// </summary>
            /// <param name="parameterName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>NineGridBrushReferenceNode.</returns>
            public static NineGridBrushReferenceNode CreateNineGridBrushReference(string parameterName)
            {
                return new NineGridBrushReferenceNode(parameterName);
            }

            /// <summary>
            /// Creates a named reference parameter to a PointLight.
            /// </summary>
            /// <param name="parameterName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>PointLightReferenceNode.</returns>
            public static PointLightReferenceNode CreatePointLightReference(string parameterName)
            {
                return new PointLightReferenceNode(parameterName);
            }

            /// <summary>
            /// Creates a named reference parameter to a PropertySet.
            /// </summary>
            /// <param name="parameterName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>PropertySetReferenceNode.</returns>
            public static PropertySetReferenceNode CreatePropertySetReference(string parameterName)
            {
                return new PropertySetReferenceNode(parameterName);
            }

            /// <summary>
            /// Creates a named reference parameter to a SpotLight.
            /// </summary>
            /// <param name="parameterName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>SpotLightReferenceNode.</returns>
            public static SpotLightReferenceNode CreateSpotLightReference(string parameterName)
            {
                return new SpotLightReferenceNode(parameterName);
            }

            /// <summary>
            /// Creates a named reference parameter to a SurfaceBrush.
            /// </summary>
            /// <param name="parameterName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>SurfaceBrushReferenceNode.</returns>
            public static SurfaceBrushReferenceNode CreateSurfaceBrushReference(string parameterName)
            {
                return new SurfaceBrushReferenceNode(parameterName);
            }

            /// <summary>
            /// Creates a named reference parameter to a Visual.
            /// </summary>
            /// <param name="parameterName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>VisualReferenceNode.</returns>
            public static VisualReferenceNode CreateVisualReference(string parameterName)
            {
                return new VisualReferenceNode(parameterName);
            }
        }
    }
}