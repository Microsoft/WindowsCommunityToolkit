// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Toolkit.Mvvm.SourceGenerators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests.Mvvm
{
    [TestClass]
    public class Test_SourceGeneratorsDiagnostics
    {
        [TestCategory("Mvvm")]
        [TestMethod]
        public void DuplicateINotifyPropertyChangedInterfaceForINotifyPropertyChangedAttributeError_Explicit()
        {
            string source = @"
            using System.ComponentModel;
            using Microsoft.Toolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class SampleViewModel : INotifyPropertyChanged
                {
                    public event PropertyChangedEventHandler? PropertyChanged;
                }
            }";

            VerifyGeneratedDiagnostics<INotifyPropertyChangedGenerator>(source, "MVVMTK0004");
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void DuplicateINotifyPropertyChangedInterfaceForINotifyPropertyChangedAttributeError_Inherited()
        {
            string source = @"
            using System.ComponentModel;
            using Microsoft.Toolkit.Mvvm.ComponentModel;

            namespace Microsoft.Toolkit.Mvvm.ComponentModel
            {
                public abstract class ObservableObject : INotifyPropertyChanged, INotifyPropertyChanging
                {
                    public event PropertyChangedEventHandler? PropertyChanged;
                    public event PropertyChangingEventHandler? PropertyChanging;
                }
            }

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class SampleViewModel : ObservableObject
                {
                }
            }";

            VerifyGeneratedDiagnostics<INotifyPropertyChangedGenerator>(source, "MVVMTK0004");
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void DuplicateINotifyPropertyChangedInterfaceForObservableObjectAttributeError_Explicit()
        {
            string source = @"
            using System.ComponentModel;
            using Microsoft.Toolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableObject]
                public partial class SampleViewModel : INotifyPropertyChanged
                {
                    public event PropertyChangedEventHandler? PropertyChanged;
                }
            }";

            VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0005");
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void DuplicateINotifyPropertyChangedInterfaceForObservableObjectAttributeError_Inherited()
        {
            string source = @"
            using System.ComponentModel;
            using Microsoft.Toolkit.Mvvm.ComponentModel;

            namespace Microsoft.Toolkit.Mvvm.ComponentModel
            {
                public abstract class ObservableObject : INotifyPropertyChanged
                {
                    public event PropertyChangedEventHandler? PropertyChanged;
                }
            }

            namespace MyApp
            {
                [ObservableObject]
                public partial class SampleViewModel : ObservableObject
                {
                }
            }";

            VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0005");
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void DuplicateINotifyPropertyChangingInterfaceForObservableObjectAttributeError_Explicit()
        {
            string source = @"
            using System.ComponentModel;
            using Microsoft.Toolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableObject]
                public partial class SampleViewModel : INotifyPropertyChanging
                {
                    public event PropertyChangingEventHandler? PropertyChanging;
                }
            }";

            VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0006");
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void DuplicateINotifyPropertyChangingInterfaceForObservableObjectAttributeError_Inherited()
        {
            string source = @"
            using System.ComponentModel;
            using Microsoft.Toolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                public abstract class MyBaseViewModel : INotifyPropertyChanging
                {
                    public event PropertyChangingEventHandler? PropertyChanging;
                }

                [ObservableObject]
                public partial class SampleViewModel : MyBaseViewModel
                {
                }
            }";

            VerifyGeneratedDiagnostics<ObservableObjectGenerator>(source, "MVVMTK0006");
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void DuplicateObservableRecipientError()
        {
            string source = @"
            using Microsoft.Toolkit.Mvvm.ComponentModel;

            namespace Microsoft.Toolkit.Mvvm.ComponentModel
            {
                public abstract class ObservableRecipient : ObservableObject
                {
                }
            }

            namespace MyApp
            {
                [ObservableRecipient]
                public partial class SampleViewModel : ObservableRecipient
                {
                }
            }";

            VerifyGeneratedDiagnostics<ObservableRecipientGenerator>(source, "MVVMTK0007");
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void MissingBaseObservableObjectFunctionalityError()
        {
            string source = @"
            using Microsoft.Toolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [ObservableRecipient]
                public partial class SampleViewModel
                {
                }
            }";

            VerifyGeneratedDiagnostics<ObservableRecipientGenerator>(source, "MVVMTK0008");
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void MissingObservableValidatorInheritanceError()
        {
            string source = @"
            using System.ComponentModel.DataAnnotations;
            using Microsoft.Toolkit.Mvvm.ComponentModel;

            namespace MyApp
            {
                [INotifyPropertyChanged]
                public partial class SampleViewModel
                {
                    [ObservableProperty]
                    [Required]
                    private string name;
                }
            }";

            VerifyGeneratedDiagnostics<ObservablePropertyGenerator>(source, "MVVMTK0009");
        }

        [TestCategory("Mvvm")]
        [TestMethod]
        public void InvalidICommandMethodSignatureError()
        {
            string source = @"
            using Microsoft.Toolkit.Mvvm.Input;

            namespace MyApp
            {
                public partial class SampleViewModel
                {
                    [ICommand]
                    private string GreetUser() => ""Hello world!"";
                }
            }";

            VerifyGeneratedDiagnostics<ICommandGenerator>(source, "MVVMTK0012");
        }

        /// <summary>
        /// Verifies the output of a source generator.
        /// </summary>
        /// <typeparam name="TGenerator">The generator type to use.</typeparam>
        /// <param name="source">The input source to process.</param>
        /// <param name="diagnosticsIds">The diagnostic ids to expect for the input source code.</param>
        private void VerifyGeneratedDiagnostics<TGenerator>(string source, params string[] diagnosticsIds)
            where TGenerator : class, ISourceGenerator, new()
        {
            Type validationAttributeType = typeof(ValidationAttribute);

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(source);

            IEnumerable<MetadataReference> references =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                where !assembly.IsDynamic
                let reference = MetadataReference.CreateFromFile(assembly.Location)
                select reference;

            CSharpCompilation compilation = CSharpCompilation.Create("original", new SyntaxTree[] { syntaxTree }, references, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            ISourceGenerator generator = new TGenerator();

            CSharpGeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

            driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

            HashSet<string> resultingIds = diagnostics.Select(diagnostic => diagnostic.Id).ToHashSet();

            Assert.IsTrue(resultingIds.SetEquals(diagnosticsIds));

            GC.KeepAlive(validationAttributeType);
        }
    }
}
