namespace Docu.Documentation
{
    using System;
    using System.Collections.Generic;

    using Docu.Parsing.Model;

    public class Event : BaseDocumentationElement, IReferencable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        public Event(EventIdentifier identifier, DeclaredType type)
            : base(identifier)
        {
            this.Type = type;
        }

        public string FullName
        {
            get
            {
                return this.Name;
            }
        }

        public string PrettyName
        {
            get
            {
                return this.Name;
            }
        }

        public DeclaredType Type { get; set; }

        public static Event Unresolved(EventIdentifier eventIdentifier, DeclaredType type)
        {
            return new Event(eventIdentifier, type) { IsResolved = false };
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            if (referencables.ContainsKey(this.identifier))
            {
                this.IsResolved = true;
                IReferencable referencable = referencables[this.identifier];
                var ev = referencable as Event;

                if (ev == null)
                {
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");
                }

                if (!this.Summary.IsResolved)
                {
                    this.Summary.Resolve(referencables);
                }

                if (!this.Remarks.IsResolved)
                {
                    this.Remarks.Resolve(referencables);
                }
            }
            else
            {
                this.ConvertToExternalReference();
            }
        }
    }
}