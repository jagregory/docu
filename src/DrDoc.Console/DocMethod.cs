using System.Collections.Generic;
using DrDoc.Model;

namespace DrDoc
{
    public class DocMethod
    {
        private readonly IList<DocParameter> parameters = new List<DocParameter>();

        public DocMethod(MethodIdentifier name, string prettyName)
        {
            Name = name;
            PrettyName = prettyName;
            Summary = new List<DocBlock>();
        }

        internal void AddParameter(DocParameter parameter)
        {
            parameters.Add(parameter);
        }

        public string PrettyName { get; set; }
        public MethodIdentifier Name { get; private set; }
        public IList<DocParameter> Parameters
        {
            get { return parameters; }
        }

        public IList<DocBlock> Summary { get; internal set; }
    }
}