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
            string namespace2 = "NoNamespace";
            if (association.TargetType.Namespace == null)
            {
                //TODO this may be better as a build warning
                System.Console.WriteLine(string.Format("There was no namespace found for {0}",
                              association.TargetType.AssemblyQualifiedName));
            }
            else
            {
                namespace2 = association.TargetType.Namespace;
            }

            var ns = Identifier.FromNamespace(namespace2);

            if (!namespaces.Exists(x => x.IsIdentifiedBy(ns)))
            {
                var doc = Namespace.Unresolved(ns);
                matchedAssociations.Add(association.Name.CloneAsNamespace(), doc);
                namespaces.Add(doc);
            }
        }
    }
}