namespace DrDoc
{
    public class DocParameter
    {
        public DocParameter(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; private set; }
        public string Type { get; private set; }
    }
}