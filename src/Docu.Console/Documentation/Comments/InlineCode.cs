namespace Docu.Documentation.Comments
{
    public class InlineCode : Comment
    {
        public InlineCode(string text)
        {
            Text = text;
        }

        public string Text { get; private set; }
    }
}
