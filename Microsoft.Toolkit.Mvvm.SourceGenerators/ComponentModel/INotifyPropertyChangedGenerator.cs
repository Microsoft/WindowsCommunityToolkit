using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace Microsoft.Toolkit.Mvvm.SourceGenerators
{
    /// <summary>
    /// A source generator for the <see cref="INotifyPropertyChangedAttribute"/> type.
    /// </summary>
    [Generator]
    public class INotifyPropertyChangedGenerator : TransitiveMembersGenerator<INotifyPropertyChangedAttribute>
    {
        /// <inheritdoc/>
        protected override IEnumerable<MemberDeclarationSyntax> FilterDeclaredMembers(
            AttributeData attributeData,
            ClassDeclarationSyntax classDeclaration,
            INamedTypeSymbol classDeclarationSymbol,
            ClassDeclarationSyntax sourceDeclaration)
        {
            foreach (KeyValuePair<string, TypedConstant> properties in attributeData.NamedArguments)
            {
                if (properties.Key == nameof(INotifyPropertyChangedAttribute.IncludeAdditionalHelperMethods) &&
                    properties.Value.Value is bool includeHelpers && !includeHelpers)
                {
                    // If requested, only include the event and the basic methods to raise it, but not the additional helpers
                    return sourceDeclaration.Members.Where(static member =>
                    {
                        return member
                            is EventFieldDeclarationSyntax
                            or MethodDeclarationSyntax { Identifier: { ValueText: "OnPropertyChanged" } };
                    });
                }
            }

            return sourceDeclaration.Members;
        }
    }
}
