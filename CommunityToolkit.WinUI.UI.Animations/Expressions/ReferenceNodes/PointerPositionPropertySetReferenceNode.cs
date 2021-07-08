// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Composition;

namespace CommunityToolkit.WinUI.UI.Animations.Expressions
{
    /// <summary>
    /// Class PointerPositionPropertySetReferenceNode. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="CommunityToolkit.WinUI.UI.Animations.Expressions.PropertySetReferenceNode" />
    public sealed class PointerPositionPropertySetReferenceNode : PropertySetReferenceNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointerPositionPropertySetReferenceNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="ps">The ps.</param>
        internal PointerPositionPropertySetReferenceNode(string paramName, CompositionPropertySet ps = null)
            : base(paramName, ps)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointerPositionPropertySetReferenceNode"/> class.
        /// Needed for GetSpecializedReference
        /// </summary>
        internal PointerPositionPropertySetReferenceNode()
            : base(null, null)
        {
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3Node Position
        {
            get { return ReferenceProperty<Vector3Node>("Position"); }
        }
    }
}