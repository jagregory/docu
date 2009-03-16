using System;
using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public interface IReferencable
    {
        string Name { get; }
        string PrettyName { get; }
        bool IsIdentifiedBy(Identifier otherIdentifier);
        IReferencable ToExternalReference();
    }
}