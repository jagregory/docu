using System.Collections.Generic;

namespace DrDoc
{
    public class DocNamespace : IReferencable
    {
        private readonly List<DocType> types = new List<DocType>();

        public DocNamespace(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public IList<DocType> Types
        {
            get { return types; }
        }

        public void Sort()
        {
            types.Sort((x, y) => x.Name.CompareTo(y.Name));
        }

        public void AddType(DocType type)
        {
            types.Add(type);
        }
    }
}