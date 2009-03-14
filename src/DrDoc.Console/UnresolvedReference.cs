namespace DrDoc
{
    public class UnresolvedReference : IReferencable
    {
        public UnresolvedReference(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}