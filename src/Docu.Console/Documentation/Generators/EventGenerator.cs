using System.Collections.Generic;
using Docu.Parsing.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation.Generators
{
    internal class EventGenerator : BaseGenerator, IGenerator<DocumentedEvent>
    {
        readonly IDictionary<Identifier, IReferencable> _matchedAssociations;

        public EventGenerator(IDictionary<Identifier, IReferencable> matchedAssociations, ICommentParser commentParser)
            : base(commentParser)
        {
            _matchedAssociations = matchedAssociations;
        }

        public void Add(List<Namespace> namespaces, DocumentedEvent association)
        {
            if (association.Event == null)
            {
                return;
            }

            DeclaredType type = FindType(association, namespaces);

            Event doc = Event.Unresolved(IdentifierFor.Event(association.Event, association.TargetType), type);

            ParseSummary(association, doc);
            ParseRemarks(association, doc);
            ParseExample(association, doc);

            _matchedAssociations.Add(association.Name, doc);
            type.AddEvent(doc);
        }
    }
}
