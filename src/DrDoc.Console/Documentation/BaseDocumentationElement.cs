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
            IsResolved = true;
        }

        public string Name { get; private set; }
        public bool IsExternal { get; private set; }
        public bool IsResolved { get; protected set; }

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