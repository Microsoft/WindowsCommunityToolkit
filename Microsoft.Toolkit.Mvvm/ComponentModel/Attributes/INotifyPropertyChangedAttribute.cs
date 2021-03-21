// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable CS1574

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
        /// <summary>
        /// Gets or sets a value indicating whether or not to also generate all the additional helper methods that are found
        /// in <see cref="ObservableObject"/> as well (eg. <see cref="ObservableObject.SetProperty{T}(ref T, T, string?)"/>.
        /// If set to <see langword="false"/>, only the <see cref="INotifyPropertyChanged.PropertyChanged"/> event and
        /// the two <see cref="ObservableObject.OnPropertyChanged(PropertyChangedEventArgs)"/> overloads will be generated.
        /// The default value is <see langword="true"/>.
        /// </summary>
        public bool IncludeAdditionalHelperMethods { get; set; } = true;
    }
}
