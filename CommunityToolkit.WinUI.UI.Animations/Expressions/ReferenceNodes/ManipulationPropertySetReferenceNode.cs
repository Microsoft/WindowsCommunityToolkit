// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Composition;

namespace CommunityToolkit.WinUI.UI.Animations.Expressions
{
    /// <summary>
    /// Class ManipulationPropertySetReferenceNode. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="CommunityToolkit.WinUI.UI.Animations.Expressions.PropertySetReferenceNode" />
    public sealed class ManipulationPropertySetReferenceNode : PropertySetReferenceNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManipulationPropertySetReferenceNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="ps">The ps.</param>
        internal ManipulationPropertySetReferenceNode(string paramName, CompositionPropertySet ps = null)
            : base(paramName, ps)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ManipulationPropertySetReferenceNode"/> class.
        /// Needed for GetSpecializedReference
        /// </summary>
        internal ManipulationPropertySetReferenceNode()
            : base(null, null)
        {
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
        /// Gets the pan.
        /// </summary>
        /// <value>The pan.</value>
        public Vector3Node Pan
        {
            get { return ReferenceProperty<Vector3Node>("Pan"); }
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
        /// Gets the translation.
        /// </summary>
        /// <value>The translation.</value>
        public Vector3Node Translation
        {
            get { return ReferenceProperty<Vector3Node>("Translation"); }
        }

        /// <summary>
        /// Gets the matrix.
        /// </summary>
        /// <value>The matrix.</value>
        public Matrix4x4Node Matrix
        {
            get { return ReferenceProperty<Matrix4x4Node>("Matrix"); }
        }
    }
}