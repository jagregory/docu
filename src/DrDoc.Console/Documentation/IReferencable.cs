using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public interface IReferencable
    {
        Identifier Name { get; }
    }
}