using System.Collections.Generic;
using DrDoc.Documentation.Comments;
using DrDoc.Documentation;
using DrDoc.Documentation.Comments;
using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public class Method
    {
        private readonly IList<MethodParameter> parameters = new List<MethodParameter>();

        public Method(MethodIdentifier name, string prettyName)
        {
            Name = name;
            PrettyName = prettyName;
            Summary = new List<IComment>();
        }

        internal void AddParameter(MethodParameter parameter)
        {
            parameters.Add(parameter);
        }

        public string PrettyName { get; set; }
        public MethodIdentifier Name { get; private set; }
        public IList<MethodParameter> Parameters
        {
            get { return parameters; }
        }

        public IList<IComment> Summary { get; internal set; }
    }
}