// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace Microsoft.Toolkit.HighPerformance
{
    /// <summary>
    /// A <see langword="class"/> that represents a boxed <typeparamref name="T"/> value on the managed heap.
    /// This is a "shadow" type that can be used in place of a non-generic <see cref="object"/> reference to a
    /// boxed value type, to make the code more expressive and reduce the chances of errors.
    /// Consider this example:
    /// <code>
    /// object obj = 42;
    ///
    /// // Manual, error prone unboxing
    /// int sum = (int)obj + 1;
    /// </code>
    /// In this example, it is not possible to know in advance what type is actually being boxed in a given
    /// <see cref="object"/> instance, making the code less robust at build time. The <see cref="Box{T}"/>
    /// type can be used as a drop-in replacement in this case, like so:
    /// <code>
    /// Box&lt;int> box = 42;
    ///
    /// // Build-time validation, automatic unboxing
    /// int sum = box.Value + 1;
    /// </code>
    /// This type can also be useful when dealing with large custom value types that are also boxed, as
    /// it allows to retrieve a mutable reference to the boxing value. This means that a given boxed
    /// value can be mutated in-place, instead of having to allocate a new updated boxed instance.
    /// </summary>
    /// <typeparam name="T">The type of value being boxed.</typeparam>
    [DebuggerDisplay("{ToString(),raw}")]
    public sealed class Box<T>
        where T : struct
    {
        // Boxed value types in the CLR are represented in memory as simple objects that store the method table of
        // the corresponding T value type being boxed, and then the data of the value being boxed:
        // [ sync block || pMethodTable || boxed T value ]
        //                 ^               ^
        //                 |               \-- Unsafe.Unbox<T>(Box<T>)
        //                 \-- Box<T> reference
        // For more info, see: https://mattwarren.org/2017/08/02/A-look-at-the-internals-of-boxing-in-the-CLR/.
        // Note that there might be some padding before the actual data representing the boxed value,
        // which might depend on both the runtime and the exact CPU architecture.
        // This is automatically handled by the unbox !!T instruction in IL, which
        // unboxes a given value type T and returns a reference to its boxed data.

        /// <summary>
        /// Initializes a new instance of the <see cref="Box{T}"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is never used, it is only declared in order to mark it with
        /// the <see langword="private"/> visibility modifier and prevent direct use.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Always thrown when this constructor is used (eg. from reflection).</exception>
        private Box()
        {
            throw new InvalidOperationException("The Microsoft.Toolkit.HighPerformance.Box<T> constructor should never be used");
        }

        /// <summary>
        /// Returns a <see cref="Box{T}"/> reference from the input <see cref="object"/> instance.
        /// </summary>
        /// <param name="obj">The input <see cref="object"/> instance, representing a boxed <typeparamref name="T"/> value.</param>
        /// <returns>A <see cref="Box{T}"/> reference pointing to <paramref name="obj"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Box<T> GetFrom(object obj)
        {
            if (obj.GetType() != typeof(T))
            {
                ThrowInvalidCastExceptionForGetFrom();
            }

            return Unsafe.As<Box<T>>(obj)!;
        }

        /// <summary>
        /// Returns a <see cref="Box{T}"/> reference from the input <see cref="object"/> instance.
        /// </summary>
        /// <param name="obj">The input <see cref="object"/> instance, representing a boxed <typeparamref name="T"/> value.</param>
        /// <returns>A <see cref="Box{T}"/> reference pointing to <paramref name="obj"/>.</returns>
        /// <remarks>
        /// This method doesn't check the actual type of <paramref name="obj"/>, so it is responsibility of the caller
        /// to ensure it actually represents a boxed <typeparamref name="T"/> value and not some other instance.
        /// </remarks>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Box<T> DangerousGetFrom(object obj)
        {
            return Unsafe.As<Box<T>>(obj)!;
        }

        /// <summary>
        /// Tries to get a <see cref="Box{T}"/> reference from an input <see cref="object"/> representing a boxed <typeparamref name="T"/> value.
        /// </summary>
        /// <param name="obj">The input <see cref="object"/> instance to check.</param>
        /// <param name="box">The resulting <see cref="Box{T}"/> reference, if <paramref name="obj"/> was a boxed <typeparamref name="T"/> value.</param>
        /// <returns><see langword="true"/> if a <see cref="Box{T}"/> instance was retrieved correctly, <see langword="false"/> otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetFrom(object obj, [NotNullWhen(true)] out Box<T>? box)
        {
            if (obj.GetType() == typeof(T))
            {
                box = Unsafe.As<Box<T>>(obj)!;

                return true;
            }

            box = null;

            return false;
        }

        /// <summary>
        /// Implicitly gets the <typeparamref name="T"/> value from a given <see cref="Box{T}"/> instance.
        /// </summary>
        /// <param name="box">The input <see cref="Box{T}"/> instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator T(Box<T> box)
        {
            return (T)(object)box;
        }

        /// <summary>
        /// Implicitly creates a new <see cref="Box{T}"/> instance from a given <typeparamref name="T"/> value.
        /// </summary>
        /// <param name="value">The input <typeparamref name="T"/> value to wrap.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator Box<T>(T value)
        {
            // The Box<T> type is never actually instantiated.
            // Here we are just boxing the input T value, and then reinterpreting
            // that object reference as a Box<T> reference. As such, the Box<T>
            // type is really only used as an interface to access the contents
            // of a boxed value type. This also makes it so that additional methods
            // like ToString() or GetHashCode() will automatically be referenced from
            // the method table of the boxed object, meaning that they don't need to
            // manually be implemented in the Box<T> type. For instance, boxing a float
            // and calling ToString() on it directly, on its boxed object or on a Box<T>
            // reference retrieved from it will produce the same result in all cases.
            return Unsafe.As<Box<T>>(value)!;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            // Here we're overriding the base object virtual methods to ensure
            // calls to those methods have a correct results on all runtimes.
            // For instance, not doing so is causing issue on .NET Core 2.1 Release
            // due to how the runtime handles the Box<T> reference to an actual
            // boxed T value (not a concrete Box<T> instance as it would expect).
            // To fix that, the overrides will simply call the expected methods
            // directly on the boxed T values. These methods will be directly
            // invoked by the JIT compiler when using a Box<T> reference. When
            // an object reference is used instead, the call would be forwarded
            // to those same methods anyway, since the method table for an object
            // representing a T instance is the one of type T anyway.
            return this.GetReference().ToString()!;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(this, obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return this.GetReference().GetHashCode();
        }

        /// <summary>
        /// Throws an <see cref="InvalidCastException"/> when a cast from an invalid <see cref="object"/> is attempted.
        /// </summary>
        private static void ThrowInvalidCastExceptionForGetFrom()
        {
            throw new InvalidCastException($"Can't cast the input object to the type Box<{typeof(T)}>");
        }
    }

#pragma warning disable SA1402 // Extensions being declared after the type they apply to
#pragma warning disable SA1204 // Extension class to replace instance methods for Box<T>

    /// <summary>
    /// Helpers for working with the <see cref="Box{T}"/> type.
    /// </summary>
    public static class BoxExtensions
    {
        /// <summary>
        /// Gets a <typeparamref name="T"/> reference from a <see cref="Box{T}"/> instance.
        /// </summary>
        /// <typeparam name="T">The type of reference to retrieve.</typeparam>
        /// <param name="box">The input <see cref="Box{T}"/> instance.</param>
        /// <returns>A <typeparamref name="T"/> reference to the boxed value within <paramref name="box"/>.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetReference<T>(this Box<T> box)
            where T : struct
        {
            // The reason why this method is an extension and is not part of
            // the Box<T> type itself is that Box<T> is really just a mask
            // used over object references, but it is never actually instantiated.
            // Because of this, the method table of the objects in the heap will
            // be the one of type T created by the runtime, and not the one of
            // the Box<T> type. To avoid potential issues when invoking this method
            // on different runtimes, which might handle that scenario differently,
            // we use an extension method, which is just syntactic sugar for a static
            // method belonging to another class. This isn't technically necessary,
            // but it's just an extra precaution since the syntax for users remains
            // exactly the same anyway. Here we just call the Unsafe.Unbox<T>(object)
            // API, which is hidden away for users of the type for simplicity.
            // Note that this API will always actually involve a conditional
            // branch, which is introduced by the JIT compiler to validate the
            // object instance being unboxed. But since the alternative of
            // manually tracking the offset to the boxed data would be both
            // more error prone, and it would still introduce some overhead,
            // this doesn't really matter in this case anyway.
            return ref Unsafe.Unbox<T>(box);
        }
    }
}