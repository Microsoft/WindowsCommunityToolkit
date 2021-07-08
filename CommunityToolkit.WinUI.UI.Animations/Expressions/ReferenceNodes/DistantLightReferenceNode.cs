// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Composition;

namespace CommunityToolkit.WinUI.UI.Animations.Expressions
{
    /// <summary>
    /// Class DistantLightReferenceNode. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="CommunityToolkit.WinUI.UI.Animations.Expressions.ReferenceNode" />
    public sealed class DistantLightReferenceNode : ReferenceNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DistantLightReferenceNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="light">The light.</param>
        internal DistantLightReferenceNode(string paramName, DistantLight light = null)
            : base(paramName, light)
        {
        }

        /// <summary>
        /// Creates the target reference.
        /// </summary>
        /// <returns>DistantLightReferenceNode.</returns>
        internal static DistantLightReferenceNode CreateTargetReference()
        {
            var node = new DistantLightReferenceNode(null);
            node.NodeType = ExpressionNodeType.TargetReference;

            return node;
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <value>The color.</value>
        public ColorNode Color
        {
            get { return ReferenceProperty<ColorNode>("Color"); }
        }

        /// <summary>
        /// Gets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public Vector3Node Direction
        {
            get { return ReferenceProperty<Vector3Node>("Direction"); }
        }
    }
}