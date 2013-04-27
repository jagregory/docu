using System.Collections.Generic;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation.Generators
{
    internal class FieldGenerator : BaseGenerator, IGenerator<DocumentedField>
    {
        readonly IDictionary<Identifier, IReferencable> _matchedAssociations;

        public FieldGenerator(IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser)
            : base(commentParser)
        {
            _matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedField association)
        {
            if (association.Field == null)
            {
                return;
            }

            DeclaredType type = FindType(association, namespaces);

            DeclaredType returnType = DeclaredType.Unresolved(
                Identifier.FromType(association.Field.FieldType),
                association.Field.FieldType,
                Namespace.Unresolved(Identifier.FromNamespace(association.Field.FieldType.Namespace)));
            Field doc = Field.Unresolved(
                Identifier.FromField(association.Field, association.TargetType), type, returnType);

            ParseSummary(association, doc);
            ParseRemarks(association, doc);
            ParseExample(association, doc);

            _matchedAssociations.Add(association.Name, doc);
            type.AddField(doc);
        }
    }
}
