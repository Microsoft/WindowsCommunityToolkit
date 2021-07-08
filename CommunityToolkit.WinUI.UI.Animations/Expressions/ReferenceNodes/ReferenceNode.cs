// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.UI.Composition;

namespace CommunityToolkit.WinUI.UI.Animations.Expressions
{
    /// <summary>
    /// Class ReferenceNode.
    /// </summary>
    /// <seealso cref="CommunityToolkit.WinUI.UI.Animations.Expressions.ExpressionNode" />
    public abstract class ReferenceNode : ExpressionNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceNode"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="compObj">The comp object.</param>
        internal ReferenceNode(string paramName, CompositionObject compObj = null)
        {
            Reference = compObj;
            NodeType = ExpressionNodeType.Reference;
            ParamName = paramName;
        }

        /// <summary>
        /// Gets the reference.
        /// </summary>
        /// <value>The reference.</value>
        public CompositionObject Reference { get; private set; }

        /// <summary>
        /// Create a reference to the specified boolean property. This maybe be a property on the CompositionObject directly, or on the its PropertySet.
        /// </summary>
        /// <param name="propertyName">The name of the property to reference.</param>
        /// <returns>BooleanNode.</returns>
        public BooleanNode GetBooleanProperty(string propertyName)
        {
            return ReferenceProperty<BooleanNode>(propertyName);
        }

        /// <summary>
        /// Create a reference to the specified float property. This maybe be a property on the CompositionObject directly, or on the its PropertySet.
        /// </summary>
        /// <param name="propertyName">The name of the property to reference.</param>
        /// <returns>ScalarNode</returns>
        public ScalarNode GetScalarProperty(string propertyName)
        {
            return ReferenceProperty<ScalarNode>(propertyName);
        }

        /// <summary>
        /// Create a reference to the specified Vector2 property. This maybe be a property on the CompositionObject directly, or on the its PropertySet.
        /// </summary>
        /// <param name="propertyName">The name of the property to reference.</param>
        /// <returns>Vector2Node</returns>
        public Vector2Node GetVector2Property(string propertyName)
        {
            return ReferenceProperty<Vector2Node>(propertyName);
        }

        /// <summary>
        /// Create a reference to the specified Vector3 property. This maybe be a property on the CompositionObject directly, or on the its PropertySet.
        /// </summary>
        /// <param name="propertyName">The name of the property to reference.</param>
        /// <returns>Vector3Node</returns>
        public Vector3Node GetVector3Property(string propertyName)
        {
            return ReferenceProperty<Vector3Node>(propertyName);
        }

        /// <summary>
        /// Create a reference to the specified Vector4 property. This maybe be a property on the CompositionObject directly, or on the its PropertySet.
        /// </summary>
        /// <param name="propertyName">The name of the property to reference.</param>
        /// <returns>Vector4Node</returns>
        public Vector4Node GetVector4Property(string propertyName)
        {
            return ReferenceProperty<Vector4Node>(propertyName);
        }

        /// <summary>
        /// Create a reference to the specified Color property. This maybe be a property on the CompositionObject directly, or on the its PropertySet.
        /// </summary>
        /// <param name="propertyName">The name of the property to reference.</param>
        /// <returns>ColorNode</returns>
        public ColorNode GetColorProperty(string propertyName)
        {
            return ReferenceProperty<ColorNode>(propertyName);
        }

        /// <summary>
        /// Create a reference to the specified Quaternion property. This maybe be a property on the CompositionObject directly, or on the its PropertySet.
        /// </summary>
        /// <param name="propertyName">The name of the property to reference.</param>
        /// <returns>QuaternionNode</returns>
        public QuaternionNode GetQuaternionProperty(string propertyName)
        {
            return ReferenceProperty<QuaternionNode>(propertyName);
        }

        /// <summary>
        /// Create a reference to the specified Matrix3x2 property. This maybe be a property on the CompositionObject directly, or on the its PropertySet.
        /// </summary>
        /// <param name="propertyName">The name of the property to reference.</param>
        /// <returns>Matrix3x2Node</returns>
        public Matrix3x2Node GetMatrix3x2Property(string propertyName)
        {
            return ReferenceProperty<Matrix3x2Node>(propertyName);
        }

        /// <summary>
        /// Create a reference to the specified Matrix4x4 property. This maybe be a property on the CompositionObject directly, or on the its PropertySet.
        /// </summary>
        /// <param name="propertyName">The name of the property to reference.</param>
        /// <returns>Matrix4x4Node</returns>
        public Matrix4x4Node GetMatrix4x4Property(string propertyName)
        {
            return ReferenceProperty<Matrix4x4Node>(propertyName);
        }

        /// <summary>
        /// Gets the reference parameter string.
        /// </summary>
        /// <returns>System.String.</returns>
        internal string GetReferenceParamString()
        {
            if (NodeType == ExpressionNodeType.TargetReference)
            {
                return "this.target";
            }
            else
            {
                return ParamName;
            }
        }

        /// <summary>
        /// References the property.
        /// </summary>
        /// <typeparam name="T">A class that derives from ExpressionNode.</typeparam>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>T.</returns>
        protected internal T ReferenceProperty<T>(string propertyName)
            where T : ExpressionNode
        {
            T newNode = ExpressionNode.CreateExpressionNode<T>();

            (newNode as ExpressionNode).NodeType = ExpressionNodeType.ReferenceProperty;
            (newNode as ExpressionNode).Children.Add(this);
            (newNode as ExpressionNode).PropertyName = propertyName;

            return newNode;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <returns>System.String.</returns>
        /// <exception cref="global::System.NotImplementedException">GetValue is not implemented for ReferenceNode and shouldn't be called</exception>
        protected internal override string GetValue()
        {
            throw new NotImplementedException("GetValue is not implemented for ReferenceNode and shouldn't be called");
        }
    }
}