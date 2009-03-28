using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public interface IResolvable
    {
        bool IsResolved { get; }
        void Resolve(IDictionary<Identifier, IReferencable> referencables);
    }

    public interface IReferencable : IResolvable
    {
        string Name { get; }
        string FullName { get; }
        string PrettyName { get; }
        bool IsExternal { get; }
        bool IsIdentifiedBy(Identifier otherIdentifier);
        void ConvertToExternalReference();
    }
}