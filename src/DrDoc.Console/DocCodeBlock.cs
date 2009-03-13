namespace DrDoc
{
    public class DocCodeBlock : DocBlock
    {
        public DocCodeBlock(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}