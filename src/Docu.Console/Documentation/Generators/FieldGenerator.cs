namespace Docu.Documentation.Generators
{
    using System.Collections.Generic;

    using Docu.Parsing.Comments;
    using Docu.Parsing.Model;

    internal class FieldGenerator : BaseGenerator
    {
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldGenerator"/> class.
        /// </summary>
        /// <param name="matchedAssociations">
        /// The matched associations.
        /// </param>
        /// <param name="commentParser">
        /// The comment parser.
        /// </param>
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

            Namespace ns = this.FindNamespace(association, namespaces);
            DeclaredType type = this.FindType(ns, association);

            DeclaredType returnType = DeclaredType.Unresolved(
                Identifier.FromType(association.Field.FieldType), 
                association.Field.FieldType, 
                Namespace.Unresolved(Identifier.FromNamespace(association.Field.FieldType.Namespace)));
            Field doc = Field.Unresolved(
                Identifier.FromField(association.Field, association.TargetType), type, returnType);

            this.ParseSummary(association, doc);
            this.ParseRemarks(association, doc);
            this.ParseExample(association, doc);

            this.matchedAssociations.Add(association.Name, doc);
            type.AddField(doc);
        }
    }
}