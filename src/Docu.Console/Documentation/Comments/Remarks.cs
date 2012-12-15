namespace Docu.Documentation.Comments
{
    using System.Collections.Generic;

    public class Remarks : BaseComment
    {
        public Remarks()
            : this(new IComment[0])
        {
        }

        public Remarks(IEnumerable<IComment> comments)
        {
            comments.ForEach(this.AddChild);
        }
    }
}