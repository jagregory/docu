using System.Collections.Generic;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation.Generators
{
    internal class PropertyGenerator : BaseGenerator, IGenerator<DocumentedProperty>
    {
        readonly IDictionary<Identifier, IReferencable> _matchedAssociations;

        public PropertyGenerator(
            IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser)
            : base(commentParser)
        {
            _matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedProperty association)
        {
            if (association.Property == null)
            {
                return;
            }

            DeclaredType type = FindType(association, namespaces);

            DeclaredType propertyReturnType =
                DeclaredType.Unresolved(
                    Identifier.FromType(association.Property.PropertyType),
                    association.Property.PropertyType,
                    Namespace.Unresolved(Identifier.FromNamespace(association.Property.PropertyType.Namespace)));
            Property doc = Property.Unresolved(
                Identifier.FromProperty(association.Property, association.TargetType), type, propertyReturnType);

            ParseSummary(association, doc);
            ParseValue(association, doc);
            ParseRemarks(association, doc);
            ParseExample(association, doc);

            if (_matchedAssociations.ContainsKey(association.Name))
            {
                return;
            }

            _matchedAssociations.Add(association.Name, doc);
            if (type == null)
            {
                return;
            }

            type.AddProperty(doc);
        }
    }
}
