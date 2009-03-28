using System;
using Docu.Documentation.Comments;

namespace Docu.UI
{
    internal interface IOutputFormatterPart
    {
        Func<IComment, bool> Criteria { get; }
        Func<IComment, string> Action { get; }
    }
}