namespace Docu.Parsing.Model
{
    public class FieldIdentifier : Identifier
    {
        private readonly Identifier typeId;

        public FieldIdentifier(string name, TypeIdentifier typeId)
            : base(name)
        {
            this.typeId = typeId;
        }

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
            if (other is FieldIdentifier)
            {
                var f = (FieldIdentifier)other;
                int comparison = ToString().CompareTo(f.ToString());

                if (comparison != 0)
                    return comparison;

                comparison = typeId.CompareTo(f.typeId);

                if (comparison != 0)
                    return comparison;

                return 0;
            }

            if (other is TypeIdentifier || other is NamespaceIdentifier ||
                other is MethodIdentifier || other is PropertyIdentifier)
                return 1;

            return -1;
        }

        public override bool Equals(Identifier obj)
        {
            if (obj is FieldIdentifier)
            {
                var other = (FieldIdentifier)obj;

                return base.Equals(obj) && typeId.Equals(other.typeId);
            }

            return false;
        }
    }
}