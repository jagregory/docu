using System.Collections.Generic;

namespace Docu.Documentation.Comments
{
    public class Value : BaseComment
    {
        public Value()
            : this(new IComment[0])
        { }

        public Value(IEnumerable<IComment> comments)
        {
            comments.ForEach(AddChild);
        }
    }
}