// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Composition;

namespace CommunityToolkit.WinUI.UI.Animations.Expressions
{
    /// <summary>
    /// Class DropShadowReferenceNode. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="CommunityToolkit.WinUI.UI.Animations.Expressions.ReferenceNode" />
    public sealed class DropShadowReferenceNode : ReferenceNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DropShadowReferenceNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="source">The source.</param>
        internal DropShadowReferenceNode(string paramName, DropShadow source = null)
            : base(paramName, source)
        {
        }

        /// <summary>
        /// Creates the target reference.
        /// </summary>
        /// <returns>DropShadowReferenceNode.</returns>
        internal static DropShadowReferenceNode CreateTargetReference()
        {
            var node = new DropShadowReferenceNode(null);
            node.NodeType = ExpressionNodeType.TargetReference;

            return node;
        }

        /// <summary>
        /// Gets the blur radius.
        /// </summary>
        /// <value>The blur radius.</value>
        public ScalarNode BlurRadius
        {
            get { return ReferenceProperty<ScalarNode>("BlurRadius"); }
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
        /// Gets the offset.
        /// </summary>
        /// <value>The offset.</value>
        public Vector3Node Offset
        {
            get { return ReferenceProperty<Vector3Node>("Offset"); }
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <value>The color.</value>
        public ColorNode Color
        {
            get { return ReferenceProperty<ColorNode>("Color"); }
        }
    }
}