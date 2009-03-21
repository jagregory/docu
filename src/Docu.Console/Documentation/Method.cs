using System;
using System.Collections.Generic;
using System.Reflection;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class Method : BaseDocumentationElement, IReferencable
    {
        private readonly IList<MethodParameter> parameters = new List<MethodParameter>();
        private MethodInfo representedMethod;

        public Method(MethodIdentifier identifier)
            : base(identifier)
        {
            Summary = new List<IComment>();
        }

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

        public IReferencable ReturnType { get; set; }

        public string FullName
        {
            get { return Name; }
        }

        public string PrettyName
        {
            get { return representedMethod == null ? Name : representedMethod.GetPrettyName(); }
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            if (referencables.ContainsKey(identifier))
            {
                IReferencable referencable = referencables[identifier];
                var method = referencable as Method;

                if (method == null)
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");

                ReturnType = method.ReturnType;
                representedMethod = method.representedMethod;
                IsResolved = true;
            }

            ConvertToExternalReference();
        }

        internal void AddParameter(MethodParameter parameter)
        {
            parameters.Add(parameter);
        }

        public static Method Unresolved(MethodIdentifier methodIdentifier)
        {
            return new Method(methodIdentifier) { IsResolved = false };
        }

        public static Method Unresolved(MethodIdentifier methodIdentifier, MethodInfo representedMethod,
                                        IReferencable returnType)
        {
            return new Method(methodIdentifier)
            { IsResolved = false, representedMethod = representedMethod, ReturnType = returnType };
        }
    }
}