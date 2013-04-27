using System;
using Docu.Documentation.Comments;

namespace Docu.Output.Rendering
{
    public class OutputFormatterPart<T> : IOutputFormatterPart where T : Comment
    {
        readonly Func<T, string> _action;

        public OutputFormatterPart(Func<T, string> action)
        {
            _action = action;
        }

        public Func<Comment, bool> Criteria
        {
            get { return x => x is T; }
        }

        public Func<Comment, string> Action
        {
            get { return x => _action((T)x); }
        }
    }
}