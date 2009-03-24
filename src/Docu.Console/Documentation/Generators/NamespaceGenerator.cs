using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation.Generators
{
    internal class NamespaceGenerator
    {
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        public NamespaceGenerator(IDictionary<Identifier, IReferencable> matchedAssociations)
        {
            this.matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, IDocumentationMember association)
        {
            var ns = Identifier.FromNamespace(association.TargetType.Namespace);

            if (!namespaces.Exists(x => x.IsIdentifiedBy(ns)))
            {
                var doc = Namespace.Unresolved(ns);
                matchedAssociations.Add(association.Name.CloneAsNamespace(), doc);
                namespaces.Add(doc);
            }
        }
    }
}