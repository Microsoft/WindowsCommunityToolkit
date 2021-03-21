// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable CS1574

using System;
using System.ComponentModel;

namespace Microsoft.Toolkit.Mvvm.ComponentModel
{
    /// <summary>
    /// An attribute that indicates that a given type should have all the members from <see cref="ObservableObject"/>
    /// generated into it, as well as the <see cref="INotifyPropertyChanged"/> and <see cref="INotifyPropertyChanging"/>
    /// interfaces. This can be useful when you want the same functionality from <see cref="ObservableObject"/> into a class
    /// that already inherits from another one (since C# doesn't support multiple inheritance). This attribute will trigger
    /// the source generator to just create the same APIs directly into the decorated class.
    /// <para>
    /// This attribute can be used as follows:
    /// <code>
    /// [ObservableObject]
    /// partial class MyViewModel : SomeOtherClass
    /// {
    ///     // Other members here...
    /// }
    /// </code>
    /// </para>
    /// And with this, the same APIs from <see cref="ObservableObject"/> will be available on this type as well.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ObservableObjectAttribute : Attribute
    {
    }
}
