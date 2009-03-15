using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public abstract class BaseDocumentationElement
    {
        protected readonly Identifier identifier;

        protected BaseDocumentationElement(Identifier name)
        {
            Name = name.ToString();
            identifier = name;
        }

        public string Name { get; private set; }

        public bool IsIdentifiedBy(Identifier otherIdentifier)
        {
            return identifier == otherIdentifier;
        }
    }
}