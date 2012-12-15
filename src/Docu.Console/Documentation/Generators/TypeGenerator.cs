namespace Docu.Documentation.Generators
{
    using System.Collections.Generic;

    using Docu.Parsing.Comments;
    using Docu.Parsing.Model;

    internal class TypeGenerator : BaseGenerator
    {
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeGenerator"/> class.
        /// </summary>
        /// <param name="matchedAssociations">
        /// The matched associations.
        /// </param>
        /// <param name="commentParser">
        /// The comment parser.
        /// </param>
        public TypeGenerator(IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser)
            : base(commentParser)
        {
            this.matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedType association)
        {
            Namespace ns = this.FindNamespace(association, namespaces);
            DeclaredType doc = DeclaredType.Unresolved((TypeIdentifier)association.Name, association.TargetType, ns);

            this.ParseSummary(association, doc);
            this.ParseRemarks(association, doc);
            this.ParseValue(association, doc);
            this.ParseExample(association, doc);

            if (this.matchedAssociations.ContainsKey(association.Name))
            {
                return;
            }

            this.matchedAssociations.Add(association.Name, doc);
            ns.AddType(doc);
        }
    }
}