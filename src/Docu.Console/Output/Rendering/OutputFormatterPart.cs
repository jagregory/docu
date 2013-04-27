using System;
using Docu.Documentation.Comments;

namespace Docu.Output.Rendering
{
    public class OutputFormatterPart<T> : IOutputFormatterPart
    {
        readonly Func<T, string> _action;

        public OutputFormatterPart(Func<T, string> action)
        {
            _action = action;
        }

        public Func<IComment, bool> Criteria
        {
            get { return x => x is T; }
        }
            
        public Func<IComment, string> Action
        {
            get { return x => _action((T)x); }
        }
    }
}