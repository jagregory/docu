using System;
using System.Collections.Generic;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class Namespace : BaseDocumentationElement, IReferencable
    {
        private readonly List<DeclaredType> types = new List<DeclaredType>();

        public Namespace(Identifier identifier)
            : base(identifier)
        {
        }

        public IList<DeclaredType> Types
        {
            get { return types; }
        }

        public string FullName
        {
            get { return Name; }
        }

        public string PrettyName
        {
            get { return Name; }
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            throw new NotImplementedException();
        }

        public void Sort()
        {
            types.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach (DeclaredType type in types)
            {
                type.Sort();
            }
        }

        public void AddType(DeclaredType type)
        {
            types.Add(type);
        }

        public override string ToString()
        {
            return base.ToString() + " { Name = '" + Name + "' }";
        }

        public static Namespace Unresolved(NamespaceIdentifier namespaceIdentifier)
        {
            return new Namespace(namespaceIdentifier) { IsResolved = false };
        }
    }
}