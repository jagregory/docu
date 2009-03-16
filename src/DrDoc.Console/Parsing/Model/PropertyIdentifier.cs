namespace DrDoc.Parsing.Model
{
    public class PropertyIdentifier : Identifier
    {
        private readonly Identifier typeId;

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
                var comparison = ToString().CompareTo(p.ToString());

                if (comparison != 0)
                    return comparison;

                comparison = typeId.CompareTo(p.typeId);

                if (comparison != 0)
                    return comparison;

                return 0;
            }

            return -1;
        }
    }
}