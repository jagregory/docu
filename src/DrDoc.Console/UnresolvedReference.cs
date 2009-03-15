using DrDoc.Model;

namespace DrDoc
{
    public class UnresolvedReference : IReferencable
    {
        public UnresolvedReference(Identifier name)
        {
            Name = name;
        }

        public Identifier Name { get; private set; }
    }
}