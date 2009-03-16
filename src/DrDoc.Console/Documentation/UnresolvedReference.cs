using System;
using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public class UnresolvedReference : BaseDocumentationElement, IReferencable
    {
        private readonly Type representedType;

        public UnresolvedReference(Identifier identifier, Type representedType)
            : base(identifier)
        {
            this.representedType = representedType;
        }

        public UnresolvedReference(Identifier identifier)
            : base(identifier)
        {}

        public string PrettyName
        {
            get { return Name; }
        }

        public IReferencable ToExternalReference()
        {
            return new ExternalReference(identifier, representedType);
        }
    }
}