// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.UI;

namespace CommunityToolkit.WinUI.UI.Animations.Expressions
{
    // Ignore warning: 'ColorNode' defines operator == or operator != but does not override Object.Equals(object o) && Object.GetHashCode()
#pragma warning disable CS0660, CS0661
    /// <summary>
    /// Class ColorNode. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="CommunityToolkit.WinUI.UI.Animations.Expressions.ExpressionNode" />
    public sealed class ColorNode : ExpressionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorNode"/> class.
        /// </summary>
        internal ColorNode()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorNode"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal ColorNode(Windows.UI.Color value)
        {
            _value = value;
            NodeType = ExpressionNodeType.ConstantValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        internal ColorNode(string paramName)
        {
            ParamName = paramName;
            NodeType = ExpressionNodeType.ConstantParameter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        internal ColorNode(string paramName, Windows.UI.Color value)
        {
            ParamName = paramName;
            _value = value;
            NodeType = ExpressionNodeType.ConstantParameter;

            SetColorParameter(paramName, value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="T:Color"/> to <see cref="ColorNode"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ColorNode(Windows.UI.Color value)
        {
            return new ColorNode(value);
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static BooleanNode operator ==(ColorNode left, ColorNode right)
        {
            return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.Equals, left, right);
        }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static BooleanNode operator !=(ColorNode left, ColorNode right)
        {
            return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.NotEquals, left, right);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>System.String.</returns>
        protected internal override string GetValue()
        {
            return $"ColorRgb({_value.A},{_value.R},{_value.G},{_value.B})";
        }

        private Windows.UI.Color _value;
    }
#pragma warning restore CS0660, CS0661
}