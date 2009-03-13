namespace DrDoc
{
    /// <summary>
    /// <see cref="IReferencable"/>
    /// </summary>
    public class UnresolvedReference : IReferencable
    {
        public UnresolvedReference(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}