using System;

namespace Docu.Parsing.Model
{
    public class NamespaceIdentifier : Identifier
    {
        public NamespaceIdentifier(string name)
            : base(name)
        {
        }

        public override NamespaceIdentifier CloneAsNamespace()
        {
            return this;
        }

        public override TypeIdentifier CloneAsType()
        {
            throw new NotImplementedException();
        }

        public override int CompareTo(Identifier other)
        {
            if (other is NamespaceIdentifier)
                return ToString().CompareTo(other.ToString());

            return -1;
        }
    }
}