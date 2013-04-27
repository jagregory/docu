namespace Docu.Documentation.Comments
{
    using System.Collections.Generic;

    public interface IComment
    {
        IEnumerable<IComment> Children { get; }
        void AddChild(IComment child);
    }
}