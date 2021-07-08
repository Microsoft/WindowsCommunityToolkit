// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace CommunityToolkit.WinUI.UI.Animations.Expressions
{
    /// <summary>
    /// Enum OperationType
    /// </summary>
    internal enum OperationType
    {
        /// <summary>
        /// The function
        /// </summary>
        Function,

        /// <summary>
        /// The operator (takes two operands)
        /// </summary>
        Operator,

        /// <summary>
        /// The operator that only takes one operand
        /// </summary>
        UnaryOperator,

        /// <summary>
        /// The constant
        /// </summary>
        Constant,

        /// <summary>
        /// The reference
        /// </summary>
        Reference,

        /// <summary>
        /// The conditional
        /// </summary>
        Conditional,

        /// <summary>
        /// The swizzle
        /// </summary>
        Swizzle,
    }
}