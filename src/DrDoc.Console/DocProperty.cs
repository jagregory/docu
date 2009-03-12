namespace DrDoc
{
    public class DocProperty
    {
        public DocProperty(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}