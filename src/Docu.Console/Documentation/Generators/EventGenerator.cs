namespace Docu.Documentation.Generators
{
    using System.Collections.Generic;

    using Docu.Parsing.Comments;
    using Docu.Parsing.Model;

    internal class EventGenerator : BaseGenerator
    {
        private readonly IDictionary<Identifier, IReferencable> matchedAssociations;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventGenerator"/> class.
        /// </summary>
        /// <param name="matchedAssociations">
        /// The matched associations.
        /// </param>
        /// <param name="commentParser">
        /// The comment parser.
        /// </param>
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

            Namespace ns = this.FindNamespace(association, namespaces);
            DeclaredType type = this.FindType(ns, association);

            Event doc = Event.Unresolved(Identifier.FromEvent(association.Event, association.TargetType), type);

            this.ParseSummary(association, doc);
            this.ParseRemarks(association, doc);
            this.ParseExample(association, doc);

            this.matchedAssociations.Add(association.Name, doc);
            type.AddEvent(doc);
        }
    }
}