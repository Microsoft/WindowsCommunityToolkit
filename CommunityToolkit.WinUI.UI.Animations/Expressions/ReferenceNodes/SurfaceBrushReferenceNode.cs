// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Composition;

namespace CommunityToolkit.WinUI.UI.Animations.Expressions
{
    /// <summary>
    /// Class SurfaceBrushReferenceNode. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="CommunityToolkit.WinUI.UI.Animations.Expressions.ReferenceNode" />
    public sealed class SurfaceBrushReferenceNode : ReferenceNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SurfaceBrushReferenceNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="brush">The brush.</param>
        internal SurfaceBrushReferenceNode(string paramName, CompositionSurfaceBrush brush = null)
            : base(paramName, brush)
        {
        }

        /// <summary>
        /// Creates the target reference.
        /// </summary>
        /// <returns>SurfaceBrushReferenceNode.</returns>
        internal static SurfaceBrushReferenceNode CreateTargetReference()
        {
            var node = new SurfaceBrushReferenceNode(null);
            node.NodeType = ExpressionNodeType.TargetReference;

            return node;
        }

        /// <summary>
        /// Gets the horizontal alignment ratio.
        /// </summary>
        /// <value>The horizontal alignment ratio.</value>
        public ScalarNode HorizontalAlignmentRatio
        {
            get { return ReferenceProperty<ScalarNode>("HorizontalAlignmentRatio"); }
        }

        /// <summary>
        /// Gets the vertical alignment ratio.
        /// </summary>
        /// <value>The vertical alignment ratio.</value>
        public ScalarNode VerticalAlignmentRatio
        {
            get { return ReferenceProperty<ScalarNode>("VerticalAlignmentRatio"); }
        }

        /// <summary>
        /// Gets the bottom inset.
        /// </summary>
        /// <value>The bottom inset.</value>
        public ScalarNode BottomInset
        {
            get { return ReferenceProperty<ScalarNode>("BottomInset"); }
        }

        /// <summary>
        /// Gets the left inset.
        /// </summary>
        /// <value>The left inset.</value>
        public ScalarNode LeftInset
        {
            get { return ReferenceProperty<ScalarNode>("LeftInset"); }
        }

        /// <summary>
        /// Gets the right inset.
        /// </summary>
        /// <value>The right inset.</value>
        public ScalarNode RightInset
        {
            get { return ReferenceProperty<ScalarNode>("RightInset"); }
        }

        /// <summary>
        /// Gets the top inset.
        /// </summary>
        /// <value>The top inset.</value>
        public ScalarNode TopInset
        {
            get { return ReferenceProperty<ScalarNode>("TopInset"); }
        }

        /// <summary>
        /// Gets the rotation angle.
        /// </summary>
        /// <value>The rotation angle.</value>
        public ScalarNode RotationAngle
        {
            get { return ReferenceProperty<ScalarNode>("RotationAngle"); }
        }

        /// <summary>
        /// Gets the rotation angle in degrees.
        /// </summary>
        /// <value>The rotation angle in degrees.</value>
        public ScalarNode RotationAngleInDegrees
        {
            get { return ReferenceProperty<ScalarNode>("RotationAngleInDegrees"); }
        }

        /// <summary>
        /// Gets the anchor point.
        /// </summary>
        /// <value>The anchor point.</value>
        public Vector2Node AnchorPoint
        {
            get { return ReferenceProperty<Vector2Node>("AnchorPoint"); }
        }

        /// <summary>
        /// Gets the center point.
        /// </summary>
        /// <value>The center point.</value>
        public Vector2Node CenterPoint
        {
            get { return ReferenceProperty<Vector2Node>("CenterPoint"); }
        }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public Vector2Node Offset
        {
            get { return ReferenceProperty<Vector2Node>("Offset"); }
        }

        /// <summary>
        /// Gets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public Vector2Node Scale
        {
            get { return ReferenceProperty<Vector2Node>("Scale"); }
        }

        /// <summary>
        /// Gets the transform matrix.
        /// </summary>
        /// <value>The transform matrix.</value>
        public Matrix3x2Node TransformMatrix
        {
            get { return ReferenceProperty<Matrix3x2Node>("TransformMatrix"); }
        }
    }
}