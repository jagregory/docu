using System.Collections.Generic;

namespace Docu.Documentation.Comments
{
    public class MultilineCode : BaseComment
    {
        public MultilineCode()
        {}

        public MultilineCode(IEnumerable<IComment> comments)
        {
            comments.ForEach(AddChild);
        }
    }
}