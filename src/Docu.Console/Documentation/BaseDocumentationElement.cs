using System.Collections.Generic;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public abstract class BaseDocumentationElement : IDocumentationElement
    {
        protected readonly Identifier identifier;

        protected BaseDocumentationElement(Identifier identifier)
        {
            Name = identifier.ToString();
            this.identifier = identifier;
            IsResolved = true;
            Summary = new List<IComment>();
            Remarks = new List<IComment>();
            Value = new List<IComment>();
        }

        public string Name { get; private set; }
        public bool IsExternal { get; private set; }
        public bool IsResolved { get; protected set; }
        public IList<IComment> Summary { get; set; }
        public IList<IComment> Remarks { get; set; }
        public IList<IComment> Value { get; set; }

        public bool IsIdentifiedBy(Identifier otherIdentifier)
        {
            return identifier == otherIdentifier;
        }

        public virtual void ConvertToExternalReference()
        {
            IsExternal = true;
        }
    }
}