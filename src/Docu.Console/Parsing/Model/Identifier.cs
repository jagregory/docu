using System;
using System.Diagnostics;

namespace Docu.Parsing.Model
{
    [DebuggerDisplay("{Name}")]
    public abstract class Identifier : IComparable<Identifier>, IEquatable<Identifier>
    {
        readonly string _name;

        protected Identifier(string name)
        {
            _name = name;
        }

        protected string Name
        {
            get { return _name; }
        }

        public abstract int CompareTo(Identifier other);
        public abstract bool Equals(Identifier obj);

        public abstract NamespaceIdentifier CloneAsNamespace();
        public abstract TypeIdentifier CloneAsType();

        public static bool operator ==(Identifier first, Identifier second)
        {
            return (((object) first) != null) && first.Equals(second);
        }

        public static bool operator !=(Identifier first, Identifier second)
        {
            return !(first == second);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Identifier);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return _name; // HACK
        }
    }
}
