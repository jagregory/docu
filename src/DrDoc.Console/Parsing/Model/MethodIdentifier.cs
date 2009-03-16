namespace DrDoc.Parsing.Model
{
    public class MethodIdentifier : Identifier
    {
        private readonly TypeIdentifier typeId;
        private readonly TypeIdentifier[] parameters;
        private readonly bool isStatic;
        private readonly bool isPublic;

        public MethodIdentifier(string name, TypeIdentifier[] parameters, bool isStatic, bool isPublic, TypeIdentifier typeId)
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
            if (obj is MethodIdentifier)
            {
                var other = (MethodIdentifier)obj;
                var parametersSame = true;

                if (parameters.Length == other.parameters.Length)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (!parameters[i].Equals(other.parameters[i]))
                            parametersSame = false;
                    }
                }
                else
                    parametersSame = false;

                return base.Equals(obj) && typeId.Equals(other.typeId) && parametersSame;
            }
            return false;
        }

        public override int CompareTo(Identifier other)
        {
            if (other is MethodIdentifier)
            {
                var m = (MethodIdentifier)other;
                var comparison = ToString().CompareTo(m.ToString());

                if (comparison != 0)
                    return comparison;

                comparison = typeId.ToString().CompareTo(m.typeId.ToString());

                if (comparison != 0)
                    return comparison;

                comparison = parameters.Length.CompareTo(m.parameters.Length);

                if (comparison != 0)
                    return comparison;

                for (int i = 0; i < parameters.Length; i++)
                {
                    comparison = parameters[i].CompareTo(m.parameters[i]);

                    if (comparison != 0)
                        return comparison;
                }

                return 0;
            }

            if (other is TypeIdentifier || other is NamespaceIdentifier)
                return 1;
            
            return -1;
        }
    }
}