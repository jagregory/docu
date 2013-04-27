namespace Docu.Documentation.Generators
{
    using System.Collections.Generic;
    using Parsing.Comments;
    using Parsing.Model;

    internal class FieldGenerator : BaseGenerator
    {
        readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        public FieldGenerator(IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser)
            : base(commentParser)
        {
            this.matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedField association)
        {
            if (association.Field == null)
            {
                return;
            }

            Namespace ns = FindNamespace(association, namespaces);
            DeclaredType type = FindType(ns, association);

            DeclaredType returnType = DeclaredType.Unresolved(
                Identifier.FromType(association.Field.FieldType),
                association.Field.FieldType,
                Namespace.Unresolved(Identifier.FromNamespace(association.Field.FieldType.Namespace)));
            Field doc = Field.Unresolved(
                Identifier.FromField(association.Field, association.TargetType), type, returnType);

            ParseSummary(association, doc);
            ParseRemarks(association, doc);
            ParseExample(association, doc);

            matchedAssociations.Add(association.Name, doc);
            type.AddField(doc);
        }
    }
}
