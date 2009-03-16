using System;
using System.Collections.Generic;
using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public interface IReferencable
    {
        string Name { get; }
        string FullName { get; }
        string PrettyName { get; }
        bool IsResolved { get; }
        bool IsExternal { get; }
        bool IsIdentifiedBy(Identifier otherIdentifier);
        void ConvertToExternalReference();
        void Resolve(IDictionary<Identifier, IReferencable> referencables);
    }
}