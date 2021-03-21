// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;

namespace Microsoft.Toolkit.Mvvm.ComponentModel
{
    /// <summary>
    /// An attribute that indicates that a given type should implement the <see cref="INotifyPropertyChanged"/> interface and
    /// have minimal built-in functionality to support it. This includes exposing the necessary event and having two methods
    /// to raise it that mirror <see cref="ObservableObject.OnPropertyChanged(PropertyChangedEventArgs)"/> and
    /// <see cref="ObservableObject.OnPropertyChanged(string?)"/>. For more extensive support, use <see cref="ObservableObjectAttribute"/>.
    /// <para>
    /// This attribute can be used as follows:
    /// <code>
    /// [INotifyPropertyChanged]
    /// partial class MyViewModel : SomeOtherClass
    /// {
    ///     // Other members here...
    /// }
    /// </code>
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class INotifyPropertyChangedAttribute : Attribute
    {
    }
}
