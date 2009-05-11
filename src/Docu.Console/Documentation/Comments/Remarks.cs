using System.Collections.Generic;

namespace Docu.Documentation.Comments
{
    public class Remarks : BaseComment
    {
        public Remarks()
            : this(new IComment[0])
        {}

        public Remarks(IEnumerable<IComment> comments)
        {
            comments.ForEach(AddChild);
        }
    }
}