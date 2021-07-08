// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace CommunityToolkit.WinUI.UI.Animations.Expressions
{
    // Ignore warning: 'Vector2Node' defines operator == or operator != but does not override Object.Equals(object o) && Object.GetHashCode()
#pragma warning disable CS0660, CS0661
    /// <summary>
    /// Class Vector2Node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="CommunityToolkit.WinUI.UI.Animations.Expressions.ExpressionNode" />
    public sealed class Vector2Node : ExpressionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2Node"/> class.
        /// </summary>
        internal Vector2Node()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2Node"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        internal Vector2Node(Vector2 value)
        {
            _value = value;
            NodeType = ExpressionNodeType.ConstantValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2Node"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        internal Vector2Node(string paramName)
        {
            ParamName = paramName;
            NodeType = ExpressionNodeType.ConstantParameter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2Node"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        internal Vector2Node(string paramName, Vector2 value)
        {
            ParamName = paramName;
            _value = value;
            NodeType = ExpressionNodeType.ConstantParameter;

            SetVector2Parameter(paramName, value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Vector2"/> to <see cref="Vector2Node"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Vector2Node(Vector2 value)
        {
            return new Vector2Node(value);
        }

        /// <summary>
        /// Implements the + operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2Node operator +(Vector2Node left, Vector2Node right)
        {
            return ExpressionFunctions.Function<Vector2Node>(ExpressionNodeType.Add, left, right);
        }

        /// <summary>
        /// Implements the - operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2Node operator -(Vector2Node left, Vector2Node right)
        {
            return ExpressionFunctions.Function<Vector2Node>(ExpressionNodeType.Subtract, left, right);
        }

        /// <summary>
        /// Implements the - operator.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2Node operator -(Vector2Node value)
        {
            return ExpressionFunctions.Function<Vector2Node>(ExpressionNodeType.Negate, value);
        }

        /// <summary>
        /// Implements the * operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2Node operator *(Vector2Node left, ScalarNode right)
        {
            return ExpressionFunctions.Function<Vector2Node>(ExpressionNodeType.Multiply, left, right);
        }

        /// <summary>
        /// Implements the * operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2Node operator *(Vector2Node left, Vector2Node right)
        {
            return ExpressionFunctions.Function<Vector2Node>(ExpressionNodeType.Multiply, left, right);
        }

        /// <summary>
        /// Implements the / operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2Node operator /(Vector2Node left, Vector2Node right)
        {
            return ExpressionFunctions.Function<Vector2Node>(ExpressionNodeType.Divide, left, right);
        }

        /// <summary>
        /// Implements the % operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static Vector2Node operator %(Vector2Node left, Vector2Node right)
        {
            return ExpressionFunctions.Function<Vector2Node>(ExpressionNodeType.Modulus, left, right);
        }

        /// <summary>
        /// Implements the == operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static BooleanNode operator ==(Vector2Node left, Vector2Node right)
        {
            return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.Equals, left, right);
        }

        /// <summary>
        /// Implements the != operator.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>The result of the operator.</returns>
        public static BooleanNode operator !=(Vector2Node left, Vector2Node right)
        {
            return ExpressionFunctions.Function<BooleanNode>(ExpressionNodeType.NotEquals, left, right);
        }

        /// <summary>
        /// Enum Subchannel
        /// </summary>
        public enum Subchannel
        {
            /// <summary>
            /// The x channel
            /// </summary>
            X,

            /// <summary>
            /// The y channel
            /// </summary>
            Y
        }

        /// <summary>
        /// Gets the x subchannel.
        /// </summary>
        /// <value>The x subchannel.</value>
        public ScalarNode X
        {
            get { return GetSubchannels(Subchannel.X); }
        }

        /// <summary>
        /// Gets the y subchannel.
        /// </summary>
        /// <value>The y subchannel.</value>
        public ScalarNode Y
        {
            get { return GetSubchannels(Subchannel.Y); }
        }

        /// <summary>
        /// Create a new type by re-arranging the Vector subchannels.
        /// </summary>
        /// <param name="s">The subchannel</param>
        /// <returns>ScalarNode</returns>
        public ScalarNode GetSubchannels(Subchannel s)
        {
            return SubchannelsInternal<ScalarNode>(s.ToString());
        }

        /// <summary>
        /// Create a new type by re-arranging the Vector subchannels.
        /// </summary>
        /// <param name="s1">The first subchannel.</param>
        /// <param name="s2">The second subchannel.</param>
        /// <returns>Vector2Node.</returns>
        public Vector2Node GetSubchannels(Subchannel s1, Subchannel s2)
        {
            return SubchannelsInternal<Vector2Node>(s1.ToString(), s2.ToString());
        }

        /// <summary>
        /// Create a new type by re-arranging the Vector subchannels.
        /// </summary>
        /// <param name="s1">The first subchannel.</param>
        /// <param name="s2">The second subchannel.</param>
        /// <param name="s3">The third subchannel.</param>
        /// <returns>Vector3Node</returns>
        public Vector3Node GetSubchannels(Subchannel s1, Subchannel s2, Subchannel s3)
        {
            return SubchannelsInternal<Vector3Node>(s1.ToString(), s2.ToString(), s3.ToString());
        }

        /// <summary>
        /// Create a new type by re-arranging the Vector subchannels.
        /// </summary>
        /// <param name="s1">The first subchannel.</param>
        /// <param name="s2">The second subchannel.</param>
        /// <param name="s3">The third subchannel.</param>
        /// <param name="s4">The fourth subchannel.</param>
        /// <returns>Vector4Node</returns>
        public Vector4Node GetSubchannels(Subchannel s1, Subchannel s2, Subchannel s3, Subchannel s4)
        {
            return SubchannelsInternal<Vector4Node>(s1.ToString(), s2.ToString(), s3.ToString(), s4.ToString());
        }

        /// <summary>
        /// Create a new type by re-arranging the Vector subchannels.
        /// </summary>
        /// <param name="s1">The first subchannel.</param>
        /// <param name="s2">The second subchannel.</param>
        /// <param name="s3">The third subchannel.</param>
        /// <param name="s4">The fourth subchannel.</param>
        /// <param name="s5">The fifth subchannel.</param>
        /// <param name="s6">The sixth subchannel.</param>
        /// <returns>Matrix3x2Node</returns>
        public Matrix3x2Node GetSubchannels(Subchannel s1, Subchannel s2, Subchannel s3, Subchannel s4, Subchannel s5, Subchannel s6)
        {
            return SubchannelsInternal<Matrix3x2Node>(s1.ToString(), s2.ToString(), s3.ToString(), s4.ToString(), s5.ToString(), s6.ToString());
        }

        /// <summary>
        /// Create a new type by re-arranging the Vector subchannels.
        /// </summary>
        /// <param name="s1">The first subchannel.</param>
        /// <param name="s2">The second subchannel.</param>
        /// <param name="s3">The third subchannel.</param>
        /// <param name="s4">The fourth subchannel.</param>
        /// <param name="s5">The fifth subchannel.</param>
        /// <param name="s6">The sixth subchannel.</param>
        /// <param name="s7">The seventh subchannel.</param>
        /// <param name="s8">The eighth subchannel.</param>
        /// <param name="s9">The ninth subchannel.</param>
        /// <param name="s10">The tenth subchannel.</param>
        /// <param name="s11">The eleventh subchannel.</param>
        /// <param name="s12">The twelfth subchannel.</param>
        /// <param name="s13">The thirteenth subchannel.</param>
        /// <param name="s14">The fourteenth subchannel.</param>
        /// <param name="s15">The fifteenth subchannel.</param>
        /// <param name="s16">The sixteenth subchannel.</param>
        /// <returns>Matrix4x4Node</returns>
#pragma warning disable SA1117 // Parameters must be on same line or separate lines
        public Matrix4x4Node GetSubchannels(Subchannel s1, Subchannel s2, Subchannel s3, Subchannel s4,
                                         Subchannel s5, Subchannel s6, Subchannel s7, Subchannel s8,
                                         Subchannel s9, Subchannel s10, Subchannel s11, Subchannel s12,
                                         Subchannel s13, Subchannel s14, Subchannel s15, Subchannel s16)
        {
            return SubchannelsInternal<Matrix4x4Node>(s1.ToString(), s2.ToString(), s3.ToString(), s4.ToString(),
                                                      s5.ToString(), s6.ToString(), s7.ToString(), s8.ToString(),
                                                      s9.ToString(), s10.ToString(), s11.ToString(), s12.ToString(),
                                                      s13.ToString(), s14.ToString(), s15.ToString(), s16.ToString());
        }
#pragma warning restore SA1117 // Parameters must be on same line or separate lines

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>System.String.</returns>
        protected internal override string GetValue()
        {
            return $"Vector2({_value.X},{_value.Y})";
        }

        private Vector2 _value;
    }
#pragma warning restore CS0660, CS0661
}