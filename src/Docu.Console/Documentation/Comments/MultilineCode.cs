namespace Docu.Documentation.Comments
{
    using System.Collections.Generic;

    public class MultilineCode : BaseComment
    {
        public MultilineCode()
        {
        }

        public MultilineCode(IEnumerable<IComment> comments)
        {
            comments.ForEach(this.AddChild);
        }
    }
}