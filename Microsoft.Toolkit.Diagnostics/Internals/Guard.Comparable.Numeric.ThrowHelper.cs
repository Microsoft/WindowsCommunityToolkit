// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Toolkit.Diagnostics
{
    /// <summary>
    /// Helper methods to verify conditions when running code.
    /// </summary>
    public static partial class Guard
    {
        /// <summary>
        /// Helper methods to efficiently throw exceptions.
        /// </summary>
        private static partial class ThrowHelper
        {
            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsCloseTo(int,int,uint,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsCloseTo(int value, int target, uint delta, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(int).ToTypeString()}) must be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs((double)((long)value - target)))}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotCloseTo(int,int,uint,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotCloseTo(int value, int target, uint delta, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(int).ToTypeString()}) must not be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs((double)((long)value - target)))}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsCloseTo(long,long,ulong,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsCloseTo(long value, long target, ulong delta, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(long).ToTypeString()}) must be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs((decimal)value - target))}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotCloseTo(long,long,ulong,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotCloseTo(long value, long target, ulong delta, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(long).ToTypeString()}) must not be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs((decimal)value - target))}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsCloseTo(float,float,float,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsCloseTo(float value, float target, float delta, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(float).ToTypeString()}) must be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs(value - target))}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotCloseTo(float,float,float,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotCloseTo(float value, float target, float delta, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(float).ToTypeString()}) must not be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs(value - target))}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsCloseTo(double,double,double,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsCloseTo(double value, double target, double delta, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(double).ToTypeString()}) must be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs(value - target))}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotCloseTo(double,double,double,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotCloseTo(double value, double target, double delta, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(double).ToTypeString()}) must not be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs(value - target))}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsCloseTo(nint,nint,nuint,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsCloseTo(nint value, nint target, nuint delta, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(nint).ToTypeString()}) must be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs(value - target))}", name);
            }

            /// <summary>
            /// Throws an <see cref="ArgumentException"/> when <see cref="Guard.IsNotCloseTo(nint,nint,nuint,string)"/> fails.
            /// </summary>
            [DoesNotReturn]
            public static void ThrowArgumentExceptionForIsNotCloseTo(nint value, nint target, nuint delta, string name)
            {
                throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(nint).ToTypeString()}) must not be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs(value - target))}", name);
            }
        }
    }
}