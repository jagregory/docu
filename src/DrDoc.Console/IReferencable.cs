using DrDoc.Associations;

namespace DrDoc
{
    public interface IReferencable
    {
        MemberName Name { get; }
    }
}