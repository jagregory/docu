namespace Docu.Documentation.Comments
{
    using System.Collections.Generic;

    public class Remarks : Comment
    {
        public Remarks()
        {
        }

        public Remarks(IEnumerable<Comment> comments)
        {
            comments.ForEach(AddChild);
        }
    }
}
