using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <summary>
    /// A source generator for the <see cref="ObservableRecipientAttribute"/> type.
    /// </summary>
    [Generator]
    public class ObservableRecipientGenerator : TransitiveMembersGenerator<ObservableRecipientAttribute>
    {
        /// <inheritdoc/>
        protected override IEnumerable<MemberDeclarationSyntax> FilterDeclaredMembers(AttributeData attributeData, ClassDeclarationSyntax sourceDeclaration)
        {
            return sourceDeclaration.Members.Where(static member => member is not ConstructorDeclarationSyntax);
        }
    }
}
