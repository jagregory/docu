using System;
using System.Collections.Generic;

namespace Docu.Documentation
{
    public class AssemblyDoc
    {
        private readonly List<Namespace> namespaces = new List<Namespace>();

        public AssemblyDoc(string name)
        {
            Name = name;
            namespaces = new List<Namespace>();
        }

        public IList<Namespace> Namespaces
        {
            get { return namespaces; }
        }

        public string Name { get; set; }

        public void Sort()
        {
            namespaces.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach (var ns in Namespaces)
            {
                ns.Sort();
            }
        }
    }
}