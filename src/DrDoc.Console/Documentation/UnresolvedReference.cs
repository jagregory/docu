using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
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