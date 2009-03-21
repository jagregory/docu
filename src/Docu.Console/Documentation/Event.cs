using System;
using System.Collections.Generic;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class Event : BaseDocumentationElement, IReferencable
    {
        public Event(EventIdentifier identifier)
            : base(identifier)
        {
            Summary = new List<IComment>();
        }

        public string FullName
        {
            get { return Name; }
        }
        public string PrettyName
        {
            get { return Name; }
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            if (referencables.ContainsKey(identifier))
            {
                var referencable = referencables[identifier];
                var ev = referencable as Event;

                if (ev == null)
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");

                IsResolved = true;
            }

            ConvertToExternalReference();
        }

        public static Event Unresolved(EventIdentifier eventIdentifier)
        {
            return new Event(eventIdentifier) { IsResolved = false };
        }
    }
}