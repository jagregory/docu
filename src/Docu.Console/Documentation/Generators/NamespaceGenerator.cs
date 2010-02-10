using System;
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
            if (association.TargetType.Namespace == null)
                throw new NullReferenceException(
                    string.Format("There was no namespace found for {0}",
                                  association.TargetType.AssemblyQualifiedName));

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