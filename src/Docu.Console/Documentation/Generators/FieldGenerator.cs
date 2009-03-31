using System.Collections.Generic;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation.Generators
{
    internal class FieldGenerator : BaseGenerator
    {
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        public FieldGenerator(IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser) : base(commentParser)
        {
            this.matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedField association)
        {
            if (association.Field == null) return;

            var ns = FindNamespace(association, namespaces);
            var type = FindType(ns, association);

            var returnType = DeclaredType.Unresolved(Identifier.FromType(association.Field.FieldType),
                                                     association.Field.FieldType,
                                                     Namespace.Unresolved(
                                                         Identifier.FromNamespace(association.Field.FieldType.Namespace)));
            var doc = Field.Unresolved(Identifier.FromField(association.Field, association.TargetType), type, returnType);

            ParseSummary(association, doc);
            ParseRemarks(association, doc);

            matchedAssociations.Add(association.Name, doc);
            type.AddField(doc);
        }
    }
}