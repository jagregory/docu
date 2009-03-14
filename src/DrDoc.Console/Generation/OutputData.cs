using System.Collections.Generic;

namespace DrDoc.Generation
{
    public class OutputData
    {
        public IList<DocNamespace> Namespaces { get; set; }
        public DocNamespace Namespace { get; set; }
        public DocType Type { get; set; }
    }
}