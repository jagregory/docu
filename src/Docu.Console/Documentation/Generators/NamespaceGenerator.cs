using System;
using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation.Generators
{
    internal class NamespaceGenerator : IGenerator<IDocumentationMember>
    {
        readonly IDictionary<Identifier, IReferencable> _matchedAssociations;

        public NamespaceGenerator(IDictionary<Identifier, IReferencable> matchedAssociations)
        {
            _matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, IDocumentationMember association)
        {
            if (association.TargetType.Namespace == null)
            {
                throw new NullReferenceException(
                    string.Format("There was no namespace found for {0}", association.TargetType.AssemblyQualifiedName));
            }

            NamespaceIdentifier ns = IdentifierFor.Namespace(association.TargetType.Namespace);

            if (!namespaces.Exists(x => x.IsIdentifiedBy(ns)))
            {
                Namespace doc = Namespace.Unresolved(ns);
                _matchedAssociations.Add(association.Name.CloneAsNamespace(), doc);
                namespaces.Add(doc);
            }
        }
    }
}
