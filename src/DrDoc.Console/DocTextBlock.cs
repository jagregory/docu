namespace DrDoc
{
    public class DocTextBlock : DocBlock
    {
        public DocTextBlock(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }

        public override string ToString()
        {
            return Text;
        }
    }
}