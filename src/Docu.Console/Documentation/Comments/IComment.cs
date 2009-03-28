using System.Collections.Generic;

namespace Docu.Documentation.Comments
{
    public interface IComment
    {
        IEnumerable<IComment> Children { get; }
    }
}