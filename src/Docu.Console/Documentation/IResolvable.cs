using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public interface IResolvable
    {
        bool IsResolved { get; }
        void Resolve(IDictionary<Identifier, IReferencable> referencables);
    }
}
