namespace DrDoc.Documentation
{
    public class Property
    {
        public Property(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}