using System;

namespace Docu.Parsing.Model
{
    public sealed class MethodIdentifier : Identifier, IEquatable<MethodIdentifier>, IComparable<MethodIdentifier>
    {
        private readonly bool _isPublic;
        private readonly bool _isStatic;
        private readonly bool _isConstructor;
        private readonly TypeIdentifier[] _parameters;
        private readonly TypeIdentifier _typeId;

        public MethodIdentifier(string name, TypeIdentifier[] parameters, bool isStatic, bool isPublic, bool isConstructor, TypeIdentifier typeId)
            : base(name)
        {
            _typeId = typeId;
            _parameters = parameters;
            _isStatic = isStatic;
            _isPublic = isPublic;
            _isConstructor = isConstructor;
        }

        public bool IsStatic
        {
            get { return _isStatic; }
        }

        public bool IsPublic
        {
            get { return _isPublic; }
        }

        public bool IsConstructor
        {
            get { return _isConstructor; }
        }

        public override NamespaceIdentifier CloneAsNamespace()
        {
            return _typeId.CloneAsNamespace();
        }

        public override TypeIdentifier CloneAsType()
        {
            return _typeId;
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
            if (((object)other) == null || Name != other.Name || !_typeId.Equals(other._typeId) || _parameters.Length != other._parameters.Length)
            {
                return false;
            }

            // verify parameter types
            for (int i = 0; i < _parameters.Length; i++)
            {
                if (!_parameters[i].Equals(other._parameters[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public override int CompareTo(Identifier other)
        {
            if (other is TypeIdentifier || other is NamespaceIdentifier)
            {
                return 1;
            }

            return CompareTo(other as MethodIdentifier);
        }

        public int CompareTo(MethodIdentifier other)
        {
            if (((object)other) == null)
            {
                return -1;
            }

            int comparison = Name.CompareTo(other.Name);
            if (comparison != 0)
            {
                return comparison;
            }

            comparison = _typeId.CompareTo(other._typeId);
            if (comparison != 0)
            {
                return comparison;
            }

            comparison = _parameters.Length.CompareTo(other._parameters.Length);
            if (comparison != 0)
            {
                return comparison;
            }

            for (int i = 0; i < _parameters.Length; i++)
            {
                comparison = _parameters[i].CompareTo(other._parameters[i]);
                if (comparison != 0)
                {
                    return comparison;
                }
            }

            return 0;
        }
    }
}