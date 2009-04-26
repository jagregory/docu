using System;

namespace Docu.Parsing.Model
{
    public sealed class NamespaceIdentifier : Identifier, IEquatable<NamespaceIdentifier>, IComparable<NamespaceIdentifier>
    {
        public NamespaceIdentifier(string name)
            : base(name)
        {
        }

        public override NamespaceIdentifier CloneAsNamespace()
        {
            return this;
        }

        public override TypeIdentifier CloneAsType()
        {
            throw new NotImplementedException();
        }

        public override bool Equals(Identifier obj)
        {
            // no need for expensive GetType calls since the class is sealed.
            return Equals(obj as NamespaceIdentifier);
        }

        public bool Equals(NamespaceIdentifier other)
        {
            // no need for expensive GetType calls since the class is sealed.
            if(((object)other) == null)
            {
                return false;
            }

            return (Name == other.Name);
        }

        public override int CompareTo(Identifier other)
        {
            return CompareTo(other as NamespaceIdentifier);
        }

        public int CompareTo(NamespaceIdentifier other)
        {
            if(((object)other) == null)
            {
                return -1;
            }

            return Name.CompareTo(other.Name);
        }
    }
}