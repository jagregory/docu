using System.Collections.Generic;
using System.Diagnostics;
using DrDoc.Associations;

namespace DrDoc
{
    public class DocNamespace : IReferencable
    {
        private readonly List<DocType> types = new List<DocType>();

        public DocNamespace(MemberName name)
        {
            Name = name;
        }

        public MemberName Name { get; private set; }

        public IList<DocType> Types
        {
            get { return types; }
        }

        public void Sort()
        {
            types.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach (var type in types)
            {
                type.Sort();
            }
        }

        public void AddType(DocType type)
        {
            types.Add(type);
        }
    }
}