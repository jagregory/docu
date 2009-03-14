using System.Collections.Generic;

namespace DrDoc
{
    public class DocParameter : IReferrer
    {
        public DocParameter(string name, IReferencable reference)
        {
            Name = name;
            Reference = reference;
            Summary = new List<DocBlock>();
        }

        public string Name { get; private set; }
        public IReferencable Reference { get; set; }
        public IList<DocBlock> Summary { get; internal set; }
    }
}