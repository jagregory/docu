using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class Method : BaseDocumentationElement, IReferencable
    {
        private readonly IList<MethodParameter> parameters = new List<MethodParameter>();
        private MethodInfo representedMethod;

        public Method(MethodIdentifier identifier, DeclaredType type)
            : base(identifier)
        {
            Type = type;
            Returns = new Summary();
        }

        public override bool HasDocumentation
        {
            get { return base.HasDocumentation || !Returns.IsEmpty; }
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

        public bool IsExtension
        {
            get
            {
                return representedMethod == null 
                    ? false
                    : representedMethod.IsStatic && representedMethod.GetCustomAttributes(typeof (ExtensionAttribute), false).Length > 0;
            }
        }

        public DeclaredType Type { get; set; }
        public IReferencable ReturnType { get; set; }
        public Summary Returns { get; set; }

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
                IsResolved = true;
                IReferencable referencable = referencables[identifier];
                var method = referencable as Method;

                if (method == null)
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");

                ReturnType = method.ReturnType;

                if (!ReturnType.IsResolved)
                    ReturnType.Resolve(referencables);

                representedMethod = method.representedMethod;

                if (!Summary.IsResolved)
                    Summary.Resolve(referencables);

                if (!Remarks.IsResolved)
                    Remarks.Resolve(referencables);

                foreach (var para in Parameters)
                {
                    if ((para.Reference != null) && (!para.Reference.IsResolved))
                        para.Reference.Resolve(referencables);
                }
            }
            else
                ConvertToExternalReference();
        }

        internal void AddParameter(MethodParameter parameter)
        {
            parameters.Add(parameter);
        }

        public static Method Unresolved(MethodIdentifier methodIdentifier, DeclaredType type)
        {
            return new Method(methodIdentifier, type) { IsResolved = false };
        }

        public static Method Unresolved(MethodIdentifier methodIdentifier, DeclaredType type, MethodInfo representedMethod, IReferencable returnType)
        {
            return new Method(methodIdentifier, type)
            {
                IsResolved = false,
                representedMethod = representedMethod,
                ReturnType = returnType
            };
        }
    }
}