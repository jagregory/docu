using System;
using System.Collections.Generic;
using System.Reflection;
using Docu.Documentation;

namespace Docu.Generation
{
    public class ViewData
    {
        public IList<Assembly> Assemblies { get; set; }
        public IList<Namespace> Namespaces { get; set; }
        public IList<DeclaredType> Types { get; set; }
        public Namespace Namespace { get; set; }
        public DeclaredType Type { get; set; }

        public ViewData()
        {
            Assemblies = new List<Assembly>();
            Namespaces = new List<Namespace>();
            Types = new List<DeclaredType>();
        }

        public ViewData Clone()
        {
            var clone = new ViewData();

            clone.Assemblies = new List<Assembly>(Assemblies);
            clone.Namespaces = new List<Namespace>(Namespaces);
            clone.Types = new List<DeclaredType>(Types);
            clone.Namespace = Namespace;
            clone.Type = Type;

            return clone;
        }
    }
}