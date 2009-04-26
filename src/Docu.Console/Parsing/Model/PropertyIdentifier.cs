using System;

namespace Docu.Parsing.Model
{
    public sealed class PropertyIdentifier : Identifier, IEquatable<PropertyIdentifier>
    {
        private readonly TypeIdentifier typeId;

        public PropertyIdentifier(string name, bool hasGet, bool hasSet, TypeIdentifier typeId)
            : base(name)
        {
            this.typeId = typeId;
            HasGet = hasGet;
            HasSet = hasSet;
        }

        public bool HasGet { get; private set; }
        public bool HasSet { get; private set; }

        public override NamespaceIdentifier CloneAsNamespace()
        {
            return typeId.CloneAsNamespace();
        }

        public override TypeIdentifier CloneAsType()
        {
            return typeId.CloneAsType();
        }

        public override int CompareTo(Identifier other)
        {
            if (other is PropertyIdentifier)
            {
                var p = (PropertyIdentifier)other;
                int comparison = ToString().CompareTo(p.ToString());

                if (comparison != 0)
                    return comparison;

                comparison = typeId.CompareTo(p.typeId);

                if (comparison != 0)
                    return comparison;

                return 0;
            }

            return -1;
        }

        public override bool Equals(Identifier obj)
        {
            // no need for expensive GetType calls since the class is sealed.
            return Equals(obj as PropertyIdentifier);
        }

        public bool Equals(PropertyIdentifier other)
        {
            // no need for expensive GetType calls since the class is sealed.
            if(((object)other) == null)
            {
                return false;
            }

            return (Name == other.Name) && typeId.Equals(other.typeId);
        }
    }
}