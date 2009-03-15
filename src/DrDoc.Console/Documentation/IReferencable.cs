using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public interface IReferencable
    {
        string Name { get; }
        bool IsIdentifiedBy(Identifier otherIdentifier);
        IReferencable ToExternalReference();
    }
}