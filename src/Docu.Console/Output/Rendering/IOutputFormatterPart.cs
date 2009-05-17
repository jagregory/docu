using System;
using Docu.Documentation.Comments;

namespace Docu.Output.Rendering
{
    internal interface IOutputFormatterPart
    {
        Func<IComment, bool> Criteria { get; }
        Func<IComment, string> Action { get; }
    }
}