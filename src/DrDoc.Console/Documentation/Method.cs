using System.Collections.Generic;
using DrDoc.Documentation.Comments;
using DrDoc.Documentation;
using DrDoc.Documentation.Comments;
using DrDoc.Parsing.Model;

namespace DrDoc.Documentation
{
    public class Method : BaseDocumentationElement
    {
        private readonly IList<MethodParameter> parameters = new List<MethodParameter>();

        public Method(MethodIdentifier identifier, string prettyName)
            : base(identifier)
        {
            PrettyName = prettyName;
            Summary = new List<IComment>();
        }

        internal void AddParameter(MethodParameter parameter)
        {
            parameters.Add(parameter);
        }

        public string PrettyName { get; set; }
        public IList<MethodParameter> Parameters
        {
            get { return parameters; }
        }

        public bool IsPublic
        {
            get { return ((MethodIdentifier)identifier).IsPublic; }
        }

        public bool IsStatic
        {
            get { return ((MethodIdentifier)identifier).IsStatic; }
        }

        public IList<IComment> Summary { get; internal set; }
    }
}