using System;

namespace Docu.Parsing.Model
{
    public sealed class TypeIdentifier : Identifier, IEquatable<TypeIdentifier>
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

        public override int CompareTo(Identifier other)
        {
            if (other is TypeIdentifier)
            {
                var t = (TypeIdentifier)other;
                int comparison = ToString().CompareTo(t.ToString());

                if (comparison != 0)
                    return comparison;

                comparison = _namespace.CompareTo(t._namespace);

                if (comparison != 0)
                    return comparison;

                return 0;
            }
            if (other is NamespaceIdentifier)
                return 1;

            return -1;
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
    }
}