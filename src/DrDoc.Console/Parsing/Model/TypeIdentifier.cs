namespace DrDoc.Parsing.Model
{
    public class TypeIdentifier : Identifier
    {
        private readonly string _namespace;

        public TypeIdentifier(string name, string ns)
            : base(name)
        {
            _namespace = ns;
        }

        public override NamespaceIdentifier CloneAsNamespace()
        {
            return new NamespaceIdentifier(_namespace);
        }

        public override TypeIdentifier CloneAsType()
        {
            return this;
        }

        public override int CompareTo(Identifier other)
        {
            if (other is TypeIdentifier)
            {
                var t = (TypeIdentifier)other;
                var comparison = ToString().CompareTo(t.ToString());

                if (comparison != 0)
                    return comparison;

                comparison = _namespace.CompareTo(t._namespace);

                if (comparison != 0)
                    return comparison;

                return 0;
            }
            if (other is NamespaceIdentifier)
                return 1;
         
            return -1;
        }
    }
}