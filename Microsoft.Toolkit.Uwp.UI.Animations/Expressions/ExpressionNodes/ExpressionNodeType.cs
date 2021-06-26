// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Toolkit.Uwp.UI.Animations.Expressions
{
    /// <summary>
    /// Enum ExpressionNodeType
    /// </summary>
    internal enum ExpressionNodeType
    {
        /// <summary>
        /// The constant value
        /// </summary>
        ConstantValue,

        /// <summary>
        /// The constant parameter
        /// </summary>
        ConstantParameter,

        /// <summary>
        /// The current value property
        /// </summary>
        CurrentValueProperty,

        /// <summary>
        /// The reference
        /// </summary>
        Reference,

        /// <summary>
        /// The reference property
        /// </summary>
        ReferenceProperty,

        /// <summary>
        /// The starting value property
        /// </summary>
        StartingValueProperty,

        /// <summary>
        /// The target reference
        /// </summary>
        TargetReference,

        /// <summary>
        /// The conditional
        /// </summary>
        Conditional,

        /// <summary>
        /// The swizzle
        /// </summary>
        Swizzle,

        /// <summary>
        /// The add
        /// </summary>
        Add,

        /// <summary>
        /// The and
        /// </summary>
        And,

        /// <summary>
        /// The divide
        /// </summary>
        Divide,

        /// <summary>
        /// The equals
        /// </summary>
        Equals,

        /// <summary>
        /// The greater than
        /// </summary>
        GreaterThan,

        /// <summary>
        /// The greater than equals
        /// </summary>
        GreaterThanEquals,

        /// <summary>
        /// The less than
        /// </summary>
        LessThan,

        /// <summary>
        /// The less than equals
        /// </summary>
        LessThanEquals,

        /// <summary>
        /// The multiply
        /// </summary>
        Multiply,

        /// <summary>
        /// The not
        /// </summary>
        Not,

        /// <summary>
        /// The not equals
        /// </summary>
        NotEquals,

        /// <summary>
        /// The or
        /// </summary>
        Or,

        /// <summary>
        /// The subtract
        /// </summary>
        Subtract,

        /// <summary>
        /// The absolute
        /// </summary>
        Absolute,

        /// <summary>
        /// The acos
        /// </summary>
        Acos,

        /// <summary>
        /// The asin
        /// </summary>
        Asin,

        /// <summary>
        /// The atan
        /// </summary>
        Atan,

        /// <summary>
        /// The cos
        /// </summary>
        Cos,

        /// <summary>
        /// The ceil
        /// </summary>
        Ceil,

        /// <summary>
        /// The clamp
        /// </summary>
        Clamp,

        /// <summary>
        /// The color HSL
        /// </summary>
        ColorHsl,

        /// <summary>
        /// The color RGB
        /// </summary>
        ColorRgb,

        /// <summary>
        /// The color lerp
        /// </summary>
        ColorLerp,

        /// <summary>
        /// The color lerp HSL
        /// </summary>
        ColorLerpHsl,

        /// <summary>
        /// The color lerp RGB
        /// </summary>
        ColorLerpRgb,

        /// <summary>
        /// The concatenate
        /// </summary>
        Concatenate,

        /// <summary>
        /// The distance
        /// </summary>
        Distance,

        /// <summary>
        /// The distance squared
        /// </summary>
        DistanceSquared,

        /// <summary>
        /// The floor
        /// </summary>
        Floor,

        /// <summary>
        /// The inverse
        /// </summary>
        Inverse,

        /// <summary>
        /// The length
        /// </summary>
        Length,

        /// <summary>
        /// The length squared
        /// </summary>
        LengthSquared,

        /// <summary>
        /// The lerp
        /// </summary>
        Lerp,

        /// <summary>
        /// The ln
        /// </summary>
        Ln,

        /// <summary>
        /// The log10
        /// </summary>
        Log10,

        /// <summary>
        /// The maximum
        /// </summary>
        Max,

        /// <summary>
        /// The matrix3x2 from rotation
        /// </summary>
        Matrix3x2FromRotation,

        /// <summary>
        /// The matrix3x2 from scale
        /// </summary>
        Matrix3x2FromScale,

        /// <summary>
        /// The matrix3x2 from skew
        /// </summary>
        Matrix3x2FromSkew,

        /// <summary>
        /// The matrix3x2 from translation
        /// </summary>
        Matrix3x2FromTranslation,

        /// <summary>
        /// The matrix3x2
        /// </summary>
        Matrix3x2,

        /// <summary>
        /// The matrix4x4 from axis angle
        /// </summary>
        Matrix4x4FromAxisAngle,

        /// <summary>
        /// The matrix4x4 from scale
        /// </summary>
        Matrix4x4FromScale,

        /// <summary>
        /// The matrix4x4 from translation
        /// </summary>
        Matrix4x4FromTranslation,

        /// <summary>
        /// The matrix4x4
        /// </summary>
        Matrix4x4,

        /// <summary>
        /// The minimum
        /// </summary>
        Min,

        /// <summary>
        /// The modulus
        /// </summary>
        Modulus,

        /// <summary>
        /// The negate
        /// </summary>
        Negate,

        /// <summary>
        /// The normalize
        /// </summary>
        Normalize,

        /// <summary>
        /// The pow
        /// </summary>
        Pow,

        /// <summary>
        /// The quaternion from axis angle
        /// </summary>
        QuaternionFromAxisAngle,

        /// <summary>
        /// The quaternion
        /// </summary>
        Quaternion,

        /// <summary>
        /// The round
        /// </summary>
        Round,

        /// <summary>
        /// The scale
        /// </summary>
        Scale,

        /// <summary>
        /// The sin
        /// </summary>
        Sin,

        /// <summary>
        /// The slerp
        /// </summary>
        Slerp,

        /// <summary>
        /// The SQRT
        /// </summary>
        Sqrt,

        /// <summary>
        /// The square
        /// </summary>
        Square,

        /// <summary>
        /// The tan
        /// </summary>
        Tan,

        /// <summary>
        /// To degrees
        /// </summary>
        ToDegrees,

        /// <summary>
        /// To radians
        /// </summary>
        ToRadians,

        /// <summary>
        /// The transform
        /// </summary>
        Transform,

        /// <summary>
        /// The vector2
        /// </summary>
        Vector2,

        /// <summary>
        /// The vector3
        /// </summary>
        Vector3,

        /// <summary>
        /// The vector4
        /// </summary>
        Vector4,

        /// <summary>
        /// The count
        /// </summary>
        Count
    }
}