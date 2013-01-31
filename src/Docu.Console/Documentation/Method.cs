namespace Docu.Documentation
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    using Docu.Documentation.Comments;
    using Docu.Parsing.Model;

    public class Method : BaseDocumentationElement, IReferencable
    {
        private readonly IList<MethodParameter> parameters = new List<MethodParameter>();

        private MethodBase representedMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="Method"/> class.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        public Method(MethodIdentifier identifier, DeclaredType type)
            : base(identifier)
        {
            this.Type = type;
            this.Returns = new Summary();
        }

        public string FullName
        {
            get
            {
                return this.Name;
            }
        }

        public override bool HasDocumentation
        {
            get
            {
                return base.HasDocumentation || !this.Returns.IsEmpty;
            }
        }

        public bool IsExtension
        {
            get
            {
                return this.representedMethod != null
                       &&
                       (this.representedMethod.IsStatic
                        && this.representedMethod.GetCustomAttributes(typeof(ExtensionAttribute), false).Length > 0);
            }
        }

        public bool IsPublic
        {
            get
            {
                return ((MethodIdentifier)this.identifier).IsPublic;
            }
        }

        public bool IsStatic
        {
            get
            {
                return ((MethodIdentifier)this.identifier).IsStatic;
            }
        }

        public bool IsConstructor
        {
            get
            {
                return ((MethodIdentifier)this.identifier).IsConstructor;
            }
        }

        public IList<MethodParameter> Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        public string PrettyName
        {
            get
            {
                return this.representedMethod == null ? this.Name : this.representedMethod.GetPrettyName();
            }
        }

        public IReferencable ReturnType { get; set; }

        public Summary Returns { get; set; }

        public DeclaredType Type { get; set; }

        public static Method Unresolved(MethodIdentifier methodIdentifier, DeclaredType type)
        {
            return new Method(methodIdentifier, type) { IsResolved = false };
        }

        public static Method Unresolved(
            MethodIdentifier methodIdentifier, DeclaredType type, MethodBase representedMethod, IReferencable returnType)
        {
            return new Method(methodIdentifier, type)
                {
                   IsResolved = false, representedMethod = representedMethod, ReturnType = returnType 
                };
        }

        public void Resolve(IDictionary<Identifier, IReferencable> referencables)
        {
            if (referencables.ContainsKey(this.identifier))
            {
                this.IsResolved = true;
                IReferencable referencable = referencables[this.identifier];
                var method = referencable as Method;

                if (method == null)
                {
                    throw new InvalidOperationException("Cannot resolve to '" + referencable.GetType().FullName + "'");
                }

                this.ReturnType = method.ReturnType;

                if (this.ReturnType != null && !this.ReturnType.IsResolved)
                {
                    this.ReturnType.Resolve(referencables);
                }

                this.representedMethod = method.representedMethod;

                if (!this.Summary.IsResolved)
                {
                    this.Summary.Resolve(referencables);
                }

                if (!this.Remarks.IsResolved)
                {
                    this.Remarks.Resolve(referencables);
                }

                foreach (MethodParameter para in this.Parameters)
                {
                    if ((para.Reference != null) && (!para.Reference.IsResolved))
                    {
                        para.Reference.Resolve(referencables);
                    }
                }
            }
            else
            {
                this.ConvertToExternalReference();
            }
        }

        internal void AddParameter(MethodParameter parameter)
        {
            this.parameters.Add(parameter);
        }
    }
}