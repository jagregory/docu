using System;

namespace Docu.Parsing.Model
{
    public sealed class NamespaceIdentifier : Identifier, IEquatable<NamespaceIdentifier>
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

        public override int CompareTo(Identifier other)
        {
            if (other is NamespaceIdentifier)
                return ToString().CompareTo(other.ToString());

            return -1;
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
    }
}