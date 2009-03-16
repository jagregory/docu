using System;
using System.Collections.Generic;
using System.Diagnostics;
using DrDoc.Documentation;
using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public class Namespace : BaseDocumentationElement, IReferencable
    {
        private readonly List<DeclaredType> types = new List<DeclaredType>();

        public Namespace(Identifier identifier)
            : base(identifier)
        {}

        public IList<DeclaredType> Types
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

        public void AddType(DeclaredType type)
        {
            types.Add(type);
        }

        public string PrettyName
        {
            get { return Name; }
        }

        public IReferencable ToExternalReference()
        {
            throw new InvalidOperationException();
        }
    }
}