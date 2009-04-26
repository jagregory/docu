using System;

namespace Docu.Parsing.Model
{
    public sealed class MethodIdentifier : Identifier, IEquatable<MethodIdentifier>, IComparable<MethodIdentifier>
    {
        private readonly bool isPublic;
        private readonly bool isStatic;
        private readonly TypeIdentifier[] parameters;
        private readonly TypeIdentifier typeId;

        public MethodIdentifier(string name, TypeIdentifier[] parameters, bool isStatic, bool isPublic,
                                TypeIdentifier typeId)
            : base(name)
        {
            this.typeId = typeId;
            this.parameters = parameters;
            this.isStatic = isStatic;
            this.isPublic = isPublic;
        }

        public bool IsStatic
        {
            get { return isStatic; }
        }

        public bool IsPublic
        {
            get { return isPublic; }
        }

        public override NamespaceIdentifier CloneAsNamespace()
        {
            return typeId.CloneAsNamespace();
        }

        public override TypeIdentifier CloneAsType()
        {
            return typeId;
        }

        public override bool Equals(Identifier obj)
        {
            // no need for expensive GetType calls since the class is sealed.
            return Equals(obj as MethodIdentifier);
        }

        public bool Equals(MethodIdentifier other)
        {
            // no need for expensive GetType calls since the class is sealed.

            // verify identifier-type, name and number of parameters
            if((((object)other) == null) || (Name != other.Name) || (parameters.Length != other.parameters.Length))
            {
                return false;
            }

            // verify parameter types
            for(int i = 0; i < parameters.Length; i++)
            {
                if(!parameters[i].Equals(other.parameters[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int CompareTo(Identifier other)
        {
            if(other is TypeIdentifier || other is NamespaceIdentifier)
            {
                return 1;
            }

            return CompareTo(other as MethodIdentifier);
        }

        public int CompareTo(MethodIdentifier other)
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

            comparison = typeId.CompareTo(other.typeId);
            if(comparison != 0)
            {
                return comparison;
            }

            comparison = parameters.Length.CompareTo(other.parameters.Length);
            if(comparison != 0)
            {
                return comparison;
            }

            for(int i = 0; i < parameters.Length; i++)
            {
                comparison = parameters[i].CompareTo(other.parameters[i]);
                if(comparison != 0)
                {
                    return comparison;
                }
            }

            return 0;
        }
    }
}