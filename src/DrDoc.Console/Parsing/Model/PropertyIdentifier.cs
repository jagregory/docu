namespace DrDoc.Parsing.Model
{
    public class PropertyIdentifier : Identifier
    {
        private readonly Identifier typeId;

        public PropertyIdentifier(string name, TypeIdentifier typeId)
            : base(name)
        {
            this.typeId = typeId;
        }

        public override NamespaceIdentifier CloneAsNamespace()
        {
            throw new System.NotImplementedException();
        }

        public override TypeIdentifier CloneAsType()
        {
            throw new System.NotImplementedException();
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