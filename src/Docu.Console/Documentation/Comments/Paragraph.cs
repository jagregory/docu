using System.Collections.Generic;

namespace Docu.Documentation.Comments
{
    public class Paragraph : BaseComment
    {
        public Paragraph(IEnumerable<IComment> comments)
        {
            comments.ForEach(AddChild);
        }
    }
}