using System.Collections.Generic;

namespace Docu.Documentation.Comments
{
    public class Value : Comment
    {
        public Value()
        {
        }

        public Value(IEnumerable<Comment> comments)
        {
            comments.ForEach(AddChild);
        }
    }
}
