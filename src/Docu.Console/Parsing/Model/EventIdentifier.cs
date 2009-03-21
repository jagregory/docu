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
            throw new System.NotImplementedException();
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