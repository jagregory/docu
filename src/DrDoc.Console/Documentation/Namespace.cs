using System.Collections.Generic;
using System.Diagnostics;
using DrDoc.Documentation;
using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public class Namespace : BaseDocumentationElement, IReferencable
    {
        private readonly List<DeclaredType> types = new List<DeclaredType>();

        public Namespace(Identifier name)
            : base(name)
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

        public IReferencable ToExternalReference()
        {
            return new ExternalReference(identifier);
        }
    }
}