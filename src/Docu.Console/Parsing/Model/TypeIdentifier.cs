using System;

namespace Docu.Parsing.Model
{
    public sealed class TypeIdentifier : Identifier, IEquatable<TypeIdentifier>, IComparable<TypeIdentifier>
    {
        private readonly string _namespace;

        public TypeIdentifier(string name, string ns)
            : base(name)
        {
            _namespace = ns;
        }

        public override NamespaceIdentifier CloneAsNamespace()
        {
            return new NamespaceIdentifier(_namespace);
        }

        public override TypeIdentifier CloneAsType()
        {
            return this;
        }

        public override bool Equals(Identifier obj)
        {
            // no need for expensive GetType calls since the class is sealed.
            return Equals(obj as TypeIdentifier);
        }

        public bool Equals(TypeIdentifier other)
        {
            // no need for expensive GetType calls since the class is sealed.
            if(((object)other) == null)
            {
                return false;
            }

            return (Name == other.Name) && (_namespace == other._namespace);
        }

        public override int CompareTo(Identifier other)
        {
            if(other is NamespaceIdentifier)
            {
                return 1;
            }

            return CompareTo(other as TypeIdentifier);
        }

        public int CompareTo(TypeIdentifier other)
        {
            if(((object)other) == null)
            {
                return -1;
            }

            int comparison = Name.CompareTo(other.Name);
            if(comparison != 0)
            {
                return comparison;
            }

            return _namespace.CompareTo(other._namespace);
        }

    }
}