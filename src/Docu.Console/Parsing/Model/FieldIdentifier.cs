using System;

namespace Docu.Parsing.Model
{
    public sealed class FieldIdentifier : Identifier, IEquatable<FieldIdentifier>, IComparable<FieldIdentifier>
    {
        private readonly TypeIdentifier typeId;

        public FieldIdentifier(string name, TypeIdentifier typeId)
            : base(name)
        {
            this.typeId = typeId;
        }

        public override NamespaceIdentifier CloneAsNamespace()
        {
            return typeId.CloneAsNamespace();
        }

        public override TypeIdentifier CloneAsType()
        {
            return typeId.CloneAsType();
        }

        public override bool Equals(Identifier obj)
        {
            // no need for expensive GetType calls since the class is sealed.
            return Equals(obj as FieldIdentifier);
        }

        public bool Equals(FieldIdentifier other)
        {
            // no need for expensive GetType calls since the class is sealed.
            if(((object)other) == null)
            {
                return false;
            }

            return (Name == other.Name) && typeId.Equals(other.typeId);
        }

        public override int CompareTo(Identifier other)
        {
            if(other is TypeIdentifier || other is NamespaceIdentifier
                || other is MethodIdentifier || other is PropertyIdentifier)
            {
                return 1;
            }

            return CompareTo(other as FieldIdentifier);
        }

        public int CompareTo(FieldIdentifier other)
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

            return typeId.CompareTo(other.typeId);
        }
    }
}