using System.Collections.Generic;

namespace Docu.Documentation.Generators
{
    public interface IGenerator<in T>
    {
        void Add(List<Namespace> namespaces, T association);
    }
}
