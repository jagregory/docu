namespace Docu.Documentation.Comments
{
    using System.Collections.Generic;

    public class Paragraph : Comment
    {
        public Paragraph(IEnumerable<Comment> comments)
        {
            comments.ForEach(AddChild);
        }
    }
}
