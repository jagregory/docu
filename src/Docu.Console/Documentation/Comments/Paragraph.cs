namespace Docu.Documentation.Comments
{
    using System.Collections.Generic;

    public class Paragraph : BaseComment
    {
        public Paragraph(IEnumerable<IComment> comments)
        {
            comments.ForEach(this.AddChild);
        }
    }
}