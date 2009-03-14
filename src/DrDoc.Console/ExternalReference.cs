namespace DrDoc
{
    public class ExternalReference : IReferencable
    {
        public ExternalReference(string name)
        {
            ParseDocName(name);
        }

        private void ParseDocName(string name)
        {
            if (name.StartsWith("N:"))
            {
                Name = name.Substring(2);
                FullName = Name;
            }
            else if (name.StartsWith("T:"))
            {
                FullName = name.Substring(2);
                Name = FullName.Substring(FullName.LastIndexOf('.') + 1);
            }
        }

        public string Name { get; private set; }
        public string FullName { get; private set; }
    }
}