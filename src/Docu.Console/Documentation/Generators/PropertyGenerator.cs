namespace Docu.Documentation
{
    using System.Collections.Generic;

    using Generators;
    using Parsing.Comments;
    using Parsing.Model;

    internal class PropertyGenerator : BaseGenerator
    {
        readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        public PropertyGenerator(
            IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser)
            : base(commentParser)
        {
            this.matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedProperty association)
        {
            if (association.Property == null)
            {
                return;
            }

            Namespace ns = FindNamespace(association, namespaces);
            DeclaredType type = FindType(ns, association);

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

            if (matchedAssociations.ContainsKey(association.Name))
            {
                return;
            }

            matchedAssociations.Add(association.Name, doc);
            if (type == null)
            {
                return;
            }

            type.AddProperty(doc);
        }
    }
}
