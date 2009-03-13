using System.Collections.Generic;

namespace DrDoc
{
    public class DocType : IReferencable
    {
        private readonly IList<DocMethod> methods = new List<DocMethod>();
        private readonly IList<DocProperty> properties = new List<DocProperty>();

        public DocType(string name)
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

        public string Name { get; private set; }
        public IList<DocMethod> Methods
        {
            get { return methods; }
        }
        public IList<DocProperty> Properties
        {
            get { return properties; }
        }
        
        public IList<DocBlock> Summary { get; internal set; }
    }
}