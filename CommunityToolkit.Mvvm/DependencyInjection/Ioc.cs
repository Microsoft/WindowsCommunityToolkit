// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;

#nullable enable

namespace CommunityToolkit.Mvvm.DependencyInjection
{
    /// <summary>
    /// A type that facilitates the use of the <see cref="IServiceProvider"/> type.
    /// The <see cref="Ioc"/> provides the ability to configure services in a singleton, thread-safe
    /// service provider instance, which can then be used to resolve service instances.
    /// The first step to use this feature is to declare some services, for instance:
    /// <code>
    /// public interface ILogger
    /// {
    ///     void Log(string text);
    /// }
    /// </code>
    /// <code>
    /// public class ConsoleLogger : ILogger
    /// {
    ///     void Log(string text) => Console.WriteLine(text);
    /// }
    /// </code>
    /// Then the services configuration should then be done at startup, by calling the <see cref="ConfigureServices"/>
    /// method and passing an <see cref="IServiceProvider"/> instance with the services to use. That instance can
    /// be from any library offering dependency injection functionality, such as Microsoft.Extensions.DependencyInjection.
    /// For instance, using that library, <see cref="ConfigureServices"/> can be used as follows in this example:
    /// <code>
    /// Ioc.Default.ConfigureServices(
    ///     new ServiceCollection()
    ///     .AddSingleton&lt;ILogger, Logger&gt;()
    ///     .BuildServiceProvider());
    /// </code>
    /// Finally, you can use the <see cref="Ioc"/> instance (which implements <see cref="IServiceProvider"/>)
    /// to retrieve the service instances from anywhere in your application, by doing as follows:
    /// <code>
    /// Ioc.Default.GetService&lt;ILogger&gt;().Log("Hello world!");
    /// </code>
    /// </summary>
    public sealed class Ioc : IServiceProvider
    {
        /// <summary>
        /// Gets the default <see cref="Ioc"/> instance.
        /// </summary>
        public static Ioc Default { get; } = new();

        /// <summary>
        /// The <see cref="IServiceProvider"/> instance to use, if initialized.
        /// </summary>
        private volatile IServiceProvider? serviceProvider;

        /// <inheritdoc/>
        public object? GetService(Type serviceType)
        {
            // As per section I.12.6.6 of the official CLI ECMA-335 spec:
            // "[...] read and write access to properly aligned memory locations no larger than the native
            // word size is atomic when all the write accesses to a location are the same size. Atomic writes
            // shall alter no bits other than those written. Unless explicit layout control is used [...],
            // data elements no larger than the natural word size [...] shall be properly aligned.
            // Object references shall be treated as though they are stored in the native word size."
            // The field being accessed here is of native int size (reference type), and is only ever accessed
            // directly and atomically by a compare exchange instruction (see below), or here. We can therefore
            // assume this read is thread safe with respect to accesses to this property or to invocations to one
            // of the available configuration methods. So we can just read the field directly and make the necessary
            // check with our local copy, without the need of paying the locking overhead from this get accessor.
            IServiceProvider? provider = this.serviceProvider;

            if (provider is null)
            {
                ThrowInvalidOperationExceptionForMissingInitialization();
            }

            return provider!.GetService(serviceType);
        }

        /// <summary>
        /// Tries to resolve an instance of a specified service type.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>An instance of the specified service, or <see langword="null"/>.</returns>
        /// <exception cref="InvalidOperationException">Throw if the current <see cref="Ioc"/> instance has not been initialized.</exception>
        public T? GetService<T>()
            where T : class
        {
            IServiceProvider? provider = this.serviceProvider;

            if (provider is null)
            {
                ThrowInvalidOperationExceptionForMissingInitialization();
            }

            return (T?)provider!.GetService(typeof(T));
        }

        /// <summary>
        /// Resolves an instance of a specified service type.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>An instance of the specified service, or <see langword="null"/>.</returns>
        /// <exception cref="InvalidOperationException">
        /// Throw if the current <see cref="Ioc"/> instance has not been initialized, or if the
        /// requested service type was not registered in the service provider currently in use.
        /// </exception>
        public T GetRequiredService<T>()
            where T : class
        {
            IServiceProvider? provider = this.serviceProvider;

            if (provider is null)
            {
                ThrowInvalidOperationExceptionForMissingInitialization();
            }

            T? service = (T?)provider!.GetService(typeof(T));

            if (service is null)
            {
                ThrowInvalidOperationExceptionForUnregisteredType();
            }

            return service!;
        }

        /// <summary>
        /// Initializes the shared <see cref="IServiceProvider"/> instance.
        /// </summary>
        /// <param name="serviceProvider">The input <see cref="IServiceProvider"/> instance to use.</param>
        public void ConfigureServices(IServiceProvider serviceProvider)
        {
            IServiceProvider? oldServices = Interlocked.CompareExchange(ref this.serviceProvider, serviceProvider, null);

            if (oldServices is not null)
            {
                ThrowInvalidOperationExceptionForRepeatedConfiguration();
            }
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> when the <see cref="IServiceProvider"/> property is used before initialization.
        /// </summary>
        private static void ThrowInvalidOperationExceptionForMissingInitialization()
        {
            throw new InvalidOperationException("The service provider has not been configured yet");
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> when the <see cref="IServiceProvider"/> property is missing a type registration.
        /// </summary>
        private static void ThrowInvalidOperationExceptionForUnregisteredType()
        {
            throw new InvalidOperationException("The requested service type was not registered");
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> when a configuration is attempted more than once.
        /// </summary>
        private static void ThrowInvalidOperationExceptionForRepeatedConfiguration()
        {
            throw new InvalidOperationException("The default service provider has already been configured");
        }
    }
}