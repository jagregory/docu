namespace Docu.Documentation.Generators
{
    using System;
    using System.Collections.Generic;

    using Docu.Parsing.Model;

    internal class NamespaceGenerator
    {
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamespaceGenerator"/> class.
        /// </summary>
        /// <param name="matchedAssociations">
        /// The matched associations.
        /// </param>
        public NamespaceGenerator(IDictionary<Identifier, IReferencable> matchedAssociations)
        {
            this.matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, IDocumentationMember association)
        {
            if (association.TargetType.Namespace == null)
            {
                throw new NullReferenceException(
                    string.Format("There was no namespace found for {0}", association.TargetType.AssemblyQualifiedName));
            }

            NamespaceIdentifier ns = Identifier.FromNamespace(association.TargetType.Namespace);

            if (!namespaces.Exists(x => x.IsIdentifiedBy(ns)))
            {
                Namespace doc = Namespace.Unresolved(ns);
                this.matchedAssociations.Add(association.Name.CloneAsNamespace(), doc);
                namespaces.Add(doc);
            }
        }
    }
}