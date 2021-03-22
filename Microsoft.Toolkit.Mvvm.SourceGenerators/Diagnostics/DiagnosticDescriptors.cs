// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators.Diagnostics
{
    /// <summary>
    /// A container for all <see cref="DiagnosticDescriptor"/> instances for errors reported by analyzers in this project.
    /// </summary>
    internal static class DiagnosticDescriptors
    {
        /// <summary>
        /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <see cref="INotifyPropertyChangedGenerator"/> failed to run on a given type.
        /// <para>
        /// Format: <c>"The generator <see cref="INotifyPropertyChangedGenerator"/> failed to execute on type {0}"</c>.
        /// </para>
        /// </summary>
        public static readonly DiagnosticDescriptor INotifyPropertyChangedGeneratorError = new(
            id: "MVVMTK0001",
            title: $"Internal error for {nameof(INotifyPropertyChangedGenerator)}",
            messageFormat: $"The generator {nameof(INotifyPropertyChangedGenerator)} failed to execute on type {{0}}",
            category: typeof(INotifyPropertyChangedGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"The {nameof(INotifyPropertyChangedGenerator)} generator encountered an error while processing a type. Please report this issue at https://aka.ms/mvvmtoolkit.",
            helpLinkUri: "https://aka.ms/mvvmtoolkit");

        /// <summary>
        /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <see cref="ObservableObjectGenerator"/> failed to run on a given type.
        /// <para>
        /// Format: <c>"The generator <see cref="ObservableObjectGenerator"/> failed to execute on type {0}"</c>.
        /// </para>
        /// </summary>
        public static readonly DiagnosticDescriptor ObservableObjectGeneratorError = new(
            id: "MVVMTK0002",
            title: $"Internal error for {nameof(ObservableObjectGenerator)}",
            messageFormat: $"The generator {nameof(ObservableObjectGenerator)} failed to execute on type {{0}}",
            category: typeof(ObservableObjectGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"The {nameof(ObservableObjectGenerator)} generator encountered an error while processing a type. Please report this issue at https://aka.ms/mvvmtoolkit.",
            helpLinkUri: "https://aka.ms/mvvmtoolkit");

        /// <summary>
        /// Gets a <see cref="DiagnosticDescriptor"/> indicating when <see cref="ObservableRecipientGenerator"/> failed to run on a given type.
        /// <para>
        /// Format: <c>"The generator <see cref="ObservableRecipientGenerator"/> failed to execute on type {0}"</c>.
        /// </para>
        /// </summary>
        public static readonly DiagnosticDescriptor ObservableRecipientGeneratorError = new(
            id: "MVVMTK0003",
            title: $"Internal error for {nameof(ObservableRecipientGenerator)}",
            messageFormat: $"The generator {nameof(ObservableRecipientGenerator)} failed to execute on type {{0}}",
            category: typeof(ObservableRecipientGenerator).FullName,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: $"The {nameof(ObservableRecipientGenerator)} generator encountered an error while processing a type. Please report this issue at https://aka.ms/mvvmtoolkit.",
            helpLinkUri: "https://aka.ms/mvvmtoolkit");
    }
}
