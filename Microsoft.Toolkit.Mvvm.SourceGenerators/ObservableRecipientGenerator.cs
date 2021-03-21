using Microsoft.CodeAnalysis;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <summary>
    /// A source generator for the <see cref="ObservableRecipientAttribute"/> type.
    /// </summary>
    [Generator]
    public class ObservableRecipientGenerator : TransitiveMembersGenerator<ObservableRecipientAttribute>
    {
    }
}
