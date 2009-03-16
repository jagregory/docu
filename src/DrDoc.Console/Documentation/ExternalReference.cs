using System;
using DrDoc.Documentation;
using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public class ExternalReference : BaseDocumentationElement, IReferencable
    {
        private readonly Type representedType;

        public ExternalReference(Identifier name, Type representedType)
            : base(name)
        {
            this.representedType = representedType;
            FullName = Name;

            if (name is TypeIdentifier)
                FullName = name.CloneAsNamespace() + "." + Name;
        }

        public string FullName { get; private set; }

        public string PrettyName
        {
            get { return representedType == null ? Name : representedType.GetPrettyName(); }
        }

        public IReferencable ToExternalReference()
        {
            return new ExternalReference(identifier, representedType);
        }
    }
}