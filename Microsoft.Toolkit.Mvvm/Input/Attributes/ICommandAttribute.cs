// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable CS1574

using System;
using System.Windows.Input;

namespace Microsoft.Toolkit.Mvvm.Input
{
    /// <summary>
    /// An attribute that can be used to automatically generate <see cref="ICommand"/> properties from declared methods. When this attribute
    /// is used to decorate a method, a generator will create a command property with the corresponding <see cref="IRelayCommand"/> interface
    /// depending on the signature of the method. If an invalid method signature is used, the generator will report an error.
    /// <para>
    /// In order to use this attribute, the containing type doesn't need to implement any interfaces. The generated properties will be lazily
    /// assigned but their value will never change, so there is no need to support property change notifications or other additional functionality.
    /// </para>
    /// <para>
    /// This attribute can be used as follows:
    /// <code>
    /// partial class MyViewModel
    /// {
    ///     [ICommand]
    ///     private void GreetUser(User? user)
    ///     {
    ///         Console.WriteLine($"Hello {user.Name}!");
    ///     }
    /// }
    /// </code>
    /// And with this, code analogous to this will be generated:
    /// <code>
    /// partial class MyViewModel
    /// {
    ///     private IRelayCommand? greetUserCommand;
    ///
    ///     public IRelayCommand GreetUserCommand => greetUserCommand ??= new RelayCommand(GreetUser);
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// The following signatures are supported for annotated methods:
    /// <code>
    /// void Method();
    /// </code>
    /// Will generate an <see cref="IRelayCommand"/> property (using a <see cref="RelayCommand"/> instance).
    /// <code>
    /// void Method(T?);
    /// </code>
    /// Will generate an <see cref="IRelayCommand{T}"/> property (using a <see cref="RelayCommand{T}"/> instance).
    /// <code>
    /// Task Method();
    /// Task Method(CancellationToken);
    /// </code>
    /// Will both generate an <see cref="IAsyncRelayCommand"/> property (using an <see cref="AsyncRelayCommand{T}"/> instance).
    /// <code>
    /// Task Method(T?);
    /// Task Method(T?, CancellationToken);
    /// </code>
    /// Will both generate an <see cref="IAsyncRelayCommand{T}"/> property (using an <see cref="AsyncRelayCommand{T}"/> instance).
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ICommandAttribute : Attribute
    {
    }
}
