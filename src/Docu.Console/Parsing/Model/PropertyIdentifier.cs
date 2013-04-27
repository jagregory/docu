using System;

namespace Docu.Parsing.Model
{
    public sealed class PropertyIdentifier : Identifier, IEquatable<PropertyIdentifier>, IComparable<PropertyIdentifier>
    {
        private readonly TypeIdentifier _typeId;

        public PropertyIdentifier(string name, bool hasGet, bool hasSet, TypeIdentifier typeId)
            : base(name)
        {
            _typeId = typeId;
            HasGet = hasGet;
            HasSet = hasSet;
        }

        public bool HasGet { get; private set; }
        public bool HasSet { get; private set; }

        public override NamespaceIdentifier CloneAsNamespace()
        {
            return _typeId.CloneAsNamespace();
        }

        public override TypeIdentifier CloneAsType()
        {
            return _typeId.CloneAsType();
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

            return (Name == other.Name) && _typeId.Equals(other._typeId);
        }

        public override int CompareTo(Identifier other)
        {
            return CompareTo(other as PropertyIdentifier);
        }

        public int CompareTo(PropertyIdentifier other)
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

            return _typeId.CompareTo(other._typeId);
        }
    }
}