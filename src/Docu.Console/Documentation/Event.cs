using System;
using System.Collections.Generic;
using System.Linq;
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
                IsResolved = true;
                var referencable = referencables[identifier];
                var ev = referencable as Event;

                if (ev == null)
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");

                foreach (IReferrer comment in Summary.Where(x => x is IReferrer))
                {
                    if (!comment.Reference.IsResolved)
                        comment.Reference.Resolve(referencables);
                }
            }

            ConvertToExternalReference();
        }

        public static Event Unresolved(EventIdentifier eventIdentifier)
        {
            return new Event(eventIdentifier) { IsResolved = false };
        }
    }
}