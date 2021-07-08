// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.UI;

namespace CommunityToolkit.WinUI.UI.Animations.Expressions
{
    // ExpressionValues is a static class instead of a namespace to improve intellisense discoverablity and consistency with the other helper classes.

    /// <summary>
    /// Class ExpressionValues.
    /// </summary>
    public static partial class ExpressionValues
    {
        /// <summary>
        /// Create a constant parameter whose value can be changed without recreating the expression.
        /// </summary>
        public static class Constant
        {
            // Constant parameters with no default value

            /// <summary>
            /// Creates a named constant parameter of type bool.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>BooleanNode</returns>
            public static BooleanNode CreateConstantBoolean(string paramName)
            {
                return new BooleanNode(paramName);
            }

            /// <summary>
            /// Creates a named constant parameter of type float.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>ScalarNode.</returns>
            public static ScalarNode CreateConstantScalar(string paramName)
            {
                return new ScalarNode(paramName);
            }

            /// <summary>
            /// Creates a named constant parameter of type Vector2.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>Vector2Node.</returns>
            public static Vector2Node CreateConstantVector2(string paramName)
            {
                return new Vector2Node(paramName);
            }

            /// <summary>
            /// Creates a named constant parameter of type Vector3.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>Vector3Node.</returns>
            public static Vector3Node CreateConstantVector3(string paramName)
            {
                return new Vector3Node(paramName);
            }

            /// <summary>
            /// Creates a named constant parameter of type Vector4.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>Vector4Node.</returns>
            public static Vector4Node CreateConstantVector4(string paramName)
            {
                return new Vector4Node(paramName);
            }

            /// <summary>
            /// Creates a named constant parameter of type Color.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>ColorNode.</returns>
            public static ColorNode CreateConstantColor(string paramName)
            {
                return new ColorNode(paramName);
            }

            /// <summary>
            /// Creates a named constant parameter of type Quaternion.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>QuaternionNode.</returns>
            public static QuaternionNode CreateConstantQuaternion(string paramName)
            {
                return new QuaternionNode(paramName);
            }

            /// <summary>
            /// Creates a named constant parameter of type Matrix3x2.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>Matrix3x2Node.</returns>
            public static Matrix3x2Node CreateConstantMatrix3x2(string paramName)
            {
                return new Matrix3x2Node(paramName);
            }

            /// <summary>
            /// Creates a named constant parameter of type Matrix4x4.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <returns>Matrix4x4Node.</returns>
            public static Matrix4x4Node CreateConstantMatrix4x4(string paramName)
            {
                return new Matrix4x4Node(paramName);
            }

            // Constant parameters with a default value

            /// <summary>
            /// Creates a named constant parameter of type bool, initialized with the specified value.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <param name="value">The value of the parameter.</param>
            /// <returns>BooleanNode.</returns>
            public static BooleanNode CreateConstantBoolean(string paramName, bool value)
            {
                return new BooleanNode(paramName, value);
            }

            /// <summary>
            /// Creates a named constant parameter of type float, initialized with the specified value.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <param name="value">The value of the parameter.</param>
            /// <returns>ScalarNode.</returns>
            public static ScalarNode CreateConstantScalar(string paramName, float value)
            {
                return new ScalarNode(paramName, value);
            }

            /// <summary>
            /// Creates a named constant parameter of type Vector2, initialized with the specified value.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <param name="value">The value of the parameter.</param>
            /// <returns>Vector2Node.</returns>
            public static Vector2Node CreateConstantVector2(string paramName, Vector2 value)
            {
                return new Vector2Node(paramName, value);
            }

            /// <summary>
            /// Creates a named constant parameter of type Vector3, initialized with the specified value.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <param name="value">The value of the parameter.</param>
            /// <returns>Vector3Node.</returns>
            public static Vector3Node CreateConstantVector3(string paramName, Vector3 value)
            {
                return new Vector3Node(paramName, value);
            }

            /// <summary>
            /// Creates a named constant parameter of type Vector4, initialized with the specified value.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <param name="value">The value of the parameter.</param>
            /// <returns>Vector4Node.</returns>
            public static Vector4Node CreateConstantVector4(string paramName, Vector4 value)
            {
                return new Vector4Node(paramName, value);
            }

            /// <summary>
            /// Creates a named constant parameter of type Color, initialized with the specified value.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <param name="value">The value of the parameter.</param>
            /// <returns>ColorNode.</returns>
            public static ColorNode CreateConstantColor(string paramName, Windows.UI.Color value)
            {
                return new ColorNode(paramName, value);
            }

            /// <summary>
            /// Creates a named constant parameter of type Quaternion, initialized with the specified value.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <param name="value">The value of the parameter.</param>
            /// <returns>QuaternionNode.</returns>
            public static QuaternionNode CreateConstantQuaternion(string paramName, Quaternion value)
            {
                return new QuaternionNode(paramName, value);
            }

            /// <summary>
            /// Creates a named constant parameter of type Matrix3x2, initialized with the specified value.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <param name="value">The value of the parameter.</param>
            /// <returns>Matrix3x2Node.</returns>
            public static Matrix3x2Node CreateConstantMatrix3x2(string paramName, Matrix3x2 value)
            {
                return new Matrix3x2Node(paramName, value);
            }

            /// <summary>
            /// Creates a named constant parameter of type Matrix4x4, initialized with the specified value.
            /// </summary>
            /// <param name="paramName">The name that will be used to refer to the parameter at a later time.</param>
            /// <param name="value">The value of the parameter.</param>
            /// <returns>Matrix4x4Node.</returns>
            public static Matrix4x4Node CreateConstantMatrix4x4(string paramName, Matrix4x4 value)
            {
                return new Matrix4x4Node(paramName, value);
            }
        }
    }
}