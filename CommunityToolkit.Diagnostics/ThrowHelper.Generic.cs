// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
#if !NETSTANDARD1_4
using System.Runtime.InteropServices;
#endif
using System.Threading;

namespace CommunityToolkit.Diagnostics
{
    /// <summary>
    /// Helper methods to efficiently throw exceptions.
    /// </summary>
    public static partial class ThrowHelper
    {
        /// <summary>
        /// Throws a new <see cref="ArrayTypeMismatchException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="ArrayTypeMismatchException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArrayTypeMismatchException<T>()
        {
            throw new ArrayTypeMismatchException();
        }

        /// <summary>
        /// Throws a new <see cref="ArrayTypeMismatchException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="ArrayTypeMismatchException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArrayTypeMismatchException<T>(string? message)
        {
            throw new ArrayTypeMismatchException(message);
        }

        /// <summary>
        /// Throws a new <see cref="ArrayTypeMismatchException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="ArrayTypeMismatchException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArrayTypeMismatchException<T>(string? message, Exception? innerException)
        {
            throw new ArrayTypeMismatchException(message, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="ArgumentException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArgumentException<T>()
        {
            throw new ArgumentException();
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="ArgumentException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArgumentException<T>(string? message)
        {
            throw new ArgumentException(message);
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="ArgumentException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArgumentException<T>(string? message, Exception? innerException)
        {
            throw new ArgumentException(message, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="name">The argument name.</param>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="ArgumentException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArgumentException<T>(string? name, string? message)
        {
            throw new ArgumentException(message, name);
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="name">The argument name.</param>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="ArgumentException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArgumentException<T>(string? name, string? message, Exception? innerException)
        {
            throw new ArgumentException(message, name, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="ArgumentNullException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArgumentNullException<T>()
        {
            throw new ArgumentNullException();
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="name">The argument name.</param>
        /// <exception cref="ArgumentNullException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArgumentNullException<T>(string? name)
        {
            throw new ArgumentNullException(name);
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="name">The argument name.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="ArgumentNullException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArgumentNullException<T>(string? name, Exception? innerException)
        {
            throw new ArgumentNullException(name, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="name">The argument name.</param>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="ArgumentNullException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArgumentNullException<T>(string? name, string? message)
        {
            throw new ArgumentNullException(name, message);
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="ArgumentOutOfRangeException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArgumentOutOfRangeException<T>()
        {
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="name">The argument name.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArgumentOutOfRangeException<T>(string? name)
        {
            throw new ArgumentOutOfRangeException(name);
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="name">The argument name.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArgumentOutOfRangeException<T>(string? name, Exception? innerException)
        {
            throw new ArgumentOutOfRangeException(name, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="name">The argument name.</param>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArgumentOutOfRangeException<T>(string? name, string? message)
        {
            throw new ArgumentOutOfRangeException(name, message);
        }

        /// <summary>
        /// Throws a new <see cref="ArgumentOutOfRangeException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="name">The argument name.</param>
        /// <param name="value">The current argument value.</param>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowArgumentOutOfRangeException<T>(string? name, object? value, string? message)
        {
            throw new ArgumentOutOfRangeException(name, value, message);
        }

#if !NETSTANDARD1_4
        /// <summary>
        /// Throws a new <see cref="COMException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="COMException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowCOMException<T>()
        {
            throw new COMException();
        }

        /// <summary>
        /// Throws a new <see cref="COMException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="COMException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowCOMException<T>(string? message)
        {
            throw new COMException(message);
        }

        /// <summary>
        /// Throws a new <see cref="COMException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The argument name.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="COMException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowCOMException<T>(string? message, Exception? innerException)
        {
            throw new COMException(message, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="COMException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The argument name.</param>
        /// <param name="error">The HRESULT of the errror to include.</param>
        /// <exception cref="COMException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowCOMException<T>(string? message, int error)
        {
            throw new COMException(message, error);
        }

        /// <summary>
        /// Throws a new <see cref="ExternalException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="ExternalException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowExternalException<T>()
        {
            throw new ExternalException();
        }

        /// <summary>
        /// Throws a new <see cref="ExternalException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="ExternalException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowExternalException<T>(string? message)
        {
            throw new ExternalException(message);
        }

        /// <summary>
        /// Throws a new <see cref="ExternalException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The argument name.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="ExternalException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowExternalException<T>(string? message, Exception? innerException)
        {
            throw new ExternalException(message, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="ExternalException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The argument name.</param>
        /// <param name="error">The HRESULT of the errror to include.</param>
        /// <exception cref="ExternalException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowExternalException<T>(string? message, int error)
        {
            throw new ExternalException(message, error);
        }
#endif

        /// <summary>
        /// Throws a new <see cref="FormatException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="FormatException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowFormatException<T>()
        {
            throw new FormatException();
        }

        /// <summary>
        /// Throws a new <see cref="FormatException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="FormatException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowFormatException<T>(string? message)
        {
            throw new FormatException(message);
        }

        /// <summary>
        /// Throws a new <see cref="FormatException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="FormatException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowFormatException<T>(string? message, Exception? innerException)
        {
            throw new FormatException(message, innerException);
        }

#if !NETSTANDARD1_4
        /// <summary>
        /// Throws a new <see cref="InsufficientMemoryException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="InsufficientMemoryException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowInsufficientMemoryException<T>()
        {
            throw new InsufficientMemoryException();
        }

        /// <summary>
        /// Throws a new <see cref="InsufficientMemoryException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="InsufficientMemoryException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowInsufficientMemoryException<T>(string? message)
        {
            throw new InsufficientMemoryException(message);
        }

        /// <summary>
        /// Throws a new <see cref="InsufficientMemoryException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="InsufficientMemoryException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowInsufficientMemoryException<T>(string? message, Exception? innerException)
        {
            throw new InsufficientMemoryException(message, innerException);
        }
#endif

        /// <summary>
        /// Throws a new <see cref="InvalidDataException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="InvalidDataException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowInvalidDataException<T>()
        {
            throw new InvalidDataException();
        }

        /// <summary>
        /// Throws a new <see cref="InvalidDataException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="InvalidDataException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowInvalidDataException<T>(string? message)
        {
            throw new InvalidDataException(message);
        }

        /// <summary>
        /// Throws a new <see cref="InvalidDataException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="InvalidDataException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowInvalidDataException<T>(string? message, Exception? innerException)
        {
            throw new InvalidDataException(message, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="InvalidOperationException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowInvalidOperationException<T>()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Throws a new <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="InvalidOperationException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowInvalidOperationException<T>(string? message)
        {
            throw new InvalidOperationException(message);
        }

        /// <summary>
        /// Throws a new <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="InvalidOperationException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowInvalidOperationException<T>(string? message, Exception? innerException)
        {
            throw new InvalidOperationException(message, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="LockRecursionException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="LockRecursionException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowLockRecursionException<T>()
        {
            throw new LockRecursionException();
        }

        /// <summary>
        /// Throws a new <see cref="LockRecursionException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="LockRecursionException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowLockRecursionException<T>(string? message)
        {
            throw new LockRecursionException(message);
        }

        /// <summary>
        /// Throws a new <see cref="LockRecursionException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="LockRecursionException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowLockRecursionException<T>(string? message, Exception? innerException)
        {
            throw new LockRecursionException(message, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="MissingFieldException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="MissingFieldException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowMissingFieldException<T>()
        {
            throw new MissingFieldException();
        }

        /// <summary>
        /// Throws a new <see cref="MissingFieldException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="MissingFieldException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowMissingFieldException<T>(string? message)
        {
            throw new MissingFieldException(message);
        }

        /// <summary>
        /// Throws a new <see cref="MissingFieldException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="MissingFieldException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowMissingFieldException<T>(string? message, Exception? innerException)
        {
            throw new MissingFieldException(message, innerException);
        }

#if !NETSTANDARD1_4
        /// <summary>
        /// Throws a new <see cref="MissingFieldException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="className">The target class being inspected.</param>
        /// <param name="fieldName">The target field being retrieved.</param>
        /// <exception cref="MissingFieldException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowMissingFieldException<T>(string? className, string? fieldName)
        {
            throw new MissingFieldException(className, fieldName);
        }
#endif

        /// <summary>
        /// Throws a new <see cref="MissingMemberException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="MissingMemberException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowMissingMemberException<T>()
        {
            throw new MissingMemberException();
        }

        /// <summary>
        /// Throws a new <see cref="MissingMemberException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="MissingMemberException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowMissingMemberException<T>(string? message)
        {
            throw new MissingMemberException(message);
        }

        /// <summary>
        /// Throws a new <see cref="MissingMemberException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="MissingMemberException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowMissingMemberException<T>(string? message, Exception? innerException)
        {
            throw new MissingMemberException(message, innerException);
        }

#if !NETSTANDARD1_4
        /// <summary>
        /// Throws a new <see cref="MissingMemberException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="className">The target class being inspected.</param>
        /// <param name="memberName">The target member being retrieved.</param>
        /// <exception cref="MissingMemberException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowMissingMemberException<T>(string? className, string? memberName)
        {
            throw new MissingMemberException(className, memberName);
        }
#endif

        /// <summary>
        /// Throws a new <see cref="MissingMethodException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="MissingMethodException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowMissingMethodException<T>()
        {
            throw new MissingMethodException();
        }

        /// <summary>
        /// Throws a new <see cref="MissingMethodException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="MissingMethodException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowMissingMethodException<T>(string? message)
        {
            throw new MissingMethodException(message);
        }

        /// <summary>
        /// Throws a new <see cref="MissingMethodException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="MissingMethodException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowMissingMethodException<T>(string? message, Exception? innerException)
        {
            throw new MissingMethodException(message, innerException);
        }

#if !NETSTANDARD1_4
        /// <summary>
        /// Throws a new <see cref="MissingMethodException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="className">The target class being inspected.</param>
        /// <param name="methodName">The target method being retrieved.</param>
        /// <exception cref="MissingMethodException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowMissingMethodException<T>(string? className, string? methodName)
        {
            throw new MissingMethodException(className, methodName);
        }
#endif

        /// <summary>
        /// Throws a new <see cref="NotSupportedException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="NotSupportedException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowNotSupportedException<T>()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Throws a new <see cref="NotSupportedException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="NotSupportedException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowNotSupportedException<T>(string? message)
        {
            throw new NotSupportedException(message);
        }

        /// <summary>
        /// Throws a new <see cref="NotSupportedException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="NotSupportedException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowNotSupportedException<T>(string? message, Exception? innerException)
        {
            throw new NotSupportedException(message, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="ObjectDisposedException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="objectName">The name of the disposed object.</param>
        /// <exception cref="ObjectDisposedException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowObjectDisposedException<T>(string? objectName)
        {
            throw new ObjectDisposedException(objectName);
        }

        /// <summary>
        /// Throws a new <see cref="ObjectDisposedException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="objectName">The name of the disposed object.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="ObjectDisposedException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowObjectDisposedException<T>(string? objectName, Exception? innerException)
        {
            throw new ObjectDisposedException(objectName, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="ObjectDisposedException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="objectName">The name of the disposed object.</param>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="ObjectDisposedException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowObjectDisposedException<T>(string? objectName, string? message)
        {
            throw new ObjectDisposedException(objectName, message);
        }

        /// <summary>
        /// Throws a new <see cref="OperationCanceledException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="OperationCanceledException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowOperationCanceledException<T>()
        {
            throw new OperationCanceledException();
        }

        /// <summary>
        /// Throws a new <see cref="OperationCanceledException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="OperationCanceledException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowOperationCanceledException<T>(string? message)
        {
            throw new OperationCanceledException(message);
        }

        /// <summary>
        /// Throws a new <see cref="OperationCanceledException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="OperationCanceledException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowOperationCanceledException<T>(string? message, Exception? innerException)
        {
            throw new OperationCanceledException(message, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="OperationCanceledException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="token">The <see cref="CancellationToken"/> in use.</param>
        /// <exception cref="OperationCanceledException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowOperationCanceledException<T>(CancellationToken token)
        {
            throw new OperationCanceledException(token);
        }

        /// <summary>
        /// Throws a new <see cref="OperationCanceledException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="token">The <see cref="CancellationToken"/> in use.</param>
        /// <exception cref="OperationCanceledException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowOperationCanceledException<T>(string? message, CancellationToken token)
        {
            throw new OperationCanceledException(message, token);
        }

        /// <summary>
        /// Throws a new <see cref="OperationCanceledException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <param name="token">The <see cref="CancellationToken"/> in use.</param>
        /// <exception cref="OperationCanceledException">Thrown with the specified parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowOperationCanceledException<T>(string? message, Exception? innerException, CancellationToken token)
        {
            throw new OperationCanceledException(message, innerException, token);
        }

        /// <summary>
        /// Throws a new <see cref="PlatformNotSupportedException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="PlatformNotSupportedException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowPlatformNotSupportedException<T>()
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        /// Throws a new <see cref="PlatformNotSupportedException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="PlatformNotSupportedException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowPlatformNotSupportedException<T>(string? message)
        {
            throw new PlatformNotSupportedException(message);
        }

        /// <summary>
        /// Throws a new <see cref="PlatformNotSupportedException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="PlatformNotSupportedException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowPlatformNotSupportedException<T>(string? message, Exception? innerException)
        {
            throw new PlatformNotSupportedException(message, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="SynchronizationLockException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="SynchronizationLockException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowSynchronizationLockException<T>()
        {
            throw new SynchronizationLockException();
        }

        /// <summary>
        /// Throws a new <see cref="SynchronizationLockException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="SynchronizationLockException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowSynchronizationLockException<T>(string? message)
        {
            throw new SynchronizationLockException(message);
        }

        /// <summary>
        /// Throws a new <see cref="SynchronizationLockException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="SynchronizationLockException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowSynchronizationLockException<T>(string? message, Exception? innerException)
        {
            throw new SynchronizationLockException(message, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="TimeoutException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="TimeoutException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowTimeoutException<T>()
        {
            throw new TimeoutException();
        }

        /// <summary>
        /// Throws a new <see cref="TimeoutException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="TimeoutException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowTimeoutException<T>(string? message)
        {
            throw new TimeoutException(message);
        }

        /// <summary>
        /// Throws a new <see cref="TimeoutException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="TimeoutException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowTimeoutException<T>(string? message, Exception? innerException)
        {
            throw new TimeoutException(message, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="UnauthorizedAccessException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="UnauthorizedAccessException">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowUnauthorizedAccessException<T>()
        {
            throw new UnauthorizedAccessException();
        }

        /// <summary>
        /// Throws a new <see cref="UnauthorizedAccessException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowUnauthorizedAccessException<T>(string? message)
        {
            throw new UnauthorizedAccessException(message);
        }

        /// <summary>
        /// Throws a new <see cref="UnauthorizedAccessException"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="UnauthorizedAccessException">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowUnauthorizedAccessException<T>(string? message, Exception? innerException)
        {
            throw new UnauthorizedAccessException(message, innerException);
        }

        /// <summary>
        /// Throws a new <see cref="Win32Exception"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <exception cref="Win32Exception">Thrown with no parameters.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowWin32Exception<T>()
        {
            throw new Win32Exception();
        }

        /// <summary>
        /// Throws a new <see cref="Win32Exception"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="error">The Win32 error code associated with this exception.</param>
        /// <exception cref="Win32Exception">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowWin32Exception<T>(int error)
        {
            throw new Win32Exception(error);
        }

        /// <summary>
        /// Throws a new <see cref="Win32Exception"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="error">The Win32 error code associated with this exception.</param>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="Win32Exception">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowWin32Exception<T>(int error, string? message)
        {
            throw new Win32Exception(error, message);
        }

        /// <summary>
        /// Throws a new <see cref="Win32Exception"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <exception cref="Win32Exception">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowWin32Exception<T>(string? message)
        {
            throw new Win32Exception(message);
        }

        /// <summary>
        /// Throws a new <see cref="Win32Exception"/>.
        /// </summary>
        /// <typeparam name="T">The type of expected result.</typeparam>
        /// <param name="message">The message to include in the exception.</param>
        /// <param name="innerException">The inner <see cref="Exception"/> to include.</param>
        /// <exception cref="Win32Exception">Thrown with the specified parameter.</exception>
        /// <returns>This method always throws, so it actually never returns a value.</returns>
        [DoesNotReturn]
        public static T ThrowWin32Exception<T>(string? message, Exception? innerException)
        {
            throw new Win32Exception(message, innerException);
        }
    }
}