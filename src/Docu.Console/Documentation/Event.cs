using System;
using System.Collections.Generic;
using System.Linq;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class Event : BaseDocumentationElement, IReferencable
    {
        public DeclaredType Type { get; set; }

        public Event(EventIdentifier identifier, DeclaredType type)
            : base(identifier)
        {
            Type = type;
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

                if (!Summary.IsResolved)
                    Summary.Resolve(referencables);

                if (!Remarks.IsResolved)
                    Remarks.Resolve(referencables);
            }
            else
                ConvertToExternalReference();
        }

        public static Event Unresolved(EventIdentifier eventIdentifier, DeclaredType type)
        {
            return new Event(eventIdentifier, type) { IsResolved = false };
        }
    }
}