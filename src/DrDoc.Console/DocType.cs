using System.Collections.Generic;
using DrDoc.Associations;

namespace DrDoc
{
    public class DocType : IReferencable
    {
        private readonly List<DocMethod> methods = new List<DocMethod>();
        private readonly List<DocProperty> properties = new List<DocProperty>();

        public DocType(MemberName name, string prettyName, DocNamespace ns)
            : this(name)
        {
            PrettyName = prettyName;
            Namespace = ns;
        }

        public DocType(MemberName name)
        {
            Name = name;
            Summary = new List<DocBlock>();
        }

        internal void AddMethod(DocMethod method)
        {
            methods.Add(method);
        }

        internal void AddProperty(DocProperty property)
        {
            properties.Add(property);
        }

        public MemberName Name { get; private set; }
        public DocNamespace Namespace { get; private set; }
        public string PrettyName { get; private set; }

        public IList<DocMethod> Methods
        {
            get { return methods; }
        }
        
        public IList<DocProperty> Properties
        {
            get { return properties; }
        }
        
        public IList<DocBlock> Summary { get; internal set; }

        public void Sort()
        {
            methods.Sort((x, y) => x.Name.CompareTo(y.Name));
            properties.Sort((x, y) => x.Name.CompareTo(y.Name));
        }
    }
}