namespace Docu.Parsing.Comments
{
    public class ParseOptions
    {
        public bool PreserveWhitespace { get; set; }

        public ParseOptions()
        {
            PreserveWhitespace = false;
        }
    }
}