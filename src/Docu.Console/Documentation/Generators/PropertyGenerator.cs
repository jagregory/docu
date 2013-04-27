namespace Docu.Documentation
{
    using System.Collections.Generic;

    using Docu.Documentation.Generators;
    using Docu.Parsing.Comments;
    using Docu.Parsing.Model;

    internal class PropertyGenerator : BaseGenerator
    {
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyGenerator"/> class.
        /// </summary>
        /// <param name="matchedAssociations">
        /// The matched associations.
        /// </param>
        /// <param name="commentParser">
        /// The comment parser.
        /// </param>
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

            Namespace ns = this.FindNamespace(association, namespaces);
            DeclaredType type = this.FindType(ns, association);

            DeclaredType propertyReturnType =
                DeclaredType.Unresolved(
                    Identifier.FromType(association.Property.PropertyType), 
                    association.Property.PropertyType, 
                    Namespace.Unresolved(Identifier.FromNamespace(association.Property.PropertyType.Namespace)));
            Property doc = Property.Unresolved(
                Identifier.FromProperty(association.Property, association.TargetType), type, propertyReturnType);

            this.ParseSummary(association, doc);
            this.ParseValue(association, doc);
            this.ParseRemarks(association, doc);
            this.ParseExample(association, doc);

            if (this.matchedAssociations.ContainsKey(association.Name))
            {
                return;
            }

            this.matchedAssociations.Add(association.Name, doc);
            if (type == null)
            {
                return;
            }

            type.AddProperty(doc);
        }
    }
}