// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI.Composition.Interactions;

namespace CommunityToolkit.WinUI.UI.Animations.Expressions
{
    /// <summary>
    /// Class InteractionTrackerReferenceNode. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="CommunityToolkit.WinUI.UI.Animations.Expressions.ReferenceNode" />
    public sealed class InteractionTrackerReferenceNode : ReferenceNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InteractionTrackerReferenceNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="it">It.</param>
        internal InteractionTrackerReferenceNode(string paramName, InteractionTracker it = null)
            : base(paramName, it)
        {
        }

        /// <summary>
        /// Creates the target reference.
        /// </summary>
        /// <returns>InteractionTrackerReferenceNode.</returns>
        internal static InteractionTrackerReferenceNode CreateTargetReference()
        {
            var node = new InteractionTrackerReferenceNode(null);
            node.NodeType = ExpressionNodeType.TargetReference;

            return node;
        }

        /// <summary>
        /// Gets the is position rounding suggested.
        /// </summary>
        /// <value>The is position rounding suggested.</value>
        public BooleanNode IsPositionRoundingSuggested
        {
            get { return ReferenceProperty<BooleanNode>("IsPositionRoundingSuggested"); }
        }

        /// <summary>
        /// Gets the minimum scale.
        /// </summary>
        /// <value>The minimum scale.</value>
        public ScalarNode MinScale
        {
            get { return ReferenceProperty<ScalarNode>("MinScale"); }
        }

        /// <summary>
        /// Gets the maximum scale.
        /// </summary>
        /// <value>The maximum scale.</value>
        public ScalarNode MaxScale
        {
            get { return ReferenceProperty<ScalarNode>("MaxScale"); }
        }

        /// <summary>
        /// Gets the natural resting scale.
        /// </summary>
        /// <value>The natural resting scale.</value>
        public ScalarNode NaturalRestingScale
        {
            get { return ReferenceProperty<ScalarNode>("NaturalRestingScale"); }
        }

        /// <summary>
        /// Gets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public ScalarNode Scale
        {
            get { return ReferenceProperty<ScalarNode>("Scale"); }
        }

        /// <summary>
        /// Gets the scale inertia decay rate.
        /// </summary>
        /// <value>The scale inertia decay rate.</value>
        public ScalarNode ScaleInertiaDecayRate
        {
            get { return ReferenceProperty<ScalarNode>("ScaleInertiaDecayRate"); }
        }

        /// <summary>
        /// Gets the scale velocity in percent per second.
        /// </summary>
        /// <value>The scale velocity in percent per second.</value>
        public ScalarNode ScaleVelocityInPercentPerSecond
        {
            get { return ReferenceProperty<ScalarNode>("ScaleVelocityInPercentPerSecond"); }
        }

        /// <summary>
        /// Gets the minimum position.
        /// </summary>
        /// <value>The minimum position.</value>
        public Vector3Node MinPosition
        {
            get { return ReferenceProperty<Vector3Node>("MinPosition"); }
        }

        /// <summary>
        /// Gets the maximum position.
        /// </summary>
        /// <value>The maximum position.</value>
        public Vector3Node MaxPosition
        {
            get { return ReferenceProperty<Vector3Node>("MaxPosition"); }
        }

        /// <summary>
        /// Gets the natural resting position.
        /// </summary>
        /// <value>The natural resting position.</value>
        public Vector3Node NaturalRestingPosition
        {
            get { return ReferenceProperty<Vector3Node>("NaturalRestingPosition"); }
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public Vector3Node Position
        {
            get { return ReferenceProperty<Vector3Node>("Position"); }
        }

        /// <summary>
        /// Gets the position inertia decay rate.
        /// </summary>
        /// <value>The position inertia decay rate.</value>
        public Vector3Node PositionInertiaDecayRate
        {
            get { return ReferenceProperty<Vector3Node>("PositionInertiaDecayRate"); }
        }

        /// <summary>
        /// Gets the position velocity in pixels per second.
        /// </summary>
        /// <value>The position velocity in pixels per second.</value>
        public Vector3Node PositionVelocityInPixelsPerSecond
        {
            get { return ReferenceProperty<Vector3Node>("PositionVelocityInPixelsPerSecond"); }
        }
    }
}