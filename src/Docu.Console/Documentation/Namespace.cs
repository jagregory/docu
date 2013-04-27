using System.Collections.Generic;
using System.Linq;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class Namespace : BaseDocumentationElement, IReferencable
    {
        readonly List<DeclaredType> types = new List<DeclaredType>();

        public Namespace(Identifier identifier)
            : base(identifier)
        {
        }

        public IEnumerable<DeclaredType> Classes
        {
            get { return Types.Where(x => !x.IsInterface); }
        }

        public bool HasClasses
        {
            get { return Classes.Any(); }
        }

        public bool HasInterfaces
        {
            get { return Interfaces.Any(); }
        }

        public bool HasTypes
        {
            get { return Types.Count > 0; }
        }

        public IEnumerable<DeclaredType> Interfaces
        {
            get { return Types.Where(x => x.IsInterface); }
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
            if (referencables.ContainsKey(identifier))
            {
                IsResolved = true;

                foreach (DeclaredType type in Types)
                {
                    if (!type.IsResolved)
                    {
                        type.Resolve(referencables);
                    }
                }
            }
            else
            {
                ConvertToExternalReference();
            }
        }

        public static Namespace Unresolved(NamespaceIdentifier namespaceIdentifier)
        {
            return new Namespace(namespaceIdentifier) {IsResolved = false};
        }

        public void AddType(DeclaredType type)
        {
            types.Add(type);
        }

        public void Sort()
        {
            types.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach (DeclaredType type in types)
            {
                type.Sort();
            }
        }

        public override string ToString()
        {
            return base.ToString() + " { Name = '" + Name + "' }";
        }
    }
}
