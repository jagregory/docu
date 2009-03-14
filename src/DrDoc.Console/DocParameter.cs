using System.Collections.Generic;

namespace DrDoc
{
    public class DocParameter
    {
        public DocParameter(string name, string type)
        {
            Name = name;
            Type = type;
            Summary = new List<DocBlock>();
        }

        public string Name { get; private set; }
        public string Type { get; private set; }
        public IList<DocBlock> Summary { get; internal set; }
    }
}