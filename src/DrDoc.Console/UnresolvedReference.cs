using DrDoc.Associations;

namespace DrDoc
{
    public class UnresolvedReference : IReferencable
    {
        public UnresolvedReference(MemberName name)
        {
            Name = name;
        }

        public MemberName Name { get; private set; }
    }
}