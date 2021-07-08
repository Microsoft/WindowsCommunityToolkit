// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Composition;

namespace CommunityToolkit.WinUI.UI.Animations.Expressions
{
    /// <summary>
    /// Class VisualReferenceNode. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="CommunityToolkit.WinUI.UI.Animations.Expressions.ReferenceNode" />
    public sealed class VisualReferenceNode : ReferenceNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualReferenceNode" /> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="v">The v.</param>
        internal VisualReferenceNode(string paramName, Visual v = null)
            : base(paramName, v)
        {
        }

        /// <summary>
        /// Creates the target reference.
        /// </summary>
        /// <returns>VisualReferenceNode.</returns>
        internal static VisualReferenceNode CreateTargetReference()
        {
            var node = new VisualReferenceNode(null);
            node.NodeType = ExpressionNodeType.TargetReference;

            return node;
        }

        /// <summary>
        /// Gets the opacity.
        /// </summary>
        /// <value>The opacity.</value>
        public ScalarNode Opacity
        {
            get { return ReferenceProperty<ScalarNode>("Opacity"); }
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
        /// Gets the size of the relative.
        /// </summary>
        /// <value>The size of the relative.</value>
        public Vector2Node RelativeSize
        {
            get { return ReferenceProperty<Vector2Node>("RelativeSize"); }
        }

        /// <summary>
        /// Gets the size.
        /// </summary>
        /// <value>The size.</value>
        public Vector2Node Size
        {
            get { return ReferenceProperty<Vector2Node>("Size"); }
        }

        /// <summary>
        /// Gets the center point.
        /// </summary>
        /// <value>The center point.</value>
        public Vector3Node CenterPoint
        {
            get { return ReferenceProperty<Vector3Node>("CenterPoint"); }
        }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public Vector3Node Offset
        {
            get { return ReferenceProperty<Vector3Node>("Offset"); }
        }

        /// <summary>
        /// Gets the relative offset.
        /// </summary>
        /// <value>The relative offset.</value>
        public Vector3Node RelativeOffset
        {
            get { return ReferenceProperty<Vector3Node>("RelativeOffset"); }
        }

        /// <summary>
        /// Gets the rotation axis.
        /// </summary>
        /// <value>The rotation axis.</value>
        public Vector3Node RotationAxis
        {
            get { return ReferenceProperty<Vector3Node>("RotationAxis"); }
        }

        /// <summary>
        /// Gets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public Vector3Node Scale
        {
            get { return ReferenceProperty<Vector3Node>("Scale"); }
        }

        /// <summary>
        /// Gets the Translation.
        /// </summary>
        public Vector3Node Translation
        {
            get { return GetVector3Property("Translation"); }
        }

        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <value>The orientation.</value>
        public QuaternionNode Orientation
        {
            get { return ReferenceProperty<QuaternionNode>("Orientation"); }
        }

        /// <summary>
        /// Gets the transform matrix.
        /// </summary>
        /// <value>The transform matrix.</value>
        public Matrix4x4Node TransformMatrix
        {
            get { return ReferenceProperty<Matrix4x4Node>("TransformMatrix"); }
        }
    }
}