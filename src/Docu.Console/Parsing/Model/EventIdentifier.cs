namespace Docu.Parsing.Model
{
    public class EventIdentifier : Identifier
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
            if (obj is EventIdentifier)
            {
                var other = (EventIdentifier)obj;

                return base.Equals(obj) && typeId.Equals(other.typeId);
            }

            return false;
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