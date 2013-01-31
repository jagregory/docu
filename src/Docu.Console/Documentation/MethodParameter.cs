namespace Docu.Documentation
{
    using Docu.Parsing.Model;
    using System;

    public class MethodParameter : BaseDocumentationElement, IReferrer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodParameter"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="reference">
        /// The reference.
        /// </param>
        public MethodParameter(string name, IReferencable reference)
            : base(new NullIdentifier(name))
        {
            this.Reference = reference;
        }

        public string PrettyName
        {
            get
            {
                var typeReference = this.Reference as DeclaredType;

                if (typeReference != null)
                {
                    return typeReference.PrettyName;
                }

                var methodReference = this.Reference as Method;

                if (methodReference != null)
                {
                    return methodReference.PrettyName;
                }

                // var externalReference = Reference as ExternalReference;

                // if (externalReference != null)
                // return externalReference.PrettyName;
                return this.Name;
            }
        }

        public IReferencable Reference { get; set; }
    }

    public sealed class NullIdentifier : Identifier, IEquatable<NullIdentifier>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NullIdentifier"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public NullIdentifier(string name)
            : base(name)
        {
        }

        public override NamespaceIdentifier CloneAsNamespace()
        {
            return null;
        }

        public override TypeIdentifier CloneAsType()
        {
            return null;
        }

        public override int CompareTo(Identifier other)
        {
            return -1;
        }

        public override bool Equals(Identifier obj)
        {
            // no need for expensive GetType calls since the class is sealed.
            return this.Equals(obj as NullIdentifier);
        }

        public bool Equals(NullIdentifier other)
        {
            // no need for expensive GetType calls since the class is sealed.
            if (((object)other) == null)
            {
                return false;
            }

            return this.Name == other.Name;
        }
    }
}