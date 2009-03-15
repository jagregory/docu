using DrDoc.Documentation;
using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public class ExternalReference : BaseDocumentationElement, IReferencable
    {
        public ExternalReference(Identifier name)
            : base(name)
        {
            FullName = Name.ToString();

            if (name is TypeIdentifier)
                FullName = name.CloneAsNamespace() + "." + Name;
        }

        public string FullName { get; private set; }

        public IReferencable ToExternalReference()
        {
            return new ExternalReference(identifier);
        }
    }
}