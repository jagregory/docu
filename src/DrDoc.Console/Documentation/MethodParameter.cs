using System.Collections.Generic;
using DrDoc.Documentation.Comments;
using DrDoc.Documentation;
using DrDoc.Documentation.Comments;

namespace DrDoc.Documentation
{
    public class MethodParameter : IReferrer
    {
        public MethodParameter(string name, IReferencable reference)
        {
            Name = name;
            Reference = reference;
            Summary = new List<IComment>();
        }

        public string Name { get; private set; }
        public string PrettyName
        {
            get
            {
                var typeReference = Reference as DeclaredType;

                if (typeReference != null)
                    return typeReference.PrettyName;

                var methodReference = Reference as Method;

                if (methodReference != null)
                    return methodReference.PrettyName;

                var externalReference = Reference as ExternalReference;

                if (externalReference != null)
                    return externalReference.PrettyName;

                return Name;
            }
        }
        public IReferencable Reference { get; set; }
        public IList<IComment> Summary { get; internal set; }
    }
}