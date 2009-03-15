using System.Collections.Generic;
using DrDoc.Documentation;

namespace DrDoc.Generation
{
    public class OutputData
    {
        public IList<Namespace> Namespaces { get; set; }
        public Namespace Namespace { get; set; }
        public DeclaredType Type { get; set; }
    }
}