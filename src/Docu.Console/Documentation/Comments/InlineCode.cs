namespace Docu.Documentation.Comments
{
    public class InlineCode : BaseComment
    {
        public InlineCode(string text)
        {
            this.Text = text;
        }

        public string Text { get; private set; }
    }
}