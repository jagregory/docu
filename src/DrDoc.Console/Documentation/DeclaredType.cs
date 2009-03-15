using System.Collections.Generic;
using DrDoc.Documentation.Comments;
using DrDoc.Documentation;
using DrDoc.Documentation.Comments;
using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public class DeclaredType : IReferencable
    {
        private readonly List<Method> methods = new List<Method>();
        private readonly List<Property> properties = new List<Property>();

        public DeclaredType(Identifier name, string prettyName, Namespace ns)
            : this(name)
        {
            PrettyName = prettyName;
            Namespace = ns;
        }

        public DeclaredType(Identifier name)
        {
            Name = name;
            Summary = new List<IComment>();
        }

        internal void AddMethod(Method method)
        {
            methods.Add(method);
        }

        internal void AddProperty(Property property)
        {
            properties.Add(property);
        }

        public Identifier Name { get; private set; }
        public Namespace Namespace { get; private set; }
        public string PrettyName { get; private set; }

        public IList<Method> Methods
        {
            get { return methods; }
        }
        
        public IList<Property> Properties
        {
            get { return properties; }
        }
        
        public IList<IComment> Summary { get; internal set; }

        public void Sort()
        {
            methods.Sort((x, y) => x.Name.CompareTo(y.Name));
            properties.Sort((x, y) => x.Name.CompareTo(y.Name));
        }
    }
}