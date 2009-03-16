using System.Collections.Generic;
using Docu.Documentation;

namespace Docu.Generation
{
    public class OutputData
    {
        public IList<Namespace> Namespaces { get; set; }
        public Namespace Namespace { get; set; }
        public DeclaredType Type { get; set; }
    }
}