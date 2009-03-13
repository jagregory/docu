namespace DrDoc
{
    public class DocParagraphBlock : DocBlock
    {
        public DocParagraphBlock(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}