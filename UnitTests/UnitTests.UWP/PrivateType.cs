// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//// https://github.com/microsoft/testfx/tree/664ac7c2ac9dbfbee9d2a0ef560cfd72449dfe34/src/TestFramework/Extension.Desktop

//// Original Copyright (c) Microsoft Corporation. All rights reserved.
//// Licensed under the MIT license. See ThirdPartyNotice.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    /// <summary>
    /// This class represents a private class for the Private Accessor functionality.
    /// </summary>
    public class PrivateType
    {
        /// <summary>
        /// Binds to everything
        /// </summary>
        private const BindingFlags BindToEveryThing = BindingFlags.Default
            | BindingFlags.NonPublic | BindingFlags.Instance
            | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy;

        /// <summary>
        /// The wrapped type.
        /// </summary>
        private Type type;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateType"/> class that contains the private type.
        /// </summary>
        /// <param name="assemblyName">Assembly name</param>
        /// <param name="typeName">fully qualified name of the </param>
        public PrivateType(string assemblyName, string typeName)
        {
            Assembly asm = Assembly.Load(assemblyName);

            this.type = asm.GetType(typeName, true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrivateType"/> class that contains
        /// the private type from the type object
        /// </summary>
        /// <param name="type">The wrapped Type to create.</param>
        public PrivateType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            this.type = type;
        }

        /// <summary>
        /// Gets the referenced type
        /// </summary>
        public Type ReferencedType => this.type;

        /// <summary>
        /// Invokes static member
        /// </summary>
        /// <param name="name">Name of the member to InvokeHelper</param>
        /// <param name="args">Arguments to the invocation</param>
        /// <returns>Result of invocation</returns>
        public object InvokeStatic(string name, params object[] args)
        {
            return this.InvokeStatic(name, null, args, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Invokes static member
        /// </summary>
        /// <param name="name">Name of the member to InvokeHelper</param>
        /// <param name="parameterTypes">An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the method to invoke</param>
        /// <param name="args">Arguments to the invocation</param>
        /// <returns>Result of invocation</returns>
        public object InvokeStatic(string name, Type[] parameterTypes, object[] args)
        {
            return this.InvokeStatic(name, parameterTypes, args, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Invokes static member
        /// </summary>
        /// <param name="name">Name of the member to InvokeHelper</param>
        /// <param name="parameterTypes">An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the method to invoke</param>
        /// <param name="args">Arguments to the invocation</param>
        /// <param name="typeArguments">An array of types corresponding to the types of the generic arguments.</param>
        /// <returns>Result of invocation</returns>
        public object InvokeStatic(string name, Type[] parameterTypes, object[] args, Type[] typeArguments)
        {
            return this.InvokeStatic(name, BindToEveryThing, parameterTypes, args, CultureInfo.InvariantCulture, typeArguments);
        }

        /// <summary>
        /// Invokes the static method
        /// </summary>
        /// <param name="name">Name of the member</param>
        /// <param name="args">Arguments to the invocation</param>
        /// <param name="culture">Culture</param>
        /// <returns>Result of invocation</returns>
        public object InvokeStatic(string name, object[] args, CultureInfo culture)
        {
            return this.InvokeStatic(name, null, args, culture);
        }

        /// <summary>
        /// Invokes the static method
        /// </summary>
        /// <param name="name">Name of the member</param>
        /// <param name="parameterTypes">An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the method to invoke</param>
        /// <param name="args">Arguments to the invocation</param>
        /// <param name="culture">Culture info</param>
        /// <returns>Result of invocation</returns>
        public object InvokeStatic(string name, Type[] parameterTypes, object[] args, CultureInfo culture)
        {
            return this.InvokeStatic(name, BindingFlags.InvokeMethod, parameterTypes, args, culture);
        }

        /// <summary>
        /// Invokes the static method
        /// </summary>
        /// <param name="name">Name of the member</param>
        /// <param name="bindingFlags">Additional invocation attributes</param>
        /// <param name="args">Arguments to the invocation</param>
        /// <returns>Result of invocation</returns>
        public object InvokeStatic(string name, BindingFlags bindingFlags, params object[] args)
        {
            return this.InvokeStatic(name, bindingFlags, null, args, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Invokes the static method
        /// </summary>
        /// <param name="name">Name of the member</param>
        /// <param name="bindingFlags">Additional invocation attributes</param>
        /// <param name="parameterTypes">An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the method to invoke</param>
        /// <param name="args">Arguments to the invocation</param>
        /// <returns>Result of invocation</returns>
        public object InvokeStatic(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args)
        {
            return this.InvokeStatic(name, bindingFlags, parameterTypes, args, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Invokes the static method
        /// </summary>
        /// <param name="name">Name of the member</param>
        /// <param name="bindingFlags">Additional invocation attributes</param>
        /// <param name="args">Arguments to the invocation</param>
        /// <param name="culture">Culture</param>
        /// <returns>Result of invocation</returns>
        public object InvokeStatic(string name, BindingFlags bindingFlags, object[] args, CultureInfo culture)
        {
            return this.InvokeStatic(name, bindingFlags, null, args, culture);
        }

        /// <summary>
        /// Invokes the static method
        /// </summary>
        /// <param name="name">Name of the member</param>
        /// <param name="bindingFlags">Additional invocation attributes</param>
        /// /// <param name="parameterTypes">An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the method to invoke</param>
        /// <param name="args">Arguments to the invocation</param>
        /// <param name="culture">Culture</param>
        /// <returns>Result of invocation</returns>
        public object InvokeStatic(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args, CultureInfo culture)
        {
            return this.InvokeStatic(name, bindingFlags, parameterTypes, args, culture, null);
        }

        /// <summary>
        /// Invokes the static method
        /// </summary>
        /// <param name="name">Name of the member</param>
        /// <param name="bindingFlags">Additional invocation attributes</param>
        /// /// <param name="parameterTypes">An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the method to invoke</param>
        /// <param name="args">Arguments to the invocation</param>
        /// <param name="culture">Culture</param>
        /// <param name="typeArguments">An array of types corresponding to the types of the generic arguments.</param>
        /// <returns>Result of invocation</returns>
        public object InvokeStatic(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args, CultureInfo culture, Type[] typeArguments)
        {
            if (parameterTypes != null)
            {
                MethodInfo member = this.type.GetMethod(name, bindingFlags | BindToEveryThing | BindingFlags.Static, null, parameterTypes, null);
                if (member == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The member specified ({0}) could not be found. You might need to regenerate your private accessor, or the member may be private and defined on a base class. If the latter is true, you need to pass the type that defines the member into PrivateObject's constructor.", name));
                }

                try
                {
                    if (member.IsGenericMethodDefinition)
                    {
                        MethodInfo constructed = member.MakeGenericMethod(typeArguments);
                        return constructed.Invoke(null, bindingFlags, null, args, culture);
                    }
                    else
                    {
                        return member.Invoke(null, bindingFlags, null, args, culture);
                    }
                }
                catch (TargetInvocationException e)
                {
                    Debug.Assert(e.InnerException != null, "Inner Exception should not be null.");
                    if (e.InnerException != null)
                    {
                        throw e.InnerException;
                    }

                    throw;
                }
            }
            else
            {
                return this.InvokeHelperStatic(name, bindingFlags | BindingFlags.InvokeMethod, args, culture);
            }
        }

        /// <summary>
        /// Gets the element in static array
        /// </summary>
        /// <param name="name">Name of the array</param>
        /// <param name="indices">
        /// A one-dimensional array of 32-bit integers that represent the indexes specifying
        /// the position of the element to get. For instance, to access a[10][11] the indices would be {10,11}
        /// </param>
        /// <returns>element at the specified location</returns>
        public object GetStaticArrayElement(string name, params int[] indices)
        {
            return this.GetStaticArrayElement(name, BindToEveryThing, indices);
        }

        /// <summary>
        /// Sets the member of the static array
        /// </summary>
        /// <param name="name">Name of the array</param>
        /// <param name="value">value to set</param>
        /// <param name="indices">
        /// A one-dimensional array of 32-bit integers that represent the indexes specifying
        /// the position of the element to set. For instance, to access a[10][11] the array would be {10,11}
        /// </param>
        public void SetStaticArrayElement(string name, object value, params int[] indices)
        {
            this.SetStaticArrayElement(name, BindToEveryThing, value, indices);
        }

        /// <summary>
        /// Gets the element in static array
        /// </summary>
        /// <param name="name">Name of the array</param>
        /// <param name="bindingFlags">Additional InvokeHelper attributes</param>
        /// <param name="indices">
        /// A one-dimensional array of 32-bit integers that represent the indexes specifying
        /// the position of the element to get. For instance, to access a[10][11] the array would be {10,11}
        /// </param>
        /// <returns>element at the specified location</returns>
        public object GetStaticArrayElement(string name, BindingFlags bindingFlags, params int[] indices)
        {
            Array arr = (Array)this.InvokeHelperStatic(name, BindingFlags.GetField | BindingFlags.GetProperty | bindingFlags, null, CultureInfo.InvariantCulture);
            return arr.GetValue(indices);
        }

        /// <summary>
        /// Sets the member of the static array
        /// </summary>
        /// <param name="name">Name of the array</param>
        /// <param name="bindingFlags">Additional InvokeHelper attributes</param>
        /// <param name="value">value to set</param>
        /// <param name="indices">
        /// A one-dimensional array of 32-bit integers that represent the indexes specifying
        /// the position of the element to set. For instance, to access a[10][11] the array would be {10,11}
        /// </param>
        public void SetStaticArrayElement(string name, BindingFlags bindingFlags, object value, params int[] indices)
        {
            Array arr = (Array)this.InvokeHelperStatic(name, BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Static | bindingFlags, null, CultureInfo.InvariantCulture);
            arr.SetValue(value, indices);
        }

        /// <summary>
        /// Gets the static field
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <returns>The static field.</returns>
        public object GetStaticField(string name)
        {
            return this.GetStaticField(name, BindToEveryThing);
        }

        /// <summary>
        /// Sets the static field
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <param name="value">Arguments to the invocation</param>
        public void SetStaticField(string name, object value)
        {
            this.SetStaticField(name, BindToEveryThing, value);
        }

        /// <summary>
        /// Gets the static field using specified InvokeHelper attributes
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <param name="bindingFlags">Additional invocation attributes</param>
        /// <returns>The static field.</returns>
        public object GetStaticField(string name, BindingFlags bindingFlags)
        {
            return this.InvokeHelperStatic(name, BindingFlags.GetField | BindingFlags.Static | bindingFlags, null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Sets the static field using binding attributes
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <param name="bindingFlags">Additional InvokeHelper attributes</param>
        /// <param name="value">Arguments to the invocation</param>
        public void SetStaticField(string name, BindingFlags bindingFlags, object value)
        {
            this.InvokeHelperStatic(name, BindingFlags.SetField | bindingFlags | BindingFlags.Static, new[] { value }, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the static field or property
        /// </summary>
        /// <param name="name">Name of the field or property</param>
        /// <returns>The static field or property.</returns>
        public object GetStaticFieldOrProperty(string name)
        {
            return this.GetStaticFieldOrProperty(name, BindToEveryThing);
        }

        /// <summary>
        /// Sets the static field or property
        /// </summary>
        /// <param name="name">Name of the field or property</param>
        /// <param name="value">Value to be set to field or property</param>
        public void SetStaticFieldOrProperty(string name, object value)
        {
            this.SetStaticFieldOrProperty(name, BindToEveryThing, value);
        }

        /// <summary>
        /// Gets the static field or property using specified InvokeHelper attributes
        /// </summary>
        /// <param name="name">Name of the field or property</param>
        /// <param name="bindingFlags">Additional invocation attributes</param>
        /// <returns>The static field or property.</returns>
        public object GetStaticFieldOrProperty(string name, BindingFlags bindingFlags)
        {
            return this.InvokeHelperStatic(name, BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Static | bindingFlags, null, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Sets the static field or property using binding attributes
        /// </summary>
        /// <param name="name">Name of the field or property</param>
        /// <param name="bindingFlags">Additional invocation attributes</param>
        /// <param name="value">Value to be set to field or property</param>
        public void SetStaticFieldOrProperty(string name, BindingFlags bindingFlags, object value)
        {
            this.InvokeHelperStatic(name, BindingFlags.SetField | BindingFlags.SetProperty | bindingFlags | BindingFlags.Static, new[] { value }, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the static property
        /// </summary>
        /// <param name="name">Name of the field or property</param>
        /// <param name="args">Arguments to the invocation</param>
        /// <returns>The static property.</returns>
        public object GetStaticProperty(string name, params object[] args)
        {
            return this.GetStaticProperty(name, BindToEveryThing, args);
        }

        /// <summary>
        /// Sets the static property
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Value to be set to field or property</param>
        /// <param name="args">Arguments to pass to the member to invoke.</param>
        public void SetStaticProperty(string name, object value, params object[] args)
        {
            this.SetStaticProperty(name, BindToEveryThing, value, null, args);
        }

        /// <summary>
        /// Sets the static property
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="value">Value to be set to field or property</param>
        /// <param name="parameterTypes">An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the indexed property.</param>
        /// <param name="args">Arguments to pass to the member to invoke.</param>
        public void SetStaticProperty(string name, object value, Type[] parameterTypes, object[] args)
        {
            this.SetStaticProperty(name, BindingFlags.SetProperty, value, parameterTypes, args);
        }

        /// <summary>
        /// Gets the static property
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="bindingFlags">Additional invocation attributes.</param>
        /// <param name="args">Arguments to pass to the member to invoke.</param>
        /// <returns>The static property.</returns>
        public object GetStaticProperty(string name, BindingFlags bindingFlags, params object[] args)
        {
            return this.GetStaticProperty(name, BindingFlags.GetProperty | BindingFlags.Static | bindingFlags, null, args);
        }

        /// <summary>
        /// Gets the static property
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="bindingFlags">Additional invocation attributes.</param>
        /// <param name="parameterTypes">An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the indexed property.</param>
        /// <param name="args">Arguments to pass to the member to invoke.</param>
        /// <returns>The static property.</returns>
        public object GetStaticProperty(string name, BindingFlags bindingFlags, Type[] parameterTypes, object[] args)
        {
            if (parameterTypes != null)
            {
                PropertyInfo pi = this.type.GetProperty(name, bindingFlags | BindingFlags.Static, null, null, parameterTypes, null);
                if (pi == null)
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The member specified ({0}) could not be found. You might need to regenerate your private accessor, or the member may be private and defined on a base class. If the latter is true, you need to pass the type that defines the member into PrivateObject's constructor.", name));
                }

                return pi.GetValue(null, args);
            }
            else
            {
                return this.InvokeHelperStatic(name, bindingFlags | BindingFlags.GetProperty, args, null);
            }
        }

        /// <summary>
        /// Sets the static property
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="bindingFlags">Additional invocation attributes.</param>
        /// <param name="value">Value to be set to field or property</param>
        /// <param name="args">Optional index values for indexed properties. The indexes of indexed properties are zero-based. This value should be null for non-indexed properties. </param>
        public void SetStaticProperty(string name, BindingFlags bindingFlags, object value, params object[] args)
        {
            this.SetStaticProperty(name, bindingFlags, value, null, args);
        }

        /// <summary>
        /// Sets the static property
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="bindingFlags">Additional invocation attributes.</param>
        /// <param name="value">Value to be set to field or property</param>
        /// <param name="parameterTypes">An array of <see cref="T:System.Type"/> objects representing the number, order, and type of the parameters for the indexed property.</param>
        /// <param name="args">Arguments to pass to the member to invoke.</param>
        public void SetStaticProperty(string name, BindingFlags bindingFlags, object value, Type[] parameterTypes, object[] args)
        {
            if (parameterTypes != null)
            {
                PropertyInfo pi = this.type.GetProperty(name, bindingFlags | BindingFlags.Static, null, null, parameterTypes, null);
                if (pi == null)
                {
                    throw new ArgumentException(
                        string.Format(CultureInfo.CurrentCulture, "The member specified ({0}) could not be found. You might need to regenerate your private accessor, or the member may be private and defined on a base class. If the latter is true, you need to pass the type that defines the member into PrivateObject's constructor.", name));
                }

                pi.SetValue(null, value, args);
            }
            else
            {
                object[] pass = new object[(args?.Length ?? 0) + 1];
                pass[0] = value;
                args?.CopyTo(pass, 1);
                this.InvokeHelperStatic(name, bindingFlags | BindingFlags.SetProperty, pass, null);
            }
        }

        /// <summary>
        /// Invokes the static method
        /// </summary>
        /// <param name="name">Name of the member</param>
        /// <param name="bindingFlags">Additional invocation attributes</param>
        /// <param name="args">Arguments to the invocation</param>
        /// <param name="culture">Culture</param>
        /// <returns>Result of invocation</returns>
        private object InvokeHelperStatic(string name, BindingFlags bindingFlags, object[] args, CultureInfo culture)
        {
            try
            {
                return this.type.InvokeMember(name, bindingFlags | BindToEveryThing | BindingFlags.Static, null, null, args, culture);
            }
            catch (TargetInvocationException e)
            {
                Debug.Assert(e.InnerException != null, "Inner Exception should not be null.");
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }

                throw;
            }
        }
    }
}