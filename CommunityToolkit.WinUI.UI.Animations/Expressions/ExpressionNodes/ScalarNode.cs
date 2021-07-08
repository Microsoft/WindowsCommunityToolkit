// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Animations.Expressions
{
    // Ignore warning: 'ScalarNode' defines operator == or operator != but does not override Object.Equals(object o) && Object.GetHashCode()
#pragma warning disable CS0660, CS0661
    /// <summary>
    /// Class ScalarNode. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="CommunityToolkit.WinUI.UI.Animations.Expressions.ExpressionNode" />
    public sealed class ScalarNode : ExpressionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScalarNode"/> class.
        /// </summary>
        internal ScalarNode()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScalarNode"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal ScalarNode(float value)
        {
            _value = value;
            NodeType = ExpressionNodeType.ConstantValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScalarNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        internal ScalarNode(string paramName)
        {
            ParamName = paramName;
            NodeType = ExpressionNodeType.ConstantParameter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScalarNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        internal ScalarNode(string paramName, float value)
        {
            ParamName = paramName;
            _value = value;
            NodeType = ExpressionNodeType.ConstantParameter;

            SetScalarParameter(paramName, value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="float"/> to <see cref="ScalarNode"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ScalarNode(float value)
        {
            return new ScalarNode(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="int"/> to <see cref="ScalarNode"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ScalarNode(int value)
        {
            return new ScalarNode((float)value);
        }

        /// <summary>
        /// Implements the + operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static ScalarNode operator +(ScalarNode left, ScalarNode right)
        {
            return ExpressionFunctions.Function<ScalarNode>(ExpressionNodeType.Add, left, right);
        }

        /// <summary>
        /// Implements the - operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static ScalarNode operator -(ScalarNode left, ScalarNode right)
        {
            return ExpressionFunctions.Function<ScalarNode>(ExpressionNodeType.Subtract, left, right);
        }

        /// <summary>
        /// Implements the - operator.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static ScalarNode operator -(ScalarNode value)
        {
            return ExpressionFunctions.Function<ScalarNode>(ExpressionNodeType.Negate, value);
        }

        /// <summary>
        /// Implements the * operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static ScalarNode operator *(ScalarNode left, ScalarNode right)
        {
            return ExpressionFunctions.Function<ScalarNode>(ExpressionNodeType.Multiply, left, right);
        }

        /// <summary>
        /// Implements the * operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2Node operator *(ScalarNode left, Vector2Node right)
        {
            return ExpressionFunctions.Function<Vector2Node>(ExpressionNodeType.Multiply, left, right);
        }

        /// <summary>
        /// Implements the * operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector3Node operator *(ScalarNode left, Vector3Node right)
        {
            return ExpressionFunctions.Function<Vector3Node>(ExpressionNodeType.Multiply, left, right);
        }

        /// <summary>
        /// Implements the * operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector4Node operator *(ScalarNode left, Vector4Node right)
        {
            return ExpressionFunctions.Function<Vector4Node>(ExpressionNodeType.Multiply, left, right);
        }

        /// <summary>
        /// Implements the * operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Matrix4x4Node operator *(ScalarNode left, Matrix4x4Node right)
        {
            return ExpressionFunctions.Function<Matrix4x4Node>(ExpressionNodeType.Multiply, left, right);
        }

        /// <summary>
        /// Implements the / operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static ScalarNode operator /(ScalarNode left, ScalarNode right)
        {
            return ExpressionFunctions.Function<ScalarNode>(ExpressionNodeType.Divide, left, right);
        }

        /// <summary>
        /// Implements the % operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static ScalarNode operator %(ScalarNode left, ScalarNode right)
        {
            return ExpressionFunctions.Function<ScalarNode>(ExpressionNodeType.Modulus, left, right);
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static BooleanNode operator ==(ScalarNode left, ScalarNode right)
        {
            return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.Equals, left, right);
        }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static BooleanNode operator !=(ScalarNode left, ScalarNode right)
        {
            return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.NotEquals, left, right);
        }

        /// <summary>
        /// Implements the &lt;= operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static BooleanNode operator <=(ScalarNode left, ScalarNode right)
        {
            return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.LessThanEquals, left, right);
        }

        /// <summary>
        /// Implements the &lt; operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static BooleanNode operator <(ScalarNode left, ScalarNode right)
        {
            return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.LessThan, left, right);
        }

        /// <summary>
        /// Implements the &gt;= operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static BooleanNode operator >=(ScalarNode left, ScalarNode right)
        {
            return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.GreaterThanEquals, left, right);
        }

        /// <summary>
        /// Implements the &gt; operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static BooleanNode operator >(ScalarNode left, ScalarNode right)
        {
            return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.GreaterThan, left, right);
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>System.String.</returns>
        protected internal override string GetValue()
        {
            // Important to use invariant culture to make sure that floats are written using a .
            return _value.ToString(global::System.Globalization.CultureInfo.InvariantCulture);
        }

        private float _value;
    }
#pragma warning restore CS0660, CS0661
}