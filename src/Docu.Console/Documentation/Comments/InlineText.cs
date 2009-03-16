namespace Docu.Documentation.Comments
{
    public class InlineText : IComment
    {
        public InlineText(string text)
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