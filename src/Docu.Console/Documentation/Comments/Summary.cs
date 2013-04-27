namespace Docu.Documentation.Comments
{
    using System.Collections.Generic;

    public class Summary : Comment
    {
        public Summary()
        {
        }

        public Summary(IEnumerable<Comment> comments)
        {
            comments.ForEach(AddChild);
        }
    }
}
