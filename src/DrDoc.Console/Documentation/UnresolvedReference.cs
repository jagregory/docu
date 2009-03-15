using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public class UnresolvedReference : BaseDocumentationElement, IReferencable
    {
        public UnresolvedReference(Identifier name)
            : base(name)
        {}

        public IReferencable ToExternalReference()
        {
            return new ExternalReference(identifier);
        }
    }
}