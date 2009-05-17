using System;
using Docu.Documentation.Comments;

namespace Docu.Output.Rendering
{
    public class OutputFormatterPart<T> : IOutputFormatterPart
    {
        public OutputFormatterPart(Func<T, string> action)
        {
            Action = action;
        }

        public Func<IComment, bool> Criteria
        {
            get { return x => x is T; }
        }

        public Func<T, string> Action { get; private set; }
            
        Func<IComment, string> IOutputFormatterPart.Action
        {
            get { return x => Action((T)x); }
        }
    }
}