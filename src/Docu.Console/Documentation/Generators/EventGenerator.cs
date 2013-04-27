namespace Docu.Documentation.Generators
{
    using System.Collections.Generic;
    using Parsing.Comments;
    using Parsing.Model;

    internal class EventGenerator : BaseGenerator
    {
        readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        public EventGenerator(IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser)
            : base(commentParser)
        {
            this.matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedEvent association)
        {
            if (association.Event == null)
            {
                return;
            }

            Namespace ns = FindNamespace(association, namespaces);
            DeclaredType type = FindType(ns, association);

            Event doc = Event.Unresolved(Identifier.FromEvent(association.Event, association.TargetType), type);

            ParseSummary(association, doc);
            ParseRemarks(association, doc);
            ParseExample(association, doc);

            matchedAssociations.Add(association.Name, doc);
            type.AddEvent(doc);
        }
    }
}
