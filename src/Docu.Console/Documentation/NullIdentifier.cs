using System;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public sealed class NullIdentifier : Identifier, IEquatable<NullIdentifier>
    {
        public NullIdentifier(string name)
            : base(name)
        {
        }

        public bool Equals(NullIdentifier other)
        {
            // no need for expensive GetType calls since the class is sealed.
            if (((object) other) == null)
            {
                return false;
            }

            return Name == other.Name;
        }

        public override NamespaceIdentifier CloneAsNamespace()
        {
            return null;
        }

        public override TypeIdentifier CloneAsType()
        {
            return null;
        }

        public override int CompareTo(Identifier other)
        {
            return -1;
        }

        public override bool Equals(Identifier obj)
        {
            // no need for expensive GetType calls since the class is sealed.
            return Equals(obj as NullIdentifier);
        }
    }
}
