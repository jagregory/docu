namespace Docu.Documentation
{
    using System.Collections.Generic;
    using System.Linq;

    using Docu.Parsing.Model;

    public class Namespace : BaseDocumentationElement, IReferencable
    {
        private readonly List<DeclaredType> types = new List<DeclaredType>();

        public Namespace(Identifier identifier)
            : base(identifier)
        {
        }

        public IEnumerable<DeclaredType> Classes
        {
            get
            {
                return this.Types.Where(x => !x.IsInterface);
            }
        }

        public string FullName
        {
            get
            {
                return this.Name;
            }
        }

        public bool HasClasses
        {
            get
            {
                return this.Classes.Any();
            }
        }

        public bool HasInterfaces
        {
            get
            {
                return this.Interfaces.Any();
            }
        }

        public bool HasTypes
        {
            get
            {
                return this.Types.Count > 0;
            }
        }

        public IEnumerable<DeclaredType> Interfaces
        {
            get
            {
                return this.Types.Where(x => x.IsInterface);
            }
        }

        public string PrettyName
        {
            get
            {
                return this.Name;
            }
        }

        public IList<DeclaredType> Types
        {
            get
            {
                return this.types;
            }
        }

        public static Namespace Unresolved(NamespaceIdentifier namespaceIdentifier)
        {
            return new Namespace(namespaceIdentifier) { IsResolved = false };
        }

        public void AddType(DeclaredType type)
        {
            this.types.Add(type);
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            if (referencables.ContainsKey(this.identifier))
            {
                this.IsResolved = true;

                foreach (DeclaredType type in this.Types)
                {
                    if (!type.IsResolved)
                    {
                        type.Resolve(referencables);
                    }
                }
            }
            else
            {
                this.ConvertToExternalReference();
            }
        }

        public void Sort()
        {
            this.types.Sort((x, y) => x.Name.CompareTo(y.Name));

            foreach (DeclaredType type in this.types)
            {
                type.Sort();
            }
        }

        public override string ToString()
        {
            return base.ToString() + " { Name = '" + this.Name + "' }";
        }
    }
}