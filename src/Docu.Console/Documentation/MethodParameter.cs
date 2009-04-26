using System;
using System.Collections.Generic;
using Docu.Documentation.Comments;
using Docu.Parsing.Model;

namespace Docu.Documentation
{
    public class MethodParameter : BaseDocumentationElement, IReferrer
    {
        public MethodParameter(string name, IReferencable reference)
            : base(new NullIdentifier(name))
        {
            Reference = reference;
        }

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

                //var externalReference = Reference as ExternalReference;

                //if (externalReference != null)
                //    return externalReference.PrettyName;

                return Name;
            }
        }

        public IReferencable Reference { get; set; }
    }

    public sealed class NullIdentifier : Identifier, IEquatable<NullIdentifier>
    {
        public NullIdentifier(string name)
            : base(name)
        {}

        public override NamespaceIdentifier CloneAsNamespace()
        {
            return null;
        }

        public override TypeIdentifier CloneAsType()
        {
            return null;
        }

        public override bool Equals(Identifier obj)
        {
            // no need for expensive GetType calls since the class is sealed.
            return Equals(obj as NullIdentifier);
        }

        public bool Equals(NullIdentifier other)
        {
            // no need for expensive GetType calls since the class is sealed.
            if(((object)other) == null)
            {
                return false;
            }

            return (Name == other.Name);
        }

        public override int CompareTo(Identifier other)
        {
            return -1;
        }
    }
}