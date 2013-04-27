namespace Docu.Documentation.Generators
{
    using System.Collections.Generic;

    using Parsing.Comments;
    using Parsing.Model;

    internal class TypeGenerator : BaseGenerator
    {
        readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        public TypeGenerator(IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser)
            : base(commentParser)
        {
            this.matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedType association)
        {
            Namespace ns = FindNamespace(association, namespaces);
            DeclaredType doc = DeclaredType.Unresolved((TypeIdentifier) association.Name, association.TargetType, ns);

            ParseSummary(association, doc);
            ParseRemarks(association, doc);
            ParseValue(association, doc);
            ParseExample(association, doc);

            if (matchedAssociations.ContainsKey(association.Name))
            {
                return;
            }

            matchedAssociations.Add(association.Name, doc);
            ns.AddType(doc);
        }
    }
}
