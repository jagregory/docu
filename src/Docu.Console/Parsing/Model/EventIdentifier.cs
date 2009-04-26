using System;

namespace Docu.Parsing.Model
{
    public sealed class EventIdentifier : Identifier, IEquatable<EventIdentifier>, IComparable<EventIdentifier>
    {
        private readonly TypeIdentifier typeId;

        public EventIdentifier(string name, TypeIdentifier typeId) : base(name)
        {
            this.typeId = typeId;
        }

        public override bool Equals(Identifier obj)
        {
            // no need for expensive GetType calls since the class is sealed.
            return Equals(obj as EventIdentifier);
        }

        public bool Equals(EventIdentifier other)
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
            return CompareTo(other as EventIdentifier);
        }

        public int CompareTo(EventIdentifier other)
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

        public override NamespaceIdentifier CloneAsNamespace()
        {
            return typeId.CloneAsNamespace();
        }

        public override TypeIdentifier CloneAsType()
        {
            return typeId;
        }
    }
}