using System.Collections.Generic;

namespace Docu.Documentation.Comments
{
    public class Summary : BaseComment
    {
        public Summary()
            : this(new IComment[0])
        {}

        public Summary(IEnumerable<IComment> comments)
        {
            comments.ForEach(AddChild);
        }
    }
}