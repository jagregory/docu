using System;
using System.Collections.Generic;
using System.Linq;
using Docu.Documentation.Generators;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    internal class PropertyGenerator : BaseGenerator
    {
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        public PropertyGenerator(IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser)
            : base(commentParser)
        {
            this.matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedProperty association)
        {
            if (association.Property == null) return;

            var ns = FindNamespace(association, namespaces);
            var type = FindType(ns, association);

            var customAttributes = association.Property.GetCustomAttributes(false).ToList();
            var attributes = customAttributes.Cast<Attribute>().ToList();

            var propertyReturnType =
                DeclaredType.Unresolved(Identifier.FromType(association.Property.PropertyType),
                                        association.Property.PropertyType,
                                        Namespace.Unresolved(
                                            Identifier.FromNamespace(association.Property.PropertyType.Namespace)));
            var doc = Property.Unresolved(Identifier.FromProperty(association.Property, association.TargetType), type, propertyReturnType, attributes);

            ParseSummary(association, doc);
            ParseValue(association, doc);
            ParseRemarks(association, doc);
            ParseExample(association, doc);

            if (matchedAssociations.ContainsKey(association.Name))
                return;

            matchedAssociations.Add(association.Name, doc);
            if (type == null) return;
            type.AddProperty(doc);
        }
    }
}