using System;
using System.Collections.Generic;
using System.Linq;
using Docu.Parsing.Model;

namespace Docu.Documentation.Comments
{
    public class Summary : BaseComment
    {
        public Summary()
            : this(new IComment[0])
        {}

        public Summary(IEnumerable<IComment> comments)
        {
            foreach (var comment in comments)
            {
                AddChild(comment);
            }
        }
    }
}