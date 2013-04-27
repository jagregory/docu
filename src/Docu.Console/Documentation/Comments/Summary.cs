namespace Docu.Documentation.Comments
{
    using System.Collections.Generic;

    public class Summary : BaseComment
    {
        public Summary()
            : this(new IComment[0])
        {
        }

        public Summary(IEnumerable<IComment> comments)
        {
            comments.ForEach(this.AddChild);
        }
    }
}