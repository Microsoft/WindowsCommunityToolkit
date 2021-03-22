using Microsoft.CodeAnalysis;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <summary>
    /// A source generator for the <see cref="ObservableObjectAttribute"/> type.
    /// </summary>
    [Generator]
    public class ObservableObjectGenerator : TransitiveMembersGenerator<ObservableObjectAttribute>
    {
    }
}
