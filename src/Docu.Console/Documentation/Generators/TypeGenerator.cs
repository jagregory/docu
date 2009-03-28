using System.Collections.Generic;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation.Generators
{
    internal class TypeGenerator : BaseGenerator
    {
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        public TypeGenerator(IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser)
            : base(commentParser)
        {
            this.matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedType association)
        {
            var ns = FindNamespace(association, namespaces);
            DeclaredType doc = DeclaredType.Unresolved((TypeIdentifier)association.Name, association.TargetType, ns);

            ParseSummary(association, doc);
            ParseRemarks(association, doc);
            ParseValue(association, doc);

            if (matchedAssociations.ContainsKey(association.Name))
                return; // weird case when a type has the same method declared twice

            matchedAssociations.Add(association.Name, doc);
            ns.AddType(doc);
        }

        public void PrePopulate(List<Namespace> namespaces, IDocumentationMember association)
        {
            if (association is DocumentedType) return;

            var ns = FindNamespace(association, namespaces);
            var type = FindType(ns, association);

            if (type == null)
                Add(namespaces, new DocumentedType(association.Name.CloneAsType(), null, association.TargetType));
        }
    }
}