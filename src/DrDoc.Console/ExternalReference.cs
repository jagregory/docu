using DrDoc.Model;

namespace DrDoc
{
    public class ExternalReference : IReferencable
    {
        public ExternalReference(Identifier name)
        {
            Name = name;
            FullName = Name.ToString();

            if (Name is TypeIdentifier)
                FullName = Name.CloneAsNamespace() + "." + Name;
        }

        public Identifier Name { get; private set; }
        public string FullName { get; private set; }
    }
}