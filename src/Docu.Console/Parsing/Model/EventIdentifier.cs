using System;

namespace Docu.Parsing.Model
{
    public sealed class EventIdentifier : Identifier, IEquatable<EventIdentifier>
    {
        private readonly TypeIdentifier typeId;

        public EventIdentifier(string name, TypeIdentifier typeId) : base(name)
        {
            this.typeId = typeId;
        }

        public override int CompareTo(Identifier other)
        {
            if (other is EventIdentifier)
            {
                var e = (EventIdentifier)other;
                int comparison = ToString().CompareTo(e.ToString());

                if (comparison != 0)
                    return comparison;

                comparison = typeId.CompareTo(e.typeId);

                if (comparison != 0)
                    return comparison;

                return 0;
            }

            return -1;
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