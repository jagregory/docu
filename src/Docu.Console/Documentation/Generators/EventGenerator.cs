using System.Collections.Generic;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation.Generators
{
    internal class EventGenerator : BaseGenerator
    {
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        public EventGenerator(IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser)
            : base(commentParser)
        {
            this.matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedEvent association)
        {
            if (association.Event == null) return;

            var ns = FindNamespace(association, namespaces);
            var type = FindType(ns, association);

            var doc = Event.Unresolved(Identifier.FromEvent(association.Event, association.TargetType), type);

            ParseSummary(association, doc);
            ParseRemarks(association, doc);

            matchedAssociations.Add(association.Name, doc);
            type.AddEvent(doc);
        }
    }
}