// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable CS1574

using System;

namespace Microsoft.Toolkit.Mvvm.ComponentModel
{
    /// <summary>
    /// An attribute that indicates that a given type should have all the members from <see cref="ObservableRecipient"/>
    /// generated into it. This can be useful when you want the same functionality from <see cref="ObservableRecipient"/> into
    /// a class that already inherits from another one (since C# doesn't support multiple inheritance). This attribute will trigger
    /// the source generator to just create the same APIs directly into the decorated class. For instance, this attribute can be
    /// used to easily combine the functionality from both <see cref="ObservableValidator"/> and <see cref="ObservableRecipient"/>,
    /// by using <see cref="ObservableValidator"/> as the base class and adding this attribute to the declared type.
    /// <para>
    /// This attribute can be used as follows:
    /// <code>
    /// [ObservableRecipient]
    /// partial class MyViewModel : ObservableValidator
    /// {
    ///     // Other members here...
    /// }
    /// </code>
    /// </para>
    /// And with this, the same APIs from <see cref="ObservableRecipient"/> will be available on this type as well.
    /// <para>
    /// To avoid conflicts with other APIs in types where the new members are being generated, constructors are omitted. Make sure to
    /// properly initialize the <see cref="ObservableRecipient.Messenger"/> property from the constructors in the type being annotated.
    /// </para>
    /// </summary>
    /// <remarks>
    /// In order to work, <see cref="ObservableRecipientAttribute"/> needs to be applied to a type that inherits from
    /// <see cref="ObservableObject"/> (either directly or indirectly), or to one decorated with <see cref="ObservableObjectAttribute"/>.
    /// This is because the <see cref="ObservableRecipient"/> methods rely on some of the inherited members to work.
    /// If this condition is not met, the code will fail to build.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ObservableRecipientAttribute : Attribute
    {
    }
}
