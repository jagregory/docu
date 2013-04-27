namespace Docu.Documentation.Comments
{
    using System.Collections.Generic;

    public class MultilineCode : Comment
    {
        public MultilineCode()
        {
        }

        public MultilineCode(IEnumerable<Comment> comments)
        {
            comments.ForEach(AddChild);
        }
    }
}
