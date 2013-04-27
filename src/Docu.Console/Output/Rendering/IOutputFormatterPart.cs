using System;
using Docu.Documentation.Comments;

namespace Docu.Output.Rendering
{
    internal interface IOutputFormatterPart
    {
        Func<Comment, bool> Criteria { get; }
        Func<Comment, string> Action { get; }
    }
}